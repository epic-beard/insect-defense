#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static UnityEngine.UI.Image;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem beam;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] ParticleSystem acidExplosion;

  // TODO: These should not be SerializeFields long-term. They exist for debugging purposes now.
  [SerializeField] Targeting.Behavior behavior;
  [SerializeField] Targeting.Priority priority;

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
      behavior = this.behavior,
      priority = this.priority
    };

    // TODO: This should be read in from a data file, not hardcoded like this.
    attributes[TowerData.Stat.RANGE] = 15.0f;
    attributes[TowerData.Stat.ATTACK_SPEED] = 1.0f;

    DisableParticleSystems();
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
        // Turn off the splash emission when upgraded to continuous attack to avoid simultaneous firing.
        var splashEmission = splash.emission;
        splashEmission.enabled = false;
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  protected override void processParticleCollision() {
    // TODO:
    //  1. Kill the particle (try using remainingLifetime)
    //  2. Trigger follow-on effects.
    //    a. If the acid splash is the parent system, do the explosion and check for AoE damage targets.
  }

  void Update() {
    // TODO: Remove these two lines, they exist for debugging purposes at the moment.
    targeting.behavior = this.behavior;
    targeting.priority = this.priority;

    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: FindObjectsOfType<Enemy>(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: towerAbilities[TowerData.TowerAbility.CAMO_SIGHT],
      antiAir: towerAbilities[TowerData.TowerAbility.ANTI_AIR]);

    // Fetch the appropriate particle system emission module.
    var emissionModule = ContinuousAttack ? beam.emission : splash.emission;

    if (enemy == null) {
      // If there is no target, stop firing.
      emissionModule.enabled = false;
    } else {
      upperMesh.LookAt(enemy.transform.GetChild(0));
      emissionModule.enabled = true;
      // TODO:
      //  1. Get attack speed to be frame rate independent.
      //  2. Make sure the attack speed is appropriately different for Splash and Beam.
      GeneralAttackHandler(ContinuousAttack ? beam : splash, enemy);
    }
  }

  // TODO: Delete this. It exists for now as a guide for what else needs to be done.
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

  // Disable all particle systems.
  private void DisableParticleSystems() {
    var emissionModule = splash.emission;
    emissionModule.enabled = false;

    emissionModule = beam.emission;
    emissionModule.enabled = false;

    emissionModule = splashExplosion.emission;
    emissionModule.enabled = false;

    emissionModule = acidExplosion.emission;
    emissionModule.enabled = false;
  }
}
