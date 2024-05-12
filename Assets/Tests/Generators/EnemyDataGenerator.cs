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
    dictionary["Ant"] = GetAntLowInfectionEnemyData();
    dictionary["Aphid"] = GetAphidLowInfectionEnemyData();
    dictionary["SlowAphid"] = GetSlowAphidLowInfectionEnemyData();
    dictionary["Beetle"] = GetBeetleLowInfectionEnemyData();
    dictionary["Tarantula"] = GetTarantulaLowInfectionEnemyData();
    dictionary["Leaf Bug"] = GetLeafBugLowInfectionEnemyData();
    Serialize<EnemyDictionary>(dictionary, "data.enemies");
  }

  private EnemyData GetAntLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.ANT,
      size = EnemyData.Size.SMALL,
      maxHP = 20.0f,
      maxArmor = 0.0f,
      speed = 0.5f,
      damage = 5,
      nu = 5,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetAphidLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 5.0f,
      maxArmor = 0.0f,
      speed = 0.5f,
      damage = 1,
      nu = 1,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 3.0f,
      infectionLevel = 0,
    };

    return data;
  }

  // TODO(nnewsom): Handle this with overrides.
  private EnemyData GetSlowAphidLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 5.0f,
      maxArmor = 0.0f,
      speed = 0.25f,
      damage = 1,
      nu = 1,
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
      maxHP = 30.0f,
      maxArmor = 10.0f,
      speed = 0.25f,
      damage = 10,
      nu = 10,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetTarantulaLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.TARANTULA,
      size = EnemyData.Size.LARGE,
      maxHP = 100.0f,
      maxArmor = 30.0f,
      speed = 0.3f,
      damage = 15,
      nu = 50,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 1.0f,
      coagulationModifier = 0.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetLeafBugLowInfectionEnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.LEAF_BUG,
      size = EnemyData.Size.NORMAL,
      maxHP = 20.0f,
      maxArmor = 0.0f,
      speed = 0.45f,
      damage = 5,
      nu = 6,
      properties = EnemyData.Properties.CAMO,
      spawnVariance = 2.5f,
      infectionLevel = 0,
    };

    return data;
  }
}
