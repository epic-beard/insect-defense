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
    Spawner.Instance.SpawnWave(Deserialize<Spawner.Wave>(filename));
  }
}
