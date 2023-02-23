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

    StartCoroutine(FollowPath());
  }

  public float HP { get { return data.currHP; } }
  public float Armor { get { return data.currArmor; } }
  public float Speed { get { return data.speed; } }
  public bool Flying { get { return data.properties == EnemyData.Properties.FLYING; } }
  public bool Camo { get { return data.properties == EnemyData.Properties.CAMO; } }
  public float MaxAcidStacks { get { return (int)data.size * acidStackMaxMultiplier; } }

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

  // Returns the amount of time this enemy is now stunned.
  public float AddStunTime(float stunTime) {
    return data.stunTime += stunTime;
  }

  public float GetDistanceToEnd() {
    if (NextWaypoint == null) {
      return float.MaxValue;
    }

    return Vector3.Distance(transform.position, NextWaypoint.transform.position) + NextWaypoint.DistanceToEnd;
  }

  private void Update() {
    // TODO: Process slows.

    // Handle stun
    if (0.0f < data.stunTime) {
      data.stunTime = Mathf.Max(data.stunTime - Time.deltaTime, 0.0f);
      data.speed = 0.0f;
      // TODO: Disable special attacks and enemy animation.
    }

    // Handle acid damage.
    if (0.0f < data.acidStacks) {
      float acidDamage = acidDamagePerStackPerSecond * Time.deltaTime;
      data.acidStacks = Mathf.Max(data.acidStacks - Time.deltaTime, 0.0f);
      data.currHP -= acidDamage;
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
