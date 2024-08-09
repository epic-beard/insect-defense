using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyPlayModeTest {
  TowerManager towerManager;
  Waypoint inRange;
  Waypoint outOfRange;

  private readonly int tileSpacing = 10;

  [SetUp]
  public void SetUp() {
    inRange = CreateWaypoint(tileSpacing * Vector3.left);
    outOfRange = CreateWaypoint(2 * tileSpacing * Vector3.left);

    CreateGameStateManager();
    CreateTestingPlayerState();

    towerManager = CreateTowerManager();
  }

  [UnityTest]
  public IEnumerator DazzleWorks() {
    ObjectPool objectPool = CreateObjectPool();
    Tower towerInRange = GetTower(inRange);
    Tower towerOutOfRange = GetTower(outOfRange);
    EnemyData data = new() {
      maxHP = 1000.0f,
      maxArmor = 1000.0f,
      dazzle = new EnemyData.DazzleProperties() {
        duration = 10.0f,
        interval = 1.0f,
        range = 12.0f
      }
    };
    Enemy enemy = CreateEnemy(Vector3.zero, data, objectPool);

    Time.captureDeltaTime = 1.0f;

    Assert.That(towerInRange.DazzleTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.DazzleTime, Is.GreaterThan(Time.time));
    Assert.That(towerOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    Time.captureDeltaTime = 10.0f;
    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.right;

    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.DazzleTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerOutOfRange.DazzleTime, Is.EqualTo(0.0f));
  }

  [UnityTest]
  public IEnumerator SlimeWorks() {
    ObjectPool objectPool = CreateObjectPool();
    Tower towerInRange = GetTower(inRange);
    Tower towerOutOfRange = GetTower(outOfRange);
    EnemyData data = new() {
      maxHP = 1000.0f,
      maxArmor = 1000.0f,
      slime = new EnemyData.SlimeProperties() {
        duration = 10.0f,
        interval = 1.0f,
        range = 12.0f,
        power = 0.5f,
      }
    };
    Enemy enemy = CreateEnemy(Vector3.zero, data, objectPool);

    Time.captureDeltaTime = 1.0f;

    Assert.That(towerInRange.SlimeTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(towerOutOfRange.SlimeTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));

    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.SlimeTime, Is.GreaterThan(Time.time));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(0.5f));
    Assert.That(towerOutOfRange.SlimeTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));

    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.right;

    Time.captureDeltaTime = 11.0f;
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.SlimeTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(towerOutOfRange.SlimeTime, Is.LessThanOrEqualTo(Time.time));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));
  }

  [UnityTest]
  public IEnumerator RendTemporarilyReducingArmorWorks() {
    ObjectPool objectPool = CreateObjectPool();
    float towerArmorPierce = 50.0f;
    float towerArmorReductionDuration = 1.0f;
    EnemyData data = new() {
      maxHP = 1000.0f,
      maxArmor = 50.0f,
    };
    Enemy enemy = CreateEnemy(Vector3.zero, data, objectPool);
    float reducedArmor = data.maxArmor - towerArmorPierce;
    
    enemy.TempReduceArmor(towerArmorPierce, towerArmorReductionDuration);

    Time.captureDeltaTime = towerArmorReductionDuration * 0.6f;
    yield return null;
    yield return new WaitForEndOfFrame();

    // Enemy armor should be reduced.
    Assert.That(enemy.Armor, Is.EqualTo(reducedArmor));

    yield return null;
    yield return new WaitForEndOfFrame();

    // Enemy armor should no longer be reduced.
    Assert.That(enemy.Armor, Is.EqualTo(data.maxArmor));
  }

  [UnityTest]
  public IEnumerator RendTemporarilyReducingArmorRepeatedApplicationWorks() {
    ObjectPool objectPool = CreateObjectPool();
    float towerArmorPierce = 50.0f;
    float towerArmorReductionDuration = 1.0f;
    EnemyData data = new() {
      maxHP = 1000.0f,
      maxArmor = 50.0f,
    };
    Enemy enemy = CreateEnemy(Vector3.zero, data, objectPool);
    float expectedHp = data.maxHP;
    float reducedArmor = data.maxArmor - towerArmorPierce;

    enemy.TempReduceArmor(towerArmorPierce, towerArmorReductionDuration);
    
    Time.captureDeltaTime = towerArmorReductionDuration * 0.6f;
    yield return null;
    yield return new WaitForEndOfFrame();

    Assert.That(enemy.Armor, Is.EqualTo(reducedArmor));
    Assert.That(enemy.TempArmorReduceEndTime, Is.LessThan(Time.time + towerArmorReductionDuration));

    enemy.TempReduceArmor(towerArmorPierce - 1, towerArmorReductionDuration);

    // The reduced armor pierce value passed in should have been ignored.
    Assert.That(enemy.TempArmorReducePower, Is.EqualTo(towerArmorPierce));
    // The duration was updated but not added to.
    Assert.That(enemy.TempArmorReduceEndTime, Is.EqualTo(Time.time + towerArmorReductionDuration));

    // A second yield return null is required here because a second pass through the coroutine is
    // needed to properly set the reduced armor value.
    yield return null;
    yield return null;
    yield return new WaitForEndOfFrame();

    // Enemy armor should still be reduced.
    Assert.That(enemy.Armor, Is.EqualTo(reducedArmor));

    yield return null;
    yield return new WaitForEndOfFrame();

    // Enemy armor should no longer be reduced.
    Assert.That(enemy.Armor, Is.EqualTo(enemy.MaxArmor));
  }

  #region TestHelperMethods

  private Tower GetTower(Waypoint waypoint, TowerData.Type towerType = TowerData.Type.SPITTING_ANT_TOWER) {
    Tower tower = towerManager.ConstructTower(waypoint, towerType);
    tower.enabled = true;
    tower.SetTargetingIndicator(null);
    return tower;
  }

  private GameStateManager CreateGameStateManager() {
    return new GameObject().AddComponent<GameStateManager>();
  }

  private TowerManager CreateTowerManager() {
    return new GameObject().AddComponent<TowerManager>();
  }

  private float CalculateDamageDone(float damage, float armor, float armorPierce) {
    float effectiveArmor = Mathf.Clamp(armor - armorPierce, 0.0f, 100.0f);

    return damage * (100.0f - effectiveArmor) / 100.0f;
  }

  private Enemy CreateEnemy(Vector3 position, EnemyData data, ObjectPool objectPool) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    var enemy = gameObject.AddComponent<Enemy>();
    enemy.Data = data;
    enemy.Initialize(CreateWaypoint(position));

    HashSet<Enemy> activeEnemies = objectPool.GetActiveEnemies();
    activeEnemies.Add(enemy);

    return enemy;
  }

  private ObjectPool CreateObjectPool() {
    ObjectPool pool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = pool;
    return pool;
  }

  private Waypoint CreateWaypoint(Vector3 position) {
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
