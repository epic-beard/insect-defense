using System.Collections;
using System.Collections.Generic;

public static class TowerData
{
    public enum Stat {
        damage,
        range,
        areaOfEffect,
        attackSpeed,
        armorPierce,
        armorTear,
        damageOverTime,
    }

    public enum Mode {
        multiplicative,
        special,
    }

    // Each divergent ability (usually at upgrades 3 and 5) should have its own entry in this enum.
    public enum AbilityEnum {
        AcidStun,  // Armor Tear level 3.
    }
}

public class Ability {
    TowerData.AbilityEnum abilityEnum;
    string name;
    string description;
    TowerData.Stat[] abilityStat;  // What stat this ability affects, can be multiple stats.
    TowerData.Mode mode;
    float[] mult;  // What multiplier should be applied to the stats, shuld be ordered the same as abilityStat;
    float cost;
}

public static class SpittingAnts {
    private static Dictionary<TowerData.Stat, float> startingStats;
    public static Ability[][] abilities;
}