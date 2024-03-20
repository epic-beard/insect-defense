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
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages =
              { "You don't have enough money for a new tower, but...",
                "You can sell this tower and build a new tower for the other track.",
                "When you sell a tower, you get back all the Nu you spent on it.",
                "It takes a little time to tear down and build a tower though." }
        },
        new SpacerWave() {
          delay = 3.0f,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 5,
          repeatDelay = 3.0f,
          spawnLocation = 1,
          spawnAmmount = 1,
        },
        new WaitUntilDeadWave() {},
        new DialogueBoxWave() {
          messages =
              { "The next few waves will have clustered enemies. The Mantis is great against those threats.",
                "It does damage in an area around its punch and its damage upgrades very well." }
        },
        new ConcurrentWave {
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
        new WaitUntilDeadWave() {},
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
        /*
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
        */
      },
    };


    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
