using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBug : Tower {
  [SerializeField] Transform assassinMesh;

  public bool ArmoredEnemyBonus { get; private set; } = false;
  public bool ArmorDepletionBonus { get; private set; } = false;
  public bool AntiAirSpeedBonus { get; private set; } = false;
  public bool Bungy { get; private set; } = false;
  public bool MultiHitBonus { get; private set; } = false;
  public bool BetterMultiHit { get; private set; } = false;
  public bool CriticalMultiHit { get; private set; } = false;
  public override TowerData.Type TowerType { get; set; } = TowerData.Type.ASSASSIN_BUG_TOWER;

  private Enemy enemy;
  private bool returning;
  private bool attacking;
  private Vector3 startingPosition;
  private float attackRange = 3.0f;

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
    throw new System.NotImplementedException();
  }

  IEnumerator Approach() {
    while (enemy != null) {
      Vector3 endPosition = enemy.AimPoint;
      assassinMesh.LookAt(endPosition);

      if (MoveTo(endPosition)) {
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
    while (true) {
      if (MoveTo(startingPosition)) {
        break;
      }
      yield return null;
    }
    returning = false;
  }

  private bool MoveTo(Vector3 endPosition) {
    Vector3 startPosition = assassinMesh.position;
    float delta = Time.deltaTime * ProjectileSpeed;
    float distance = (endPosition - startPosition).magnitude;
    if (distance < attackRange) {
      return true;
    } else {
      assassinMesh.Translate(delta * (endPosition - startPosition).normalized);
      return false;
    }
  }

  private void ProcessDamageAndEffects(Enemy target) {
    Debug.Log("hit: " + target.HP);
    target.DamageEnemy(Damage, ArmorPierce);
    Debug.Log("after hit: " + target.HP);
  }
  }
