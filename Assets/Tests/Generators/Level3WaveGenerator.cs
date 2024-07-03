using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;
using static EpicBeardLib.XmlSerializationHelpers;
using static Spawner;

public class Level3WaveGenerator : MonoBehaviour {
  public string aphid = "Aphid_IL0";
  public string aphid2 = "Aphid_IL1";
  public string ant = "Ant_IL0";
  public string ant2 = "Ant_IL1";
  public string beetle = "Beetle_IL0";
  public string tarantula = "Tarantula_IL0";
  public string leafBug = "Leaf Bug_IL0";
  public string filename = "Waves/level3.waves";

  [Test]
  public void WaveGeneratorTest() {
    GenerateWave();
  }

  private void GenerateWave() {
    SequentialWave firstWave = new() {
    };

    ConcurrentWave testWave = new() {
      GetConcurrentWaveWithDefaults(
        defaults: new() {
          enemyDataKey = beetle,
          spawnLocation = 1,
          duration = 80.0f,
        },
        metrics: new WaveMetrics[] {
          new() {
            warmup = 2.0f,
            repeatDelay = 14.0f
          },
          new() {
            warmup = 37.0f,
            repeatDelay = 14.0f,
          }
        }
      )
    };

    Waves waves = new() {
      waves = { firstWave },
    };

    Serialize<Waves>(waves, filename);
  }
}
