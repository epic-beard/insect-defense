using System.IO;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

public class EnemyDataManagerTest {
  readonly string enemyName = "Beetle";
  EnemyData enemyData;

  [SetUp]
  public void SetUp() {
    enemyData = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.NORMAL,
      maxHP = 1,
      maxArmor = 2,
      speed = 3.5f,
      damage = 4,
      nu = 5,
      properties = EnemyData.Properties.CAMO,
      spawner = new() {
        num = 6,
        interval = 7.5f,
        childKey = "child",
      },
      carrier = new() {
        num = 8,
        childKey = "child",
      },
    };
  }

  // Round trip an enemy data through serialization then deserialization.
 [Test]
  public void SerializationWorks() {
    EnemyDictionary expected = new() {
      { enemyName, enemyData }
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expected, writer);

    memStream.Position = 0;

    EnemyDictionary actual = Deserialize<EnemyDictionary>(memStream);
    CollectionAssert.AreEquivalent(actual, expected);
  }
}