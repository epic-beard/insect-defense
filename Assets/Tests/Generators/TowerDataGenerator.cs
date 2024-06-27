using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using static TowerData;
using static TowerAbility;
public class TowerDataGenerator {
  // Comment this out when not in use.
  [Test]
  public void TowerDataGeneratorTest() {
    // This does the tower data generation, it is not a test.
    GenerateTowerData();
  }

  private void GenerateTowerData() {
    TowerDictionary dictionary = new();
    dictionary[TowerData.Type.SPITTING_ANT_TOWER] = GetSpittingAntTowerData();
    dictionary[TowerData.Type.WEB_SHOOTING_SPIDER_TOWER] = GetWebShootingSpiderTowerData();
    dictionary[TowerData.Type.ASSASSIN_BUG_TOWER] = GetAssassinBugTowerData();
    dictionary[TowerData.Type.MANTIS_TOWER] = GetMantisTowerData();
    Serialize<TowerDictionary>(dictionary, "data.towers");
  }

  private TowerData GetSpittingAntTowerData() {
    var venomUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.VENOM_POWER, Mode.SET, 0.25f),
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.15f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.VENOM_STACKS, Mode.SET, 1),
        },
        name = "Venom!",
        description = "Attacks apply 1 stack of 25% venom and inflict 15% extra damage.",
        upgradePath = 0,
        cost = 50,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.VENOM_POWER, Mode.SET, 0.4f),
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.25f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.VENOM_STACKS, Mode.SET, 2),
        },
        name = "More venom",
        description = "Attacks apply 2 stacks of 40% venom and inflict 25% extra damage.",
        upgradePath = 0,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_3_ACIDIC_SYNERGY,
        name = "Acid Breakdown",
        description = "Within 30 radius of the tower, acid benefits from venom without consuming stacks.",
        upgradePath = 0,
        cost = 800,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.VENOM_POWER, Mode.SET, 0.6f),
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.VENOM_STACKS, Mode.SET, 3),
        },
        name = "Max Venom",
        description = "Attacks apply 3 stacks of 60% venom.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_5_VENOM_CORPSEPLOSION,
        name = "Armor Tear Explosion",
        description = "Armor tear now applies to all enemies caught in the splash explosion.",
        upgradePath = 0,
        cost = 10,
      }
    };
    var acidUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.15f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.ACID_STACKS, Mode.SET, 5),
        },
        name = "Acid!",
        description = "Attacks inflict 5 stacks of acid and grants a 15% bonus to damage.",
        upgradePath = 1,
        cost = 50,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.25f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.ACID_STACKS, Mode.ADDITIVE, 5),
        },
        name = "More Acid",
        description = "Acid stacks inflicted is raised to 10 and grants a 25% bonus to damage.",
        upgradePath = 1,
        cost = 150,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.AREA_OF_EFFECT, Mode.SET, 5.0f)
        },
        specialAbility = SpecialAbility.SA_2_3_AOE_ACID,
        name = "AoE Acid",
        description = "Attacks inflict 1/2 (rounded down, min 1) acid stacks within 5 of the target.",
        upgradePath = 1,
        cost = 800,
      },
      new() {
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.ACID_STACKS, Mode.ADDITIVE, 5)
        },
        name = "Spray that acid",
        description = "Acid inflicted is raised to 15",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_5_DOT_ENHANCEMENT,
        name = "Acid Explosion",
        description = "When acid stacks reach max, an explosion of acid is triggered, applying all acid stacks instantly and clearing them.",
        upgradePath = 1,
        cost = 10,
      }
    };
    var utilityUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.RANGE, Mode.ADDITIVE, 5.0f),
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.ADDITIVE, 0.1f)
        },
        name = "Nozzle Control",
        description = "Attacks gain 5 range and 10 attack speed.",
        upgradePath = 2,
        cost = 50,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.RANGE, Mode.ADDITIVE, 5.0f),
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.ADDITIVE, 0.15f)
        },
        name = "Shot Production Speed",
        description = "Attacks gain 5 more range and 15 more attack speed.",
        upgradePath = 2,
        cost = 250,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_3_ANTI_AIR,
        name = "Elevated Shots",
        description = "The tower gains the anti-air ability.",
        upgradePath = 2,
        cost = 800,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Even Faster Production",
        description = "Increases attack speed by 50%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_5_CONSTANT_FIRE,
        name = "Continuous Stream",
        description = "This tower's attack becomes single-target and constantly fires",
        upgradePath = 2,
        cost = 10,
      }
    };

    TowerData data = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      upgradeTreeData = new() {
        first = "Venom",
        second = "Acid",
        third = "Utility",
        firstPathUpgrades = venomUpgrades,
        secondPathUpgrades = acidUpgrades,
        thirdPathUpgrades = utilityUpgrades,
      },

      tooltip = new() {
        tooltipText = "A ranged tower with moderate damage, armor tear, and acid DoTs.",
      },

      name = "Spitting Ant Tower",
      icon_path = "Icons/test",
      area_of_effect = 0,
      armor_pierce = 0,
      attack_speed = 0.5f,
      cost = 100,
      damage = 10,
      acid_stacks = 0,
      projectile_speed = 100,
      range = 20,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0.0f,
      stun_time = 0,
      venom_power = 0,
      venom_stacks = 0,
    };

    return data;
  }

  private TowerData GetWebShootingSpiderTowerData() {
    var slowUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.ADDITIVE, 2.0f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.ADDITIVE, 0.1f)
        },
        name = "Improved Slow",
        description = "Increasses slow power to 40% and duration to 6 seconds.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.ADDITIVE, 2.0f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.ADDITIVE, 0.1f)
        },
        name = "Improved Slow",
        description = "Increase slow power to 50% and duration to 8 seconds.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.WSS_1_3_SLOW_STUN,
        name = "Stunning Slow",
        description = "Stuns an enemy the first time it is hit by this tower.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.ADDITIVE, 2.0f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.ADDITIVE, 0.1f)
        },
        name = "Improved Slow",
        description = "Increase slow power to 60% and duration to 10 seconds.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.WSS_1_5_PERMANENT_SLOW,
        name = "Enduring Webs",
        description = "Permanently reduces enemy speed the first time it is slowed.  This effect is applied once per enemy per tower.",
        upgradePath = 0,
        cost = 10,
      }
    };
    var secondarySlowUpgrades = new TowerAbility[] {
      new() {
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1)
        },
        name = "AoE Slow",
        description = "1/2 slow also hits the enemy nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1)
        },
        name = "AoE Slow",
        description = "1/2 slow also hits the 2 enemies nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_POTENCY, Mode.SET, 0.75f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1),
        },
        name = "Superior Secondary Threads",
        description = "3/4 slow also hits the 3 enemies nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_POTENCY, Mode.SET, 1.0f)
        },
        name = "Perfect Secondary Threads",
        description = "100% slow also hits the 3 enemies nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.WSS_2_5_LINGERING_SLOW,
        name = "Enduring Threads",
        description = "Tower shots leave behind webs that slow the next enemy entering them.",
        upgradePath = 1,
        cost = 10,
      }
    };
    var utilityUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.ADDITIVE, 0.1f)
        },
        name = "Faster Spinnerets",
        description = "Increase attack speed by 10.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.ADDITIVE, 0.15f),
          GetAttributeModifier(Stat.RANGE, Mode.ADDITIVE, 5.0f),
          GetAttributeModifier(Stat.PROJECTILE_SPEED, Mode.ADDITIVE, 5.0f)
        },
        name = "Faster Spinnerets",
        description = "Increases attack speed by 15 and range by 5.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.WSS_3_3_ANTI_AIR,
        name = "Elevated Shots",
        description = "This tower gains the anti air ability.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.RANGE, Mode.ADDITIVE, 10.0f),
          GetAttributeModifier(Stat.PROJECTILE_SPEED, Mode.ADDITIVE, 10.0f)
        },
        name = "Improved Trajectory Analysis",
        description = "Increases range by 10.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.WSS_3_5_GROUNDING_SHOT,
        name = "Grounding Shot",
        description = "Any enemy hit by this tower briefly loses the flying ability.",
        upgradePath = 2,
        cost = 10,
      }
    };

    TowerData data = new() {
      type = TowerData.Type.WEB_SHOOTING_SPIDER_TOWER,
      upgradeTreeData = new() {
        first = "Improved Slow",
        second = "AoE Slow",
        third = "Utility",
        firstPathUpgrades = slowUpgrades,
        secondPathUpgrades = secondarySlowUpgrades,
        thirdPathUpgrades = utilityUpgrades,
      },

      tooltip = new() {
        tooltipText = "Support tower that slows enemy movement.",
      },

      name = "Web Shooting Spider Tower",
      icon_path = "Icons/test",
      area_of_effect = 10,
      armor_pierce = 0,
      attack_speed = 0.75f,
      cost = 50,
      damage = 0,
      acid_stacks = 0,
      projectile_speed = 20,
      range = 25,
      secondary_slow_potency = 0.5f,
      secondary_slow_targets = 0,
      slow_duration = 4,
      slow_power = 0.3f,
      stun_time = 0,
      venom_power = 0,
      venom_stacks = 0,
    };

    return data;
  }

  private TowerData GetAssassinBugTowerData() {
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Armor Penetration",
        description = "Increases armor penetration by 10%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Armor Penetration",
        description = "Increases armor penetration by 20%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_1_3_ARMORED_ENEMY_BONUS,
        name = "Wall Cracker",
        description = "Doubles damage to armored enemies.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        name = "<UPGRADE NAME>",
        description = "<UPGRADE DESCRIPTION>",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_1_5_ARMOR_DEPLETION_BONUS,
        name = "Depletion Bonus",
        description = "Doubles damage to enemies which have lost their armor.",
        upgradePath = 0,
        cost = 10,
      }
    };
    var secondPathUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.PROJECTILE_SPEED, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Flight Speed",
        description = "Increases movement speed by 10%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.PROJECTILE_SPEED, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Flight Speed",
        description = "Increases movement speed by 20%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_2_3_ANTI_AIR,
        name = "Anti Air",
        description = "Allows the tower to target flying enemies. Bonus speed when returning from flying enemies.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_2_4_CAMO_SIGHT,
        name = "Camo Sight",
        description = "Allows the tower to target camo enemies.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_2_5_ELASTIC_SPINERETTES,
        name = "Elastic Spinerrettes",
        description = "The tower has a bungie that returns it a percentage of the distance back to its lair.  Scales with trophies collected.",
        upgradePath = 1,
        cost = 10,
      }
    };
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Damage Increase",
        description = "Increases damage by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Damage Increase",
        description = "Increases damage by 20%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_3_3_CONSECUTIVE_HITS,
        name = "Punishing Hits",
        description = "Consecutive hits against the same enemy do 20% more damage, stacking up to 5 times.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "Damage Increase",
        description = "Increase damage by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.AB_3_5_COMBO_FINISHER,
        name = "Combo Finisher",
        description = "Double damage against enemyies with max consecutive hits.",
        upgradePath = 2,
        cost = 10,
      }
    };

    TowerData data = new() {
      type = TowerData.Type.ASSASSIN_BUG_TOWER,
      upgradeTreeData = new() {
        first = "Armor Penetration",
        second = "Utility",
        third = "Trophy and Damage",
        firstPathUpgrades = firstPathUpgrades,
        secondPathUpgrades = secondPathUpgrades,
        thirdPathUpgrades = thirdPathUpgrades,
      },

      tooltip = new() {
        tooltipText = "Long range tower with massive damage and armor pen, but slow attack speed.",
      },

      name = "Assassin Bug",
      icon_path = "Icons/test",
      area_of_effect = 0,
      armor_pierce = 0.2f,
      attack_speed = 0.5f,
      cost = 50,
      damage = 30,
      acid_stacks = 0,
      projectile_speed = 0,
      range = 40,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0,
      stun_time = 0,
      venom_power = 0,
      venom_stacks = 0,
    };

    return data;
  }

  private TowerData GetMantisTowerData() {
    var damageUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.5f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 50%.",
        upgradePath = 0,
        cost = 80,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.5f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 50%.",
        upgradePath = 0,
        cost = 300,
      },
      new() {
        specialAbility = SpecialAbility.M_1_3_DOUBLE_SLASH,
        name = "Double Slash",
        description = "The Mantis attacks with both its scythe claws, from both sides.",
        upgradePath = 0,
        cost = 800,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.3f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 30%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_1_5_FOUR_ARMS,
        name = "Make it Double!",
        description = "The Mantis grows a second pair of scythe arms, each of which attacks normally.",
        upgradePath = 0,
        cost = 10,
      }
    };
    var bleedUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.25f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.BLEED_STACKS, Mode.SET, 15),
        },
        name = "Vicious Claws",
        description = "Attacks now inflict 15 bleed and gain 25% bonus damage.",
        upgradePath = 1,
        cost = 50,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.25f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.BLEED_STACKS, Mode.ADDITIVE, 15),
        },
        name = "Anticoagulant Microstructures",
        description = "Bleed inflicted is 30. and gain 25% bonus damage",
        upgradePath = 1,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.M_2_3_JAGGED_CLAWS,
        name = "Jagged Claws",
        description = "Enemies dealt full damage are crippled. This applies only to the first enemy hit if that enemy" +
            " has low enough armor to take full damage.",
        upgradePath = 1,
        cost = 800,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.25f)
        },
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.BLEED_STACKS, Mode.ADDITIVE, 30),
        },
        name = "Anticoagulant Chemicals",
        description = "Bleed inflicted is 60 and gain 25% bonus damage.",
        upgradePath = 1,
        cost = 1600,
      },
      new() {
        intAttributeModifiers = new AttributeModifier<int>[] {
          GetAttributeModifier(Stat.BLEED_STACKS, Mode.ADDITIVE, 20)
        },
        specialAbility = SpecialAbility.M_2_5_BLOODY_EXECUTION,
        name = "Bloody Execution",
        description = "Execute doomed (too much bleed to survive) enemies. Bleed is now 80.",
        upgradePath = 1,
        cost = 3200,
      },
    };
    var utilityUpgrades = new TowerAbility[] {
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.6f)
        },
        name = "Improved Attack Speed",
        description = "Increases attack speed by 60%.",
        upgradePath = 2,
        cost = 60,
      },
      new() {
        specialAbility = SpecialAbility.M_3_2_CAMO_SIGHT,
        name = "Camo Sight",
        description = "The Tower gains camo sight.",
        upgradePath = 2,
        cost = 150,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.AREA_OF_EFFECT, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Enhanced Spines",
        description = "Mantis attack spines are reformed to fly further, increasing the AoE range by 50%.",
        upgradePath = 2,
        cost = 800,
      },
      new() {
        floatAttributeModifiers = new AttributeModifier<float>[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.5f),
        },
        name = "Synergetic Claws",
        description = "Increases attack speed by 50%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_3_5_SHRIKE,
        name = "Shrike",
        description = "Mantis attacks shed vicious spines, AoE damage equals main attack damage.",
        upgradePath = 2,
        cost = 10,
      },
    };

    TowerData data = new() {
      type = TowerData.Type.MANTIS_TOWER,
      upgradeTreeData = new UpgradeTreeData {
        first = "Damage",
        second = "Debilitation",
        third = "Utility",
        firstPathUpgrades = damageUpgrades,
        secondPathUpgrades = bleedUpgrades,
        thirdPathUpgrades = utilityUpgrades,
      },

      tooltip = new() {
        tooltipText = "Melee tower with heavy damage, moderate armor pen, and bleed DoTs",
      },

      name = "Mantis",
      icon_path = "Icons/test",
      area_of_effect = 3.0f,
      armor_pierce = 5.0f,
      attack_speed = 0.5f,
      cost = 150,
      damage = 7,
      acid_stacks = 0,
      projectile_speed = 0,
      range = 15.0f,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0,
      stun_time = 0,
      venom_power = 0,
      venom_stacks = 0,
    };

    return data;
  }

  private AttributeModifier<float> GetAttributeModifier(Stat stat, Mode mode, float mod) {
    return new AttributeModifier<float>() {
      attribute = stat,
      mode = mode,
      mod = mod
    };
  }
  private AttributeModifier<int> GetAttributeModifier(Stat stat, Mode mode, int mod) {
    return new AttributeModifier<int>() {
      attribute = stat,
      mode = mode,
      mod = mod
    };
  }
}
