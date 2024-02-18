using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

public class EnemyDataGenerator {
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
    dictionary["Tarantula"] = GetTarantulaEnemyData();
    dictionary["Leaf Bug"] = GetLeafBugEnemyData();
    Serialize<EnemyDictionary>(dictionary, "data.enemies");
  }

  private EnemyData GetAntEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.ANT,
      size = EnemyData.Size.SMALL,
      maxHP = 20.0f,
      maxArmor = 0.0f,
      speed = 1.0f,
      damage = 5,
      nu = 20,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
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
      spawnVariance = 3.0f,
    };

    return data;
  }

  private EnemyData GetBeetleEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.SMALL,
      maxHP = 30.0f,
      maxArmor = 10.0f,
      speed = 0.5f,
      damage = 10,
      nu = 40,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
    };

    return data;
  }

  private EnemyData GetTarantulaEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.TARANTULA,
      size = EnemyData.Size.LARGE,
      maxHP = 60.0f,
      maxArmor = 10.0f,
      speed = 0.4f,
      damage = 15,
      nu = 75,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 1.0f,
    };

    return data;
  }

  private EnemyData GetLeafBugEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.LEAF_BUG,
      size = EnemyData.Size.NORMAL,
      maxHP = 20.0f,
      maxArmor = 0.0f,
      speed = 1.0f,
      damage = 5,
      nu = 25,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
    };

    return data;
  }
}
