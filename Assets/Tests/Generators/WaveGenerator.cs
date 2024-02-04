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
          enemyDataKey = ant,
          repetitions = 6,
          repeatDelay = 2.0f,
          spawnLocation = 0,
          spawnAmmount = 1,
        },
        new SpacerWave() {
          delay = 2.0f,

        },
        new DialogueBoxWave() {
          messages = { "Now might be a good time to build a second tower." }

        },
        new CannedEnemyWave() {
          enemyDataKey = ant,
          repetitions = 3,
          repeatDelay = 1.0f,
          spawnLocation = 0,
          spawnAmmount = 2,
        },
      },
    };

    SequentialWave secondWave = new() {
      Subwaves = {
        new DialogueBoxWave() {
          messages = { "It looks like you have some Nu left, it is about time to upgrade a tower.",
                       "First, click on the tower you wish to ugprade to select it.",
                       "Now check out the available upgrades.",
                       "I'll leave the next wave to you." }
        },
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 5,
              repeatDelay = 3.0f,
              spawnLocation = 0,
              spawnAmmount = 2,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 15,
              repeatDelay = 1.0f,
              spawnLocation = 0,
              spawnAmmount = 1,
            },
          },
        },
      },
    };

    SequentialWave thirdWave = new() {
      Subwaves = {
        new ConcurrentWave {
          Subwaves = {
            new CannedEnemyWave() {
              enemyDataKey = ant,
              repetitions = 3,
              repeatDelay = 2.0f,
              spawnLocation = 0,
              spawnAmmount = 2,
            },
            new CannedEnemyWave() {
              enemyDataKey = aphid,
              repetitions = 3,
              repeatDelay = 2.0f,
              spawnLocation = 0,
              spawnAmmount = 4,
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
      waves = { firstWave, secondWave, thirdWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
