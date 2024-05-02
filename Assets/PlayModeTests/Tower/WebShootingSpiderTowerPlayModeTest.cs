using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WebShootingSpiderTowerPlayModeTest {
  Waypoint targetWaypoint;
  Waypoint enemyInRangeWaypoint;
  Waypoint enemyOutOfRangeWaypoint;

  readonly float baseEnemySpeed = 10.0f;
  readonly float enemyArmor = 90.0f;
  readonly float targetArmor = 85.0f;

  EnemyData normalData;
  EnemyData targetData;

  ObjectPool objectPool;

  [OneTimeSetUp]
  public void OneTimeSetUp() {
    // Create the waypoints for enemies to be spawned on.
    targetWaypoint = CreateWaypoint(Vector3.right);
    enemyInRangeWaypoint = CreateWaypoint(Vector3.right * 2);
    enemyOutOfRangeWaypoint = CreateWaypoint(Vector3.right * 100);

    // Set up enemy data.
    normalData = new() {
      type = EnemyData.Type.ANT,
      maxArmor = enemyArmor,
      maxHP = 1.0f,
      properties = EnemyData.Properties.FLYING,
      size = EnemyData.Size.NORMAL,
      speed = baseEnemySpeed,
    };
    targetData = new() {
      type = EnemyData.Type.ANT,
      maxArmor = targetArmor,
      maxHP = 1.0f,
      properties = EnemyData.Properties.FLYING,
      size = EnemyData.Size.NORMAL,
      speed = baseEnemySpeed,
    };

    // Setup the Object Pool
    objectPool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = objectPool;
    var types = new HashSet<EnemyData.Type>() { EnemyData.Type.ANT };
    objectPool.InitializeObjectPool(types);

    Time.captureDeltaTime = 0;
  }

  [TearDown]
  public void TearDown() {
    ObjectPool.Instance.DestroyAllEnemies();
    Time.captureDeltaTime = 0;
  }

  // The most basic playtest, this is a slow tower without any special abilities.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_Basic() {
    float slowPower = 0.8f;
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

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
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: 0.8f,
        stunTime: stunTime);
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_3_SLOW_STUN);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    Assert.That(target.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.StunTime, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

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
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    Assert.That(target.OriginalSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyInRange.OriginalSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.OriginalSpeed, Is.EqualTo(baseEnemySpeed));

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.Speed, Is.LessThan(baseEnemySpeed * slowPower));
    Assert.That(enemyInRange.OriginalSpeed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.OriginalSpeed, Is.EqualTo(baseEnemySpeed));

    yield return null;
  }

  // Check to make sure the secondary slow is applied appropriately and to the right number of secondary targets.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_SecondarySlows() {
    float slowPower = 0.8f;
    float secondarySlowPower = 0.5f;
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: slowPower);
    TowerAbility ability = CreateTowerAbility(secondarySlowPower, 2.0f);
    wssTower.Upgrade(ability);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    Assert.That(target.Speed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyInRange.Speed, Is.EqualTo(baseEnemySpeed));
    Assert.That(enemyOutOfRange.Speed, Is.EqualTo(baseEnemySpeed));

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return new WaitForEndOfFrame();

    Assert.That(target.Speed, Is.EqualTo(baseEnemySpeed * (1 - slowPower)));
    Assert.That(enemyInRange.Speed, Is.GreaterThan(baseEnemySpeed * (1 - slowPower)));
    Assert.That(enemyInRange.Speed, Is.LessThan(baseEnemySpeed));
    Assert.That(enemyOutOfRange.Speed, Is.EqualTo(baseEnemySpeed));

    yield return null;
  }

  // Make sure the grounding shot is both temporary and limited to the target.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_GroundingShot() {
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: 0.8f);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_5_GROUNDING_SHOT);

    Assert.That(target.Flying, Is.EqualTo(true));
    Assert.That(enemyInRange.Flying, Is.EqualTo(true));
    Assert.That(enemyOutOfRange.Flying, Is.EqualTo(true));

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.Flying, Is.EqualTo(false));
    Assert.That(enemyInRange.Flying, Is.EqualTo(true));
    Assert.That(enemyOutOfRange.Flying, Is.EqualTo(true));

    yield return null;
  }

  // Test to make sure the secondary slows are applied appropriately.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffects_SecondarySlow() {
    WebShootingSpiderTower wssTower = CreateWebShootingSpiderTower(Vector3.zero);
    SetWebShootingSpiderTowerProperties(
        wssTower,
        attackSpeed: 0.1f,
        areaOfEffect: 10.0f,
        range: 1.0f,
        secondarySlowPotency: 0.5f,
        secondarySlowTargets: 2,
        slowDuration: 10.0f,
        slowPower: 0.8f);
    float secondarySlowDuration = wssTower.SlowDuration * wssTower.SecondarySlowPotency;
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    target.Initialize(targetWaypoint);
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    enemyInRange.Initialize(enemyInRangeWaypoint);
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();
    enemyOutOfRange.Initialize(enemyOutOfRangeWaypoint);

    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowPower, Is.EqualTo(0.0f));
    Assert.That(enemyOutOfRange.SlowDuration, Is.EqualTo(0.0f));

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.SlowPower, Is.EqualTo(wssTower.SlowPower));
    // The primary slow should be stronger than the secondary slow.
    Assert.That(target.SlowDuration, Is.GreaterThan(secondarySlowDuration));
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

    wssTower.Behavior = Targeting.Behavior.ALL;
    wssTower.Priority = Targeting.Priority.LEAST_ARMOR;
    wssTower.ProjectileSpeed = 10.0f;
    wssTower.AntiAir = true;

    // Mandatory setup.
    MeshRenderer mesh = new GameObject().AddComponent<MeshRenderer>();
    wssTower.SetMesh(mesh);

    ParticleSystem webShot = new GameObject().AddComponent<ParticleSystem>();
    wssTower.SetPrimaryWebShot(webShot);

    ParticleSystem secondaryWebShot = new GameObject().AddComponent<ParticleSystem>();
    wssTower.SetSecondaryWebShot(secondaryWebShot);

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

  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  private TowerAbility CreateTowerAbility(float secondarySlow, float secondaryTargets) {
    TowerAbility ability = new();
    ability.attributeModifiers = new TowerAbility.AttributeModifier[2];
    ability.attributeModifiers[0].attribute = TowerData.Stat.SECONDARY_SLOW_POTENCY;
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
