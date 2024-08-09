using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerAbility;

public class SpittingAntTower : Tower {
  public readonly static int VenomRange = 30;

  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem projectile;
  [SerializeField] ParticleSystem splashExplosion;
  [SerializeField] LineRenderer beam;

  [SerializeField] float splashExplosionMultiplier = 1.0f;

  public bool AcidicSynergy { get; private set; } = false;
  public bool VenomCorpseplosion { get; private set; } = false;
  public bool ContinuousAttack { get; private set; } = false;
  public bool AoEAcid { get; private set; } = false;
  public bool AcidEnhancement { get; private set; } = false;
  public float AcidDecayDelay { get; private set; } = 10.0f;
  public float SplashExplosionRange {
    get { return data.area_of_effect * splashExplosionMultiplier; }
  }
  public override TowerData.Type Type { get; set; } = TowerData.Type.SPITTING_ANT_TOWER;

  private ProjectileHandler projectileHandler;
  private bool canFire {
    get {
      return Target != null && Target.isActiveAndEnabled;
    }
  }

  protected override void TowerStart() {
    projectileHandler = new(projectile, ProjectileSpeed, hitRange);

    StartCoroutine(ProjectileFiringHandler());
    StartCoroutine(ContinuousFireVenomStackHandler());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.SA_1_3_ACIDIC_SYNERGY:
        AcidicSynergy = true;
        break;
      case SpecialAbility.SA_1_5_VENOM_CORPSEPLOSION:
        VenomCorpseplosion = true;
        break;
      case SpecialAbility.SA_2_3_AOE_ACID:
        AoEAcid = true;
        break;
      case SpecialAbility.SA_2_5_DOT_ENHANCEMENT:
        AcidEnhancement = true;
        break;
      case SpecialAbility.SA_3_3_ANTI_AIR:
        AntiAir = true;
        break;
      case SpecialAbility.SA_3_5_CONSTANT_FIRE:
        var splashEmission = projectile.emission;
        splashEmission.enabled = false;
        ContinuousAttack = true;
        break;
      default:
        break;
    }
  }

  private void ProcessDamageAndEffects(Enemy target) {
    float onHitDamage = Damage;
    float acidStacks = AcidStacks;

    if (ContinuousAttack) {
      // Calculate continuous damage, armor tear, etc. for application below.
      onHitDamage *= EffectiveAttackSpeed * Time.deltaTime;
      acidStacks *= EffectiveAttackSpeed * Time.deltaTime;
    }

    // Acid DoT effects.
    if (AoEAcid) {
      var enemiesForAcid = Targeting.GetAllValidEnemiesInRange(
          enemies: ObjectPool.Instance.GetActiveEnemies(),
          origin: target.AimPoint,
          range: AreaOfEffect,
          camoSight: true,
          antiAir: AntiAir);
      foreach (Enemy enemy in enemiesForAcid) {
        if (enemy == target) continue;
        enemy.AddAcidStacks(AcidStacks / 2, AcidEnhancement);
      }
      splashExplosion.transform.position = target.transform.position;
      splashExplosion.Play();
    }
    target.AddAcidStacks(acidStacks, AcidEnhancement);

    if (AcidEnhancement) {
      target.AddAdvancedAcidDecayDelay(this, AcidDecayDelay);
    }

    target.DealPhysicalDamage(onHitDamage, ArmorPierce, ContinuousAttack);

    // Venom effects.
    if (VenomStacks > 0) {
      target.AddVenomStacks(VenomPower, VenomStacks);
    }
  }

  protected override void TowerUpdate() {
    if (!ContinuousAttack) {
      projectileHandler.UpdateParticles(Target, ProcessDamageAndEffects);
    }

    Target = targeting.FindTarget(
        oldTarget: Target,
        enemies: ObjectPool.Instance.GetActiveEnemies(),
        towerPosition: transform.position,
        towerRange: Range,
        camoSight: CamoSight,
        antiAir: AntiAir);
    // If there is no target, stop firing.
    if (Target == null) {
      beam.enabled = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      upperMesh.LookAt(Target.AimPoint);

      if (ContinuousAttack) {
        beam.enabled = true;
        beam.SetPosition(
            1,  // The end point of the line renderer.
            Target.AimPoint - beam.transform.position);  // The place to aim.

        ProcessDamageAndEffects(Target);
      }
    }
  }

  protected override void UpdateAnimationSpeed(float newAttackSpeed) { }

  // Handle firing the projectile out of the way of the main program path. This should be robust
  // enough to handle update selling without restarting the coroutine.
  private IEnumerator ProjectileFiringHandler() {
    while (true) {
      yield return new WaitUntil(() => !ContinuousAttack);
      while (!ContinuousAttack) {
        while (canFire && !IsDazzled()) {
          projectile.Emit(1);
          yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
        }
        yield return new WaitUntil(() => canFire);
      }
    }
  }

  // Handle the application of venom stacks during continuous fire. This should be robust enough
  // to handle update selling without restarting the coroutine.
  private IEnumerator ContinuousFireVenomStackHandler() {
    while (true) {
      yield return new WaitUntil(() => ContinuousAttack);
      while (ContinuousAttack) {
        Enemy target = Target;
        float time = Time.time;
        yield return new WaitUntil(() => target != Target || Time.time - time > 1);
      }
    }
  }
}
