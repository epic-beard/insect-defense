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

    Assert.That(towerInRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(towerOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.DazzleTime, Is.EqualTo(10.0f));
    Assert.That(towerOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    Time.captureDeltaTime = 10.0f;
    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.right;

    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.DazzleTime, Is.EqualTo(0.0f));
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

    Assert.That(towerInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(towerOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));

    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.SlimeTime, Is.EqualTo(10.0f));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(0.5f));
    Assert.That(towerOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));

    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.right;

    Time.captureDeltaTime = 10.0f;
    yield return new WaitForEndOfFrame();
    Assert.That(towerInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(towerInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(towerOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(towerOutOfRange.SlimePower, Is.EqualTo(1.0f));
  }

  #region TestHelperMethods

  private Tower GetTower(Waypoint waypoint) {
    Tower tower = towerManager.ConstructTower(waypoint, TowerData.Type.SPITTING_ANT_TOWER);
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
