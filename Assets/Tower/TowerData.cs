#nullable enable
using System;

[Serializable]
public struct TowerData {
  public enum Type {
    ASSASSIN_BUG_TOWER,
    MANTIS_TOWER,
    SPITTING_ANT_TOWER,
    WEB_SHOOTING_SPIDER_TOWER,
  }
  public enum Stat {
    AREA_OF_EFFECT,  // The radius of area of effect effects.
    ARMOR_PIERCE,  // The amount of enemy armor each attack ignores.
    ARMOR_TEAR,  // The amount by which this tower permanently reduces enemy armor.
    ATTACK_SPEED,  // The number of attacks per second.
    COST, // Cost to build this tower.
    DAMAGE,  // The raw damage this tower inflicts per attack.
    DAMAGE_OVER_TIME,  // Damage per second.
    ENEMIES_HIT,  // How many enemies can be hit
    PROJECTILE_SPEED,  // How quickly the projectile this tower fires will move towards the targeted enemy.
    RANGE,  // The maximum range of a tower's attacks.
    SECONDARY_SLOW_POTENCY,  // The percentage of the regular slow to apply.
    SECONDARY_SLOW_TARGETS,  // The max number of targets hit by the secondary slow AoE, this should be an integar.
    SLOW_DURATION,  // Slow duration in seconds.
    SLOW_POWER,  // This is a percentage slow from 0.0 - 1.0.
    STUN_TIME,  // Stun duration in seconds.
  }

  public class Tooltip {
    public string tooltipText = "";

    public Tooltip() {}
  }

  // This class is required because list and array equality checking look at references.
  public class UpgradeTreeData {
    public string first = "";
    public string second = "";
    public string third = "";

    public TowerAbility[] firstPathUpgrades = new TowerAbility[5];
    public TowerAbility[] secondPathUpgrades = new TowerAbility[5];
    public TowerAbility[] thirdPathUpgrades = new TowerAbility[5];

    public UpgradeTreeData() {}

    public override bool Equals(object obj) {
      if (obj == null) return false;
      UpgradeTreeData? treeData = obj as UpgradeTreeData;
      if (treeData == null) return false;
      if (first.Equals(treeData.first) && second.Equals(treeData.second) && third.Equals(treeData.third)) {
        return true;
      }
      return false;
    }

    public override int GetHashCode() {
      return Tuple.Create(first, second, third).GetHashCode();
    }

    public override string ToString() {
      return "\n"
          + "first upgrade path name: " + first + "\n"
          + "second upgrade path name: " + second + "\n"
          + "third upgrade path name: " + third + "\n\n"
          + first + " path upgrades\n" + FormatUpgradeStringForToString(firstPathUpgrades) + "\n\n"
          + second + " path upgrades\n" + FormatUpgradeStringForToString(secondPathUpgrades) + "\n\n"
          + third + " path upgrades\n" + FormatUpgradeStringForToString(thirdPathUpgrades) + "\n\n";
    }

    public string FormatUpgradeStringForToString(TowerAbility[] path) {
      return string.Join("\n", path);
    }
  }

  public Type type;
  public Tooltip tooltip;
  public UpgradeTreeData upgradeTreeData;
  public string name;
  public int enemies_hit;
  public float area_of_effect;
  public float armor_pierce;
  public float armor_tear;
  public float attack_speed;
  public float cost;
  public float damage;
  public float damage_over_time;
  public float projectile_speed;
  public float range;
  public float secondary_slow_potency;
  public float secondary_slow_targets;
  public float slow_duration;
  public float slow_power;
  public float stun_time;


  public float this[Stat stat] {
    get {
      return stat switch {
        Stat.AREA_OF_EFFECT => area_of_effect,
        Stat.ARMOR_PIERCE => armor_pierce,
        Stat.ARMOR_TEAR => armor_tear,
        Stat.ATTACK_SPEED => attack_speed,
        Stat.COST => cost,
        Stat.DAMAGE => damage,
        Stat.DAMAGE_OVER_TIME => damage_over_time,
        Stat.PROJECTILE_SPEED => projectile_speed,
        Stat.RANGE => range,
        Stat.SECONDARY_SLOW_POTENCY => secondary_slow_potency,
        Stat.SECONDARY_SLOW_TARGETS => secondary_slow_targets,
        Stat.SLOW_DURATION => slow_duration,
        Stat.SLOW_POWER => slow_power,
        Stat.STUN_TIME => stun_time,
        _ => 0.0f,
      };
    }
    set {
      switch (stat) {
        case Stat.AREA_OF_EFFECT: area_of_effect = value; break;
        case Stat.ARMOR_PIERCE: armor_pierce = value; break;
        case Stat.ARMOR_TEAR: armor_tear = value; break;
        case Stat.ATTACK_SPEED: attack_speed = value; break;
        case Stat.COST: cost = value; break;
        case Stat.DAMAGE: damage = value; break;
        case Stat.DAMAGE_OVER_TIME: damage_over_time = value; break;
        case Stat.PROJECTILE_SPEED: projectile_speed = value; break;
        case Stat.RANGE: range = value; break;
        case Stat.SECONDARY_SLOW_POTENCY: secondary_slow_potency = value; break;
        case Stat.SECONDARY_SLOW_TARGETS: secondary_slow_targets = value; break;
        case Stat.SLOW_DURATION: slow_duration = value; break;
        case Stat.SLOW_POWER: slow_power = value; break;
        case Stat.STUN_TIME: stun_time = value; break;
      }
    }
  }
 }
