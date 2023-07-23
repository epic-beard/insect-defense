using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyTest {
  Enemy enemy;
  SpittingAntTower spittingAntInRange;
  SpittingAntTower spittingAntOutOfRange;
  WebShootingSpiderTower webShootingSpiderInRange;
  WebShootingSpiderTower webShootingSpiderOutOfRange;
  GameStateManager gameStateManager;

  [SetUp]
  public void SetUp() {
    Vector3 inRange = Vector3.left;
    Vector3 outOfRange = 2 * Vector3.left;
    spittingAntInRange = CreateSpittingAnt(inRange);
    spittingAntOutOfRange = CreateSpittingAnt(outOfRange);

    webShootingSpiderInRange = CreateWebShootingSpider(inRange);
    webShootingSpiderOutOfRange = CreateWebShootingSpider(outOfRange);

    gameStateManager = CreateGameStateManager();
    gameStateManager.AddTower(Vector2Int.left, spittingAntInRange);
    gameStateManager.AddTower(2 * Vector2Int.left, spittingAntOutOfRange);
    gameStateManager.AddTower(Vector2Int.right, webShootingSpiderInRange);
    gameStateManager.AddTower(2 * Vector2Int.right, webShootingSpiderOutOfRange);
    Debug.Log(GameStateManager.Instance.activeTowerMap.Count);
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
      range = 1.0f
    };
    enemy.gameObject.SetActive(true);

    Assert.That(spittingAntInRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntOutOfRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderInRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    Time.captureDeltaTime = 1.0f;
    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(spittingAntInRange.DazzleTime, Is.EqualTo(10.0f));
    Assert.That(spittingAntOutOfRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderInRange.DazzleTime, Is.EqualTo(10.0f));
    Assert.That(webShootingSpiderOutOfRange.DazzleTime, Is.EqualTo(0.0f));

    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.left;

    Time.captureDeltaTime = 10.0f;
    yield return new WaitForEndOfFrame();
    Assert.That(spittingAntInRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntOutOfRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderInRange.DazzleTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderOutOfRange.DazzleTime, Is.EqualTo(0.0f));
  }

  [UnityTest]
  public IEnumerator SlimeWorks() {
    enemy.Slime = new() {
      duration = 10.0f,
      interval = 1.0f,
      range = 1.0f,
      power = 0.5f,
    };
    enemy.gameObject.SetActive(true);

    Assert.That(spittingAntInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(spittingAntOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntOutOfRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(webShootingSpiderInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(webShootingSpiderOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderOutOfRange.SlimePower, Is.EqualTo(1.0f));

    Time.captureDeltaTime = 1.0f;
    // Start the timer to the first shot.
    yield return null;
    // Activate the ability.
    yield return new WaitForEndOfFrame();
    Assert.That(spittingAntInRange.SlimeTime, Is.EqualTo(10.0f));
    Assert.That(spittingAntInRange.SlimePower, Is.EqualTo(0.5f));
    Assert.That(spittingAntOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntOutOfRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(webShootingSpiderInRange.SlimeTime, Is.EqualTo(10.0f));
    Assert.That(webShootingSpiderInRange.SlimePower, Is.EqualTo(0.5f));
    Assert.That(webShootingSpiderOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderOutOfRange.SlimePower, Is.EqualTo(1.0f));

    // Move the enemy out of range of all the towers.
    enemy.transform.position = 10 * Vector3.left;

    Time.captureDeltaTime = 10.0f;
    yield return new WaitForEndOfFrame();
    Assert.That(spittingAntInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(spittingAntOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(spittingAntOutOfRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(webShootingSpiderInRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderInRange.SlimePower, Is.EqualTo(1.0f));
    Assert.That(webShootingSpiderOutOfRange.SlimeTime, Is.EqualTo(0.0f));
    Assert.That(webShootingSpiderOutOfRange.SlimePower, Is.EqualTo(1.0f));
  }

  #region TestHelperMethods
  private SpittingAntTower CreateSpittingAnt(Vector3 position) {
    var spittingAntTower = CreateTower<SpittingAntTower>(position);
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

    return spittingAntTower;
  }

  private WebShootingSpiderTower CreateWebShootingSpider(Vector3 position) {
    var webShootingSpider = CreateTower<WebShootingSpiderTower>(position);
    MeshRenderer mesh = new GameObject().AddComponent<MeshRenderer>();
    webShootingSpider.SetMesh(mesh);

    ParticleSystem webShot = new GameObject().AddComponent<ParticleSystem>();
    webShootingSpider.SetPrimaryWebShot(webShot);

    ParticleSystem secondaryWebShot = new GameObject().AddComponent<ParticleSystem>();
    webShootingSpider.SetSecondaryWebShot(secondaryWebShot);

    return webShootingSpider;
  }

  private T CreateTower<T>(Vector3 position) where T : Tower {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    T tower = gameObject.AddComponent<T>();
    tower.AttackSpeed = 0.1f;
    tower.Damage = 0;
    tower.Priority = Targeting.Priority.LEAST_ARMOR;
    tower.ProjectileSpeed = 0.0f;

    return tower;
  }

  private GameStateManager CreateGameStateManager() {
    GameObject gameObject = new();
    return gameObject.AddComponent<GameStateManager>();
  }

  private Enemy CreateEnemy(Vector3 position) {
    GameObject gameObject = new();
    gameObject.SetActive(false);
    gameObject.transform.position = position;
    var enemy = gameObject.AddComponent<Enemy>();
    EnemyData data = new() {
      maxHP = 10.0f
    };
    enemy.Data = data;
    enemy.PrevWaypoint = new GameObject().AddComponent<Waypoint>();

    return enemy;
  }

  private TerrariumHealthUI CreateTerrariumHealthUI() {
    GameObject gameObject = new();
    return gameObject.AddComponent<TerrariumHealthUI>();
  }
  #endregion
}
