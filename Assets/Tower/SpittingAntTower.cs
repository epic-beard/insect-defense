using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class SpittingAntTower : Tower {
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem beam;

  public bool AcidStun { get; private set; }
  public bool TearBonusDamage { get; private set; }
  public bool DotSlow { get; private set; }
  public bool DotExplosion { get; private set; }
  public bool ContinuousAttack { get; private set; }
  public float SlowPercentage { get; private set; }
  public float StunLength { get; private set; }
  public float AcidDPS { get; private set; }

  public override void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {
    switch (ability) {
      case SpecialAbilityEnum.SA_1_3_ACID_STUN:
        AcidStun = true;
        break;
      case SpecialAbilityEnum.SA_1_5_TOTAL_TEAR_DAMAGE:
        TearBonusDamage = true;
        break;
      case SpecialAbilityEnum.SA_2_3_DOT_SLOW:
        DotSlow = true;
        break;
      case SpecialAbilityEnum.SA_2_5_DOT_EXPLOSION:
        DotExplosion = true;
        break;
      case SpecialAbilityEnum.SA_3_3_CAMO_SIGHT:
        towerAbilities.Add(TowerData.TowerAbility.CAMO_SIGHT);
        break;
      case SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE:
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  void Update() {
    // Getting the enemy this way is a short term measure.
    Targeting targeting = new();
    Enemy? enemy = targeting.findTarget();

    if (ContinuousAttack) {
      // Turn on continuous attack.
    } else {
      // Turn on AoE attack.
    }
  }

  public float getOnHitDamage() {
    return attributes[TowerData.Stat.DAMAGE];
  }

  public float getDamageOverTime() {
    return attributes[TowerData.Stat.DAMAGE_OVER_TIME];
  }

  public float getArmorTear() {
    return attributes[TowerData.Stat.ARMOR_TEAR];
  }
}
