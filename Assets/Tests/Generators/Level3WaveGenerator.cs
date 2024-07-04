using NUnit.Framework;
using UnityEngine;
using static LevelGeneratorStatics;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level3WaveGenerator : MonoBehaviour {
  public string filename = directory + "level3.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    SequentialWave firstWave = new() {
      Subwaves = {
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
        ),
        //GetConcurrentWaveWithDefaults(
        //  defaults: new() {
        //    enemyDataKey = ant0,
        //    spawnLocation = 1,
        //    duration = 40.0f,
        //  },
        //  metrics: new WaveMetrics[] {
        //    new() {
        //      repeatDelay = 5.0f,
        //    },
        //    new() {
        //      warmup = 20.0f,
        //      enemyDataKey = ant1,
        //      repetitions = 3,
        //    },
        //  }
        //),
      }
    };

    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
