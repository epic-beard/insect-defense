using Codice.CM.Client.Differences;
using System.Collections;
using System.Linq;
using UnityEngine;
using static TowerAbility;

public class WebShootingSpiderTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem primaryWebShot;
  [SerializeField] ParticleSystem secondaryWebShot;
  [SerializeField] ParticleSystem webEffect;

  [SerializeField] public Targeting.Behavior behavior;
  [SerializeField] public Targeting.Priority priority;

  public int lingeringWebNumUses = 3;

  public bool SlowStun { get; private set; } = false;
  public bool PermanentSlow { get; private set; } = false;
  public bool LingeringSlow { get; private set; } = false;
  public bool GroundingShot { get; private set; } = false;
  public float GroundedTime { get; } = 0.5f;

  private Enemy enemy;
  private bool firing = false;
  private ProjectileHandler primaryProjectileHandler;
  private ProjectileHandler secondaryProjectileHandler;
  private Targeting targeting = new();
  protected ObjectPool objectPool;

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    AttackSpeed = 1.0f;
    AreaOfEffect = 20.0f;
    Range = 30.0f;
    ProjectileSpeed = 20.0f;
    SecondarySlowPotency = 0.5f;
    SecondarySlowTargets = 2;
    SlowDuration = 5.0f;
    SlowPower = 0.8f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();
    primaryProjectileHandler = new(primaryWebShot, ProjectileSpeed, hitRange);
    secondaryProjectileHandler = new(secondaryWebShot, ProjectileSpeed, hitRange);

    DisableAttackSystems();
    StartCoroutine(WebShoot());
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

  protected override void ProcessDamageAndEffects(Enemy target) {
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
      // TODO: Place a lingering web with lingeringWebNumUses hits on the tile the enemy was on at collision.
    }
    if (GroundingShot && target.Flying) {
      target.TemporarilyStripFlying(GroundedTime);
    }

    target.ApplySlow(SlowPower, SlowDuration);
  }

  private void SlowNearbyEnemies(Enemy target) {
    var closestEnemies = objectPool.GetActiveEnemies()
        .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < AreaOfEffect)
        .Where(e => !e.Equals(target))
        .OrderBy(e => Vector3.Distance(target.transform.position, e.transform.position))
        .Take(SecondarySlowTargets)
        .ToList();
    foreach (var enemy in closestEnemies) {
      // Make sure the origin point for the secondary web is the target just hit.
      secondaryWebShot.transform.position = secondaryProjectileHandler.GetSafeChildPosition(target.transform);
      secondaryWebShot.Emit(1);
      secondaryProjectileHandler.AssociateOrphanParticlesWithEnemy(enemy);
    }
  }

  private void ProcessSecondarySlowEffects(Enemy enemy) {
    enemy.ApplySlow(SlowPower * SecondarySlowPotency, SlowDuration * SecondarySlowPotency);
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

    if (enemy is null) {
      firing = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      upperMesh.LookAt(primaryProjectileHandler.GetSafeChildPosition(enemy.transform));
      firing = true;

      primaryProjectileHandler.UpdateParticles(enemy, ProcessDamageAndEffects);
      secondaryProjectileHandler.UpdateParticles(null, ProcessSecondarySlowEffects);
    }
  }

  private IEnumerator WebShoot() {
    while (true) {
      while (firing) {
        primaryWebShot.Emit(1);
        yield return new WaitForSeconds(1 / AttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }

  private void DisableAttackSystems() {
    var emissionModule = primaryWebShot.emission;
    emissionModule.enabled = false;

    emissionModule = secondaryWebShot.emission;
    emissionModule.enabled = false;
  }
}
