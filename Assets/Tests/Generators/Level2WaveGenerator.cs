using System.Collections;
using System.Collections.Generic;
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
          repetitions = 5,
          repeatDelay = 3.0f,
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
          repetitions = 5,
          repeatDelay = 3.0f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 2,
          repeatDelay = 4.0f,
          spawnLocation = 1,
          spawnAmmount = 2,
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages =
              { "The next few waves will have clustered enemies, against whom the Mantis tower excels.",
                "It does damage in an area around its punch and its damage upgrades very well." }
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 6,
              repeatDelay = 4.0f,
              spawnLocation = 0,
              spawnAmmount = 4,
            },
          },
        },
        new WaitUntilDeadWave() {},
        new SpacerWave() {
          delay = 4.0f,
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 4,
              repeatDelay = 4.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 4,
              repeatDelay = 4.0f,
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
              repeatDelay = 12.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new DelayedWave() {
              warmup = 36.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = beetle,
                repetitions = 3,
                repeatDelay = 6.0f,
                spawnLocation = 1,
                spawnAmmount = 1,
              },
            },
            new CannedEnemyWave() {
              enemyDataKey = slowAphid,
              repetitions = 50,
              repeatDelay = 1.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
            new DelayedWave() {
              warmup = 1.0f,
              wave = new CannedEnemyWave() {
                enemyDataKey = slowAphid,
                repetitions = 18,
                repeatDelay = 2.6f,
                spawnLocation = 1,
                spawnAmmount = 2,
              },
            },
          },
        },
      },
    };
    // Nu: 440

    SequentialWave secondWave = new() {
      Subwaves = {
        new ConcurrentWave {
          Subwaves = {
            new SequentialWave {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 5,
                  repeatDelay = 4.0f,
                  spawnLocation = 1,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 6,
                  repeatDelay = 2.8f,
                  spawnLocation = 1,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 6,
                  repeatDelay = 2.8f,
                  spawnLocation = 1,
                  spawnAmmount = 2,
                },
                new SpacerWave() {
                  delay = 3.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 4,
                  repeatDelay = 2.5f,
                  spawnLocation = 1,
                  spawnAmmount = 3,
                },
              },  // Subwaves
            },  // SequentialWave
            new SequentialWave {
              Subwaves = {
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 5,
                  repeatDelay = 4.0f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 6,
                  repeatDelay = 2.8f,
                  spawnLocation = 0,
                  spawnAmmount = 1,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 6,
                  repeatDelay = 2.8f,
                  spawnLocation = 0,
                  spawnAmmount = 2,
                },
                new SpacerWave() {
                  delay = 3.0f,
                },
                new CannedEnemyWave() {
                  enemyDataKey = aphid,
                  repetitions = 4,
                  repeatDelay = 2.5f,
                  spawnLocation = 0,
                  spawnAmmount = 3,
                },
              },  // Subwaves
            },  // SequentialWave
          },  // Subwaves
        },  // ConcurrentWave
      },  // Subwaves
    };  // SequentialWave

    // wave ideas:
    // beetles w/ tarantula miniboss
    // 

    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
