using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WebShootingSpiderTowerPlayModeTest {

  WebShootingSpiderTower wssTower;
  Enemy target;
  Enemy enemyInRange;
  Enemy enemyOutOfRange;

  readonly float armor = 10.0f;
  readonly float baseEnemySpeed = 10.0f;

  [SetUp]
  public void SetUp() {
    target = CreateEnemy(Vector3.zero, armor: (armor - 5.0f), speed: baseEnemySpeed);
    enemyInRange = CreateEnemy(Vector3.right, armor: armor, speed: baseEnemySpeed);
    enemyOutOfRange = CreateEnemy(Vector3.right * 100, armor: armor, speed: baseEnemySpeed);

    wssTower = CreateWebShootingSpiderTower(Vector3.up);

    // Mandatory setup.
    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    wssTower.SetUpperMesh(upperMesh);

    ParticleSystem webShot = new GameObject().AddComponent<ParticleSystem>();
    wssTower.SetPrimaryWebShot(webShot);

    ParticleSystem secondaryWebShot = new GameObject().AddComponent<ParticleSystem>();
    wssTower.SetSecondaryWebShot(secondaryWebShot);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);
  }

  // The most basic playtest, this is a slow tower without any special abilities.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_Basic() {
    float slowPower = 0.8f;
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);

    yield return new WaitForSeconds(0.08f);

    Assert.That(target.SlowDuration, Is.GreaterThan(0.0f));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));
    Assert.That(enemyInRange.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowPower, Is.EqualTo(0.0f));

    yield return null;
  }

  // Test to make sure that the stuntime functions as expected and is only applied once.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_StunTarget() {
    float stunTime = 10.0f;
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: 0.8f,
        stunTime: stunTime);
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_3_SLOW_STUN);

    Assert.That(target.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));

    yield return new WaitForSeconds(0.08f);

    Assert.That(target.StunTime, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));

    yield return new WaitForSeconds(0.08f);

    // Ensure that the stuntime is not applied a second time.
    Assert.That(target.StunTime, Is.GreaterThan(0.0f));
    Assert.That(target.StunTime, Is.LessThanOrEqualTo(stunTime));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));

    yield return null;
  }

  // Ensure the permanent slow applies properly and only once. This test uses BaseSpeed to look at the
  // enemy's unmodified speed (so as not to count the tower's slows).
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_PermanentSlow() {
    float slowPower = 0.8f;
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW);

    Assert.That(target.BaseSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyInRange.BaseSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.BaseSpeed, Is.EqualTo(baseEnemySpeed));

    yield return new WaitForSeconds(0.08f);

    Assert.That(target.BaseSpeed, Is.EqualTo(baseEnemySpeed * slowPower));
    Assert.That(target.Speed, Is.LessThan(baseEnemySpeed * slowPower));
    Assert.That(enemyInRange.BaseSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.BaseSpeed, Is.EqualTo(baseEnemySpeed));

    yield return null;
  }

  // Check to make sure the secondary slow is applied appropriately and to the right number of secondary targets.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_SecondarySlows() {
    Time.captureDeltaTime = 1.0f;
    float slowPower = 0.8f;
    float secondarySlowPower = 0.5f;
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 0.7f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);
    TowerAbility ability = CreateTowerAbility(secondarySlowPower, 2.0f);
    wssTower.Upgrade(ability);

    Assert.That(target.Speed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyInRange.Speed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.Speed, Is.EqualTo(baseEnemySpeed));

    yield return null;

    Assert.That(target.Speed, Is.EqualTo(baseEnemySpeed * (1 - slowPower)));
    Assert.That(enemyInRange.Speed, Is.GreaterThan(baseEnemySpeed * (1 - slowPower)));
    Assert.That(enemyInRange.Speed, Is.LessThan(baseEnemySpeed));
    Assert.That(enemyOutOfRange.Speed, Is.EqualTo(baseEnemySpeed));

    yield return null;
  }

  // Make sure the grounding shot is both temporary and limited to the target.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_GroundingShot() {
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: 0.8f);
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_5_GROUNDING_SHOT);

    Assert.That(target.Flying, Is.EqualTo(true));
    Assert.That(enemyInRange.Flying, Is.EqualTo(true));
    Assert.That(enemyOutOfRange.Flying, Is.EqualTo(true));

    yield return new WaitForSeconds(0.08f);

    Assert.That(target.Flying, Is.EqualTo(false));
    Assert.That(enemyInRange.Flying, Is.EqualTo(true));
    Assert.That(enemyOutOfRange.Flying, Is.EqualTo(true));

    yield return null;
  }

  // Test to make sure the secondary slows are applied appropriately.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_SecondarySlow() {
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        range: 10.0f,
        secondarySlowPotency: 0.5f,
        secondarySlowTargets: 2,
        slowDuration: 10.0f,
        slowPower: 0.8f);
    Time.captureDeltaTime = 1.0f;

    float secondarySlowDuration = wssTower.SlowDuration * wssTower.SecondarySlowPotency;

    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowDuration, Is.EqualTo(0.0f));

    yield return new WaitForSeconds(0.08f);

    Assert.That(target.SlowPower, Is.EqualTo(wssTower.SlowPower));
    // The primary slow should be stronger than the secondary slow.
    Assert.That(target.SlowDuration, Is.InRange(secondarySlowDuration, wssTower.SlowDuration));
    Assert.That(enemyInRange.SlowPower, Is.EqualTo(wssTower.SlowPower * wssTower.SecondarySlowPotency));
    Assert.That(enemyInRange.SlowDuration, Is.InRange(0.0f, secondarySlowDuration));
    Assert.That(enemyOutOfRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowDuration, Is.EqualTo(0.0f));

    yield return null;
  }

  #region TestHelperMethods

  // Create a web shooting spider tower with all the values explicitly set for clarity.
  private WebShootingSpiderTower CreateWebShootingSpiderTower(Vector3 position) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    WebShootingSpiderTower wssTower = gameObject.AddComponent<WebShootingSpiderTower>();

    wssTower.Priority = Targeting.Priority.LEASTARMOR;
    wssTower.ProjectileSpeed = 100.0f;
    wssTower.AntiAir = true;

    return wssTower;
  }

  private void SetWebShootingSpiderTowerProperties(
      WebShootingSpiderTower wssTower,
      float attackSpeed = 0.0f,
      float areaOfEffect = 0.0f,
      float range = 0.0f,
      float secondarySlowPotency = 0.0f,
      int secondarySlowTargets = 0,
      float slowDuration = 0.0f,
      float slowPower = 0.0f,
      float stunTime = 0.0f) {
    wssTower.AttackSpeed = attackSpeed;
    wssTower.AreaOfEffect = areaOfEffect;
    wssTower.Range = range;
    wssTower.SecondarySlowPotency = secondarySlowPotency;
    wssTower.SecondarySlowTargets = secondarySlowTargets;
    wssTower.SlowDuration = slowDuration;
    wssTower.SlowPower = slowPower;
    wssTower.StunTime = stunTime;
  }

  // Create and return an enemy with optional args
  private Enemy CreateEnemy(
      Vector3 position,
      float armor = 1.0f,
      float speed = 1.0f,
      EnemyData.Size size = EnemyData.Size.NORMAL) {
    GameObject gameObject = new();
    gameObject.SetActive(false);

    gameObject.transform.position = position;
    Enemy enemy = gameObject.AddComponent<Enemy>();
    Waypoint waypoint = CreateWaypoint(position);
    enemy.PrevWaypoint = waypoint;

    EnemyData data = new() {
      properties = EnemyData.Properties.FLYING,
      maxArmor = armor,
      maxHP = 1.0f,
      speed = speed,
      size = size,
    };
    enemy.Data = data;
    gameObject.SetActive(true);

    return enemy;
  }

  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  private TowerAbility CreateTowerAbility(float secondarySlow, float secondaryTargets) {
    TowerAbility ability = new();
    ability.attributeModifiers = new TowerAbility.AttributeModifier[2];
    ability.attributeModifiers[0].attribute = TowerData.Stat.SECDONARY_SLOW_POTENCY;
    ability.attributeModifiers[0].mode = TowerAbility.Mode.SET;
    ability.attributeModifiers[0].mod = secondarySlow;
    ability.attributeModifiers[1].attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS;
    ability.attributeModifiers[1].mode = TowerAbility.Mode.SET;
    ability.attributeModifiers[1].mod = secondaryTargets;
    ability.upgradePath = 1;

    return ability;
  }

  #endregion
}
