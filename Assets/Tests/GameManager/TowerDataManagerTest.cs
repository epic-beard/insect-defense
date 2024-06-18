using System.IO;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using AbilityDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerAbility[][]>;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using System.Reflection;

public class TowerDataManagerTest {
  TowerManager tm;
  TowerData towerData;
  TowerAbility[][] abilities;

  [SetUp]
  public void SetUp() {
    tm = new GameObject().AddComponent<TowerManager>();

    towerData = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      upgradeTreeData = new() {
        first = "first",
        second = "second",
        third = "third"
      },
      area_of_effect = 1.0f,
      armor_pierce = 2.0f,
      attack_speed = 4.0f,
      cost = 10,
      damage = 5.0f,
      acid_stacks = 6,
      projectile_speed = 7.0f,
      range = 8.0f,
      slow_duration = 9.0f,
      slow_power = 10.0f,
      stun_time = 11.0f,

    };

    TowerAbility.AttributeModifier<float> modifier = new() {
      attribute = TowerData.Stat.RANGE,
      mode = TowerAbility.Mode.MULTIPLICATIVE,
      mod = 2.0f,
    };

    TowerAbility ability = new() {
      specialAbility = TowerAbility.SpecialAbility.SA_1_3_ACIDIC_SYNERGY,
      floatAttributeModifiers = new TowerAbility.AttributeModifier<float>[] {
        modifier,
      },
      upgradePath = 3,
      name = "ability",
      description = "an ability",
      cost = 4,
    };
    abilities = new TowerAbility[1][] { new TowerAbility[] { ability } };
  }

  [Test]
  public void GetTowerCostWorks() {
    Assert.That(tm.GetTowerCost(towerData), Is.EqualTo(10));
    tm.IncrementTowerCounts(towerData);
    Assert.That(tm.GetTowerCost(towerData), Is.EqualTo(12));
    tm.IncrementTowerCounts(towerData);
    Assert.That(tm.GetTowerCost(towerData), Is.EqualTo(14));
  }

  // Round trip a TowerData object.
  [Test]
  public void TowerDataSerializes() {
    TowerDictionary expected = new() {
      {TowerData.Type.SPITTING_ANT_TOWER, towerData},
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expected, writer);

    memStream.Position = 0;

    TowerDictionary actual = Deserialize<TowerDictionary>(memStream);

    CollectionAssert.AreEqual(expected, actual);
  }

  // Round trip a TowerAbility multidimensional array.
  [Test]
  public void TowerAbilitySerializes() {
    AbilityDictionary expectedDictionary = new() {
      {TowerData.Type.SPITTING_ANT_TOWER, abilities},
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expectedDictionary, writer);

    memStream.Position = 0;

    AbilityDictionary actualDictionary = Deserialize<AbilityDictionary>(memStream);

    Assert.That(actualDictionary.Count, Is.EqualTo(1));
    TowerAbility expected = expectedDictionary[TowerData.Type.SPITTING_ANT_TOWER][0][0];
    TowerAbility actual = actualDictionary[TowerData.Type.SPITTING_ANT_TOWER][0][0];

    Assert.That(expected.specialAbility, Is.EqualTo(actual.specialAbility));
    Assert.That(expected.floatAttributeModifiers, Is.EqualTo(actual.floatAttributeModifiers));
    Assert.That(expected.intAttributeModifiers, Is.EqualTo(actual.intAttributeModifiers));
    Assert.That(expected.upgradePath, Is.EqualTo(actual.upgradePath));
    Assert.That(expected.name, Is.EqualTo(actual.name));
    Assert.That(expected.description, Is.EqualTo(actual.description));
    Assert.That(expected.cost, Is.EqualTo(actual.cost));
  }
}

#region TowerManagerUtils
public static class TowerManagerUtils {

  public static float GetBuildDelay(this TowerManager stateManager) {
    return (float)typeof(TowerManager)
        .GetField("buildDelay", BindingFlags.Instance | BindingFlags.NonPublic)
        .GetValue(stateManager);
  }
  public static void IncrementTowerCounts(this TowerManager towerManager, TowerData data) {
    var towerPrices = towerManager.TowerPrices;
    if (!towerPrices.ContainsKey(data.type)) {
      towerPrices.Add(data.type, new());
    }
    towerPrices[data.type].Push(towerManager.GetTowerCost(data));
  }
}
#endregion
