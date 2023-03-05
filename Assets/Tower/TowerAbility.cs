using System;

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
    SA_1_3_ARMOR_TEAR_STUN,      // Spitting Ant Armor Tear upgrade tree level 3.
    SA_1_5_ARMOR_TEAR_EXPLOSION,  // Spitting Ant Armor Tear upgrade tree level 5.
    SA_2_3_DOT_SLOW,       // Spitting Ant Acit DoT upgrade tree level 3.
    SA_2_5_DOT_EXPLOSION,    // Spitting Ant Acid DoT upgrade tree level 5.
    SA_3_3_ANTI_AIR,     // Spitting Ant Utility upgrade tree level 3.
    SA_3_5_CONSTANT_FIRE,    // Spitting Ant Utility upgrade tree level 5.
  }

  public enum Mode {
    MULTIPLICATIVE,
    SPECIAL,
  }

  // This describes a single change to an attribute.
  public struct AttributeModifier {
    public TowerData.Stat attribute;
    public float mult;
  }

  public Mode mode;
  public SpecialAbility specialAbility;
  public AttributeModifier[] attributeModifiers;
  public int upgradePath;
  public string name;
  public string description;
  public int cost;
}
