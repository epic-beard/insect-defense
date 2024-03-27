using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level2WaveGenerator {
  public string aphid = "Aphid";
  public string slowAphid = "SlowAphid";
  public string ant = "Ant";
  public string beetle = "Beetle";
  public string tarantula = "Tarantula";
  public string leafBug = "Leaf Bug";
  public string filename = "Waves/level2.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {

    // Starting Nu: 140
    SequentialWave firstWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages = { "Enemies will spawn on the right, be ready!" }
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 6,
          repeatDelay = 3.5f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 2,
          repeatDelay = 1.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          delay = 2.0f,
          messages =
              { "You don't have enough money for a new tower, but...",
                "You can sell this tower and build a new tower for the other track.",
                "When you sell a tower, you get back all the Nu you spent on it (upgrades included).",
                "Keep in mind: It takes a little time to tear down and then build a tower" }
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 6,
          repeatDelay = 3.5f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },
        new SpacerWave() {
          delay = 3.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 2,
          repeatDelay = 5.0f,
          spawnLocation = 1,
          spawnAmmount = 2,
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages =
              { "The next few waves will have clustered enemies, against whom the Mantis tower excels.",
                "It does damage in an area around its punch and its damage upgrades very well." }
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 12,
              repeatDelay = 2.25f,
              spawnLocation = 0,
              spawnAmmount = 2,
            },
          },
        },
        new WaitUntilDeadWave() {},
        new SpacerWave() {
          delay = 4.0f,
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 4,
              repeatDelay = 5.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 4,
              repeatDelay = 5.0f,
              spawnLocation = 1,
              spawnAmmount = 4,
            },
          },
        },
        new SpacerWave() {
          delay = 4.0f,
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 2,
              repeatDelay = 1.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 2,
              repeatDelay = 1.0f,
              spawnLocation = 1,
              spawnAmmount = 2,
            },
          },
        },
        new SpacerWave() {
          delay = 5.0f,
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = beetle,
              repetitions = 3,
              repeatDelay = 15.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new DelayedWave() {
              warmup = 45.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = beetle,
                repetitions = 3,
                repeatDelay = 8.0f,
                spawnLocation = 1,
                spawnAmmount = 1,
              },
            },
            new CannedEnemyWave() {
              enemyDataKey = slowAphid,
              repetitions = 70,
              repeatDelay = 1.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new DelayedWave() {
              warmup = 1.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = slowAphid,
                repetitions = 28,
                repeatDelay = 2.6f,
                spawnLocation = 1,
                spawnAmmount = 2,
              },
            },
          },
        },
      },
    };
    // Nu: 490

    SequentialWave secondWave = new() {
      Subwaves = {
        new ConcurrentWave {
          Subwaves = {
            // Left side wave
            new SequentialWave {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 4,
                  repeatDelay = 3.5f,
                  spawnLocation = 1,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 4,
                  repeatDelay = 2.5f,
                  spawnLocation = 1,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 10,
                  repeatDelay = 2.5f,
                  spawnLocation = 1,
                  spawnAmmount = 2,
                },
              },  // Subwaves
            },  // SequentialWave
            // Right side wave
            new SequentialWave {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 3,
                  repeatDelay = 3.5f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 3,
                  repeatDelay = 2.5f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = ant,
                  repetitions = 8,
                  repeatDelay = 4.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
              },  // Subwaves
            },  // SequentialWave
          },  // Subwaves
        },  // ConcurrentWave
        new SpacerWave() {
          delay = 3.0f,
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 9,
              repeatDelay = 2.5f,
              spawnLocation = 1,
              spawnAmmount = 3,
            },
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 9,
              repeatDelay = 2.5f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },  // ConcurrentWave -- End of subwave 1
        new WaitUntilDeadWave() {},
        new ConcurrentWave() {
          Subwaves = {
            // Left side wave
            new DelayedWave() {
              warmup = 2.0f,
              wave = new ConcurrentWave() {
                Subwaves = {
                  new SequentialWave() {
                    Subwaves = {
                      new CannedEnemyWave() {
                        enemyDataKey = beetle,
                        repetitions = 5,
                        repeatDelay = 12.0f,
                        spawnLocation = 1,
                        spawnAmmount = 2,
                      },  // CannedEnemyWave
                      new CannedEnemyWave() {
                        enemyDataKey = beetle,
                        repetitions = 4,
                        repeatDelay = 12.0f,
                        spawnLocation = 1,
                        spawnAmmount = 3,
                      },
                    },  // Subwaves
                  },  // SequentialWave
                  new SequentialWave() {
                    Subwaves = {
                      new SpacerWave() {
                        delay = 12.0f,
                      },
                      new CannedEnemyWave() {
                        enemyDataKey = aphid,
                        repetitions = 40,
                        repeatDelay = 0.6f,
                        spawnLocation = 1,
                        spawnAmmount = 1,
                      },
                      new SpacerWave() {
                        delay = 10.0f,
                      },
                      new CannedEnemyWave() {
                        enemyDataKey = aphid,
                        repetitions = 30,
                        repeatDelay = 0.6f,
                        spawnLocation = 1,
                        spawnAmmount = 1,
                      },
                      new SpacerWave() {
                        delay = 10.0f,
                      },
                      new CannedEnemyWave() {
                        enemyDataKey = aphid,
                        repetitions = 30,
                        repeatDelay = 0.6f,
                        spawnLocation = 1,
                        spawnAmmount = 1,
                      },
                    },  // Subwaves
                  },  // SequentialWave
                },  // Subwave
              },  // Wave ConcurrentWave
            },  // DelayedWave
            // Right side wave
            new SequentialWave() {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = ant,
                  repetitions = 10,
                  repeatDelay = 3.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new SpacerWave {
                  delay = 15.0f,
                },
                new DialogueBoxWave() {
                  messages =
                      { "It looks like the right side is nearly out of enemies...",
                        "This should free you up to sell some towers and reinforce the left." },
                },
              },
            }  // SequentialWave
          }  // Subwaves
        },  // ConcurrentWave  End of subwave 2
      },  // Subwaves
    };  // SequentialWave
    // Nu: 956

    SequentialWave thirdWave = new() {
      Subwaves = {
        new ConcurrentWave() {
          Subwaves = {
            // Left side waves
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 10,
              repeatDelay = 8.0f,
              spawnLocation = 1,
              spawnAmmount = 2,
            },
            new CannedEnemyWave() {
              enemyDataKey = beetle,
              repetitions = 5,
              repeatDelay = 16.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = beetle,
              repetitions = 8,
              repeatDelay = 10.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            // Right side wave
            new SequentialWave {
              Subwaves = {
                new SpacerWave() {
                  delay = 10.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = leafBug,
                  repetitions = 1,
                  repeatDelay = 1.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new SpacerWave() {
                  delay = 6.0f,
                },
                new DialogueBoxWave() {
                  messages =
                    { "The insect on the right path has the Camoflauge special ability. Only towers with the Camo Sight ability can target them.",
                      "Fortunately, the Mantis tower's second Utility upgrade grants it Camo Sight." },
                  delay = 6.0f,
                },
                new ConcurrentWave {
                  Subwaves = {
                    new SequentialWave() {
                      Subwaves = {
                        new SpacerWave() {
                          delay = 10.0f,
                        },
                        new CannedEnemyWave() {
                          enemyDataKey = aphid,
                          repetitions = 8,
                          repeatDelay = 3.0f,
                          spawnLocation = 0,
                          spawnAmmount = 2,
                        },
                        new CannedEnemyWave() {
                          enemyDataKey = aphid,
                          repetitions = 12,
                          repeatDelay = 1.5f,
                          spawnLocation = 0,
                          spawnAmmount = 1,
                        }
                      },
                    },
                    new CannedEnemyWave() {
                      enemyDataKey = leafBug,
                      repetitions = 7,
                      repeatDelay = 8.0f,
                      spawnLocation = 0,
                      spawnAmmount = 1,
                    },
                  },  // Subwaves
                },  // ConcurrentWave
              },  // Subwaves
            },  // SequentialWave
          },  // Subwaves
        },  // ConcurrentWave
      },  // Subwaves
    };  // SequentialWave
    // Nu: 1156

    SequentialWave fourthWave = new() {
      Subwaves = {
        new DialogueBoxWave {
          messages =
            { "Something new is coming, it's much bigger than a beetle!",
              "Consider using a mix of towers to take it on." },
        },
        new CannedEnemyWave() {
          enemyDataKey = tarantula,
          repetitions = 2,
          repeatDelay = 5.0f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },  // CannedEnemyWave
      },  // Subwaves - overall
    };
    // Nu: 1256

    // TODO(emonzon): Remove this before submitting.
    SequentialWave totalingWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages =
            { "Total them points, bitches!" },
          delay = 60.0f,
        },
      },
    };

    Waves waves = new() {
      waves = { firstWave, secondWave, thirdWave, fourthWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
