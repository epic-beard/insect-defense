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
        delay = 8.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          spawnLocation = 1,
          duration = 60.0f,
          repeatDelay = 7.0f,
        },
        waveOrMetrics: new WaveMetrics[] {
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
        delay = 4.0f,
      },
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 60.0f,
          spawnLocation = 1,
          repeatDelay = 6.0f,
        },
        waveOrMetrics: new WaveMetrics[] {
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
        waveOrMetrics: new WaveMetrics[] {
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
    // Sec~: 4m

    SequentialWave secondWave = new(
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 105.0f,
        },
        waveOrMetrics: new WaveMetrics[] {
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
        waveOrMetrics: new WaveMetrics[] {
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
          },
          new() {
            enemyDataKey = wolfSpider0,
            spawnLocation = 1,
            repeatDelay = 6.0f,
            warmup = 50.0f,
          },
        }
      )  // GetConcurrentWaveWithDefaults
    );
    // Nu: 5240
    // Sec~: 8 min 30 sec

    SequentialWave thirdWave = new(
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 120.0f,
        },
        waveOrMetrics: new IWaveOrMetric[] {
          // Left side
          new WaveMetrics() {
            enemyDataKey = tarantula0,
            repetitions = 7,
            spawnLocation = 0,
            cooldown = 60.0f,
          },
          new WaveMetrics() {
            enemyDataKey = beetle0,
            repetitions = 20,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            warmup = 60.0f,
            enemyDataKey = ant1,
            repetitions = 20,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            warmup = 60.0f,
            enemyDataKey = tarantula0,
            repetitions = 12,
            spawnLocation = 0,
          },
          // Right side
          new WaveMetrics() {
            enemyDataKey = wolfSpider0,
            repetitions = 30,
            spawnLocation = 1,
          },
          new DelayedWave() {
            warmup = 25.0f,
            wave = new DialogueBoxWave() {
              messages =
                  { "Flies escaped containment on the right side feeder, they're coming!",
                    "You can't hit airborne enemies without anti-air. Luckily, the Web Shooting Spider and Spitting Ant towers have just that upgrade." },
            }
          },
          new WaveMetrics() {
            warmup = 30.0f,
            enemyDataKey = fly0,
            repetitions = 25,
            spawnLocation = 1,
          },
          new WaveMetrics() {
            warmup = 62.0f,
            enemyDataKey = fly0,
            repetitions = 10,
            spawnLocation = 1,
          },
          new WaveMetrics() {
            warmup = 90.0f,
            enemyDataKey = wolfSpiderMother0,
            repetitions = 6,
            spawnLocation = 1,
          },
        }  // IWaveOrMetrics
      )
    );
    // Nu: 10170
    // Sec~: 4m

    SequentialWave fourthWave = new(
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 40.0f,
        },
        waveOrMetrics: new IWaveOrMetric[] {
          new WaveMetrics() {
            enemyDataKey = beetle1,
            repetitions = 5,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            enemyDataKey = beetle0,
            repetitions = 20,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            enemyDataKey = fly0,
            repetitions = 20,
            spawnLocation = 1,
          },
          new WaveMetrics() {
            enemyDataKey = wolfSpiderMother0,
            repetitions = 12,
            spawnLocation = 1,
          },
        }
      ),
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          duration = 40.0f,
        },
        waveOrMetrics: new IWaveOrMetric[] {
          new WaveMetrics() {
            enemyDataKey = beetle1,
            repetitions = 10,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            enemyDataKey = tarantula0,
            repetitions = 6,
            spawnLocation = 0,
          },
          new WaveMetrics() {
            enemyDataKey = fly0,
            repetitions = 30,
            spawnLocation = 1,
          },
          new WaveMetrics() {
            enemyDataKey = wolfSpiderMother0,
            repetitions = 10,
            spawnLocation = 1,
          },
        }
      )
    );

    Waves waves = new() {
      waves = { firstWave, secondWave, thirdWave, fourthWave },
    };
    waves.Level = 3;

    Serialize<Waves>(waves, filename);
  }
}
