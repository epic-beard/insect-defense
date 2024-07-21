using System.Collections;
using System.Linq;
using UnityEngine;
using static TowerAbility;

public class WebShootingSpiderTower : Tower {
  [SerializeField] Transform mesh;
  [SerializeField] ParticleSystem primaryWebShot;
  [SerializeField] ParticleSystem secondaryWebShot;

  public int lingeringWebNumUses = 3;

  public bool SlowStun { get; private set; } = false;
  public bool PermanentSlow { get; private set; } = false;
  public bool LingeringSlow { get; private set; } = false;
  public bool GroundingShot { get; private set; } = false;
  public int LingeringWebHits { get; private set; } = 3;
  public float GroundedTime { get; } = 0.5f;
  public override TowerData.Type Type { get; set; } = TowerData.Type.WEB_SHOOTING_SPIDER_TOWER;

  private bool firing = false;
  private ProjectileHandler primaryProjectileHandler;
  private ProjectileHandler secondaryProjectileHandler;

  protected override void TowerStart() {
    primaryProjectileHandler = new(primaryWebShot, ProjectileSpeed, hitRange);
    secondaryProjectileHandler = new(secondaryWebShot, ProjectileSpeed, hitRange);
    StartCoroutine(WebShoot());

    targeting.behavior = Targeting.Behavior.SLOW_EM_ALL;
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.WSS_1_3_SLOW_STUN:
        SlowStun = true; break;
      case SpecialAbility.WSS_1_5_PERMANENT_SLOW:
        PermanentSlow = true; break;
      case SpecialAbility.WSS_2_5_LINGERING_SLOW:
        LingeringSlow = true; break;
      case SpecialAbility.WSS_3_3_ANTI_AIR:
        AntiAir = true; break;
      case SpecialAbility.WSS_3_5_GROUNDING_SHOT:
        GroundingShot = true; break;
      default:
        break;
    }
  }

  private void ProcessDamageAndEffects(Enemy target) {
    if (SlowStun && !target.webShootingTowerStuns.Contains(this)) {
      target.AddStunTime(StunTime);
      target.webShootingTowerStuns.Add(this);
    }
    if (PermanentSlow && !target.webShootingTowerPermSlow.Contains(this)) {
      target.ApplyPermanentSlow(SlowPower);
      target.webShootingTowerPermSlow.Add(this);
    }

    if (0 < SecondarySlowTargets) {
      SlowNearbyEnemies(target);
    }
    if (LingeringSlow) {
      target.GetClosestWaypoint().GetComponent<Tile>()
        .AddLingeringWeb(this, SlowPower, SlowDuration, LingeringWebHits);
    }
    if (GroundingShot && target.Flying) {
      target.TemporarilyStripFlying(GroundedTime);
    }

    target.ApplySlow(SlowPower, SlowDuration);
  }

  private void SlowNearbyEnemies(Enemy target) {
    var closestEnemies = ObjectPool.Instance.GetActiveEnemies()
        .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < AreaOfEffect)
        .Where(e => !e.Equals(target))
        .OrderBy(e => Vector3.Distance(target.transform.position, e.transform.position))
        .Take(SecondarySlowTargets)
        .ToList();
    foreach (var enemy in closestEnemies) {
      // Make sure the origin point for the secondary web is the target just hit.
      secondaryWebShot.transform.position = target.AimPoint;
      // The association must be done here because this is the only place the knowledge of the secondary slow's target
      // exists. To ensure consistent updates, only the association is handled here.
      secondaryWebShot.Emit(1);
      secondaryProjectileHandler.AssociateOrphanParticlesWithEnemy(enemy);
    }
  }

  private void ProcessSecondarySlowEffects(Enemy enemy) {
    enemy.ApplySlow(SlowPower * SecondarySlowPotency, SlowDuration * SecondarySlowPotency);
  }

  protected override void TowerUpdate() {
    Target = targeting.FindTarget(
      oldTarget: Target,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);
    if (Target == null) {
      firing = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      mesh.LookAt(Target.AimPoint);
      firing = true;
    }
    primaryProjectileHandler.UpdateParticles(Target, ProcessDamageAndEffects);
    // Null is passed because secondaryProjectileHanlder should never have unassociated particles at this point.
    secondaryProjectileHandler.UpdateParticles(null, ProcessSecondarySlowEffects);
  }

  protected override void UpdateAnimationSpeed(float newAttackSpeed) {}

  private IEnumerator WebShoot() {
    while (true) {
      while (firing && !IsDazzled()) {
        primaryWebShot.Emit(1);
        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }
}
