using Codice.Client.BaseCommands.CheckIn.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  public EnemyData data;
  public Waypoint PrevWaypoint;
  public Waypoint NextWaypoint;
  public ObjectPool pool;

  readonly private int acidStackMaxMultiplier = 25;
  readonly private float acidDamagePerStackPerSecond = 1;

  // PrevWaypoint should be set before OnEnable is called.
  void OnEnable() {
    transform.position = PrevWaypoint.transform.position;
    NextWaypoint = PrevWaypoint.GetNextWaypoint();

    if (NextWaypoint == null) {
      Debug.Log("[ERROR] NextWaypoint not found.");
      return;
    }

    StartCoroutine(HandleStun());
    StartCoroutine(FollowPath());
  }

  public float HP { get { return data.currHP; } }
  public float Armor { get { return data.currArmor; } }
  public float Speed { get { return data.speed * (1 - SlowPower); } }
  public bool Flying { get { return data.properties == EnemyData.Properties.FLYING; } }
  public bool Camo { get { return data.properties == EnemyData.Properties.CAMO; } }
  public float MaxAcidStacks { get { return (int)data.size * acidStackMaxMultiplier; } }
  public float AcidStacks { get { return data.acidStacks; } }
  public float AcidDamagePerStack { get { return acidDamagePerStackPerSecond; } }
  public float SlowPower { get { return data.slowPower; } }
  public float SlowDuration { get { return data.slowDuration; } }

  // Damage this enemy while taking armor piercing into account. This method is responsible for initiating death.
  // No other method should try to handle Enemy death.
  public float DamageEnemy(float damage, float armorPierce) {
    data.currHP -= damage - Mathf.Clamp(Armor - armorPierce, 0.0f, damage);
    if (data.currHP <= 0.0f) {
      // TODO: Award the player Nu
      pool.DestroyEnemy(gameObject);
    }
    return data.currHP;
  }

  // Return the new armor total after the tear is applied.
  public float TearArmor(float tear) {
    return data.currArmor = Mathf.Max(data.currArmor - tear, 0);
  }

  // Returns true if stacks are at max.
  public bool AddAcidStacks(float stacks) {
    data.acidStacks = Mathf.Min(data.acidStacks + stacks, MaxAcidStacks);
    return data.acidStacks == MaxAcidStacks;
  }

  public void ResetAcidStacks() {
    data.acidStacks = 0;
  }

  // Returns the amount of time this enemy is now stunned.
  public float AddStunTime(float stunTime) {
    return data.stunTime += stunTime;
  }

  // Slows work thusly:
  //     The most powerful slow applied to an enemy is the one actually used. The time is amortized with respect to
  //   the difference.
  //     For example, if an enemy is suffering a 10% slow with a duraiton of 10 seconds and then is hit with a
  //   20% slow with an additional duratin of 7 seconds, the new slow stats would be a 20% slow with a 12 second
  //   duration (20 = 10 * 2 for power so 10 / 2 = 5 for duration).
  public void ApplySlow(float slowPower, float slowDuration) {
    float newPower;
    float newDuration;
    float multiplier = SlowPower / slowPower;
    if (SlowPower < slowPower) {
      newPower = slowPower;
      newDuration = slowDuration + (SlowDuration * multiplier);
    } else {
      newPower = SlowPower;
      newDuration = SlowDuration + (slowDuration / multiplier);
    }
    data.slowPower = newPower;
    data.slowDuration = newDuration;
  }

  public float GetDistanceToEnd() {
    if (NextWaypoint == null) {
      return float.MaxValue;
    }

    return Vector3.Distance(transform.position, NextWaypoint.transform.position) + NextWaypoint.DistanceToEnd;
  }

  private void Update() {
    // TODO: Process slows.
    if (0.0f < SlowDuration) {
      data.slowDuration = Mathf.Max(data.slowDuration - Time.deltaTime, 0.0f);
      if (SlowDuration <= 0.0f) {
        data.slowPower = 0.0f;
      }
    }

    // Handle acid damage.
    if (0.0f < data.acidStacks) {
      float acidDamage = acidDamagePerStackPerSecond * Time.deltaTime;
      data.acidStacks = Mathf.Max(data.acidStacks - Time.deltaTime, 0.0f);
      data.currHP -= acidDamage;
    }
    
  }

  private IEnumerator HandleStun() {
    float originalSpeed = data.speed;
    while (true) {
      yield return new WaitUntil(() => 0.0f < data.stunTime);
      data.speed = 0.0f;
      float interimStunTime = data.stunTime;
      data.stunTime = 0.0f;
      yield return new WaitForSeconds(interimStunTime);
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

  private void FinishPath() {
    // [TODO nnewsom] handle player damage.
    pool.DestroyEnemy(gameObject);
  }
}
