using System.Collections;
using System.Linq;
using UnityEngine;
using static TowerAbility;

public class WebShootingSpiderTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem webShot;
  [SerializeField] ParticleSystem webEffect;
  [SerializeField] ParticleSystem webExplosion;

  [SerializeField] public Targeting.Behavior behavior;
  [SerializeField] public Targeting.Priority priority;

  public int numLingeringWebUses = 3;

  public bool SlowStun { get; private set; } = false;
  public bool PermanentSlow { get; private set; } = false;
  public bool LingeringSlow { get; private set; } = false;
  public bool AAAssist { get; private set; } = false;
  // This uses the upgrade levels to track how many enemies are effected by the slow (it maxes at 3).
  public int EnemiesHitBySlow {
    get { return Mathf.Min(upgradeLevels[1], 3); }
  }
  public float SlowAppliedToSecondaryTargets {
    get {
      return upgradeLevels[1] switch {
        0 => 0.0f,
        1 => 0.5f,
        2 => 0.5f,
        3 => 0.75f,
        4 => 1.0f,
        5 => 1.0f,
        _ => 0.0f,
      };
    }
  }
  public float GroundedTime { get; } = 0.5f;

  private Enemy enemy;
  private bool firing = false;
  private ProjectileHandler projectileHandler;
  private Targeting targeting = new();
  protected ObjectPool objectPool;

  private void Start() {
    // TODO: The user should be able to set the default for each tower type.
    targeting = new() {
      behavior = this.behavior,
      priority = this.priority
    };

    //Range = 20.0f;
    //ProjectileSpeed = 20.0f;
    //AttackSpeed = 1.0f;
    //SlowDuration = 5.0f;
    //SlowPower = 0.5f;
    //AreaOfEffect = 10.0f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();
    projectileHandler = new(webShot, ProjectileSpeed, hitRange);

    DisableAttackSystems();
    var coroutine = StartCoroutine(WebShoot());
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
      case SpecialAbility.WSS_3_5_AA_ASSIST:
        AAAssist = true; break;
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

    if (EnemiesHitBySlow > 0) {
      SlowNearbyEnemies(target);
    }
    if (LingeringSlow) {
      // TODO: Place a lingering web with numLingeringWebUses hits on the tile the enemy was on at collision.
    }
    if (AAAssist && target.Flying) {
      target.TemporarilyStripFlying(GroundedTime);
    }

    target.ApplySlow(SlowPower, SlowDuration);
  }

  private void SlowNearbyEnemies(Enemy target) {
    var closestEnemies = objectPool.GetActiveEnemies()
        .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < AreaOfEffect)
        .Where(e => !e.Equals(target))
        .OrderBy(e => Vector3.Distance(target.transform.position, e.transform.position))
        .Take(EnemiesHitBySlow)
        .ToList();
    float secondarySlowPower = SlowPower * SlowAppliedToSecondaryTargets;
    float secondarySlowDuration = SlowDuration * SlowAppliedToSecondaryTargets;
    foreach (var enemy in closestEnemies) {
      enemy.ApplySlow(secondarySlowPower, secondarySlowDuration);
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

    if (enemy is null) {
      firing = false;
      // TODO: Have the tower go back to an 'idle' animation or neutral pose.
    } else {
      upperMesh.LookAt(projectileHandler.GetSafeChildPosition(enemy.transform));
      firing = true;

      projectileHandler.UpdateParticles(enemy, ProcessDamageAndEffects);
    }
  }

  private IEnumerator WebShoot() {
    while (true) {
      while (firing) {
        webShot.Emit(1);
        yield return new WaitForSeconds(1 / AttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }

  private void DisableAttackSystems() {
    var emissionModule = webShot.emission;
    emissionModule.enabled = false;
  }
}
