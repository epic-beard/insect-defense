using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using UnityEngine;
using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

public class EnemyDataGenerator : MonoBehaviour {
  // Comment this out when not in use.
  [Test]
  public void EnemyDataGeneratorTest() {
    // This does the enemy data generation, it is not a test.
    GenerateEnemyData();
  }

  private void GenerateEnemyData() {
    EnemyDictionary dictionary = new();
    dictionary["Ant"] = GetAntEnemyData();
    dictionary["Aphid"] = GetAphidEnemyData();
    dictionary["Beetle"] = GetBeetleEnemyData();
    Serialize<EnemyDictionary>(dictionary, "data.enemies");
  }

  private EnemyData GetAntEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.ANT,
      size = EnemyData.Size.SMALL,
      maxHP = 20.0f,
      maxArmor = 1.0f,
      speed = 1.0f,
      damage = 5,
      nu = 20,
      properties = EnemyData.Properties.NONE,
    };

    return data;
  }

  private EnemyData GetAphidEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 5.0f,
      maxArmor = 0.0f,
      speed = 1.0f,
      damage = 1,
      nu = 4,
      properties = EnemyData.Properties.NONE,
    };

    return data;
  }

  private EnemyData GetBeetleEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.SMALL,
      maxHP = 30.0f,
      maxArmor = 5.0f,
      speed = 0.5f,
      damage = 10,
      nu = 40,
      properties = EnemyData.Properties.NONE,
    };

    return data;
  }
}
