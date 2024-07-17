using NUnit.Framework;
using static LevelGeneratorStatics;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level3WaveGenerator {
  public string filename = directory + "level3.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    // Side differences: Slow things on the left, fast on the right.
    // Nu: 500
    SequentialWave firstWave = new(
      new ConcurrentWave(
        new CannedEnemyWave() {
          enemyDataKey = ant0,
          repetitions = 8,
          repeatDelay = 5.0f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },
        new DelayedWave() {
          warmup = 20.0f,
          wave = new CannedEnemyWave() {
            enemyDataKey = ant1,
            repetitions = 3,
            repeatDelay = 7.0f,
            spawnLocation = 1,
            spawnAmmount = 1,
          },
        },
        new DelayedWave() {
          warmup = 22.0f,
          wave = new DialogueBoxWave() {
            messages =
                { "The infection looks stronger in some of those ants.",
                  "Look out for enhanced attributes!" },
          }
        }
      ),  // ConcurrentWave
      new SpacerWave() {
        delay = 12.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          spawnLocation = 1,
          duration = 60.0f,
          repeatDelay = 7.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            enemyDataKey = ant1,
          },
          new() {
            enemyDataKey = aphid0,
            warmup = 2.0f,
            cooldown = 40.0f,
          },
          new() {
            enemyDataKey = aphid1,
            warmup = 22.0f,
            cooldown = 20.0f,
          },
          new() {
            enemyDataKey = aphid1,
            warmup = 42.0f,
            Positions = new() { new(0, -2), new(0, 2) }
          },
        }
      ),  // GetConcurrentWaveWithDefaults
      new SpacerWave() {
        delay = 5.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 60.0f,
          spawnLocation = 1,
          repeatDelay = 6.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            enemyDataKey = ant1,
          },
          new() {
            enemyDataKey = aphid1,
            cooldown = 30.0f,
          },
          new() {
            enemyDataKey = aphid1,
            warmup = 30.0f,
            Positions = new() { new(-3, 2), new(3, 2) }
          }
        }
      ),  // GetConcurrentWaveWithDefaults
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 40.0f,
          spawnLocation = 1,
        },
        metrics: new WaveMetrics[] {
          new() {
            enemyDataKey = ant1,
            repeatDelay = 5.0f,
          },
          new() {
            enemyDataKey = aphid1,
            repeatDelay = 5.0f,
            Positions = new() { new(-3, 3), new(0, 3), new(3, 3) }
          }
        }
      )  // GetConcurrentWaveWithDefaults
    );
    // Nu: 1482

    SequentialWave secondWave = new(
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 105.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            enemyDataKey = ant0,
            spawnLocation = 0,
            repeatDelay = 5.0f,
            warmup = 15.0f,  // 20 sec duration
            cooldown = 70.0f,
          },
          new() {
            enemyDataKey = beetle0,
            spawnLocation = 0,
            repeatDelay = 7.0f,
            warmup = 35.0f,  // 40 sec duration
            cooldown = 30.0f,
          },
          new() {
            enemyDataKey = tarantula0,
            spawnLocation = 0,
            repeatDelay = 10.0f,
            warmup = 75.0f,  // 30 seconds
          },
          new() {
            enemyDataKey = aphid1,
            spawnLocation = 1,
            repeatDelay = 1.5f,
          },
          new() {
            enemyDataKey = ant1,
            spawnLocation = 1,
            repeatDelay = 7.0f,
          },
          new() {
            enemyDataKey = wolfSpider0,
            spawnLocation = 1,
            warmup = 65.0f,
            repeatDelay = 12.0f,
          }
        }
      ),  // Nu: 3530
      new SpacerWave() {
        delay = 10.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 105.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            enemyDataKey = tarantula0,
            spawnLocation = 0,
            repeatDelay = 10.0f,
          },
          new() {
            enemyDataKey = beetle0,
            spawnLocation = 0,
            repeatDelay = 10.0f,
            warmup = 20.0f,
            cooldown = 85.0f,
          },
          new() {
            enemyDataKey = beetle0,
            spawnLocation = 0,
            repeatDelay = 7.0f,
            warmup = 50.0f,
            cooldown = 25.0f,
          },
          new() {
            enemyDataKey = beetle0,
            spawnLocation = 0,
            repeatDelay = 4.0f,
            warmup = 85.0f,
          },
          new() {
            enemyDataKey = wolfSpiderMother0,
            spawnLocation = 1,
            repeatDelay = 7.0f,
          },  // Wolf spider mother.
          new() {
            enemyDataKey = wolfSpider0,
            spawnLocation = 1,
            repeatDelay = 6.0f,
            warmup = 50.0f,
          },
        }
      )
    );
    //  Nu: 

    SequentialWave thirdWave = new();

    Waves waves = new() {
      waves = { secondWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
