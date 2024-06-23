using System;
using System.Linq;
using Assets;
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
      OriginalSpeed = value.speed;
    }
  }

  // The Event to update the context panel in the Terrarium UI.
  public event Action<Enemy> StatChangedEvent;

  public Waypoint PrevWaypoint;
  public Waypoint NextWaypoint;

  public HashSet<Tower> spittingAntTowerSlows = new();
  public HashSet<Tower> webShootingTowerStuns = new();
  public HashSet<Tower> webShootingTowerPermSlow = new();
  // Map of SpittingAntTowers to the duration of their acid decay delay.
  public Dictionary<Tower, float> AdvancedAcidDecayDelay = new();

  public readonly float venomSpreadRange = 10.0f;

  private Animator animator;

  private Transform target;
  private float xVariance;
  private float zVariance;

  private float accumulatedAcidDamage = 0.0f;
  private float accumulatedPoisonDamage = 0.0f;
  private float accumulatedBleedDamage = 0.0f;
  private float accumulatedContinuousDamage = 0.0f;

  [SerializeField] private float continuousDamagePollingDelay = 1.0f;
  [SerializeField] private float statusDamagePollingDelay = 1.0f;

  public SortedDictionary<float, int> venomStacks = new();
  private float continuousDamageWeakenPower = 0.0f;

  #region Properties

  public float AcidStacks {
    get { return data.acidStacks; }
    private set { data.acidStacks = value; }
  }
  public float AcidExplosionStackModifier { get { return data.acidExplosionStackModifier; } }
  public float AcidStackExplosionThreshold {
    get {
      return data.acidExplosionStackModifier == 0
          ? EnemyData.SizeToAcidExplosionThreshold[data.size]
          : EnemyData.SizeToAcidExplosionThreshold[data.size] * data.acidExplosionStackModifier;
    }
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
  private float armor;
  public float Armor {
    get { return armor; }
    set {
      armor = value;
      StatChangedEvent?.Invoke(this);
    }
  }
  public bool BigTarget { get { return data.properties == EnemyData.Properties.BIG_TARGET; } }
  public float BleedStacks {
    get { return data.bleedStacks; }
    private set { data.bleedStacks = value; }
  }
  public bool Camo { get { return data.properties == EnemyData.Properties.CAMO; } }
  public float CamoTransparency { get; private set; } = 0.5f;
  public float Coagulation {
    get {
      return data.coagulationModifier == 0
          ? EnemyData.SizeToCoagulation[data.size]
          : EnemyData.SizeToCoagulation[data.size] * data.coagulationModifier;
    }
  }
  public bool Crippled { get; private set; }
  public bool CrippleImmunity {
    get { return (data.properties & EnemyData.Properties.CRIPPLE_IMMUNITY) != 0; }
    set {
      if (value) {
        data.properties |= EnemyData.Properties.CRIPPLE_IMMUNITY;
      } else {
        data.properties &= ~EnemyData.Properties.CRIPPLE_IMMUNITY;
      }
    }
  }
  public float CrippleSlow { get; private set; } = 0.8f;
  public float Damage {
    get { return data.damage; }
    set {
      data.damage = value;
      StatChangedEvent?.Invoke(this);
    }
  }
  public EnemyData.DazzleProperties? Dazzle {
    get { return data.dazzle; }
    set { data.dazzle = value; }
  }
  public bool Flying { get { return data.properties == EnemyData.Properties.FLYING; } }
  public float GroundedTime { get; private set; }
  private float hp;
  public float HP {
    get { return hp; }
    set {
      hp = value;
      StatChangedEvent?.Invoke(this);
      if (hp <= 0.0f) {
        if (data.carrier != null) {
          var carrier = data.carrier.Value;
          SpawnChildren(carrier.childKey, carrier.num);
        }
        DistributeVenomStacksIfNecessary();

        ConditionalContextReset();
        ObjectPool.Instance.DestroyEnemy(gameObject);
        GameStateManager.Instance.Nu += Mathf.RoundToInt(data.nu);
      }
    }
  }
  public int InfectionLevel { get { return data.infectionLevel; } }
  public float MaxArmor { get { return data.maxArmor; } }
  public float MaxHp { get { return data.maxHP; } }
  public float Nu {
    get { return data.nu; }
    set {
      data.nu = value;
      StatChangedEvent?.Invoke(this);
    }
  }
  public float OriginalSpeed { get; private set; }
  public Renderer[] Renderers { get; private set; }
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
    private set {
      data.slowPower = value;
      StatChangedEvent?.Invoke(this);
    }
  }
  public float Speed { get { return data.speed * (1 - SlowPower); } }
  public float StunTime {
    get { return data.stunTime; }
    private set { data.stunTime = value; }
  }
  public EnemyData.Type Type { get { return data.type; } }
  public int? WaveTag { get; set; }

  #endregion

  public void Initialize(Waypoint start) {
    PrevWaypoint = start;
    NextWaypoint = start.GetNextWaypoint();
    if (transform.childCount > 0) {
      target = transform.GetChild(0).Find("target");
    }
    animator = this.GetComponentInChildren<Animator>();
    Renderers = this.GetComponentsInChildren<Renderer>();

    StartCoroutine(HandleStun());
    StartCoroutine(HandleAdvancedAcidDecay());
    StartCoroutine(HandleContinuousDamage());
    StartCoroutine(HandleStatusDamage());
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

    // Handle camo
    if (Camo) {
      foreach (Renderer r in Renderers) {
        r.AllMaterialsToFadeMode();
        r.AllMaterialsToTransluscent(CamoTransparency);
      }
    }

    if (NextWaypoint == null) {
      Debug.Log("[ERROR] NextWaypoint not found.");
      return;
    }

    StartCoroutine(FollowPath());
  }

  private void Update() {
    // Handle slows.
    if (0.0f < SlowDuration) {
      SlowDuration = Mathf.Max(SlowDuration - Time.deltaTime, 0.0f);
      if (SlowDuration <= 0.0f) {
        SlowPower = 0.0f;
        UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
      }
    }

    // Handle acid damage.
    if (AcidStacks > 0.0f) {
      float damage = StacksToDamage(AcidStacks);
      float weakenFraction = TowerManager.Instance.ActiveTowerMap.Values.ToList()
          .Where<Tower>((Tower tower) => tower.Type == TowerData.Type.SPITTING_ANT_TOWER)
          .Select<Tower, SpittingAntTower>((Tower tower) => (SpittingAntTower)tower)
          .Where<SpittingAntTower>((SpittingAntTower tower) => {
            return tower.AcidicSynergy
              && Vector2.Distance(tower.transform.position.DropY(),
                                  transform.position.DropY())
                 < SpittingAntTower.VenomRange;
          })
          .Sum((SpittingAntTower tower) => tower.VenomPower);
      accumulatedAcidDamage += damage * (1 + weakenFraction);
      HP -= damage;
      AcidStacks -= Mathf.Max(0.0f, damage - (AdvancedAcidDecayDelay.Count * Time.deltaTime));
    }

    // Handle bleed damage.
    if (BleedStacks > 0.0f) {
      float damage = StacksToDamage(BleedStacks);
      accumulatedBleedDamage += damage;
      HP -= damage;
      BleedStacks = Mathf.Max(0.0f, BleedStacks - (Coagulation * Time.deltaTime));
    }
  }

  // Convert stacks of [DAMAGE TYPE] to actual damage.
  private float StacksToDamage(float stacks) {
    int tenStacks = (int)AcidStacks / 10;
    return (tenStacks + 1) * Time.deltaTime;
  }

  // Return the new armor total after the tear is applied.
  public float TearArmor(float armorTear) {
    Armor = Mathf.Max(Armor - armorTear, 0.0f);
    return Armor;
  }

  // Add acid stacks to this enemy, and react appropriately to the acid capstone.
  public void AddAcidStacks(float stacks, bool acidEnhancement) {
    AcidStacks += stacks;
    if (AcidStackExplosionThreshold <= AcidStacks) {
      // TODO(emonzon): Trigger explosion animation.
      float damage = acidEnhancement ? AcidStacks * 2.0f : AcidStacks;

      DealDamage(damage, DamageText.DamageType.ACID);
      AcidStacks = 0.0f;
    }
  }

  public void AddBleedStacks(float stacks) {
    BleedStacks += stacks;
  }

  public bool IsDoomedByBlood() {
    return HP <= TotalBleedDamage();
  }

  public float TotalBleedDamage() {
    float k = Mathf.Floor(BleedStacks / 10);
    return (5 * k * (k + 1) + (BleedStacks % 10) * (k + 1)) / Coagulation;
  }

  public void AddVenomStacks(float power, int stacks) {
    if (venomStacks.ContainsKey(power)) {
      venomStacks[power] += stacks;
    } else {
      venomStacks.Add(power, stacks);
    }
  }

  public float PopVenomStack() {
    if (venomStacks.Count == 0) return 0.0f;
    var stack = venomStacks.First();
    venomStacks[stack.Key]--;
    if (venomStacks[stack.Key] <= 0) venomStacks.Remove(stack.Key);
    return stack.Key;
  }

  public bool IsVenomStacksEmpty() {
    return venomStacks.Count == 0;
  }

  // Damage this enemy while taking armor piercing into account. This method is responsible for initiating death.
  // No other method should try to handle Enemy death.
  public float DealPhysicalDamage(float damage, float armorPierce, bool continuous = false) {
    float effectiveArmor = Mathf.Clamp(Armor - armorPierce, 0.0f, 100.0f);

    damage *= (100.0f - effectiveArmor) / 100.0f;

    if (continuous) {
      accumulatedContinuousDamage += damage * (1 + continuousDamageWeakenPower);
      HP -= damage;
    } else {
      DealDamage(damage * (1 + PopVenomStack()), DamageText.DamageType.PHYSICAL);
    }

    return HP;
  }

  // To apply physical damage call DealPhysicalDamage, which will call this.
  public void DealDamage(float damage, DamageText.DamageType type) {
    HP -= damage;
    ShowDamageText(damage, type);
  }

  private void ShowDamageText(float damage, DamageText.DamageType type) {
    if (PlayerState.Instance == null || !PlayerState.Instance.Settings.ShowDamageText) return;

    GameObject prefab = Resources.Load<GameObject>("UI/Screens/Terrarium/DamageText/DamageText");

    GameObject gameObject = Instantiate(prefab, this.transform.position + 6 * Vector3.up, this.transform.rotation);
    DamageText damageText = gameObject.GetComponent<DamageText>();
    damageText.DisplayDamage(Mathf.FloorToInt(damage), type);
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
    UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
    SlowDuration = newDuration;
  }

  // This permanently reduces the enemy's speed. Care should be taken with calling this method.
  // slow is expected to be 0.0 - 1.0 and is used as a multiplier.
  public void ApplyPermanentSlow(float slow) {
    data.speed *= slow;
    UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
  }

  public void ApplyCripple() {
    if (!CrippleImmunity && !Crippled) {
      // [TODO] nnewsom: handle removing the leg, or downing the flyer here.
      data.speed *= CrippleSlow;
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

  public void AddAdvancedAcidDecayDelay(SpittingAntTower tower, float delay) {
    if (AdvancedAcidDecayDelay.ContainsKey(tower)) {
      AdvancedAcidDecayDelay[tower] = delay;
    } else {
      AdvancedAcidDecayDelay.Add(tower, delay);
    }
  }

  public void SetVariance(float x, float z) {
    xVariance = x;
    zVariance = z;
  }

  public void SetStat(EnemyData.Stat stat, float value) {
    switch (stat) {
      case EnemyData.Stat.MAX_HP: data.maxHP = value; break;
      case EnemyData.Stat.MAX_ARMOR: data.maxArmor = value; break;
      case EnemyData.Stat.SPEED: data.speed = value; break;
      case EnemyData.Stat.DAMAGE: data.damage = value; break;
      case EnemyData.Stat.NU: data.nu = value; break;
      case EnemyData.Stat.COAGULATION_MODIFIER: data.coagulationModifier = value; break;
      case EnemyData.Stat.ACID_EXPLOSION_STACK_MODIFIER: data.acidExplosionStackModifier = value; break;
    }
  }

  public void SetCarrier(EnemyData.CarrierProperties carrier) {
    data.carrier = carrier;
  }

  public void SetSpawner(EnemyData.SpawnerProperties spawner) {
    data.spawner = spawner;
  }

  public void SetProperties(EnemyData.Properties properties) {
    data.properties = properties;
  }

  public void SetDazzle(EnemyData.DazzleProperties dazzle) {
    data.dazzle = dazzle;
  }

  public void SetSlime(EnemyData.SlimeProperties slime) {
    data.slime = slime;
  }

  private float GetAnimationSpeedMultiplier() {
    return Speed / OriginalSpeed;
  }

  private void UpdateAnimationSpeed(float newSpeed) {
    if (animator != null) {
      animator.SetFloat("Speed", newSpeed);
    }
  }

  private void ResetTransientState() {
    StatChangedEvent = delegate{ };
    spittingAntTowerSlows.Clear();
    webShootingTowerStuns.Clear();
    webShootingTowerPermSlow.Clear();
    AdvancedAcidDecayDelay.Clear();
    accumulatedAcidDamage = 0;
    accumulatedBleedDamage = 0;
    accumulatedContinuousDamage = 0;
    accumulatedPoisonDamage = 0;
    venomStacks.Clear();
    Crippled = false;
    GroundedTime = 0;
    WaveTag = null;
  }

  // Handle ensuring that the advanced acid decay delay decays at the appropriate pace.
  private IEnumerator HandleAdvancedAcidDecay() {
    while (true) {
      if (AdvancedAcidDecayDelay.Count > 0) {
        HashSet<Tower> towersToBeDeleted = new();
        foreach (var tower in AdvancedAcidDecayDelay.Keys) {
          // Reduce the timer.
          AdvancedAcidDecayDelay[tower] -= Time.deltaTime;
          if (AdvancedAcidDecayDelay[tower] <= 0.0f) {
            towersToBeDeleted.Add(tower);
          }
        }
        // Remove those towers that are no longer affecting this enemy from the mapping.
        foreach (Tower tower in towersToBeDeleted) {
          AdvancedAcidDecayDelay.Remove(tower);
        }
      }
      yield return null;
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
      UpdateAnimationSpeed(0.0f);
      float interimStunTime = StunTime;
      yield return new WaitForSeconds(interimStunTime);
      StunTime -= interimStunTime;
      data.speed = originalSpeed;
      UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
    }
  }

  private IEnumerator FollowPath() {
    while (NextWaypoint != null) {
      Vector3 startPosition = transform.position;
      Vector3 endPosition = NextWaypoint.transform.position;
      endPosition += new Vector3(xVariance, 0, zVariance);
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

  private IEnumerator HandleContinuousDamage() {
    while (true) {
      if (accumulatedContinuousDamage > 1.0f) {

        ShowDamageText(accumulatedContinuousDamage, DamageText.DamageType.PHYSICAL);
        accumulatedContinuousDamage = 0.0f;
      }
      continuousDamageWeakenPower = PopVenomStack();

      yield return new WaitForSeconds(continuousDamagePollingDelay);
    }
  }

  private IEnumerator HandleStatusDamage() {
    while (true) {
      if (accumulatedAcidDamage > 1.0f) {
        ShowDamageText(accumulatedAcidDamage, DamageText.DamageType.ACID);
        accumulatedAcidDamage = 0.0f;
      }
      yield return new WaitForSeconds(statusDamagePollingDelay / 3);

      if (accumulatedPoisonDamage > 1.0f) {
        ShowDamageText(accumulatedPoisonDamage, DamageText.DamageType.POISON);
        accumulatedPoisonDamage = 0.0f;
      }
      yield return new WaitForSeconds(statusDamagePollingDelay / 3);

      if (accumulatedBleedDamage > 1.0f) {
        ShowDamageText(accumulatedBleedDamage, DamageText.DamageType.BLEED);
        accumulatedBleedDamage = 0.0f;
      }
      yield return new WaitForSeconds(statusDamagePollingDelay / 3);
    }
  }

  // Reset the contextual panel if an enemy dies or completes its path.
  private void ConditionalContextReset() {
    if (MouseManager.SelectedEnemy == this) {
      EnemyDetail.Instance.DesbuscribeToEnemyStateBroadcast(this);
      ContextPanel.Instance.SetNoContextPanel();
    }
  }

  private void FinishPath() {
    ConditionalContextReset();
    GameStateManager.Instance.DealDamage(Mathf.RoundToInt(Damage));
    ObjectPool.Instance.DestroyEnemy(gameObject);
  }

  // On enemy death, check to see if there are venom stacks to be distributed.
  private void DistributeVenomStacksIfNecessary() {
    if (venomStacks.Count > 0) {
      int venomSpreaders = TowerManager.Instance.ActiveTowerMap.Values.ToList()
        .Where<Tower>((Tower tower) => tower.Type == TowerData.Type.SPITTING_ANT_TOWER)
        .Select<Tower, SpittingAntTower>((Tower tower) => (SpittingAntTower)tower)
        .Where<SpittingAntTower>((SpittingAntTower tower) => {
          return tower.VenomCorpseplosion
            && Vector2.Distance(tower.transform.position.DropY(),
                                transform.position.DropY())
               < SpittingAntTower.VenomRange;
        })
        .ToList().Count;

      if (venomSpreaders > 0) {
        var enemiesInRange = Targeting.GetAllValidEnemiesInRange(
            enemies: ObjectPool.Instance.GetActiveEnemies(),
            origin: transform.position,
            range: venomSpreadRange,
            camoSight: true,
            antiAir: true);

        // ei is the index of enemiesInRange, the advance statement of the for loop
        // should produce a counting that goes up to but does not exceed meaningful
        // elements of enemiesInRange.
        for (int ei = 0; venomStacks.Count > 0; ei = (ei + 1) % enemiesInRange.Count) {
          enemiesInRange[ei].AddVenomStacks(PopVenomStack(), 1);
        }
      }
    }
  }

  private IEnumerator HandleAbility(Action<Tower> ability, float interval, float range) {
    // Wait for a random time in (0, interval)
    // before using the ability for the first time;
    yield return new WaitForSeconds(UnityEngine.Random.Range(0, interval));
    while (true) {
      var towers =
          TowerManager.Instance.GetTowersInRange(range, transform.position);
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
      Spawner.Instance.Spawn(childKey, NextWaypoint, null, transform);
    }
  }

  public override string ToString() {
    return data.type + "\n"
        + "  Current HP: " + HP + "\n"
        + "  Current Armor: " + Armor + "\n"
        + data.ToString();
  }
}
