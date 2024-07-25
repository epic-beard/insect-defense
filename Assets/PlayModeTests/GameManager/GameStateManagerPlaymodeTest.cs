using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateManagerPlaymodeTest {
  GameStateManager gameStateManager;
  TowerManager towerManager;

  [SetUp]
  public void SetUp() {
    gameStateManager = CreateGameStateManager();
    
    towerManager = new GameObject().AddComponent<TowerManager>();
    TowerManager.Instance = towerManager;

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = objectPool;
  }

  [UnityTest]
  public IEnumerator BuildTowerWorks() {
    Waypoint waypoint = CreateWaypoint();

    Assert.That(towerManager.BuildTower(waypoint), Is.False);

    TowerManager.Instance.SelectedTowerType = TowerData.Type.SPITTING_ANT_TOWER;

    Assert.That(towerManager.BuildTower(waypoint), Is.True);

    yield return new WaitForSeconds(towerManager.GetBuildDelay());

    Assert.That(towerManager.TowerPrices[TowerData.Type.SPITTING_ANT_TOWER].Count, Is.EqualTo(1));
  }

  #region TestHelperMethods

  private GameStateManager CreateGameStateManager() {
    GameObject gameObject = new();
    return gameObject.AddComponent<GameStateManager>();
  }

  private Waypoint CreateWaypoint() {
    return new GameObject().AddComponent<Waypoint>();
  }

  private GameObject CreateGameObjectWithSpittingAntTower() {
    GameObject gameObject = new();

    SpittingAntTower spittingAntTower = gameObject.AddComponent<SpittingAntTower>();
    spittingAntTower.Priority = Targeting.Priority.LEAST_ARMOR;
    spittingAntTower.ProjectileSpeed = 1.0f;

    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    spittingAntTower.SetUpperMesh(upperMesh);

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetProjectile(splash);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    spittingAntTower.SetBeam(beam);

    return gameObject;
  }

  #endregion

}
