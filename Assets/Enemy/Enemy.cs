using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  private EnemyData data;
  public EnemyData Data {
    get { return data; }
    set {
      HP = value.maxHP;
      Armor = value.maxArmor;
      data = value;
    }
  }
  public Waypoint PrevWaypoint;
  public Waypoint NextWaypoint;

  public static int acidStackMaxMultiplier = 25;
  public static float acidDamagePerStackPerSecond = 1.0f;
  public HashSet<Tower> spittingAntTowerSlows = new();
  public HashSet<Tower> webShootingTowerStuns = new();
  public HashSet<Tower> webShootingTowerPermSlow = new();

  private float crippleSlow = 0.8f;
  private Transform target;

  // PrevWaypoint should be set before OnEnable is called.
  void OnEnable() {
    transform.position = PrevWaypoint.transform.position;
    NextWaypoint = PrevWaypoint.GetNextWaypoint();
    if (transform.childCount > 0) {
      target = transform.GetChild(0).Find("target");
    }

    StartCoroutine(HandleStun());

    if (Flying) {
      StartCoroutine(GroundFlierForDuration());
    }

    if (data.spawner != null) {
      StartCoroutine(Spawn());
    }

    if (data.dazzle != null) {
      EnemyData.DazzleProperties dazzle = data.dazzle.Value;
      StartCoroutine(HandleAbility(
          GetDazzleAction(dazzle.duration), dazzle.interval, dazzle.range));
    }

    if (data.slime != null) {
      EnemyData.SlimeProperties slime = data.slime.Value;
      StartCoroutine(HandleAbility(
          GetSlimeAction(slime.duration, slime.power), slime.interval, slime.range));
    }

    if (NextWaypoint == null) {
      Debug.Log("[ERROR] NextWaypoint not found.");
      return;
    }

    StartCoroutine(FollowPath());
  }

  #region Properties

  public float AcidDamagePerStackPerSecond { get { return acidDamagePerStackPerSecond; } }
  public float AcidStacks {
    get { return data.acidStacks; }
    private set { data.acidStacks = value; }
  }
  public Vector3 AimPoint {
    get {
      if (target != null) {
        return target.transform.position;
      } else {
        return transform.position;
      }
    }
  }
  public float Armor { get; set; }
  public float BaseSpeed { get { return data.speed; } }
  public bool BigTarget { get { return data.properties == EnemyData.Properties.BIG_TARGET; } }
  public bool Camo { get { return data.properties == EnemyData.Properties.CAMO; } }
  public bool Crippled { get; set; }
  public bool CrippleImmunity { get { return data.properties == EnemyData.Properties.CRIPPLE_IMMUNITY; } }
  public int Damage { get { return data.damage; } }
  public EnemyData.DazzleProperties? Dazzle {
    get { return data.dazzle; }
    set { data.dazzle = value; }
  }
  public bool Flying { get { return data.properties == EnemyData.Properties.FLYING; } }
  public float GroundedTime { get; private set; }
  public float HP { get; set; }
  // The enemy's unmodified core speed. This is not what is called to determine move speed.
  public float MaxAcidStacks { get { return (int)data.size * acidStackMaxMultiplier; } }
  public float MaxArmor { get { return data.maxArmor; } }
  public float MaxHp { get { return data.maxHP; } }
  public int Nu { get { return data.nu; } }
  public EnemyData.Size Size { get { return data.size; } }
  public EnemyData.SlimeProperties? Slime {
    get { return data.slime; }
    set { data.slime = value; }
  }
  public float SlowDuration {
    get { return data.slowDuration; }
    private set { data.slowDuration = value; }
  }
  public float SlowPower {
    get { return data.slowPower; }
    private set { data.slowPower = value; }
  }
  public float Speed { get { return data.speed * (1 - SlowPower); } }
  public float StunTime {
    get { return data.stunTime; }
    private set { data.stunTime = value; }
  }
  public EnemyData.Type Type { get { return data.type; } }

  #endregion

  // Damage this enemy while taking armor piercing into account. This method is responsible for initiating death.
  // No other method should try to handle Enemy death.
  public float DamageEnemy(float damage, float armorPierce, bool continuous = false) {
    float effectiveArmor = (continuous) ? Armor * Time.deltaTime : Armor;
    HP -= damage - Mathf.Clamp(effectiveArmor - armorPierce, 0.0f, damage);
    if (HP <= 0.0f) {
      // TODO: Award the player Nu
      if (data.carrier != null) {
        var carrier = data.carrier.Value;
        SpawnChildren(carrier.childKey, carrier.num);
      }
      ConditionalContextualReset();
      ObjectPool.Instance.DestroyEnemy(gameObject);
    }
    return HP;
  }

  // Return the new armor total after the tear is applied.
  public float TearArmor(float armorTear) {
    Armor = Mathf.Max(Armor - armorTear, 0.0f);
    return Armor;
  }

  // Returns true if stacks are at max.
  public bool AddAcidStacks(float stacks) {
    AcidStacks = Mathf.Min(AcidStacks + stacks, MaxAcidStacks);
    return AcidStacks == MaxAcidStacks;
  }

  public void ResetAcidStacks() {
    AcidStacks = 0.0f;
  }

  // Returns the amount of time this enemy is now stunned.
  public float AddStunTime(float stunTime) {
    return StunTime += stunTime;
  }

  // Slows work thusly:
  //     The most powerful slow applied to an enemy is the one actually used. The time is amortized with respect to
  //   the difference.
  //     For example, if an enemy is suffering a 10% slow with a duraiton of 10 seconds and then is hit with a
  //   20% slow with an additional duration of 7 seconds, the new slow stats would be a 20% slow with a 12 second
  //   duration (20 = 10 * 2 for power so 10 / 2 = 5 for duration).
  public void ApplySlow(float incomingSlowPower, float incomingSlowDuration) {
    float newDuration;
    float multiplier = SlowPower / incomingSlowPower;
    if (SlowPower < incomingSlowPower) {
      newDuration = incomingSlowDuration + (SlowDuration * multiplier);
    } else {
      newDuration = SlowDuration + (incomingSlowDuration / multiplier);
    }
    SlowPower = Mathf.Max(SlowPower, incomingSlowPower);
    SlowDuration = newDuration;
  }

  // This permanently reduces the enemy's speed. Care should be taken with calling this method.
  // slow is expected to be 0.0 - 1.0 and is used as a multiplier.
  public void ApplyPermanentSlow(float slow) {
    data.speed *= slow;
  }

  public void ApplyCripple() {
    if (!CrippleImmunity && !Crippled) {
      // [TODO] nnewsom: handle removing the leg, or downing the flyer here.
      data.speed *= crippleSlow;
      Crippled = true;
    }
  }

  public void TemporarilyStripFlying(float duration) {
    GroundedTime += duration;
  }

  public Waypoint GetClosestWaypoint() {
    if (Vector3.Distance(PrevWaypoint.transform.position, this.transform.position)
        < Vector3.Distance(NextWaypoint.transform.position, this.transform.position)) {
      return PrevWaypoint;
    }
    return NextWaypoint;
  }

  public float GetDistanceToEnd() {
    if (NextWaypoint == null) {
      return float.MaxValue;
    }

    return Vector3.Distance(transform.position, NextWaypoint.transform.position) + NextWaypoint.DistanceToEnd;
  }

  private void Update() {
    // Handle slows.
    if (0.0f < SlowDuration) {
      SlowDuration = Mathf.Max(SlowDuration - Time.deltaTime, 0.0f);
      if (SlowDuration <= 0.0f) {
        SlowPower = 0.0f;
      }
    }

    // Handle acid damage.
    if (0.0f < AcidStacks) {
      float acidDamage = AcidDamagePerStackPerSecond * Time.deltaTime;
      AcidStacks = Mathf.Max(AcidStacks - Time.deltaTime, 0.0f);
      HP -= acidDamage;
    }
  }

  private IEnumerator GroundFlierForDuration() {
    while (true) {
      yield return new WaitUntil(() => GroundedTime > 0.0f);
      EnemyData.Properties properties = data.properties;
      data.properties = EnemyData.Properties.NONE;
      // TODO: Ground the enemy mesh and initiate walking animation.
      float interimGroundedTime = GroundedTime;
      yield return new WaitForSeconds(interimGroundedTime);
      GroundedTime -= interimGroundedTime;
      data.properties = properties;
    }
  }

  private IEnumerator HandleStun() {
    float originalSpeed = data.speed;
    while (true) {
      // Wait until the enemy has a nonzero StunTime.
      yield return new WaitUntil(() => 0.0f < StunTime);
      data.speed = 0.0f;
      float interimStunTime = StunTime;
      yield return new WaitForSeconds(interimStunTime);
      StunTime -= interimStunTime;
      data.speed = originalSpeed;
    }
  }

  private IEnumerator FollowPath() {
    while (NextWaypoint != null) {
      Vector3 startPosition = transform.position;
      Vector3 endPosition = NextWaypoint.transform.position;
      float travelPercent = 0.0f;

      transform.LookAt(endPosition);

      while (travelPercent < 1.0f) {
        travelPercent += Time.deltaTime * Speed;
        transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
        yield return null;
      }

      PrevWaypoint = NextWaypoint;
      NextWaypoint = PrevWaypoint.GetNextWaypoint();
    }

    FinishPath();
  }

  private void ConditionalContextualReset() {
    if (GameStateManager.Instance.SelectedEnemy != null
          && this == GameStateManager.Instance.SelectedEnemy) {
      GameStateManager.Instance.DeselectEnemy();
      TerrariumContextUI.Instance.SetNoContextPanel();
    }
  }

  private void FinishPath() {
    ConditionalContextualReset();
    GameStateManager.Instance.DealDamage(Damage);
    ObjectPool.Instance.DestroyEnemy(gameObject);
  }

  private IEnumerator HandleAbility(Action<Tower> ability, float interval, float range) {
    // Wait for a random time in (0, interval)
    // before using the ability for the first time;
    yield return new WaitForSeconds(UnityEngine.Random.Range(0, interval));
    while (true) {
      var towers =
          GameStateManager.Instance.GetTowersInRange(range, transform.position);
      foreach (var tower in towers) {
        ability(tower);
      }
      yield return new WaitForSeconds(interval);
    }
  }

  private Action<Tower> GetDazzleAction(float duration) {
    return (tower) => tower.ApplyDazzle(duration);
  }

  private Action<Tower> GetSlimeAction(float duration, float power) {
    return (tower) => tower.ApplySlime(duration, power);
  }

  private IEnumerator Spawn() {
    EnemyData.SpawnerProperties properties = data.spawner.Value;
    while (true) {
      yield return new WaitForSeconds(properties.interval);
      SpawnChildren(properties.childKey, properties.num);
    }
  }

  private void SpawnChildren(string childKey, int num) {
    for (int i = 0; i < num; i++) {
      Spawner.Instance.Spawn(childKey, NextWaypoint, transform);
    }
  }

  // This event should only be executed by the GameStateManager.
  private void OnEnemyStatUpdate() {
    TerrariumContextUI.Instance.SetContextForEnemy(this);
  }

  public void StartEnemyStatBroadcast() {
    GameStateManager.EnemyStatChanged += OnEnemyStatUpdate;
  }

  public void StopEnemyStatBroadcast() {
    GameStateManager.EnemyStatChanged -= OnEnemyStatUpdate;
  }

  public override string ToString() {
    return data.type + "\n"
        + "  Current HP: " + HP + "\n"
        + "  Current Armor: " + Armor + "\n"
        + data.ToString();
  }
}
