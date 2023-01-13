using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class SpittingAntTower : Tower {
  bool acidStun = false;
  bool tearBonusDamage = false;
  bool dotSlow = false;
  bool dotExplosion = false;
  bool continuousAttack = false;
  float slowPercentage = 10.0f;
  float stunLength = 0.5f;
  float acidDPS = 2.0f;
  float continuousMult = 5.0f;

  public override void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {
    switch (ability) {
      case SpecialAbilityEnum.SA_1_3_ACID_STUN:
        acidStun = true;
        break;
      case SpecialAbilityEnum.SA_1_5_TOTAL_TEAR_DAMAGE:
        tearBonusDamage = true;
        break;
      case SpecialAbilityEnum.SA_2_3_DOT_SLOW:
        dotSlow = true;
        break;
      case SpecialAbilityEnum.SA_2_5_DOT_EXPLOSION:
        dotExplosion = true;
        break;
      case SpecialAbilityEnum.SA_3_3_CAMO_SIGHT:
        towerAbilities.Add(TowerData.TowerAbility.CAMO_SIGHT);
        break;
      case SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE:
        continuousAttack = true;
        break;
      default:
        break;
    }
  }

  void Update() {
    // Getting the enemy this way is a short term measure.
    Targeting targeting = new();
    Enemy enemy = targeting.findTarget();

    float damage = attributes[TowerData.Stat.DAMAGE];
    if (tearBonusDamage && enemy.GetArmor() == 0.0f) {
      damage *= attributes[TowerData.Stat.ARMOR_TEAR];
    }

    if (continuousAttack) {
      // The continuous attack should tic damage much more frequently than the normal attack and do the same damage per
      // tic.
    } else {
      // Trigger the expected particle system and set the damage.
    }

    // Status conditions
    if (acidStun && enemy.GetArmor() == 0.0f) {
      // Stun the enemy.
    }
    if (dotSlow && enemy.IsAcidDOTMax()) {
      // Slow the enemy.
    }

    if (dotExplosion && enemy.IsAcidDOTMax()) {
      // TODO:
      //  - Trigger a particle explosion.
      //  - Use acid stacks and acidDPS to calculate and set damage.
      //  - Clear acid stacks on the enemy.
    }
  }
}
