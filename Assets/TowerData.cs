using System.Collections;
using System.Collections.Generic;

public static class TowerData
{
    public enum Stat {
        DAMAGE,
        RANGE,
        AREA_OF_EFFECT,
        ATTACK_SPEED,
        ARMOR_PIERCE,
        ARMOR_TEAR,
        DAMAGE_OVER_TIME,
    }

    public enum Mode {
        MULTIPLICATIVE,
        SPECIAL,
    }

    public enum Tower {
        SPITTING_ANT_TOWER,
    }

    struct TowerInfo {
        public Dictionary<Stat, float> startingStats;
        public static Ability[][] abilities;
    }

    private static Dictionary<Tower, TowerInfo> towerInfoDic = new Dictionary<Tower, TowerInfo>();
}

public class Ability {

    // Each divergent ability (usually at upgrades 3 and 5) should have its own entry in this enum.
    public enum SpecialAbilityEnum {
        ACID_STUN,  // Armor Tear upgrade tree level 3.
    }

    // This describes a single change to an attribute.
    struct AttributeModifier {
        public TowerData.Stat attribute;
        public float mult;
    }

    SpecialAbilityEnum specialAbilityEnum;
    string name;
    string description;
    TowerData.Mode mode;
    float cost;
    AttributeModifier[] attributeModifiers;
}
