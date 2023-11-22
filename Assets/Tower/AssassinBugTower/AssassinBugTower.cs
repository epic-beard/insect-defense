using System.Collections;
using System;
using UnityEngine;
using static TowerAbility;
using Codice.CM.WorkspaceServer.DataStore;

public class AssassinBugTower : Tower {
  [SerializeField] Transform assassinMesh;

  public bool ArmoredEnemyBonus { get; private set; } = false;
  public bool ArmorDepletionBonus { get; private set; } = false;
  public bool Bungy { get; private set; } = false;
  public bool MultiHitBonus { get; private set; } = false;
  public bool CriticalMultiHit { get; private set; } = false;
  public override TowerData.Type TowerType { get; set; } = TowerData.Type.ASSASSIN_BUG_TOWER;

  private Enemy enemy;
  private Enemy oldEnemy;
  private bool returning;
  private bool attacking;
  private Vector3 startingPosition;
  private float attackRange = 3.0f;
  private int multiHit = 0;

  private void Start() {
    startingPosition = assassinMesh.position;
  }

  protected override void TowerUpdate() {
    // If we are in the process of attacking or returning do nothing.
    if (returning || attacking) return;
    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);
    if (enemy != null) {
      StartCoroutine(Approach());
      attacking = true;
    }
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.AB_1_3_ARMORED_ENEMY_BONUS:
        ArmoredEnemyBonus = true; break;
      case SpecialAbility.AB_1_5_ARMOR_DEPLETION_BONUS:
        ArmorDepletionBonus = true; break;
      case SpecialAbility.AB_2_3_ANTI_AIR:
        AntiAir = true; break;
      case SpecialAbility.AB_2_4_CAMO_SIGHT:
        CamoSight = true; break;
      case SpecialAbility.AB_2_5_ELASTIC_SPINERETTES:
        Bungy = true; break;
      case SpecialAbility.AB_3_3_CONSECUTIVE_HITS:
        MultiHitBonus = true; break;
      case SpecialAbility.AB_3_5_COMBO_FINISHER:
        CriticalMultiHit = true; break;
      default:
        break;
    }
  }

  IEnumerator Approach() {
    while (enemy != null) {
      Vector3 endPosition = enemy.AimPoint;
      assassinMesh.LookAt(endPosition);

      if (MoveTo(endPosition, ProjectileSpeed)) {
        break;
      }
      yield return null;
    }
    ProcessDamageAndEffects(enemy);
    attacking = false;
    returning = true;
    StartCoroutine(Return());
  }

  IEnumerator Return() {
    float distance = (startingPosition - assassinMesh.position).magnitude;
    while (true) {
      float speed =
        (Bungy && (startingPosition - assassinMesh.position).magnitude > 0.5 * distance)
        ? ProjectileSpeed * 4 : ProjectileSpeed;
      if (MoveTo(startingPosition, speed)) {
        break;
      }
      yield return null;
    }
    returning = false;
  }

  // Moves a frame dependent distance, based on speed, in the direction of
  // endPosition, stopping when in attack range.
  // Returns true if in attack range.
  private bool MoveTo(Vector3 endPosition, float speed) {
    Vector3 startPosition = assassinMesh.position;
    float delta = Time.deltaTime * speed;
    float distance = (endPosition - startPosition).magnitude;
    Vector3 direction = (endPosition - startPosition).normalized;

    // If moving delta brings us within attack range, stop at the attack range.
    if (distance - delta < attackRange) {
      float travel = Math.Max(0, distance - attackRange);
      assassinMesh.Translate(travel * direction);
      return true;
    } else {
      assassinMesh.Translate(delta * direction);
      return false;
    }
  }

  private float ProcessDamageAndEffects(Enemy target) {
    if (target != oldEnemy) multiHit = 0;
    float damage = Damage;
    if (ArmoredEnemyBonus && target.Armor > 0) damage *= 2;
    if (ArmorDepletionBonus && target.MaxArmor > 0 && target.Armor == 0) damage *= 2;
    if (MultiHitBonus) damage *= (1 + 0.2f * multiHit);
    if (CriticalMultiHit && multiHit == 5) damage *= 2;
    target.DamageEnemy(damage, ArmorPierce);
    if (MultiHitBonus) {
      multiHit++;
      multiHit = Math.Min(5, multiHit);
    }
    oldEnemy = target;
    return damage;
  }
  }
