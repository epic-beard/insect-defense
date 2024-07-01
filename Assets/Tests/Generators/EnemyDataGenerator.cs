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
    dictionary["Ant_IL0"] = GetAntIL0EnemyData();
    dictionary["Aphid_IL0"] = GetAphidIL0EnemyData();
    dictionary["Beetle_IL0"] = GetBeetleLowInfectionEnemyData();
    dictionary["Tarantula_IL0"] = GetTarantulaIL0EnemyData();
    dictionary["Leaf Bug_IL0"] = GetLeafBugIL0EnemyData();
    Serialize<EnemyDictionary>(dictionary, "data.enemies");
  }

  private EnemyData GetAntIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.ANT,
      size = EnemyData.Size.SMALL,
      maxHP = 22.0f,
      maxArmor = 0.0f,
      speed = 0.5f,
      damage = 5,
      nu = 7,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetAphidIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 5.0f,
      maxArmor = 0.0f,
      speed = 0.5f,
      damage = 1,
      nu = 2,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 3.0f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetBeetleLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.SMALL,
      maxHP = 65.0f,
      maxArmor = 25.0f,
      speed = 0.35f,
      damage = 10,
      nu = 20,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.0f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetTarantulaIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.TARANTULA,
      size = EnemyData.Size.LARGE,
      maxHP = 250.0f,
      maxArmor = 30.0f,
      speed = 0.3f,
      damage = 25,
      nu = 100,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 1.0f,
      coagulationModifier = 0.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetLeafBugIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.LEAF_BUG,
      size = EnemyData.Size.NORMAL,
      maxHP = 35.0f,
      maxArmor = 0.0f,
      speed = 0.45f,
      damage = 8,
      nu = 10,
      properties = EnemyData.Properties.CAMO,
      spawnVariance = 2.5f,
      infectionLevel = 0,
    };

    return data;
  }
}
