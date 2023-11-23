using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using static TowerData;
using static TowerAbility;
public class TowerDataGenerator {
  // Comment this out when not in use.
  [Test]
  public void TowerDataGeneratorTest() {
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
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Armor Tear",
        description = "Increases armor tear by 10%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ARMOR_TEAR, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Armor Tear",
        description = "Increases armor tear by 20%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_1_3_ARMOR_TEAR_STUN,
        name = "Stunning Reveal",
        description = "When enemy's armor is reduced to 0, it is breifly stunned.",
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
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Acid Power",
        description = "Increases acid build rate by 10%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Acid Power",
        description = "Increases acid build rate by 20%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_3_DOT_SLOW,
        name = "Hindering Acid",
        description = "If acid stacks reach max, apply a 20% slow for 2 seconds.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.3f)
        },
        name = "Acid Power",
        description = "Increases acid build rate by 30%.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_2_5_DOT_EXPLOSION,
        name = "Acid Explosion",
        description = "When acid stacks reach max, an explosion of acid is triggered, applying all acid stacks instantly and clearing them.",
        upgradePath = 1,
        cost = 10,
      }
    };
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.AREA_OF_EFFECT, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Bigger Booms",
        description = "Increases area of effect by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.RANGE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Longer Shots",
        description = "Increases range by 10%.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.SA_3_3_ANTI_AIR,
        name = "Elevated Shots",
        description = "The tower gains the anti-air ability.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Faster Production",
        description = "Increases attack speed by 20%.",
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

      name = "Spitting Ant Tower",
      area_of_effect = 5,
      armor_pierce = 0,
      armor_tear = 5,
      attack_speed = 1,
      cost = 50,
      damage = 5,
      damage_over_time = 5,
      enemies_hit = 0,
      projectile_speed = 20,
      range = 20,
      secondary_slow_potency = 0,
      secondary_slow_targets = 0,
      slow_duration = 2,
      slow_power = 0.2f,
      stun_time = 1,
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

      name = "Web Shooting Spider Tower",
      area_of_effect = 10,
      armor_pierce = 0,
      armor_tear = 0,
      attack_speed = 1,
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

      name = "Assassin Bug",
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
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.1f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 10%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.2f),
          GetAttributeModifier(Stat.ARMOR_PIERCE, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Damage and Armor Pen",
        description = "Increases damage and armor penetration by 20%.",
        upgradePath = 0,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_1_3_DOUBLE_SLASH,
        name = "Double Slash",
        description = "The Mantis attacks with both its scythe claws, from each direction.",
        upgradePath = 0,
        cost = 10,
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
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.ADDITIVE, 10)
        },
        name = "Vicious Claws",
        description = "Attacks now inflict bleed",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.2f)
        },
        name = "Anticoagulant Microstructures",
        description = "Bleed is 20% stronger.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_2_3_JAGGED_CLAWS,
        name = "Jagged Claws",
        description = "Enemies dealt full damage are crippled. This applies only to the first enemy hit if that enemy" +
            " has low enough armor to take full damage.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE_OVER_TIME, Mode.MULTIPLICATIVE, 1.4f)
        },
        name = "Anticoagulant Chemicals",
        description = "Bleed is 40% stronger.",
        upgradePath = 1,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_2_5_SERRATED_CLAWS,
        name = "Serrated Claws",
        description = "Grants the 'Cripple' special ability on a short cooldown.",
        upgradePath = 1,
        cost = 10,
      },
    };
    var thirdPathUpgrades = new TowerAbility[] {
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.1f)
        },
        name = "Improved Attack Speed",
        description = "Increases attack speed by 10%",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ENEMIES_HIT, Mode.ADDITIVE, 1)
        },
        name = "Durable Claws",
        description = "Attacks hit one more enemy than normal.",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        specialAbility = SpecialAbility.M_3_3_CAMO_SIGHT,
        name = "Camo Sight",
        description = "The Tower gains camo sight",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.ATTACK_SPEED, Mode.MULTIPLICATIVE, 1.4f),
          GetAttributeModifier(Stat.ENEMIES_HIT, Mode.ADDITIVE, 1)
        },
        name = "Synergetic Claws",
        description = "Increases attack speed by 20% and increase number of enemies hit by 1",
        upgradePath = 2,
        cost = 10,
      },
      new() {
        attributeModifiers = new AttributeModifier[] {
          GetAttributeModifier(Stat.DAMAGE, Mode.MULTIPLICATIVE, 1.2f)
        },
        specialAbility = SpecialAbility.M_3_5_VORPAL_CLAWS,
        name = "Vorpal Claws",
        description = "There is no limit on enemies hit. Previous limit bonuses increase damage by 10% each.",
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

      name = "Mantis",
      area_of_effect = 0,
      armor_pierce = 0.1f,
      armor_tear = 0,
      attack_speed = 1.0f,
      damage = 10,
      damage_over_time = 0,
      enemies_hit = 4,
      projectile_speed = 0,
      range = 10.0f,
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
        specialAbility = SpecialAbility.SA_1_3_ARMOR_TEAR_STUN,
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
        specialAbility = SpecialAbility.SA_2_3_DOT_SLOW,
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
        specialAbility = SpecialAbility.SA_2_5_DOT_EXPLOSION,
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
