using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static Ability;
using static UnityEngine.GraphicsBuffer;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] ParticleSystem acidExplosion;
  [SerializeField] LineRenderer beam;

  // TODO: These should not be SerializeFields long-term. They exist for debugging purposes now.
  [SerializeField] public Targeting.Behavior behavior;
  [SerializeField] public Targeting.Priority priority;
  [SerializeField] float splashExplosionMultiplier = 1.0f;
  [SerializeField] float acidExplosionMultiplier = 1.0f;

  public bool ArmorTearStun { get; private set; } = false;
  public bool ArmorTearExplosion { get; private set; } = false;
  public bool ContinuousAttack { get; private set; } = false;
  public bool DotSlow { get; private set; } = false;
  public bool DotExplosion { get; private set; } = false;
  public float SplashExplosionRange {
    get { return attributes[TowerData.Stat.AREA_OF_EFFECT] * splashExplosionMultiplier; } }
  public float AcidExplosionRange {
    get { return attributes[TowerData.Stat.AREA_OF_EFFECT] * acidExplosionMultiplier; } }

  private Enemy enemy;
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
    //attributes[TowerData.Stat.RANGE] = 15.0f;
    //attributes[TowerData.Stat.ATTACK_SPEED] = 1.0f;
    //attributes[TowerData.Stat.PROJECTILE_SPEED] = 30.0f;
    //attributes[TowerData.Stat.DAMAGE] = 10.0f;
    //attributes[TowerData.Stat.DAMAGE_OVER_TIME] = 30.0f;
    //attributes[TowerData.Stat.ARMOR_TEAR] = 1.0f;
    //attributes[TowerData.Stat.STUN_TIME] = 1.0f;
    //attributes[TowerData.Stat.SLOW_DURATION] = 0.5f;
    //attributes[TowerData.Stat.SLOW_POWER] = 0.5f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();

    DisableAttackSystems();
    var coroutine = StartCoroutine(SplashShoot());

    // -----0-----

    // TODO: Remove this section, it is for practical testing only.

    //DotExplosion = true;
    //AcidStun = true;
    //DotSlow = true;

    //var splashEmission = splash.emission;
    //splashEmission.enabled = false;
    //ContinuousAttack = true;
  }

  public override void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {
    switch (ability) {
      case SpecialAbilityEnum.SA_1_3_ARMOR_TEAR_STUN:
        ArmorTearStun = true;
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
        CamoSight = true;
        break;
      case SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE:
        var splashEmission = splash.emission;
        splashEmission.enabled = false;
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  protected override void ProcessDamageAndEffects(Enemy target) {
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
    if (ApplyArmorTearAndCheckForArmorTearStun(target, armorTear)) {
      target.AddStunTime(attributes[TowerData.Stat.STUN_TIME]);
    }

    // Acid DoT effects.
    if (target.AddAcidStacks(acidStacks)) {
      HandleAcidEffects(target);
    }

    // Splash explosion handling, unnecessary if the tower is continuous attack.
    if (!ContinuousAttack) {
      HandleSplashEffects(target, onHitDamage);
    }

    target.DamageEnemy(onHitDamage, ArmorPierce);
  }

  private void HandleAcidEffects(Enemy target) {
    if (DotSlow && !target.data.spittingAntTowerSlows.Contains(this)) {
      target.ApplySlow(attributes[TowerData.Stat.SLOW_POWER], attributes[TowerData.Stat.SLOW_DURATION]);
      target.data.spittingAntTowerSlows.Add(this);
    }
    if (DotExplosion) {
      acidExplosion.transform.position = GetSafeChildPosition(target);
      acidExplosion.Play();

      float totalAcidDamage = target.MaxAcidStacks * target.AcidDamagePerStack;
      List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(target, AcidExplosionRange);
      Debug.Log(totalAcidDamage + " damage to be applied to " + enemiesInAoe.Count + " enemies");

      // Cause totalAcidDamage to all enemies in range (including target).
      foreach (Enemy enemy in enemiesInAoe) {
        enemy.DamageEnemy(totalAcidDamage, 0.0f);
      }
      // Target is excluded from enemiesInAoe, so make sure to cause the damage here.
      target.DamageEnemy(totalAcidDamage, 0.0f);

      target.ResetAcidStacks();
    }
  }

  private void HandleSplashEffects(Enemy target, float onHitDamage) {
    splashExplosion.transform.position = GetSafeChildPosition(target);
    splashExplosion.Play();

    // Get a list of enemies caught in the AoE that are not the enemy targeted.
    List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(target, SplashExplosionRange);

    foreach (Enemy enemy in enemiesInAoe) {
      enemy.DamageEnemy(onHitDamage, ArmorPierce);

      if (ArmorTearExplosion && ApplyArmorTearAndCheckForArmorTearStun(enemy, ArmorTear)) {
        enemy.AddStunTime(attributes[TowerData.Stat.STUN_TIME]);
      }
    }
  }

  private void Update() {
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

    // If there is no target, stop firing.
    if (enemy is null) {
      firing = false;
      beam.enabled = false;
    } else {
      upperMesh.LookAt(GetSafeChildPosition(enemy));
      firing = true;

      if (!ContinuousAttack) {
        GeneralAttackHandler(splash, enemy, ProjectileSpeed);
      } else {
        beam.enabled = true;
        beam.SetPosition(
            1,  // The destination of the system.
            enemy.transform.position - Vector3.up);  // Target a little below the top of the enemy position.

        ProcessDamageAndEffects(enemy);
      }
    }
  }

  // Handle the splash shot outside of the Update method, so it won't interrupt the program flow.
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
  private bool ApplyArmorTearAndCheckForArmorTearStun(Enemy enemy, float armorTear) {
    return 0.0f < enemy.Armor && enemy.TearArmor(armorTear) == 0.0f && ArmorTearStun;
  }

  // Fetch enemies in explosionRange of target. This excludes target itself.
  private List<Enemy> GetEnemiesInExplosionRange(Enemy target, float explosionRange) {
    return objectPool.GetActiveEnemies()
          .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < explosionRange)
          .Where(e => !e.Equals(target))
          .ToList();
  }

  // Disable the shooty systems.
  private void DisableAttackSystems() {
    var emissionModule = splash.emission;
    emissionModule.enabled = false;

    beam.enabled = false;
  }
}
