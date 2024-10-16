using NUnit.Framework;
using static LevelGeneratorStatics;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level1WaveGenerator {
  public string filename = directory + "level1.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    SequentialWave firstWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages =
              { "Welcome to Insect Defense, where you kill bugs, with bugs.",
                "Select a tower from the right hand menu and click on a space of grass to build it.",
                "Towers get much more expensive very quickly, be careful!",
                "For now, just build one Spitting Ant Tower." }
        },
        new CannedEnemyWave() {
          enemyDataKey = aphid0,
          repetitions = 5,
          repeatDelay = 3.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = aphid0,
              repetitions = 5,
              repeatDelay = 3.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid0,
              repetitions = 3,
              repeatDelay = 5.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
        new ConcurrentWave() {
          Subwaves = {
            new SequentialWave() {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = aphid0,
                  repetitions = 4,
                  repeatDelay = 4.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new SpacerWave() {
                  delay = 4.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid0,
                  repetitions = 4,
                  repeatDelay = 5.0f,
                  spawnLocation = 0,
                  spawnAmmount = 2,
                },
              },
            },
            new SequentialWave() {
              Subwaves = {
                new SpacerWave() {
                  delay = 3.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = ant0,
                  repetitions = 3,
                  repeatDelay = 10.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
              },
            },
          },
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages = { "It looks like you have some Nu left, it is about time to upgrade a tower.",
                       "First, click on the tower you wish to ugprade to select it.",
                       "Now check out the available upgrades.",
                       "I'll leave the next wave to you." }
        },
        new CannedEnemyWave() {
          enemyDataKey = ant0,
          repetitions = 5,
          repeatDelay = 6.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new WaitUntilDeadWave() {},
        new ConcurrentWave() {
          Subwaves = {
            new DelayedWave() {
              warmup = 2.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = ant0,
                repetitions = 3,
                repeatDelay = 9.0f,
                spawnLocation = 0,
                spawnAmmount = 1,
              },
            },
            new CannedEnemyWave() {
              enemyDataKey = ant0,
              repetitions = 4,
              repeatDelay = 7.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
      },
    };
    // Nu: 255

    SequentialWave secondWave = new() {
      Subwaves = {
        new CannedEnemyWave() {
          enemyDataKey = ant0,
          repetitions = 5,
          repeatDelay = 3.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new SpacerWave() {
          delay = 3.0f,
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant0,
              repetitions = 4,
              repeatDelay = 6.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid0,
              repetitions = 12,
              repeatDelay = 2.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
        new WaitUntilDeadWave() {},
        new ConcurrentWave {
          Subwaves = {
            new DelayedWave {
              warmup = 4.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = ant0,
                repetitions = 3,
                repeatDelay = 7.0f,
                spawnLocation = 0,
                spawnAmmount = 1,
              },
            },
            new CannedEnemyWave() {
              enemyDataKey = ant0,
              repetitions = 3,
              repeatDelay = 9.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid0,
              repetitions = 20,
              repeatDelay = 1.5f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          }
        }
      },
    };
    // Nu: 414

    SequentialWave thirdWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages = {
              "Beetles are armored enemies, do more damage to handle this.",
              "Check the Spitting Ant Tower upgrades."
          },
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = beetle0,
              repetitions = 3,
              repeatDelay = 8.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new SequentialWave() {
              Subwaves = {
                new SpacerWave() {
                  delay = 2.0f,
                },
                new ConcurrentWave {
                  Subwaves = {
                    new CannedEnemyWave() {
                      enemyDataKey = aphid0,
                      repetitions = 8,
                      repeatDelay = 2.2f,
                      spawnLocation = 0,
                      spawnAmmount = 1,
                    },
                    new CannedEnemyWave() {
                      enemyDataKey = aphid0,
                      repetitions = 6,
                      repeatDelay = 3.3f,
                      spawnLocation = 0,
                      spawnAmmount = 1,
                    },
                  },
                },
              },
            },
          },
        },
        new SpacerWave() {
          delay = 8.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = aphid0,
          repetitions = 1,
          repeatDelay = 8.0f,
          spawnLocation = 0,
          spawnAmmount = 3,
        },
        new SpacerWave() {
          delay = 3.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant0,
          repetitions = 1,
          repeatDelay = 8.0f,
          spawnLocation = 0,
          spawnAmmount = 3,
        },
        new SpacerWave() {
          delay = 2.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = beetle0,
          repetitions = 1,
          repeatDelay = 8.0f,
          spawnLocation = 0,
          spawnAmmount = 3,
        },
        new WaitUntilDeadWave {},
        new DialogueBoxWave {
          messages = {
              "Congratulations! You beat the first level. Time to return to the lab and plan your next steps."
          },
        },
      },
    };

    Waves waves = new() {
      waves = { firstWave, secondWave, thirdWave },
    };
    waves.Level = 1;

    Serialize<Waves>(waves, filename);
  }
}
