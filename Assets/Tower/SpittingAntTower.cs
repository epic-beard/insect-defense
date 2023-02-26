using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;
using static TowerData;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] ParticleSystem acidExplosion;
  [SerializeField] LineRenderer beam;

  // TODO: These should not be SerializeFields long-term. They exist for debugging purposes now.
  [SerializeField] Targeting.Behavior behavior;
  [SerializeField] Targeting.Priority priority;
  [SerializeField] float splashExplosionRange = 1.0f;

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
  private ObjectPool objectPool;

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    // TODO: These should be read in from a data file, not hardcoded like this.
    attributes[TowerData.Stat.RANGE] = 15.0f;
    attributes[TowerData.Stat.ATTACK_SPEED] = 1.0f;
    attributes[TowerData.Stat.PROJECTILE_SPEED] = 30.0f;
    attributes[TowerData.Stat.DAMAGE] = 10.0f;
    attributes[TowerData.Stat.DAMAGE_OVER_TIME] = 5.0f;
    attributes[TowerData.Stat.ARMOR_TEAR] = 1.0f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();
    DisableSystems();
    
    var coroutine = StartCoroutine(SplashShoot());

    // -----0-----

    var splashEmission = splash.emission;
    splashEmission.enabled = false;
    ContinuousAttack = true;
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
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  protected override void processParticleCollision(Enemy target) {
    float onHitDamage = Damage;
    float acidStacks = DamageOverTime;
    float armorTear = ArmorTear;

    if (ContinuousAttack) {
      // Calculate continuous damage, armor tear, etc. for application below.
      // This explicitly ignores the potential for the Spitting Ant tower to ever have armor piercing.
      onHitDamage *= AttackSpeed * Time.deltaTime;
      acidStacks *= AttackSpeed * Time.deltaTime;
      armorTear *= AttackSpeed * Time.deltaTime;
    }

    // Armor tear effects.
    if (ApplyArmorTearAndCheckForAcidStun(target, armorTear)) {
      // TODO: Stun the enemy.
    }

    // DoT effects.
    if (target.AddAcidStacks(acidStacks)) {
      if (DotSlow) {
        // TODO: Apply a slow to the enemy unless the enemy is already slowed.
      }
      if (DotExplosion) {
        // TODO: Trigger an explosion.
        // TODO: Reset acid stacks to 0.
      }
    }

    if (!ContinuousAttack) {
      splashExplosion.transform.position = target.transform.GetChild(0).position;
      splashExplosion.Play();

      // Get a list of enemies caught in the AoE that are not the enemy targeted.
      List<Enemy> enemiesInAoe= objectPool.GetActiveEnemies()
          .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < splashExplosionRange)
          .Where(e => !e.Equals(target))
          .ToList();

      foreach (Enemy enemy in enemiesInAoe) {
        enemy.DamageEnemy(onHitDamage, ArmorPierce);

        if (ArmorTearExplosion && ApplyArmorTearAndCheckForAcidStun(enemy, ArmorTear)) {
          // TODO: Stun enemy.
        }
      }
    }

    target.DamageEnemy(onHitDamage, ArmorPierce);
  }

  void Update() {
    // TODO: Remove these two lines, they exist for debugging purposes at the moment.
    targeting.behavior = this.behavior;
    targeting.priority = this.priority;

    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: objectPool.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: towerAbilities[TowerData.TowerAbility.CAMO_SIGHT],
      antiAir: towerAbilities[TowerData.TowerAbility.ANTI_AIR]);

    if (enemy == null) {
      // If there is no target, stop firing.
      firing = false;
      beam.enabled = false;
    } else {
      upperMesh.LookAt(enemy.transform.GetChild(0));
      firing = true;

      if (!ContinuousAttack) {
        GeneralAttackHandler(splash, enemy, ProjectileSpeed);
      } else {
        beam.enabled = true;
        beam.SetPosition(
            1,  // The destination of the system.
            enemy.transform.position - Vector3.up);  // Target a little below the top of the enemy position.

        processParticleCollision(enemy);
      }
    }
  }

  private IEnumerator SplashShoot() {
    while (!ContinuousAttack) {
      while (firing) {
        splash.Emit(1);
        yield return new WaitForSeconds(1 / AttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }

  // Apply Armor tear to an enemy and simultaneously check to see if it should be stunned as a result of 
  // SA_1_3_ACID_STUN.
  private bool ApplyArmorTearAndCheckForAcidStun(Enemy enemy, float armorTear) {
    return enemy.Armor != 0.0f && enemy.TearArmor(armorTear) == 0.0f && AcidStun;
  }

  // Disable the shooty systems.
  private void DisableSystems() {
    var emissionModule = splash.emission;
    emissionModule.enabled = false;

    beam.enabled = false;
  }
}
