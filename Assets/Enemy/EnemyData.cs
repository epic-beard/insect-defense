#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public struct EnemyData {
  public enum Type {
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

  public class Spawner {
    public float num;
    public float interval;
    public EnemyData child = new();
    public override bool Equals(object obj) {
      Spawner? spawner = obj as Spawner;
      if (spawner is null) return false;
      return spawner.num == num
        && spawner.interval == interval
        && spawner.child.Equals(child);
    }
    public override int GetHashCode() => (num, interval, child).GetHashCode();
    public static bool operator ==(Spawner lhs, Spawner rhs) => lhs.Equals(rhs);
    public static bool operator !=(Spawner lhs, Spawner rhs) => !(lhs == rhs);
    public override string ToString() {
      return "Spawner:\n" + "Number: " + num + "\nInterval: " + interval
        + "\nChild: " + child;
    }
  }

  public class Carrier {
    public float num;
    public EnemyData child = new();
    public override bool Equals(object obj) {
      Carrier? carrier = obj as Carrier;
      if (carrier is null) return false;
      return carrier.num == num
        && carrier.child.Equals(child);
    }
    public override int GetHashCode() => (num, child).GetHashCode();
    public static bool operator ==(Carrier lhs, Carrier rhs) => lhs.Equals(rhs);
    public static bool operator !=(Carrier lhs, Carrier rhs) => !(lhs == rhs);
    public override string ToString() {
      return "Spawner:\n" + "Number: " + num + "\nChild: " + child;
    }
  }

  public Type type;
  public Size size;

  public float currHP;
  public float maxHP;

  public float currArmor;
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
  [XmlIgnore]
  public float acidDamagePerStackPerSecond;

  public Properties properties;
  // Once enabled the Enemy will start a spawner coroutine if spawner is not null.
  // The Spawner object contains all the information for starting this coroutine.
  public Spawner? spawner;
  // On death the Enemy will spawn a given number of child Enemies.
  // The Carrier object contains all the information required for spawning.
  public Carrier? carrier;
  public override string ToString() {
    return "EnemyData:" + "\nType: " + type + "\nSize: " + size
      + "\nCurrent HP: " + currHP + "\nMax HP" + maxHP
      + "\nCurrent Armor: " + currArmor + "\nMax Armor: " + maxArmor
      + "\nSpeed: " + speed + "\nDamage: " + damage + "\nnu: " + nu
      + "\nProperties: " + properties + "\n" + spawner + "\n" + carrier;
  }

  public void Initialize() {
    currArmor = maxArmor;
    currHP = maxHP;
    acidDamagePerStackPerSecond = 1.0f;
  }
}
