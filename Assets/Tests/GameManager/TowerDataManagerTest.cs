using System.IO;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using AbilityDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerAbility[][]>;
using UnityEngine;

public class TowerDataManagerTest {
  TowerData towerData;
  TowerAbility[][] abilities;

  [SetUp]
  public void SetUp() {
    towerData = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      upgradeTreeData = new() {
        first = "first",
        second = "second",
        third = "third"
      },
      area_of_effect = 1.0f,
      armor_pierce = 2.0f,
      armor_tear = 3.0f,
      attack_speed = 4.0f,
      damage = 5.0f,
      damage_over_time = 6.0f,
      projectile_speed = 7.0f,
      range = 8.0f,
      slow_duration = 9.0f,
      slow_power = 10.0f,
      stun_time = 11.0f,
    };

    TowerAbility.AttributeModifier modifier = new() {
      attribute = TowerData.Stat.RANGE,
      mode = TowerAbility.Mode.MULTIPLICATIVE,
      mod = 2.0f,
    };

    TowerAbility ability = new() {
      specialAbility = TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN,
      attributeModifiers = new TowerAbility.AttributeModifier[] {
        modifier,
      },
      upgradePath = 3,
      name = "ability",
      description = "an ability",
      cost = 4,
    };
    abilities = new TowerAbility[1][] { new TowerAbility[] { ability } };
  }

  // Round trip a TowerData object.
  [Test]
  public void TowerDataSerializes() {
    TowerDictionary expected = new() {
      {TowerData.Type.SPITTING_ANT_TOWER, towerData},
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expected, writer);

    memStream.Position = 0;

    TowerDictionary actual = Deserialize<TowerDictionary>(memStream);

    CollectionAssert.AreEqual(expected, actual);
  }

  // Round trip a TowerAbility multidimensional array.
  [Test]
  public void TowerAbilitySerializes() {
    AbilityDictionary expectedDictionary = new() {
      {TowerData.Type.SPITTING_ANT_TOWER, abilities},
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expectedDictionary, writer);

    memStream.Position = 0;

    AbilityDictionary actualDictionary = Deserialize<AbilityDictionary>(memStream);

    Assert.That(actualDictionary.Count, Is.EqualTo(1));
    TowerAbility expected = expectedDictionary[TowerData.Type.SPITTING_ANT_TOWER][0][0];
    TowerAbility actual = actualDictionary[TowerData.Type.SPITTING_ANT_TOWER][0][0];

    Assert.That(expected.specialAbility, Is.EqualTo(actual.specialAbility));
    Assert.That(expected.attributeModifiers, Is.EqualTo(actual.attributeModifiers));
    Assert.That(expected.upgradePath, Is.EqualTo(actual.upgradePath));
    Assert.That(expected.name, Is.EqualTo(actual.name));
    Assert.That(expected.description, Is.EqualTo(actual.description));
    Assert.That(expected.cost, Is.EqualTo(actual.cost));
  }

  // This is to generate tower data for reading in.
  //[Test]
  public void GenerateTowerData() {
    TowerData spittingAntTowerTest = new() {
      type = TowerData.Type.SPITTING_ANT_TOWER,
      upgradeTreeData = new TowerData.UpgradeTreeData() {
        first = "Armor Tear",
        second = "Acid Power",
        third = "Utility",
        // Armor Tear path
        firstPathUpgrades = new TowerAbility[5] {
            // Armor Tear path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ARMOR_TEAR,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Armor Tear",
              description = "Increases armor tear by 10%.",
              upgradePath = 0,
              cost = 10,
            },
            // Armor Tear path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ARMOR_TEAR,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Armor Tear",
              description = "Increases armor tear by 20%.",
              upgradePath = 0,
              cost = 10,
            },
            // Armor Tear path, upgrade 3
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Stunning Reveal",
              description = "If an enemy's armor is reduced to 0, it is briefly stunned.",
              upgradePath = 0,
              cost = 10,
            },
            // Armor Tear path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ARMOR_TEAR,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.3f
                }
              },
              name = "Armor Tear",
              description = "Increases armor tear by 30%.",
              upgradePath = 0,
              cost = 10,
            },
            // Armor Tear path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Armor Tear Explosion",
              description = "Armor tear now applies to all enemies caught in the splash explosion.",
              upgradePath = 0,
              cost = 10,
            }
          },
        // Acid Power path
        secondPathUpgrades = new TowerAbility[5] {
            // Acid Power path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.DAMAGE_OVER_TIME,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Acid Power",
              description = "Increases acid build rate by 10%.",
              upgradePath = 1,
              cost = 10,
            },
            // Acid Power path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.DAMAGE_OVER_TIME,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Acid Power",
              description = "Increases acid build rate by 20%.",
              upgradePath = 1,
              cost = 10,
            },
            // Acid Power path, upgrade 3
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_2_3_DOT_SLOW,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Hindering Acid",
              description = "If acid stacks reach max, apply a 20% slow for 2 seconds.",
              upgradePath = 1,
              cost = 10,
            },
            // Acid Power path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.DAMAGE_OVER_TIME,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.3f
                }
              },
              name = "Acid Power",
              description = "Increases acid build rate by 30%.",
              upgradePath = 1,
              cost = 10,
            },
            // Acid Power path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_2_5_DOT_EXPLOSION,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Acid Explosion",
              description = "If acid stacks reach max, an explosion of acid is triggered. This explosion does damage"
                  + " equal to the full duration damage of all acid stacks and clears acid stacks from the target",
              upgradePath = 1,
              cost = 10,
            }
          },
        // Utility path
        thirdPathUpgrades = new TowerAbility[5] {
            // Utility path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.AREA_OF_EFFECT,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Bigger Booms",
              description = "Increases area of effects by 1%.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.RANGE,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Longer Shots",
              description = "Increases range by 10%.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 3
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_3_3_ANTI_AIR,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Elevated Shots",
              description = "This tower gains the anti air ability.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ATTACK_SPEED,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Faster Production",
              description = "Increases attack speed by 20%.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Continous Stream",
              description = "This tower's attack becomes single-target and constantly fires.",
              upgradePath = 2,
              cost = 10,
            }
          }
      },
      name = "Spitting Ant Tower",
      area_of_effect = 5.0f,
      armor_pierce = 0.0f,
      armor_tear = 5.0f,
      attack_speed = 1.0f,  // Attacks per second.
      damage = 5.0f,
      damage_over_time = 5.0f,  // Acid stacks per hit.
      projectile_speed = 20.0f,
      range = 20.0f,
      secondary_slow_potency = 0.0f,
      secondary_slow_targets = 0.0f,
      slow_duration = 2.0f,
      slow_power = 0.2f,  // 20% slow
      stun_time = 1.0f,
    };
    TowerData webShootingSpiderTowerTest = new() {
      type = TowerData.Type.WEB_SHOOTING_SPIDER_TOWER,
      upgradeTreeData = new TowerData.UpgradeTreeData() {
        first = "Improved Slow",
        second = "Area Slow",
        third = "Utility",
        // Improved Slow path
        firstPathUpgrades = new TowerAbility[5] {
            // Armor Tear path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[2] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_DURATION,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                },
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_POWER,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Improved Slow",
              description = "Increases slow effectiveness by 10%.",
              upgradePath = 0,
              cost = 10,
            },
            // Improved Slow path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[2] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_DURATION,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                },
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_POWER,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Improved Slow",
              description = "Increases slow effectiveness by 20%.",
              upgradePath = 0,
              cost = 10,
            },
            // Improved Slow path, upgrade 3
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.WSS_1_3_SLOW_STUN,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Stunning Slow",
              description = "Stuns an enemy the first time it is hit by this tower.",
              upgradePath = 0,
              cost = 10,
            },
            // Improved Slow path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[2] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_DURATION,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.3f
                },
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SLOW_POWER,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.3f
                }
              },
              name = "Improved Slow",
              description = "Increases slow effectiveness by 30%.",
              upgradePath = 0,
              cost = 10,
            },
            // Improved Slow path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Permanent Slow",
              description = "When first slowing an enemy, this tower permanently reduces their movement speed."
                  + " This effect can apply at most once per enemy per tower.",
              upgradePath = 0,
              cost = 10,
            }
          },
        // Area Slow path
        secondPathUpgrades = new TowerAbility[5] {
            // Area Slow path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS,
                  mode = TowerAbility.Mode.ADDITIVE,
                  mod = 1.0f
                }
              },
              name = "Area Slow",
              description = "1/2 slow also hits the enemy nearest the target within its AoE.",
              upgradePath = 1,
              cost = 10,
            },
            // Area Slow path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS,
                  mode = TowerAbility.Mode.ADDITIVE,
                  mod = 1.0f
                }
              },
              name = "Area Slow",
              description = "1/2 slow also hits the 2 enemies nearest the target within its AoE.",
              upgradePath = 1,
              cost = 10,
            },
            // Area Slow path, upgrade 3
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[2] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS,
                  mode = TowerAbility.Mode.ADDITIVE,
                  mod = 1.0f
                },
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SECDONARY_SLOW_POTENCY,
                  mode = TowerAbility.Mode.SET,
                  mod = 0.75f
                }
              },
              name = "Superior Secondary Threads",
              description = "3/4 slow also hits the 3 enemies nearest the target within its AoE",
              upgradePath = 1,
              cost = 10,
            },
            // Area Slow path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.SECDONARY_SLOW_POTENCY,
                  mode = TowerAbility.Mode.SET,
                  mod = 1.0f
                }
              },
              name = "Perfect Secondary Threads",
              description = "100% slow also hits the 3 enemies nearest the target within its AoE",
              upgradePath = 1,
              cost = 10,
            },
            // Acid Power path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.WSS_2_5_LINGERING_SLOW,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Enduring Threads",
              description = "Tower shots leave behind webs that slow the next enemy entering them.",
              upgradePath = 1,
              cost = 10,
            }
          },
        // Utility path
        thirdPathUpgrades = new TowerAbility[5] {
            // Utility path, upgrade 1
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ATTACK_SPEED,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.1f
                }
              },
              name = "Faster Spinnerets",
              description = "Increases Attack Speed by 10%",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 2
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.ATTACK_SPEED,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Faster Spinnerets",
              description = "Increases attack speed by 20%.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 3
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.WSS_3_3_ANTI_AIR,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Elevated Shots",
              description = "This tower gains the anti air ability.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 4
            new TowerAbility {
              attributeModifiers = new TowerAbility.AttributeModifier[1] {
                new TowerAbility.AttributeModifier() {
                  attribute = TowerData.Stat.RANGE,
                  mode = TowerAbility.Mode.MULTIPLICATIVE,
                  mod = 1.2f
                }
              },
              name = "Improved Trajectory Analysis",
              description = "Increases range by 20%.",
              upgradePath = 2,
              cost = 10,
            },
            // Utility path, upgrade 5
            new TowerAbility {
              specialAbility = TowerAbility.SpecialAbility.WSS_3_5_GROUNDING_SHOT,
              attributeModifiers = new TowerAbility.AttributeModifier[0],
              name = "Grounding Shot",
              description = "Any enemy hit by this tower briefly loses the flying ability.",
              upgradePath = 2,
              cost = 10,
            }
          }
      },
      name = "Web Shooting Spider Tower",
      area_of_effect = 10.0f,
      armor_pierce = 0.0f,
      armor_tear = 0.0f,
      attack_speed = 1.0f,
      damage = 0.0f,
      damage_over_time = 0.0f,
      projectile_speed = 20.0f,
      range = 20.0f,
      secondary_slow_potency = 0.5f,
      secondary_slow_targets = 0.0f,
      slow_duration = 2.0f,
      slow_power = 0.4f,
      stun_time = 0.0f,
    };
    TowerDictionary testTowers = new() {
      { TowerData.Type.SPITTING_ANT_TOWER, spittingAntTowerTest },
      { TowerData.Type.WEB_SHOOTING_SPIDER_TOWER, webShootingSpiderTowerTest },
    };
    string filename = "data.towers";
    Serialize<TowerDictionary>(testTowers, filename);

    TowerDictionary towers = Deserialize<TowerDictionary>(filename);
  }
}
