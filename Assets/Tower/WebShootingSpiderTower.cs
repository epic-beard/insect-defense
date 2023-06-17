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
  public int LingeringWebHits { get; private set; } = 3;
  public float GroundedTime { get; } = 0.5f;
  public override TowerData.Type TowerType { get; set; } = TowerData.Type.WEB_SHOOTING_SPIDER_TOWER;

  private Enemy enemy;
  private bool firing = false;
  private ProjectileHandler primaryProjectileHandler;
  private ProjectileHandler secondaryProjectileHandler;
  private Targeting targeting = new();
  protected ObjectPool objectPool;

  private void Start() {
    //TowerType = TowerData.Type.WEB_SHOOTING_SPIDER_TOWER;
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    //AttackSpeed = 1.0f;
    //AreaOfEffect = 20.0f;
    //Range = 30.0f;
    //ProjectileSpeed = 20.0f;
    //SlowDuration = 2.0f;
    //SlowPower = 0.5f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();
    primaryProjectileHandler = new(primaryWebShot, ProjectileSpeed, hitRange);
    secondaryProjectileHandler = new(secondaryWebShot, ProjectileSpeed, hitRange);

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
      target.GetClosestWaypoint().GetComponent<Tile>()
        .AddLingeringWeb(this, SlowPower, SlowDuration, LingeringWebHits);
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
      // The association must be done here because this is the only place the knowledge of the secondary slow's target
      // exists. To ensure consistent updates, only the association is handled here.
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
    }
    primaryProjectileHandler.UpdateParticles(enemy, ProcessDamageAndEffects);
    // Null is passed because secondaryProjectileHanlder should never have unassociated particles at this point.
    secondaryProjectileHandler.UpdateParticles(null, ProcessSecondarySlowEffects);
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
}
