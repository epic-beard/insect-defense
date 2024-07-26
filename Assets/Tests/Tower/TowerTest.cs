using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerTest {

  SpittingAntTower spittingAntTower;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    spittingAntTower = gameObject.AddComponent<SpittingAntTower>();
  }

  [Test]
  public void IsLegalUpgradeUpgradeZero([Values(0, 1, 2)] int upgradePath) {
    Assert.True(spittingAntTower.IsLegalUpgrade(upgradePath, 0));
  }

  [Test]
  public void IsLegalUpgradeUpgradeOneTwo() {
    Assert.True(spittingAntTower.IsLegalUpgrade(0, 0));
  }

  #region TestHelperMethods
  private TowerAbility CreateUpgrade() {
    TowerAbility ability = new();

    return ability;
  }

  #endregion
}

#region TowerUtils

public static class TowerUtils {
  public static void SetTargetingIndicator(this Tower tower, Transform transform) {
    typeof(Tower)
        .GetField("targetingIndicator", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(tower, transform);
  }
}

#endregion