#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static UnityEngine.UI.Image;

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

  private Enemy? enemy;
  private Targeting targeting;

  private void Start() {
    targeting = new();
  }

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
    enemy = targeting.FindTarget();
    if (enemy == null) {
      // Turn off particle systems.
    } else {
      if (ContinuousAttack) {
        // Turn on continuous attack.
      } else {
        // Turn on AoE attack.
      }
    }
  }

  private void OnParticleCollision(GameObject other) {
    float onHitDamage = Damage;
    float acidStacks = DamageOverTime;
    Enemy enemy = other.GetComponentInChildren<Enemy>();

    // Armor tear effects.
    if (enemy.TearArmor(ArmorTear) == 0.0f) {
      if (AcidStun) {
        // Stun the enemy.
      }
      if (TearBonusDamage) {
        onHitDamage *= ArmorTear;
        acidStacks *= ArmorTear;
      }
    }

    // DoT effects.
    if (enemy.AddAcidStacks(acidStacks)) {
      if (DotSlow) {
        // Apply a slow to the enemy unless the enemy is already slowed.
      }
      if (DotExplosion) {
        // Trigger an explosion.
        // Reset acid stacks to 0.
      }
    }

    enemy.DamageEnemy(onHitDamage);
  }
}
