#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static UnityEngine.UI.Image;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash = new();
  [SerializeField] ParticleSystem beam = new();

  public bool AcidStun { get; private set; }
  public bool TearBonusDamage { get; private set; }
  public bool DotSlow { get; private set; }
  public bool DotExplosion { get; private set; }
  public bool ContinuousAttack { get; private set; }
  public float SlowPercentage { get; private set; }
  public float StunLength { get; private set; }
  public float AcidDPS { get; private set; }

  private Enemy? enemy;
  private Targeting targeting = new();

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = Targeting.Behavior.NONE,
      priority = Targeting.Priority.FIRST
    };

    // TODO: This should be read in from a data file, not hardcoded like this.
    attributes[TowerData.Stat.RANGE] = 100.0f;
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
        towerAbilities[TowerData.TowerAbility.CAMO_SIGHT] = true;
        break;
      case SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE:
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  void Update() {
    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: FindObjectsOfType<Enemy>(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: towerAbilities[TowerData.TowerAbility.CAMO_SIGHT],
      antiAir: towerAbilities[TowerData.TowerAbility.ANTI_AIR]);
    if (enemy == null) {
      // Turn off particle systems.
    } else {

      upperMesh.LookAt(enemy.transform);

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
