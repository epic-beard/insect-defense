using System.IO;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using AbilityDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerAbility[][]>;
using System.Linq;

public class TowerDataManagerTest {
  TowerData towerData;
  TowerAbility[][] abilities;

  [SetUp]
  public void SetUp() {
    towerData = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      area_of_effect = 1.0f,
      armor_pierce = 2.0f,
      armor_tear = 3.0f,
      attack_speed = 4.0f,
      damage = 5.0f,
      damage_over_time = 6.0f,
      projectile_speed = 7.0f,
      range = 8.0f,
      slow_duration = 9.0f,
      slow_power = 10.0f,
      stun_time = 11.0f,
    };

    TowerAbility.AttributeModifier modifier = new() {
      attribute = TowerData.Stat.RANGE,
      mode = TowerAbility.Mode.MULTIPLICATIVE,
      mod = 2.0f,
    };

    TowerAbility ability = new() {
      specialAbility = TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN,
      attributeModifiers = new TowerAbility.AttributeModifier[] {
        modifier,
      },
      upgradePath = 3,
      name = "ability",
      description = "an ability",
      cost = 4,
    };
    abilities = new TowerAbility[1][] { new TowerAbility[]{ ability } };
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
    Assert.That(expected.attributeModifiers, Is.EqualTo(actual.attributeModifiers));
    Assert.That(expected.upgradePath, Is.EqualTo(actual.upgradePath));
    Assert.That(expected.name, Is.EqualTo(actual.name));
    Assert.That(expected.description, Is.EqualTo(actual.description));
    Assert.That(expected.cost, Is.EqualTo(actual.cost));
  }
}