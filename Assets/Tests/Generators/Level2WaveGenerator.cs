using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level2WaveGenerator {
  public string aphid = "Aphid";
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

    SequentialWave firstWave = new() {
      Subwaves = {
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 2.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new SpacerWave() {
          delay = 2.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 2.0f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },
        new WaitUntilDeadWave() {},
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 4,
              repeatDelay = 3.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 4,
              repeatDelay = 3.0f,
              spawnLocation = 1,
              spawnAmmount = 1,
            },
          },
        },
        new ConcurrentWave {
          Subwaves = {
            new SequentialWave() {
              Subwaves = {
                new SpacerWave() {
                  delay = 2.0f,
                },
                new ConcurrentWave() {
                  Subwaves = {
                    new CannedEnemyWave() {
                      enemyDataKey = ant,
                      repetitions = 4,
                      repeatDelay = 4.0f,
                      spawnLocation = 0,
                      spawnAmmount = 1,
                    },
                    new CannedEnemyWave() {
                      enemyDataKey = aphid,
                      repetitions = 4,
                      repeatDelay = 4.0f,
                      spawnLocation = 0,
                      spawnAmmount = 4,
                    },
                  },
                },
              },
            },
            new ConcurrentWave() {
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
              }
            }
          },
        },
      },
    };


    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
