#nullable enable
using System;
using System.Xml.Serialization;

[Serializable]
public struct EnemyData {
  public enum Type {
    ANT,
    APHID,
    BEETLE
  }

  public enum Size {
    TINY = 1,
    SMALL = 2,
    NORMAL = 4,
    LARGE = 8,
    COLOSSAL = 12
  }

  [Flags]
  public enum Properties {
    NONE = 0,
    CAMO = 1,
    FLYING = 2,
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

  public Type type;
  public Size size;

  public float maxHP;
  public float maxArmor;

  public float speed;

  public int damage;
  public int nu;

  [XmlIgnore]
  public float slowPower;
  [XmlIgnore]
  public float slowDuration;
  [XmlIgnore]
  public float stunTime;

  [XmlIgnore]
  public float acidStacks;

  public Properties properties;
  // Once enabled the Enemy will start a spawner coroutine if spawner is not null.
  // The Spawner object contains all the information for starting this coroutine.
  public SpawnerProperties? spawner;
  // On death the Enemy will spawn a given number of child Enemies.
  // The Carrier object contains all the information required for spawning.
  public CarrierProperties? carrier;
  public override string ToString() {
    return "EnemyData:" + "\nType: " + type + "\nSize: " + size
      + "\nMax HP" + maxHP + "\nMax Armor: " + maxArmor
      + "\nSpeed: " + speed + "\nDamage: " + damage + "\nnu: " + nu
      + "\nProperties: " + properties + "\n" + "Spawner:\n" + spawner
      + "\n" + "Carrier:\n" + carrier;
  }
}
