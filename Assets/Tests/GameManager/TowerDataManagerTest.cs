using System.IO;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using AbilityDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerAbility[][]>;

public class TowerDataManagerTest {
  TowerData towerData;
  TowerAbility[,] abilities;

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
}
