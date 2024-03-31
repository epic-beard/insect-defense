using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyPlayModeTest {
  Enemy enemy;
  Tower towerInRange;
  Tower towerOutOfRange;
  TowerManager towerManager;

  private readonly int tileSpacing = 10;

  [SetUp]
  public void SetUp() {
    Waypoint inRange = CreateWaypoint(tileSpacing * Vector3.left);
    Waypoint outOfRange = CreateWaypoint(2 * tileSpacing * Vector3.left);

    CreateGameStateManager();
    CreateTestingPlayerState();

    towerManager = CreateTowerManager();

    // towerInRamge
    towerInRange = towerManager.ConstructTower(inRange, TowerData.Type.SPITTING_ANT_TOWER);
    towerInRange.enabled = true;
    towerOutOfRange = towerManager.ConstructTower(outOfRange, TowerData.Type.SPITTING_ANT_TOWER);
    towerOutOfRange.enabled = true;

    enemy = CreateEnemy(Vector3.zero);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { enemy };
    objectPool.SetActiveEnemies(activeEnemies);
  }

  [UnityTest]
  public IEnumerator DazzleWorks() {
    enemy.Dazzle = new() {
      duration = 10.0f,
      interval = 1.0f,
      range = 10.01f
    };
    Time.captureDeltaTime = 1.0f;

    enemy.gameObject.SetActive(true);

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
    enemy.Slime = new() {
      duration = 10.0f,
      interval = 1.0f,
      range = 10.01f,
      power = 0.5f,
    };
    Time.captureDeltaTime = 1.0f;

    enemy.gameObject.SetActive(true);

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

  private GameStateManager CreateGameStateManager() {
    return new GameObject().AddComponent<GameStateManager>();
  }

  private TowerManager CreateTowerManager() {
    return new GameObject().AddComponent<TowerManager>();
  }

  private Enemy CreateEnemy(Vector3 position) {
    GameObject gameObject = new();
    gameObject.SetActive(false);
    gameObject.transform.position = position;
    var enemy = gameObject.AddComponent<Enemy>();
    EnemyData data = new() {
      maxHP = 1000.0f,
      maxArmor = 1000.0f
    };
    enemy.Data = data;
    enemy.PrevWaypoint = new GameObject().AddComponent<Waypoint>();

    return enemy;
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
