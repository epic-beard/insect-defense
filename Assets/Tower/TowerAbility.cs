using System;
using UnityEngine.WSA;

[Serializable]
public struct TowerAbility {
  public enum Type {
    ANTI_AIR,
    CAMO_SIGHT,
    CRIPPLE,
    SLOW,
    STUN,
  }

  // Each divergent ability (usually at upgrades 3 and 5) should have its own entry in this enum.
  public enum SpecialAbility {
    NONE,
    AB_1_3_ARMORED_ENEMY_BONUS, // Assassin Bug Tower, Armor Pen upgrade tree level 3.
    AB_1_5_ARMOR_DEPLETION_BONUS, // Assassin Bug Tower, Armor Pen upgrade tree level 5.
    AB_2_3_ANTI_AIR, // Assassin Bug Tower, Utility upgrade tree level 3.
    AB_2_4_CAMO_SIGHT, // Assassin Bug Tower, Utility upgrade tree level 4.
    AB_2_5_ELASTIC_SPINERETTES, // Assassin Bug Tower, Utility upgrade tree level 5.
    AB_3_3_CONSECUTIVE_HITS, // Assassin Bug Tower, Damage upgrade tree level 3.
    AB_3_5_COMBO_FINISHER, // Assassin Bug Tower, Damage upgrade tree level 5.
    M_1_3_DOUBLE_SLASH,  // Mantis Tower, Damage upgrade tree level 3.
    M_1_5_FOUR_ARMS,  // Mantis Tower, Damage upgrade tree level 5.
    M_2_3_JAGGED_CLAWS,  // Mantis Tower, Debilitation upgrade tree level 3.
    M_2_5_BLOODY_EXECUTION, // Mantis Tower, Debilitation upgrade tree level 5.
    M_3_2_CAMO_SIGHT,  // Mantis Tower, Utility upgrade tree level 3.
    M_3_5_SHRIKE,  // Mantis Tower, Utility upgrade tree level 5.
    SA_1_3_ACIDIC_SYNERGY,  // Spitting Ant Tower, Armor Tear upgrade tree level 3.
    SA_1_5_VENOM_CORPSEPLOSION,  // Spitting Ant Tower, Armor Tear upgrade tree level 5.
    SA_2_3_AOE_ACID,  // Spitting Ant Tower, Acid DoT upgrade tree level 3.
    SA_2_5_DOT_ENHANCEMENT,  // Spitting Ant Tower, Acid DoT upgrade tree level 5.
    SA_3_3_ANTI_AIR,  // Spitting Ant Tower, Utility upgrade tree level 3.
    SA_3_5_CONSTANT_FIRE,  // Spitting Ant Tower, Utility upgrade tree level 5.
    WSS_1_3_SLOW_STUN,  // Web Shooting Spider Tower, Slow upgrade tree level 3.
    WSS_1_5_PERMANENT_SLOW,  // Web Shooting Spider Tower, Slow upgrade tree level 5.
    WSS_2_3_IMPROVE_AOE,  // Web Shooting Spider Tower, AoE upgrade tree level 3.
    WSS_2_5_LINGERING_SLOW,  // Web Shooting Spider Tower, AoE upgrade tree level 5.
    WSS_3_3_ANTI_AIR,  // Web Shooting Spider Tower, Utility upgrade tree level 3.
    WSS_3_5_GROUNDING_SHOT,  // Web Shooting Spider Tower, Utility upgrade tree level 5.
  }

  public enum Mode {
    MULTIPLICATIVE,
    ADDITIVE,
    SET,
  }

  // This describes a single change to an attribute.
  [Serializable]
  public struct AttributeModifier<T> {
    public TowerData.Stat attribute;
    public Mode mode;
    public T mod;
  }

  public SpecialAbility specialAbility;
  public AttributeModifier<float>[] floatAttributeModifiers;
  public AttributeModifier<int>[] intAttributeModifiers;
  public string name;
  public string description;
  public int upgradePath;
  public int cost;

  public override string ToString() {
    return "Name: " + name + ", description: " + description
        + ", upgrade path: " + upgradePath + ", cost: " + cost;
  }
}
