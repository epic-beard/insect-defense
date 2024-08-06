using NUnit.Framework;
using static LevelGeneratorStatics;
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
    dictionary[ant0] = GetAntIL0EnemyData();
    dictionary[ant1] = GetAntIL1EnemyData();
    dictionary[aphid0] = GetAphidIL0EnemyData();
    dictionary[aphid1] = GetAphidIL1EnemyData();
    dictionary[beetle0] = GetBeetleIL0EnemyData();
    dictionary[beetle1] = GetBeetleIL1EnemyData();
    dictionary[fly0] = GetFlyIL0EnemyData();
    dictionary[leafBug0] = GetLeafBugIL0EnemyData();
    dictionary[spiderling0] = GetSpiderlingIL0EnemyData();
    dictionary[tarantula0] = GetTarantulaIL0EnemyData();
    dictionary[wolfSpider0] = GetWolfSpiderIL0EnemyData();
    dictionary[wolfSpiderMother0] = GetWolfSpiderMotherIL0EnemyData();
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

  private EnemyData GetAntIL1EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.ANT,
      size = EnemyData.Size.SMALL,
      maxHP = 65.0f,
      maxArmor = 0.0f,
      speed = 0.65f,
      damage = 8,
      nu = 15,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.5f,
      infectionLevel = 1,
    };

    return data;
  }

  private EnemyData GetAphidIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 5.0f,
      maxArmor = 0.0f,
      speed = 0.65f,
      damage = 1,
      nu = 2,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 3.0f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetAphidIL1EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.APHID,
      size = EnemyData.Size.TINY,
      maxHP = 13.0f,
      maxArmor = 0.0f,
      speed = 0.65f,
      damage = 2,
      nu = 4,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 3.0f,
      infectionLevel = 1,
    };

    return data;
  }

  private EnemyData GetBeetleIL0EnemyData() {
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

  private EnemyData GetBeetleIL1EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.SMALL,
      maxHP = 200.0f,
      maxArmor = 25.0f,
      speed = 0.45f,
      damage = 15,
      nu = 50,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 2.0f,
      infectionLevel = 1,
    };

    return data;
  }
  
  private EnemyData GetFlyIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.FLY,
      size = EnemyData.Size.SMALL,
      maxHP = 45.0f,
      maxArmor = 0.0f,
      speed = 0.75f,
      damage = 10,
      nu = 20,
      properties = EnemyData.Properties.FLYING,
      spawnVariance = 2.0f,
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

  private EnemyData GetWolfSpiderIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.WOLF_SPIDER,
      size = EnemyData.Size.NORMAL,
      maxHP = 120.0f,
      maxArmor = 25.0f,
      speed = 0.7f,
      damage = 10,
      nu = 65,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 1.5f,
      coagulationModifier = 0.5f,
      infectionLevel = 0,
    };

    return data;
  }

  private EnemyData GetWolfSpiderMotherIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.WOLF_SPIDER_MOTHER,
      size = EnemyData.Size.NORMAL,
      maxHP = 120.0f,
      maxArmor = 25.0f,
      speed = 0.7f,
      damage = 10,
      nu = 65,
      properties = EnemyData.Properties.NONE,
      carrier = new() {
        childKey = LevelGeneratorStatics.spiderling0,
        num = 5,
      },
      spawnVariance = 1.5f,
      coagulationModifier = 0.5f,
      infectionLevel = 0,
    };

    return data;
  }

  // TODO(emonzon): Replace the enemy type and model with appropriate spiderling data once it is made.
  private EnemyData GetSpiderlingIL0EnemyData() {
    EnemyData data = new() {
      type = EnemyData.Type.SPIDERLING,
      size = EnemyData.Size.TINY,
      maxHP = 10.0f,
      maxArmor = 0.0f,
      speed = 0.7f,
      damage = 2,
      nu = 4,
      properties = EnemyData.Properties.NONE,
      spawnVariance = 3.0f,
      coagulationModifier = 0.5f,
      infectionLevel = 0,
    };

    return data;
  }
}
