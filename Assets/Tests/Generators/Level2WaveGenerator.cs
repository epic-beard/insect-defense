using NUnit.Framework;
using System;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level2WaveGenerator {
  public string aphid = "Aphid_IL0";
  public string ant = "Ant_IL0";
  public string beetle = "Beetle_IL0";
  public string tarantula = "Tarantula_IL0";
  public string leafBug = "Leaf Bug_IL0";
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
          repetitions = 3,
          repeatDelay = 6.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 4.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 2.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new SpacerWave() {
          delay = 6.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 8.0f,
          spawnLocation = 0,
          spawnAmmount = 0,
          Positions = new() { new(-2, 0), new(2, 0) }
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages =
              { "A horde of aphids is coming on the left! Luckily, the Mantis tower excels against clustered, weak enemies and upgrades very well.",
                "You can sell your Spitting Ant tower for enough Nu to afford a Mantis.",
                "Don't worry! All Nu spent on a tower is refunded when sold."}
        },
        GetConcurrentWaveWithDefaults(
          defaultEnemyDataKey: aphid,
          metrics: new() {
            new() {
              repeatDelay = 2.0f,
              duration = 25.0f,
            },
            new() {
              warmup = 0.7f,
              repeatDelay = 2.5f,
              duration = 25.0f,
            }
          },
          defaultSpawnLocation: 1
        ),
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 3,
              repeatDelay = 6.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 3,
              repeatDelay = 6.0f,
              spawnLocation = 1,
              spawnAmmount = 4,
            },
          },
        },
        new ConcurrentWave(
          new CannedEnemyWave() {
            enemyDataKey = ant,
            repetitions = 2,
            repeatDelay = 1.5f,
            spawnLocation = 1,
            spawnAmmount = 1,
          },
          new CannedEnemyWave() {
            enemyDataKey = aphid,
            repetitions = 2,
            repeatDelay = 1.5f,
            spawnLocation = 1,
            spawnAmmount = 2,
          }
        ),
        new SpacerWave() {
          delay = 5.0f,
        },
        new ConcurrentWave(
          GetSequentialWaveWithDefaults(
            defaults: new() {
              enemyDataKey = beetle,
              duration = 50.0f,
              spawnLocation = 1,
            },
            metrics: new WaveMetrics[] {
              new() {
                repeatDelay = 16.0f,
              },
              new() {
                repeatDelay = 9.0f,
              }
            }
          ),
          GetConcurrentWaveWithDefaults(
            defaultEnemyDataKey: aphid,
            defaultSpawnLocation: 1,
            metrics: new() {
              new() {
                repeatDelay = 1.5f,
                duration = 50.0f,
              },
              new() {
                warmup = 1.0f,
                repeatDelay = 3.0f,
                duration = 50.0f
              },
            }
          )
        ),
      },
    };
    // Nu: 564

    SequentialWave secondWave = new(
      new ConcurrentWave(
        // Left side wave
        GetConcurrentWaveWithDefaults(
          defaultEnemyDataKey: aphid,
          defaultSpawnLocation: 1,
          metrics: new() {
            new() {
              repeatDelay = 2.0f,
              duration = 32.0f,
            },
            new() {
              warmup = 1.0f,
              repeatDelay = 2.5f,
              duration = 32.0f,
              },
            new() {
              warmup = 16.0f,
              repeatDelay = 3.0f,
              duration = 32.0f,
            }
          }
        ),
        // Right side wave
        GetConcurrentWaveWithDefaults(
          defaults: new() {
            enemyDataKey = ant,
            duration = 35.0f,
            spawnLocation = 0,
          },
          metrics: new WaveMetrics[] {
            new() {
              repeatDelay = 5.0f,
              repetitions = 7,
            },
            new() {
              warmup = 12.0f,
              repeatDelay = 7.0f,
              repetitions = 3,
            }
          }
        )
      ),
      new SpacerWave() {
        delay = 3.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          enemyDataKey = aphid,
          spawnLocation = 1,
          duration = 25.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            repeatDelay = 1.5f,
          },
          new() {
            warmup = 1.0f,
            repeatDelay = 2.0f,
          },
          new() {
            warmup = 2.0f,
            repeatDelay = 2.5f,
          },
          new() {
            enemyDataKey = ant,
            repeatDelay = 2.5f,
            spawnLocation = 0,
          }
        }
      ),
      // nu 789
      // ConcurrentWave -- End of subwave 1
      new WaitUntilDeadWave(),
      new ConcurrentWave(
        // Left side wave
        GetConcurrentWaveWithDefaults(
          defaults: new() {
            enemyDataKey = beetle,
            spawnLocation = 1,
            duration = 80.0f,
          },
          metrics: new WaveMetrics[] {
            new() {
              warmup = 2.0f,
              repeatDelay = 14.0f
            },
            new() {
              warmup = 37.0f,
              repeatDelay = 14.0f,
            }
          }
        ),
        GetSequentialWaveWithDefaults(
          defaults: new() {
            enemyDataKey = aphid,
            warmup = 10.0f,
            duration = 30.0f,
            repeatDelay = 0.7f,
            spawnLocation = 1,
            spawnAmount = 1,
          },
          metrics: new WaveMetrics[] {
            new() { warmup = 0.0f },
            new(),
            new(),
          }
        ),
        // Right side wave
        GetConcurrentWaveWithDefaults(
          defaults: new() {
            enemyDataKey = ant,
            duration = 100.0f,
            spawnAmount = 1,
            spawnLocation = 0,
          },
          metrics: new WaveMetrics[] {
            new () {
              repeatDelay = 5.0f,
            },
            new () {
              warmup = 24.0f,
              repeatDelay = 10.0f,
              cooldown = 30.0f,
            },
            new() {
              warmup = 50.0f,
              repeatDelay = 5.0f,
            }
          }
        )
      )  // ConcurrentWave  End of subwave 2
    );  // SequentialWave End of wave 2
    // Nu: 1248

    SequentialWave thirdWave = new() {
      Subwaves = {
        new ConcurrentWave() {
          Subwaves = {
            // Left side wave
            new SequentialWave {
              Subwaves = {
                new SpacerWave() {
                  delay = 8.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = leafBug,
                  repetitions = 1,
                  repeatDelay = 1.0f,
                  spawnAmmount = 1,
                  spawnLocation = 1,
                },
                new SpacerWave() {
                  delay = 2.0f,
                },
                new DialogueBoxWave() {
                  messages =
                    { "The insect on the left path has the Camoflauge special ability. Only towers with the Camo Sight ability can target them.",
                      "Fortunately, the Mantis tower's second Utility upgrade grants it Camo Sight." },
                  delay = 6.0f,
                },
                GetConcurrentWaveWithDefaults(
                  defaults: new() {
                    enemyDataKey = aphid,
                    duration = 50.0f,
                    spawnAmount = 1,
                    spawnLocation = 1,
                  },
                  metrics: new WaveMetrics[] {
                    new() {
                      repeatDelay = 3.0f,
                      spawnAmount = 2,
                    },
                    new() {
                      warmup = 25.0f,
                      repeatDelay = 2.0f,
                      spawnAmount = 2,
                    },
                    new() {
                      enemyDataKey = leafBug,
                      repeatDelay = 8.0f,
                    },
                  }
                ),
              },  // Subwaves
            },  // SequentialWave
            // Right side wave
            GetConcurrentWaveWithDefaults(
              defaults: new() {
                enemyDataKey = beetle,
                duration = 78.0f,
                spawnAmount = 1,
                spawnLocation = 0,
              },
              metrics: new WaveMetrics[] {
                new() {
                  enemyDataKey = ant,
                  repeatDelay = 8.0f,
                  spawnAmount = 2,
                },
                new() {
                  repeatDelay = 18.0f,
                },
                new() {
                  warmup = 24.0f,
                  repeatDelay = 10.0f,
                },
              }
            ),
          },  // Subwaves
        },  // ConcurrentWave
        new ConcurrentWave(
          // Left
          GetConcurrentWaveWithDefaults(
            defaults: new() {
              enemyDataKey = leafBug,
              duration = 40.0f,
              spawnAmount = 1,
              repeatDelay = 5.0f,
              spawnLocation = 1,
            },
            metrics: new WaveMetrics[] {
              new() {
                Positions = new() { new(-3, 3), new(0, 0), new(3, 3) }
              },
              new() {
                warmup = 15.0f,
                Positions = new() { new(0, -3) }
              }
            }
          ),
          // Right
          GetConcurrentWaveWithDefaults(
            defaults: new() {
              enemyDataKey = beetle,
              duration = 40.0f,
              spawnAmount = 1,
              repeatDelay = 8.0f,
              spawnLocation = 0,
            },
            metrics: new WaveMetrics[] {
              new() {
                Positions = new() { new(-3, 3), new(0, 0), new(3, 3) }
              },
              new() {
                warmup = 16.0f,
                Positions = new() { new(0, -3) }
              }
            }
          )
        )
      },  // Subwaves
    };  // SequentialWave
    // Nu: 2487

    // message about targeting priority
    // aphid + tarantula on one side
    // ant swarm on the other.
    SequentialWave fourthWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages = {"A tough new enemy is approaching on the left.",
                      "Try setting targeting priority to Max HP on some towers to focus attacks.",
                      "Set behavior to Stubborn to target one enemy until it is dead or out of range."
          },
        },
        new ConcurrentWave(
          // Left
          GetConcurrentWaveWithDefaults(
            defaults: new() {
              enemyDataKey = aphid,
              duration = 60.0f,
              spawnAmount = 1,
              spawnLocation = 1,
            },
            metrics: new WaveMetrics[] {
              new() {
                repeatDelay = 0.7f,
              },
              new() {
                repeatDelay = 0.8f,
              },
              new() {
                repeatDelay = 1.0f,
              },
              new() {
                enemyDataKey = tarantula,
                repeatDelay = 10.0f,
              }
            }
          ),
          // Right
          GetConcurrentWaveWithDefaults(
            defaults: new() {
              enemyDataKey = ant,
              duration = 60.0f,
              spawnAmount = 1,
              spawnLocation = 0,
            },
            metrics: new WaveMetrics[] {
              new() {
                repeatDelay = 3.0f,
              },
              new() {
                repeatDelay = 5.0f,
              },
              new() {
                warmup = 30.0f,
                repeatDelay = 4.0f,
              },
              new() {
                warmup = 30.0f,
                repeatDelay = 7.0f,
              },
            }
          )
        ),
      },  // Subwaves - overall
    };
    // Nu: 3737

    Waves waves = new() {
      waves = { firstWave, secondWave, thirdWave, fourthWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
