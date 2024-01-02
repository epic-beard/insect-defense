using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerAbility;

public class SpittingAntTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem splash;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] LineRenderer beam;

  [SerializeField] float splashExplosionMultiplier = 1.0f;

  public bool ArmorTearAcidBonus { get; private set; } = false;
  public bool ArmorTearExplosion { get; private set; } = false;
  public bool ContinuousAttack { get; private set; } = false;
  public bool AcidBuildupBonus { get; private set; } = false;
  public bool AcidEnhancement { get; private set; } = false;
  public float AcidDecayDelay { get; private set; } = 10.0f;
  public float SplashExplosionRange {
    get { return data[TowerData.Stat.AREA_OF_EFFECT] * splashExplosionMultiplier; }
  }
  public override TowerData.Type TowerType { get; set; } = TowerData.Type.SPITTING_ANT_TOWER;

  private Enemy enemy;
  private bool firing = false;
  private ProjectileHandler projectileHandler;

  private void Start() {
    projectileHandler = new(splash, ProjectileSpeed, hitRange);

    StartCoroutine(SplashShoot());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.SA_1_3_ARMOR_TEAR_ACID_BONUS:
        ArmorTearAcidBonus = true;
        break;
      case SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION:
        ArmorTearExplosion = true;
        break;
      case SpecialAbility.SA_2_3_ACID_BUILDUP_BONUS:
        AcidBuildupBonus = true;
        break;
      case SpecialAbility.SA_2_5_DOT_ENHANCEMENT:
        AcidEnhancement = true;
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

  private void ProcessDamageAndEffects(Enemy target) {
    float onHitDamage = Damage;
    float acidStacks = DamageOverTime;
    float armorTear = ArmorTear;
    float armorPierce = ArmorPierce;

    if (ContinuousAttack) {
      // Calculate continuous damage, armor tear, etc. for application below.
      onHitDamage *= EffectiveAttackSpeed * Time.deltaTime;
      acidStacks *= EffectiveAttackSpeed * Time.deltaTime;
      armorTear *= EffectiveAttackSpeed * Time.deltaTime;
      armorPierce *= EffectiveAttackSpeed * Time.deltaTime;
    }

    // Armor tear effects.
    if (ArmorTearAcidBonus && (target.AcidStackExplosionThreshold / 2) <= target.AcidStacks) {
      target.TearArmor(armorTear * 1.5f);
    } else {
      target.TearArmor(armorTear);
    }
    if (ArmorTearExplosion) {
      HandleArmorTearExplosion(target, armorTear);
    }

    // Acid DoT effects.
    if (AcidBuildupBonus && target.Armor <= 0.0f) {
      target.AddAcidStacks(acidStacks * 1.5f, AcidEnhancement);
    } else {
      target.AddAcidStacks(acidStacks, AcidEnhancement);
    }

    if (AcidEnhancement) {
      target.AddAdvancedAcidDecayDelay(this, AcidDecayDelay);
    }

    target.DamageEnemy(onHitDamage, armorPierce, ContinuousAttack);
  }

  private void HandleArmorTearExplosion(Enemy target, float armorTear) {
    splashExplosion.transform.position = target.AimPoint;
    splashExplosion.Play();

    // Get a list of enemies caught in the AoE that are not the enemy targeted.
    List<Enemy> enemiesInAoe = GetEnemiesInExplosionRange(
        ObjectPool.Instance.GetActiveEnemies(), target, SplashExplosionRange);

    foreach (Enemy enemy in enemiesInAoe) {
      // Any tower that has armor tear explosion, also has armor tear acid bonus.
      if ((enemy.AcidStackExplosionThreshold / 2) <= enemy.AcidStacks) {
        enemy.TearArmor(armorTear * 1.5f);
      } else {
        enemy.TearArmor(armorTear);
      }
    }
  }

  protected override void TowerUpdate() {
    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);
    // If there is no target, stop firing.
    if (enemy == null) {
      firing = false;
      beam.enabled = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      upperMesh.LookAt(enemy.AimPoint);
      firing = true;

      if (ContinuousAttack) {
        beam.enabled = true;
        beam.SetPosition(
            1,  // The end point of the line renderer.
            enemy.AimPoint - beam.transform.position);  // The place to aim.

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
      while (firing && DazzleTime == 0.0f) {
        splash.Emit(1);
        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }
}
