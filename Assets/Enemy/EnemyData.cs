#nullable enable
using System;

public class EnemyData {
  public enum EnemyType {
    BEETLE
  }

  [Flags]
  public enum EnemyProperties {
    NONE = 0,
    CAMO = 1,
    FLYING = 2,
  }

  public class Spawner {
    public float num;
    public float interval;
    public EnemyData child = new();
  }

  public class Carrier {
    public float num;
    public EnemyData child = new();
  }

  public EnemyType type;
  public int HitPoints;
  public float Armor;
  public float speed;
  public int damage;
  public int nu;

  public EnemyProperties properties;
  public Spawner? spawner;
  public Carrier? carrier;
}
