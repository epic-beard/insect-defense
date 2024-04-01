using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class TaggedWaitTestWaveGenerator {
  public string ant = "Ant";
  public string filename = "Waves/tag_wait_test.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    SequentialWave firstWave = new() {
      Subwaves = {
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 1,
          repeatDelay = 4.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
          WaveTag = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 1,
          repeatDelay = 0.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new WaitUntilDeadWave() {
          WaveTag = 1,
        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 1,
          repeatDelay = 0.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        }
      },
    };

    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
