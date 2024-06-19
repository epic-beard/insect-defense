using NUnit.Framework;
using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class PositionBasedSpawningTestGenerator {
  public string ant = "Ant_IL0";
  public string filename = "Waves/position_spawning_test.waves";

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
          repeatDelay = 4.0f,
          spawnLocation = 0,
          spawnAmmount = 0,
          Positions = new() {
            new(0, 3), new(-3, -3), new(3, -3)
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
