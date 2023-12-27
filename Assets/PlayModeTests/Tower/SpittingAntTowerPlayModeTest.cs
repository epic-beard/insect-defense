using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpittingAntTowerPlayModeTest {

  SpittingAntTower spittingAntTower;
  Waypoint targetWaypoint;
  Waypoint enemyInRangeWaypoint;
  Waypoint enemyOutOfRangeWaypoint;

  readonly float hp = 100000.0f;
  readonly float enemyArmor = 90.0f;
  readonly float targetArmor = 85.0f;

  EnemyData normalData;
  EnemyData targetData;

  [OneTimeSetUp]
  public void OneTimeSetUp() {
    // Create the waypoints for enemies to be spawned on.
    targetWaypoint = CreateWaypoint(Vector3.right);
    enemyInRangeWaypoint = CreateWaypoint(Vector3.right * 2);
    enemyOutOfRangeWaypoint = CreateWaypoint(Vector3.right * 100);

    // Set up enemy data.
    normalData = new() {
      maxArmor = enemyArmor,
      maxHP = hp,
      size = EnemyData.Size.NORMAL,
      speed = 0.0f,
    };
    targetData = new() {
      maxArmor = targetArmor,
      maxHP = hp,
      size = EnemyData.Size.NORMAL,
      speed = 0.0f,
    };

    // Setup the Object Pool
    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = objectPool;
    objectPool.AddPrefab(EnemyData.Type.ANT, CreateEnemyPrefab());
    objectPool.InvokeInitializeObjectPool();
  }

  [SetUp]
  public void SetUp() {
    // Create and setup the basic spitting ant tower.
    spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    spittingAntTower.SetUpperMesh(upperMesh);

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplash(splash);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);

    ParticleSystem acidExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetAcidExplosion(acidExplosion);

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    spittingAntTower.SetBeam(beam);
  }

  [TearDown]
  public void Teardown() {
    spittingAntTower.gameObject.SetActive(false);
    ObjectPool.Instance.DestroyAllEnemies();
  }

  // Play test for the basic splash attack. This test focuses on those effects only available to the
  // splash effect. It does not delve into the DoT effects, SA_2_3_DOT_SLOW and SA_2_5_DOT_EXPLOSION should
  // not be true for this test.
  [UnityTest]
  public IEnumerator SplashAttackTestWithNoArmorTearExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: targetArmor * 0.8f,
        damage: 100.0f,
        range: 1.0f);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.HP, Is.LessThan(hp));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(enemyInRange.HP, Is.LessThan(hp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(hp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }
  
  // Like the above, this test focuses on splash, but includs the ArmorTearExplosion to make sure it damages
  // the armor of nearby enemies.
  [UnityTest]
  public IEnumerator SplashAttackTestWithArmorTearExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: targetArmor * 0.8f,
        damage: 100.0f,
        range: 1.0f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.HP, Is.LessThan(hp));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(enemyInRange.HP, Is.LessThan(hp));
    Assert.That(enemyInRange.Armor, Is.LessThan(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(hp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }
  
  // Play test for the continuous attack and the acid DoT effects.
  [UnityTest]
  public IEnumerator BeamAttackTestNoSlowOrExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 50.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.HP, Is.LessThan(hp));
    Assert.That(target.Armor, Is.LessThan(enemyArmor));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(hp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(hp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }
  
  // Make sure that the target is slowed when the dot reaches max.
  [UnityTest]
  public IEnumerator BeamAttackTestWithSlowNoExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 100.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_3_ACID_BUILDUP_BONUS);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.HP, Is.LessThan(hp));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(target.SlowDuration, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(hp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(hp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }
  
  // Make sure nearby enemies are damaged by the acid explosion when dot reaches max.
  [UnityTest]
  public IEnumerator BeamAttackTestWithExplosionNoSlow() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 100.0f,
        range: 1.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_5_DOT_ENHANCEMENT);
    var target = ObjectPool.Instance.InstantiateEnemy(targetData, targetWaypoint).GetComponent<Enemy>();
    var enemyInRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyInRangeWaypoint).GetComponent<Enemy>();
    var enemyOutOfRange = ObjectPool.Instance.InstantiateEnemy(normalData, enemyOutOfRangeWaypoint).GetComponent<Enemy>();

    Time.captureDeltaTime = 0.001f;
    // Yield once to target enemy.
    yield return null;
    // Yield once to trigger a shot.
    yield return new WaitForEndOfFrame();

    // Wait two seconds and skip to the end of the frame.
    Time.captureDeltaTime = 2.0f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(target.HP, Is.LessThan(hp));
    Assert.That(target.Armor, Is.LessThanOrEqualTo(targetArmor));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.LessThan(hp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(hp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  #region TestHelperMethods

  // Create a spitting ant tower with all the values explicitly set for clarity.
  private SpittingAntTower CreateSpittingAntTower(Vector3 position) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    SpittingAntTower spittingAntTower = gameObject.AddComponent<SpittingAntTower>();

    spittingAntTower.Priority = Targeting.Priority.LEAST_ARMOR;
    spittingAntTower.ProjectileSpeed = 1.0f;

    return spittingAntTower;
  }

  private void SetSpittingAntTowerProperties(
      SpittingAntTower spittingAntTower,
      float attackSpeed = 0.0f,
      float areaOfEffect = 0.0f,
      float armorTear = 0.0f,
      float damage = 0.0f,
      float damageOverTime = 0.0f,
      float range = 0.0f,
      float slowDuration = 0.0f,
      float slowPower = 0.0f,
      float stunTime = 0.0f) {
    spittingAntTower.AttackSpeed = attackSpeed;
    spittingAntTower.AreaOfEffect = areaOfEffect;
    spittingAntTower.ArmorTear = armorTear;
    spittingAntTower.Damage = damage;
    spittingAntTower.DamageOverTime = damageOverTime;
    spittingAntTower.Range = range;
    spittingAntTower.SlowDuration = slowDuration;
    spittingAntTower.SlowPower = slowPower;
    spittingAntTower.StunTime = stunTime;
  }

  // Create an enemy prefab.
  private GameObject CreateEnemyPrefab() {
    GameObject prefab = new();
    prefab.SetActive(false);
    prefab.AddComponent<Enemy>();

    return prefab;
  }

  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  #endregion
}
