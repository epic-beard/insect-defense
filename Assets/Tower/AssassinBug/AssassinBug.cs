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
    if (enemy != null ) {
      StartCoroutine(Approach());
      attacking = true;
    }
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    throw new System.NotImplementedException();
  }

  IEnumerator Approach() {
    while (enemy != null) {
      Vector3 startPosition = assassinMesh.position;
      Vector3 endPosition = enemy.transform.position;
      assassinMesh.LookAt(endPosition);

      if (MoveTo(startPosition, endPosition)) {
        break;
      }
      yield return null;
    }
    // Deal damage
    attacking = false;
    returning = true;
    StartCoroutine(Return());
  }

  IEnumerator Return() {
    while (true) {
      if (MoveTo(assassinMesh.position, transform.position)) {
        break;
      }
      yield return null;
    }
    returning = false;
  }

  private bool MoveTo(Vector3 startPosition, Vector3 endPosition) {
    float delta = Time.deltaTime * AttackSpeed;
    float distance = (endPosition - startPosition).magnitude;
    if (distance < delta) {
      assassinMesh.transform.position = endPosition;
      return true;
    } else {
      assassinMesh.transform.position += delta * (endPosition - startPosition);
      return false;
    }
  }

  private void ProcessDamageAndEffects(Enemy target) {
  }
  }
