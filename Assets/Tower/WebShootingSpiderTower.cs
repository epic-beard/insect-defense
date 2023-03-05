using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebShootingSpiderTower : Tower {
  [SerializeField] Transform upperMesh;
  [SerializeField] ParticleSystem webShot;
  [SerializeField] ParticleSystem webEffect;
  [SerializeField] ParticleSystem webExplosion;

  [SerializeField] public Targeting.Behavior behavior;
  [SerializeField] public Targeting.Priority priority;

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

    Range = 20.0f;
    ProjectileSpeed = 20.0f;
    AttackSpeed = 1.0f;

    // -----0-----

    objectPool = FindObjectOfType<ObjectPool>();

    DisableAttackSystems();
    var coroutine = StartCoroutine(WebShoot());
  }

  public override void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {
    // TODO: Implement
  }

  protected override void ProcessDamageAndEffects(Enemy target) {
    // TODO: Implement
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
    } else {
      upperMesh.LookAt(GetSafeChildPosition(enemy));
      firing = true;

      GeneralAttackHandler(webShot, enemy, ProjectileSpeed);
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
