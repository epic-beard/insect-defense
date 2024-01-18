using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class WaveGenerator {
  public string aphid = "Aphid";
  public string ant = "Ant";
  public string beetle = "Beetle";
  public string filename = "Waves/level1.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    DialogueBoxWave greeting = new() {
      message = "Hello"
    };

    CannedEnemyWave firstWaveAnt = new() {
      enemyDataKey = ant,
      repetitions = 6,
      repeatDelay = 2.0f,
      spawnLocation = 0,
      spawnAmmount = 1,
    };

    SequentialWave firstWave = new() {
      Subwaves = { greeting, firstWaveAnt },
    };

    CannedEnemyWave secondWaveAnt = new() {
      enemyDataKey = ant,
      repetitions = 3,
      repeatDelay = 3.0f,
      spawnLocation = 0,
      spawnAmmount = 1,
    };
    CannedEnemyWave secondWaveAphid = new() {
      enemyDataKey = aphid,
      repetitions = 3,
      repeatDelay = 3.0f,
      spawnLocation = 0,
      spawnAmmount = 3,
    };
    ConcurrentWave secondWave = new() {
      Subwaves = { secondWaveAnt, secondWaveAphid },
    };

    CannedEnemyWave thirdWaveAnt = new() {
      enemyDataKey = ant,
      repetitions = 3,
      repeatDelay = 2.0f,
      spawnLocation = 0,
      spawnAmmount = 1,
    };
    CannedEnemyWave thirdWaveAphid = new() {
      enemyDataKey = aphid,
      repetitions = 3,
      repeatDelay = 2.0f,
      spawnLocation = 0,
      spawnAmmount = 3,
    };
    ConcurrentWave thirdWaveAntAphid = new() {
      Subwaves = { thirdWaveAnt, thirdWaveAphid },
    };
    SpacerWave thirdWaveSpacer = new() {
      delay = 1.0f,
    };
    DialogueBoxWave beetleWarning = new() {
      message = "Beetles are armored enemies, make sure you have some armor tear or peirce to damage them!"
    };
    CannedEnemyWave thirdWaveBeetle = new() {
      enemyDataKey = beetle,
      repetitions = 1,
      repeatDelay = 0.0f,
      spawnLocation = 0,
      spawnAmmount = 1,
    };
    SequentialWave thirdWave = new() {
      Subwaves = { thirdWaveAntAphid, thirdWaveSpacer, beetleWarning, thirdWaveBeetle },
    };

    Waves waves = new() {
      waves = { firstWave, secondWave, thirdWave },
    };

    Serialize<Wave>(waves, filename);
  }
}
