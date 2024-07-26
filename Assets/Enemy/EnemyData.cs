#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public struct EnemyData {
  public enum Type {
    ANT,
    APHID,
    BEETLE,
    HERCULES_BEETLE,
    FLY,
    LEAF_BUG,
    SLUG,
    SNAIL,
    SPIDERLING,
    STINK_BUG,
    TARANTULA,
    TERMITE,
    WOLF_SPIDER,
    WOLF_SPIDER_MOTHER,
  }

  public enum Size {
    TINY,
    SMALL,
    NORMAL,
    LARGE,
    HUGE
  }

  public static Dictionary<Size, float> SizeToAcidExplosionThreshold = new() {
    {Size.TINY, 10.0f},
    {Size.SMALL, 30.0f},
    {Size.NORMAL, 50.0f},
    {Size.LARGE, 80.0f},
    {Size.HUGE, 120.0f},
  };

  public static Dictionary<Size, float> SizeToScale = new() {
    {Size.TINY, 1/2.0f },
    {Size.SMALL, 2/3.0f },
    {Size.NORMAL, 1.0f },
    {Size.LARGE, 5/3.0f},
    {Size.HUGE, 1.0f },
  };

  public static Dictionary<Size, float> SizeToCoagulation = new() {
    {Size.TINY, 1.0f},
    {Size.SMALL, 3.0f},
    {Size.NORMAL, 5.0f},
    {Size.LARGE, 8.0f},
    {Size.HUGE, 12.0f},
  };

  [Flags]
  public enum Properties {
    NONE = 0,
    CAMO = 1,
    FLYING = 2,
    CRIPPLE_IMMUNITY = 4,
    BIG_TARGET = 8,
  }

  public struct SpawnerProperties {
    public int num;
    public float interval;
    public string childKey;

    public override string ToString() {
      return "Number: " + num
        + "\nInterval: " + interval
        + "\nChild Key: " + childKey;
    }
  }

  public struct CarrierProperties {
    public int num;
    public string childKey;

    public override string ToString() {
      return "Number: " + num
        + "\nChild Key: " + childKey;
    }
  }

  public struct DazzleProperties {
    public float duration;
    public float interval;
    public float range;
  }

  public struct SlimeProperties {
    public float duration;
    public float interval;
    public float range;
    public float power;
  }

  public enum Stat {
    NONE,
    MAX_HP,
    MAX_ARMOR,
    SPEED,
    DAMAGE,
    NU,
    COAGULATION_MODIFIER,
    ACID_EXPLOSION_STACK_MODIFIER,
  }

  public Type type;
  public Size size;

  public float maxHP;
  public float maxArmor;

  public float speed;

  public float damage;
  public float nu;
  public float coagulationModifier;
  public float acidExplosionStackModifier;

  // Generally runs from 0-2 inclusive.
  public int infectionLevel;

  [XmlIgnore]
  public float slowPower;
  [XmlIgnore]
  public float slowDuration;
  [XmlIgnore]
  public float stunTime;

  [XmlIgnore]
  public float acidStacks;
  [XmlIgnore]
  public float bleedStacks;

  public Properties properties;
  // Once enabled the Enemy will start a spawner coroutine if spawner is not null.
  // The Spawner object contains all the information for starting this coroutine.
  public SpawnerProperties? spawner;
  // On death the Enemy will spawn a given number of child Enemies.
  // The Carrier object contains all the information required for spawning.
  public CarrierProperties? carrier;
  public DazzleProperties? dazzle;
  public SlimeProperties? slime;

  public float spawnVariance;
  public override string ToString() {
    return "EnemyData:" + "\nType: " + type + "\nSize: " + size
      + "\nMax HP: " + maxHP + "\nMax Armor: " + maxArmor
      + "\nBase Speed: " + speed + "\nDamage: " + damage + "\nnu: " + nu
      + "\nProperties: " + properties + "\n" + "Spawner: \n" + spawner
      + "\n" + "Carrier: \n" + carrier;
  }
}
