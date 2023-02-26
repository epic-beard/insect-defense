using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;
using static UnityEngine.GraphicsBuffer;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] ParticleSystem acidExplosion;
  [SerializeField] LineRenderer laser;

  // TODO: These should not be SerializeFields long-term. They exist for debugging purposes now.
  [SerializeField] Targeting.Behavior behavior;
  [SerializeField] Targeting.Priority priority;
  [SerializeField] float splashExplosionRange = 1.0f;
  [SerializeField] float acidExplosionRange = 1.0f;

  public bool AcidStun { get; private set; } = false;
  public bool ArmorTearExplosion { get; private set; } = false;
  public bool ContinuousAttack { get; private set; } = false;
  public bool DotSlow { get; private set; } = false;
  public bool DotExplosion { get; private set; } = false;

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
    attributes[TowerData.Stat.RANGE] = 15.0f;
    attributes[TowerData.Stat.ATTACK_SPEED] = 1.0f;
    attributes[TowerData.Stat.PROJECTILE_SPEED] = 30.0f;
    attributes[TowerData.Stat.DAMAGE] = 10.0f;
    attributes[TowerData.Stat.DAMAGE_OVER_TIME] = 30.0f;
    attributes[TowerData.Stat.ARMOR_TEAR] = 1.0f;
    attributes[TowerData.Stat.STUN_TIME] = 1.0f;
    attributes[TowerData.Stat.SLOW_DURATION] = 0.5f;
    attributes[TowerData.Stat.SLOW_POWER] = 0.5f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();

    DisableAttackSystems();
    var coroutine = StartCoroutine(SplashShoot());

    // -----0-----

    DotExplosion = true;
    //AcidStun = true;
    DotSlow = true;

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
    if (ApplyArmorTearAndCheckForAcidStun(target, armorTear)) {
      target.AddStunTime(attributes[TowerData.Stat.STUN_TIME]);
    }

    // Acid DoT effects.
    if (target.AddAcidStacks(acidStacks)) {
      HandleAcidEffects(target);
    }

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
      List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(target, acidExplosionRange);

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
    List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(target, splashExplosionRange);

    foreach (Enemy enemy in enemiesInAoe) {
      enemy.DamageEnemy(onHitDamage, ArmorPierce);

      if (ArmorTearExplosion && ApplyArmorTearAndCheckForAcidStun(enemy, ArmorTear)) {
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

    if (enemy == null) {
      // If there is no target, stop firing.
      firing = false;
      laser.enabled = false;
    } else {
      upperMesh.LookAt(enemy.transform.GetChild(0));
      firing = true;

      if (!ContinuousAttack) {
        GeneralAttackHandler(splash, enemy, ProjectileSpeed);
      } else {
        laser.enabled = true;
        laser.SetPosition(
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

  // Get a safe position for the shots of the tower. Ideally, the actual mesh, but if that isn't present,
  // the enemy container itself.
  private Vector3 GetSafeChildPosition(Enemy enemy) {
    if (enemy.transform.childCount == 0) {
      return enemy.transform.position;
    }
    return enemy.transform.GetChild(0).position;
  }

  // Apply Armor tear to an enemy and simultaneously check to see if it should be stunned as a result of 
  // SA_1_3_ACID_STUN.
  private bool ApplyArmorTearAndCheckForAcidStun(Enemy enemy, float armorTear) {
    return enemy.Armor != 0.0f && enemy.TearArmor(armorTear) == 0.0f && AcidStun;
  }

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

    laser.enabled = false;
  }
}
