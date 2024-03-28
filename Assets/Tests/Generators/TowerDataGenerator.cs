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
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.SET, 1.0f)
        },
        name = "Armor Tear!",
        description = "Adds 1 point of armor tear.",
        upgradePath = 0,
        cost = 50,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.SET, 2.0f)
        },
        name = "More Armor Tear",
        description = "Increases armor tear to 2.",
        upgradePath = 0,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_3_ARMOR_TEAR_ACID_BONUS,
        name = "Acid Breakdown",
        description = "When an enemy is more than halfway to an acid explosion, they take 50% bonus armor tear from this tower.",
        upgradePath = 0,
        cost = 800,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.SET, 5.0f)
        },
        name = "Max Armor Tear",
        description = "Increases armor tear to 5.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION,
        name = "Armor Tear Explosion",
        description = "Armor tear now applies to all enemies caught in the splash explosion.",
        upgradePath = 0,
        cost = 10,
      }
    };
    var secondPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 5.0f)
        },
        name = "Acid!",
        description = "Attacks inflict 5 stacks of acid.",
        upgradePath = 1,
        cost = 50,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 10.0f)
        },
        name = "More Acid",
        description = "Acid stacks inflicted is raised to 10.",
        upgradePath = 1,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_3_ACID_BUILDUP_BONUS,
        name = "Armorless Acid",
        description = "If an enemy has no armor, they gain 50% more acid stacks from this tower.",
        upgradePath = 1,
        cost = 800,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 15.0f)
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
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.RANGE, Mode.MULTIPLICATIVE, 1.3f),
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Nozzle Control",
        description = "Attacks gain 30% more range and 20% faster attacks.",
        upgradePath = 2,
        cost = 50,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Shot Production Speed",
        description = "Increases attack speed by 50%.",
        upgradePath = 2,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_3_ANTI_AIR,
        name = "Elevated Shots",
        description = "The tower gains the anti-air ability.",
        upgradePath = 2,
        cost = 800,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
        first = "Armor Tear",
        second = "Acid Power",
        third = "Utility",
        firstPathUpgrades = firstPathUpgrades,
        secondPathUpgrades = secondPathUpgrades,
        thirdPathUpgrades = thirdPathUpgrades,
      },

      tooltip = new() {
        tooltipText = "A ranged tower with moderate damage, armor tear, and acid DoTs.",
      },

      name = "Spitting Ant Tower",
      icon_path = "Icons/test",
      area_of_effect = 10,
      armor_pierce = 0,
      armor_tear = 0,
      attack_speed = 0.5f,
      cost = 100,
      damage = 10,
      damage_over_time = 0,
      enemies_hit = 0,
      projectile_speed = 100,
      range = 20,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0.0f,
      stun_time = 0,
    };

    return data;
  }

  private TowerData GetWebShootingSpiderTowerData() {
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.MULTIPLICATIVE, 1.1f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Improved Slow",
        description = "Increasses slow effectiveness and duration by 10%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.MULTIPLICATIVE, 1.2f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Improved Slow",
        description = "Increase slow effectiveness and duration by 20%.",
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SLOW_DURATION, Mode.MULTIPLICATIVE, 1.3f),
          GetAttributeModifier(Stat.SLOW_POWER, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "Improved Slow",
        description = "Increase slow effectiveness and duration by 30%.",
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
    var secondPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1)
        },
        name = "AoE Slow",
        description = "1/2 slow also hits the enemy nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1)
        },
        name = "AoE Slow",
        description = "1/2 slow also hits the 2 enemies nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.SECONDARY_SLOW_TARGETS, Mode.ADDITIVE, 1),
          GetAttributeModifier(Stat.SECONDARY_SLOW_POTENCY, Mode.SET, 0.75f)
        },
        name = "Superior Secondary Threads",
        description = "3/4 slow also hits the 3 enemies nearest the target within its AoE.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Faster Spinnerets",
        description = "Increases Attack Speed by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Faster Spinnerets",
        description = "Increases attack speed by 20%.",
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.RANGE, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Improved Trajectory Analysis",
        description = "Increases range by 20%.",
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
        firstPathUpgrades = firstPathUpgrades,
        secondPathUpgrades = secondPathUpgrades,
        thirdPathUpgrades = thirdPathUpgrades,
      },

      tooltip = new() {
        tooltipText = "Support tower that slows enemy movement.",
      },

      name = "Web Shooting Spider Tower",
      icon_path = "Icons/test",
      area_of_effect = 10,
      armor_pierce = 0,
      armor_tear = 0,
      attack_speed = 0.5f,
      cost = 50,
      damage = 0,
      damage_over_time = 0,
      enemies_hit = 0,
      projectile_speed = 20,
      range = 20,
      secondary_slow_potency = 0.5f,
      secondary_slow_targets = 0,
      slow_duration = 2,
      slow_power = 0.4f,
      stun_time = 0,
    };

    return data;
  }

  private TowerData GetAssassinBugTowerData() {
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Armor Penetration",
        description = "Increases armor penetration by 10%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "Armor Tear",
        description = "Increases armor tear by 30%.",
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.PROJECTILE_SPEED, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Flight Speed",
        description = "Increases movement speed by 10%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Damage Increase",
        description = "Increases damage by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
        attributeModifiers = new AttributeModifier[] {
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
      armor_tear = 0,
      attack_speed = 0.5f,
      cost = 50,
      damage = 30,
      damage_over_time = 0,
      enemies_hit = 0,
      projectile_speed = 0,
      range = 0,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0,
      stun_time = 0,
    };

    return data;
  }

  private TowerData GetMantisTowerData() {
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.5f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 50%.",
        upgradePath = 0,
        cost = 50,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.5f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 50%.",
        upgradePath = 0,
        cost = 200,
      },
      new() {
        specialAbility = SpecialAbility.M_1_3_DOUBLE_SLASH,
        name = "Double Slash",
        description = "The Mantis attacks with both its scythe claws, from both sides.",
        upgradePath = 0,
        cost = 800,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
    var secondPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 15)
        },
        name = "Vicious Claws",
        description = "Attacks now inflict 15 bleed.",
        upgradePath = 1,
        cost = 50,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 30)
        },
        name = "Anticoagulant Microstructures",
        description = "Bleed inflicted is 30.",
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
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 60)
        },
        name = "Anticoagulant Chemicals",
        description = "Bleed inflicted is 60.",
        upgradePath = 1,
        cost = 1600,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.SET, 80)
        },
        specialAbility = SpecialAbility.M_2_5_BLOODY_EXECUTION,
        name = "Bloody Execution",
        description = "Execute doomed (too much bleed to survive) enemies. Bleed is now 80.",
        upgradePath = 1,
        cost = 3200,
      },
    };
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Improved Attack Speed",
        description = "Increases attack speed by 50%.",
        upgradePath = 2,
        cost = 50,
      },
      new() {
        specialAbility = SpecialAbility.M_3_2_CAMO_SIGHT,
        name = "Camo Sight",
        description = "The Tower gains camo sight.",
        upgradePath = 2,
        cost = 150,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.AREA_OF_EFFECT, Mode.MULTIPLICATIVE, 1.5f)
        },
        name = "Enhanced Spines",
        description = "Mantis attack spines are reformed to fly further, increasing the AoE range by 50%.",
        upgradePath = 2,
        cost = 800,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
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
        firstPathUpgrades = firstPathUpgrades,
        secondPathUpgrades = secondPathUpgrades,
        thirdPathUpgrades = thirdPathUpgrades,
      },

      tooltip = new() {
        tooltipText = "Melee tower with heavy damage, moderate armor pen, and bleed DoTs",
      },

      name = "Mantis",
      icon_path = "Icons/test",
      area_of_effect = 3.0f,
      armor_pierce = 5.0f,
      armor_tear = 0,
      attack_speed = 0.5f,
      cost = 150,
      damage = 7,
      damage_over_time = 0,
      projectile_speed = 0,
      range = 15.0f,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 0,
      slow_power = 0,
      stun_time = 0,
    };

    return data;
  }

  private TowerData GetTowerData() {
    var firstPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "",
        description = "",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "",
        description = "",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_3_ARMOR_TEAR_ACID_BONUS,
        name = "",
        description = "",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "",
        description = "",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION,
        name = "",
        description = "",
        upgradePath = 0,
        cost = 10,
      }
    };
    var secondPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "",
        description = "",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "",
        description = "",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_3_ACID_BUILDUP_BONUS,
        name = "",
        description = "",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "",
        description = "",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_5_DOT_ENHANCEMENT,
        name = "",
        description = "",
        upgradePath = 1,
        cost = 10,
      }
    };
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.AREA_OF_EFFECT, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "",
        description = "",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.RANGE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "",
        description = "",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_3_ANTI_AIR,
        name = "",
        description = "",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "",
        description = "",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_5_CONSTANT_FIRE,
        name = "",
        description = "",
        upgradePath = 2,
        cost = 10,
      }
    };

    TowerData data = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      upgradeTreeData = new() {
        first = "",
        second = "",
        third = "",
        firstPathUpgrades = firstPathUpgrades,
        secondPathUpgrades = secondPathUpgrades,
        thirdPathUpgrades = thirdPathUpgrades,
      },

      name = "",
      icon_path = "",
      area_of_effect = 10,
      armor_pierce = 0,
      armor_tear = 0,
      attack_speed = 1,
      cost = 50,
      damage = 0,
      damage_over_time = 0,
      projectile_speed = 20,
      range = 20,
      secondary_slow_potency = 0.5f,
      secondary_slow_targets = 0,
      slow_duration = 2,
      slow_power = 0.4f,
      stun_time = 0,
    };

    return data;
  }

  private AttributeModifier GetAttributeModifier(Stat stat, Mode mode, float mod) {
    return new AttributeModifier() {
      attribute = stat,
      mode = mode,
      mod = mod
    };
  }
}
