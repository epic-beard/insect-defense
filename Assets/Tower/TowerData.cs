using System;

[Serializable]
public struct TowerData {
  public enum Type {
    SPITTING_ANT_TOWER,
  }
  public enum Stat {
    DAMAGE,
    RANGE,
    AREA_OF_EFFECT,
    ATTACK_SPEED,
    ARMOR_PIERCE,
    ARMOR_TEAR,
    DAMAGE_OVER_TIME,
    PROJECTILE_SPEED,
    SLOW_DURATION,
    SLOW_POWER,  // This is a percentage slow from 0.0 - 1.0.
    STUN_TIME,  // Slow duration in seconds.
  }

  public float damage;
  public float range;
  public float area_of_effect;
  public float attack_speed;
  public float armor_pierce;
  public float armor_tear;
  public float damage_over_time;
  public float projectile_speed;
  public float slow_duration;
  public float slow_power;
  public float stun_time;

  public float this[Stat stat] {
    get {
      return stat switch {
        Stat.DAMAGE => damage,
        Stat.RANGE => range,
        Stat.AREA_OF_EFFECT => area_of_effect,
        Stat.ATTACK_SPEED => attack_speed,
        Stat.ARMOR_PIERCE => armor_pierce,
        Stat.ARMOR_TEAR => armor_tear,
        Stat.DAMAGE_OVER_TIME => damage_over_time,
        Stat.PROJECTILE_SPEED => projectile_speed,
        Stat.SLOW_DURATION => slow_duration,
        Stat.SLOW_POWER => slow_power,
        Stat.STUN_TIME => stun_time,
        _ => 0.0f,
      };
    }
    set {
      switch (stat) {
        case Stat.DAMAGE: damage = value; break;
        case Stat.RANGE: range = value; break;
        case Stat.AREA_OF_EFFECT: area_of_effect = value; break;
        case Stat.ATTACK_SPEED: attack_speed = value; break;
        case Stat.ARMOR_PIERCE: armor_pierce = value; break;
        case Stat.ARMOR_TEAR: armor_tear = value; break;
        case Stat.DAMAGE_OVER_TIME: damage_over_time = value; break;
        case Stat.PROJECTILE_SPEED: projectile_speed = value; break;
        case Stat.SLOW_DURATION: slow_duration = value; break;
        case Stat.SLOW_POWER: slow_power = value; break;
        case Stat.STUN_TIME: stun_time = value; break;
      }
    }
  }
 }
