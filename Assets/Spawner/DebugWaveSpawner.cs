using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EpicBeardLib.XmlSerializationHelpers;

public class DebugWaveSpawner : MonoBehaviour {
  [SerializeField] private string filename;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.A)) {
      SpawnWave();
    }
  }

  private void SpawnWave() {
    Spawner.Instance.SpawnWaves(Deserialize<Spawner.Waves>(filename));
  }
}
