using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level1WaveGenerator {
  public string aphid = "Aphid_IL0";
  public string ant = "Ant_IL0";
  public string beetle = "Beetle_IL0";
  public string filename = "Waves/level1.waves";

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
          enemyDataKey = aphid,
          repetitions = 5,
          repeatDelay = 3.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new ConcurrentWave() {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 5,
              repeatDelay = 3.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
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
                  enemyDataKey = aphid,
                  repetitions = 4,
                  repeatDelay = 4.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new SpacerWave() {
                  delay = 4.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
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
                  enemyDataKey = ant,
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
          enemyDataKey = ant,
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
                enemyDataKey = ant,
                repetitions = 3,
                repeatDelay = 9.0f,
                spawnLocation = 0,
                spawnAmmount = 1,
              },
            },
            new CannedEnemyWave() {
              enemyDataKey = ant,
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
          enemyDataKey = ant,
          repetitions = 7,
          repeatDelay = 3.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        //new WaitUntilDeadWave() {},
        new SpacerWave() {
          delay = 3.0f,
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 4,
              repeatDelay = 6.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 12,
              repeatDelay = 2.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
      },
    };
    // Nu: 356

    SequentialWave thirdWave = new() {
      Subwaves = {
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 3,
              repeatDelay = 5.0f,
              spawnLocation = 0,
              spawnAmmount = 2,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 8,
              repeatDelay = 2.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
        new SpacerWave() {
          delay = 1.0f,
        },
        new DialogueBoxWave() {
          messages = {
              "Beetles are armored enemies, make sure you have some armor tear or peirce to damage them!",
              "Check the Spitting Ant Tower upgrades."
          },
        },
        new CannedEnemyWave {
          enemyDataKey = beetle,
          repetitions = 2,
          repeatDelay = 2.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
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
      waves = { secondWave, thirdWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
