using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using EnemyKey = System.Tuple<EnemyData.Type, int>;

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
    var keys = new HashSet<EnemyKey>() { new(EnemyData.Type.ANT, 0) };
    objectPool.InitializeObjectPool(keys);
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

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    spittingAntTower.SetBeam(beam);

    CreateTestingPlayerState();
  }

  [TearDown]
  public void Teardown() {
    spittingAntTower.gameObject.SetActive(false);
    ObjectPool.Instance.DestroyAllEnemies();
  }
    
  // Play test for the continuous attack and the acid DoT effects.
  [UnityTest]
  public IEnumerator BeamAttackTest() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        acidStacks: 50.0f,
        attackSpeed: 1.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
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
    Assert.That(enemyInRange.HP, Is.EqualTo(hp));
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
      float acidStacks = 0.0f,
      float attackSpeed = 0.0f,
      float areaOfEffect = 0.0f,
      float armorTear = 0.0f,
      float damage = 0.0f,
      float range = 0.0f,
      float slowDuration = 0.0f,
      float slowPower = 0.0f,
      float stunTime = 0.0f) {
    spittingAntTower.AttackSpeed = attackSpeed;
    spittingAntTower.AreaOfEffect = areaOfEffect;
    spittingAntTower.ArmorTear = armorTear;
    spittingAntTower.Damage = damage;
    spittingAntTower.AcidStacks = acidStacks;
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

  private void CreateTestingPlayerState() {
    PlayerState state = new();
    state.Settings.ShowDamageText = false;
    PlayerState.Instance = state;
  }

  #endregion
}
