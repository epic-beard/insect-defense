using System.Collections.Generic;

// TODO: Rename this to TowerDataManager.
public static class TowerData {
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

  public enum Mode {
    MULTIPLICATIVE,
    SPECIAL,
  }

  public enum Tower {
    SPITTING_ANT_TOWER,
  }

  public enum TowerAbility {
    ANTI_AIR,
    CAMO_SIGHT,
    CRIPPLE,
    SLOW,
    STUN,
  }

  public struct TowerInfo {
    public Dictionary<Stat, float> startingStats;
    public Ability[][] abilities;

    public TowerInfo(Dictionary<Stat, float> startingStats, Ability[][] abilities) {
      this.startingStats = startingStats;
      this.abilities = abilities;
    }
  }

  private static Dictionary<Tower, TowerInfo> towerInfoDic = new();
}

public class Ability {

  // Each divergent ability (usually at upgrades 3 and 5) should have its own entry in this enum.
  public enum SpecialAbilityEnum {
    SA_1_3_ARMOR_TEAR_STUN,  // Spitting Ant, Armor Tear upgrade tree level 3.
    SA_1_5_ARMOR_TEAR_EXPLOSION,  // Spitting Ant, Armor Tear upgrade tree level 5.
    SA_2_3_DOT_SLOW,  // Spitting Ant, Acit DoT upgrade tree level 3.
    SA_2_5_DOT_EXPLOSION,  // Spitting Ant, Acid DoT upgrade tree level 5.
    SA_3_3_ANTI_AIR,  // Spitting Ant, Utility upgrade tree level 3.
    SA_3_5_CONSTANT_FIRE,  // Spitting Ant, Utility upgrade tree level 5.
    WSS_1_3_SLOW_STUN,  // Web Shooting Spider, Improved Slow upgrade tree level 3.
    WSS_1_5_PERMANENT_SLOW,  // Web Shooting Spider, Improved Slow upgrade tree level 5.
    WSS_2_3_AREA_SLOW,  // Web Shooting Spider, AoE upgrade tree level 3.
    WSS_2_5_LINGERING_SLOW,  // Web Shooting Spider, AoE upgrade tree level 5.
    WSS_3_3_ANTI_AIR,  // Web Shooting Spider, Utility tree level 3.
    WSS_3_5_AA_ASSISTANCE,  // Web Shooting Spider, Utility tree level 5.
  }

  // This describes a single change to an attribute.
  public struct AttributeModifier {
    public TowerData.Stat attribute;
    public float mult;

    public AttributeModifier(TowerData.Stat attribute, float mult) {
      this.attribute = attribute;
      this.mult = mult;
    }
  }

  // TODO: Make SpecialAbilityEnum nullable and remove mode entirely.
  public TowerData.Mode Mode { get; private set; }
  public SpecialAbilityEnum SpecialAbility { get; private set; }
  public AttributeModifier[] AttributeModifiers { get; private set; }
  private int upgradePath;
  public int UpgradePath {
    get { return upgradePath; }
    set {
      if (0 <= value && value <= 2) upgradePath = value;
      else UnityEngine.Debug.Log("Access attempt to upgrade paths out of bounds.");
    }
  }

  string name;
  string description;
  float cost;
}
