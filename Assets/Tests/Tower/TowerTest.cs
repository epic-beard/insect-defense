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
    // Upgrade all trees to level 1.
    spittingAntTower.Upgrade(CreateUpgrade(0));
    spittingAntTower.Upgrade(CreateUpgrade(1));
    spittingAntTower.Upgrade(CreateUpgrade(2));

    // Check legality before, and after, buying 2 level 2 upgrades.
    Assert.True(spittingAntTower.IsLegalUpgrade(0, 1));
    Assert.True(spittingAntTower.IsLegalUpgrade(1, 1));
    spittingAntTower.Upgrade(CreateUpgrade(0));
    spittingAntTower.Upgrade(CreateUpgrade(1));
    Assert.False(spittingAntTower.IsLegalUpgrade(2, 1));

    // Check legality of level 3 upgrades.
    Assert.True(spittingAntTower.IsLegalUpgrade(0, 2));
    Assert.True(spittingAntTower.IsLegalUpgrade(1, 2));
    Assert.False(spittingAntTower.IsLegalUpgrade(2, 2));
  }

  [Test]
  public void IsLegalUpgradeUpgradeThreeFour() {
    // Upgrade the first two trees to level 3.
    spittingAntTower.Upgrade(CreateUpgrade(0));
    spittingAntTower.Upgrade(CreateUpgrade(1));
    spittingAntTower.Upgrade(CreateUpgrade(0));
    spittingAntTower.Upgrade(CreateUpgrade(1));
    spittingAntTower.Upgrade(CreateUpgrade(0));
    spittingAntTower.Upgrade(CreateUpgrade(1));

    // Check legality before, and after, buying a level 4 upgrade.
    Assert.True(spittingAntTower.IsLegalUpgrade(0, 3));
    Assert.True(spittingAntTower.IsLegalUpgrade(1, 3));
    spittingAntTower.Upgrade(CreateUpgrade(0));
    Assert.False(spittingAntTower.IsLegalUpgrade(1, 3));

    // Check legality of level 5 upgrades.
    Assert.True(spittingAntTower.IsLegalUpgrade(0, 4));
    Assert.False(spittingAntTower.IsLegalUpgrade(1, 4));
  }

  #region TestHelperMethods
  private TowerAbility CreateUpgrade(int upgradePath) {
    TowerAbility ability = new();

    ability.specialAbility = TowerAbility.SpecialAbility.NONE;
    ability.floatAttributeModifiers = new TowerAbility.AttributeModifier<float>[1];
    ability.floatAttributeModifiers[0].attribute = TowerData.Stat.DAMAGE;
    ability.floatAttributeModifiers[0].mode = TowerAbility.Mode.SET;
    ability.floatAttributeModifiers[0].mod = 0.0f;
    ability.intAttributeModifiers = null;
    ability.upgradePath = upgradePath;

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