using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

[TestFixture]
public class GameStateManagerTest {
  GameStateManager gsm;
  GameObject sat;

  [SetUp]
  public void SetUp() {
    gsm = new GameObject().AddComponent<GameStateManager>();
    sat = new GameObject();
    sat.AddComponent<SpittingAntTower>();

    GameStateManager.SelectedTowerType = sat;
  }

  [Test]
  public void GetTowerCostWorks() {
    var comparer = new FloatEqualityComparer(10e-6f);
    TowerData.Type type = TowerData.Type.SPITTING_ANT_TOWER;
    Assert.That(gsm.GetTowerCost(type, 10.0f), Is.EqualTo(10.0f).Using(comparer));
    gsm.IncrementTowerCounts(type, 10);
    Assert.That(gsm.GetTowerCost(type, 10.0f), Is.EqualTo(12.0f).Using(comparer));
    gsm.IncrementTowerCounts(type, 10);
    Assert.That(gsm.GetTowerCost(type, 10.0f), Is.EqualTo(14.0f).Using(comparer));
  }
}

#region GameStateManagerUtils
public static class GameStateManagerUtils {

  public static void InvokeAddTower(this GameStateManager stateManager, Vector2Int coordinates, Tower tower) {
    object[] args = { coordinates, tower };
    Type[] argTypes = { typeof(Vector2Int), typeof(Tower) };
    MethodInfo addTower = typeof(GameStateManager).GetMethod(
      "AddTower",
      BindingFlags.NonPublic | BindingFlags.Instance,
      null, CallingConventions.Standard, argTypes, null);
    addTower.Invoke(stateManager, args);
  }

  public static void IncrementTowerCounts(this GameStateManager stateManager, TowerData.Type type, int cost) {
    var towerPrices = stateManager.TowerPrices;
    if (!towerPrices.ContainsKey(type)) {
      towerPrices.Add(type, new());
    }
    towerPrices[type].Push(stateManager.GetTowerCost(type, cost));
  }
}
#endregion