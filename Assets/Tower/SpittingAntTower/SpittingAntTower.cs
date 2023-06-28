using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerAbility;

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
    get { return data[TowerData.Stat.AREA_OF_EFFECT] * splashExplosionMultiplier; }
  }
  public float AcidExplosionRange {
    get { return data[TowerData.Stat.AREA_OF_EFFECT] * acidExplosionMultiplier; }
  }
  public override TowerData.Type TowerType { get; set; } = TowerData.Type.SPITTING_ANT_TOWER;

  private Enemy enemy;
  private bool firing = false;
  private ProjectileHandler projectileHandler;
  protected ObjectPool objectPool;

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    Range = 20.0f;
    ProjectileSpeed = 20.0f;
    AttackSpeed = 1.0f;
    Damage = 5.0f;
    AreaOfEffect = 10.0f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();
    projectileHandler = new(splash, ProjectileSpeed, hitRange);

    StartCoroutine(SplashShoot());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.SA_1_3_ARMOR_TEAR_STUN:
        ArmorTearStun = true;
        break;
      case SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION:
        ArmorTearExplosion = true;
        break;
      case SpecialAbility.SA_2_3_DOT_SLOW:
        DotSlow = true;
        break;
      case SpecialAbility.SA_2_5_DOT_EXPLOSION:
        DotExplosion = true;
        break;
      case SpecialAbility.SA_3_3_ANTI_AIR:
        AntiAir = true;
        break;
      case SpecialAbility.SA_3_5_CONSTANT_FIRE:
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
      target.AddStunTime(data[TowerData.Stat.STUN_TIME]);
    }

    // Acid DoT effects.
    if (target.AddAcidStacks(acidStacks)) {
      HandleMaxAcidStackEffects(target);
    }

    // Splash explosion handling, unnecessary if the tower is continuous attack.
    if (!ContinuousAttack) {
      HandleSplashEffects(target, onHitDamage);
    }

    target.DamageEnemy(onHitDamage, ArmorPierce);
  }

  // This is only called when the target's acid stacks are at max.
  private void HandleMaxAcidStackEffects(Enemy target) {
    if (DotSlow && !target.spittingAntTowerSlows.Contains(this)) {
      target.ApplySlow(data[TowerData.Stat.SLOW_POWER], data[TowerData.Stat.SLOW_DURATION]);
      target.spittingAntTowerSlows.Add(this);
    }
    if (DotExplosion) {
      acidExplosion.transform.position = projectileHandler.GetSafeChildPosition(target.transform);
      acidExplosion.Play();

      float totalAcidDamage = target.MaxAcidStacks * target.AcidDamagePerStackPerSecond;
      List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(objectPool.GetActiveEnemies(), target, AcidExplosionRange);

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
    splashExplosion.transform.position = projectileHandler.GetSafeChildPosition(target.transform);
    splashExplosion.Play();

    // Get a list of enemies caught in the AoE that are not the enemy targeted.
    List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(objectPool.GetActiveEnemies(), target, SplashExplosionRange);

    foreach (Enemy enemy in enemiesInAoe) {
      enemy.DamageEnemy(onHitDamage, ArmorPierce);

      if (ArmorTearExplosion && ApplyArmorTearAndCheckForArmorTearStun(enemy, ArmorTear)) {
        enemy.AddStunTime(data[TowerData.Stat.STUN_TIME]);
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
      camoSight: CamoSight,
      antiAir: AntiAir);

    // If there is no target, stop firing.
    if (enemy is null) {
      firing = false;
      beam.enabled = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      upperMesh.LookAt(projectileHandler.GetSafeChildPosition(enemy.transform));
      firing = true;

      if (ContinuousAttack) {
        beam.enabled = true;
        beam.SetPosition(
            1,  // The destination of the system.
            enemy.transform.position - Vector3.up);  // Target a little below the top of the enemy position.

        ProcessDamageAndEffects(enemy);
      }
    }

    if (!ContinuousAttack) {
      projectileHandler.UpdateParticles(enemy, ProcessDamageAndEffects);
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
  // SA_1_3_ARMOR_TEAR_STUN.
  private bool ApplyArmorTearAndCheckForArmorTearStun(Enemy enemy, float armorTear) {
    return 0.0f < enemy.Armor && enemy.TearArmor(armorTear) == 0.0f && ArmorTearStun;
  }
}
