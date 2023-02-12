#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static TowerData;
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
  [SerializeField] float beamAttackSpeedModifier = 10.0f;

  public bool AcidStun { get; private set; } = false;
  public bool ArmorTearExplosion { get; private set; }
  public bool ContinuousAttack { get; private set; } = false;
  public bool DotSlow { get; private set; }
  public bool DotExplosion { get; private set; }
  public float SlowPercentage { get; private set; }
  public float StunLength { get; private set; }
  public float AcidDPS { get; private set; }

  private Enemy? enemy;
  private Targeting targeting = new();
  private bool firing = false;
  private ParticleSystem activeParticleSystem;

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    // TODO: This should be read in from a data file, not hardcoded like this.
    attributes[TowerData.Stat.RANGE] = 15.0f;
    attributes[TowerData.Stat.ATTACK_SPEED] = 1.0f;
    attributes[TowerData.Stat.PROJECTILE_SPEED] = 30.0f;

    activeParticleSystem = splash;
    DisableParticleSystems();
  }

  public override void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {
    switch (ability) {
      case SpecialAbilityEnum.SA_1_3_ACID_STUN:
        AcidStun = true;
        break;
      case SpecialAbilityEnum.SA_1_5_ARMOR_TEAR_EXPLOSION:
        ArmorTearExplosion = true;
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
        // Ensure the splash particle system is not emitting particles.
        var splashEmission = splash.emission;
        splashEmission.enabled = false;
        activeParticleSystem = beam;
        AttackSpeed *= beamAttackSpeedModifier;
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  protected override void processParticleCollision(Enemy target) {
    float onHitDamage = Damage;
    float acidStacks = DamageOverTime;

    // Armor tear effects.
    if (target.TearArmor(ArmorTear) == 0.0f) {
      if (AcidStun) {
        // Stun the enemy.
      }
    }

    // DoT effects.
    if (target.AddAcidStacks(acidStacks)) {
      if (DotSlow) {
        // Apply a slow to the enemy unless the enemy is already slowed.
      }
      if (DotExplosion) {
        // Trigger an explosion.
        // Reset acid stacks to 0.
      }
    }

    if (!ContinuousAttack) {
      splashExplosion.Play();
      // check for armortearexplosion and act accordingly.
    }

    target.DamageEnemy(onHitDamage, ArmorPierce);
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

    if (enemy == null) {
      // If there is no target, stop firing.
      firing = false;
    } else {
      upperMesh.LookAt(enemy.transform.GetChild(0));

      if (!firing) {
        firing = true;
        StartCoroutine(Shoot());
      }

      GeneralAttackHandler(activeParticleSystem, enemy, ProjectileSpeed);
    }
  }

  private IEnumerator Shoot() {
    while (firing) {
      activeParticleSystem.Emit(1);
      yield return new WaitForSeconds(1 / AttackSpeed);
    }
  }

  // Disable all particle systems.
  private void DisableParticleSystems() {
    var emissionModule = splash.emission;
    emissionModule.enabled = false;

    emissionModule = beam.emission;
    emissionModule.enabled = false;
  }
}
