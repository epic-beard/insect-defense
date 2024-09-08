using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class EnemySpawnIndicatorManager : MonoBehaviour {
  public static EnemySpawnIndicatorManager Instance;

  public EnemySpawnTimes EnemySpawnTimes;

  private float leadWarnTime = 5.0f;
  private List<Material> spawnMaterials = new();
  private Color road;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    Spawner.Instance.UpdateSpawnIndicatorData += UpdateData;
    road = new Color(0.7169812f, 0.4447915f, 0.3273763f);

    foreach (Waypoint spawnPoint in Spawner.Instance.SpawnLocations) {
      spawnMaterials.Add(spawnPoint.transform.Find("Cube").GetComponent<Renderer>().material);
    }
  }

  public void UpdateData() {
    EnemySpawnTimes = Spawner.Instance.GetSpawnTimes();
  }

  void Update() {
    if (EnemySpawnTimes == null) return;

    for (int i = 0; i < EnemySpawnTimes.Count; i++) {
      var spawnPointSpawns = EnemySpawnTimes[i];
      bool warning = false;
      bool active = false;
      // TODO(emonzon): Upgrade this to fire per-enemy per-spawn point.
      foreach (var enemyType in spawnPointSpawns.Keys) {

        var spawnsAtPointAndType = spawnPointSpawns[enemyType];
        if (spawnsAtPointAndType.Count == 0) continue;

        var firstSpawnOfEnemyType = spawnsAtPointAndType[0];
        // Remove any old intervals.
        while (firstSpawnOfEnemyType.Item2 < Time.time) {
          spawnsAtPointAndType.RemoveAt(0);
          if (spawnsAtPointAndType.Count > 0) {
            firstSpawnOfEnemyType = spawnsAtPointAndType[0];
          } else {
            break;
          }
        }
        // Check for spawn indiciator changes.
        if ((firstSpawnOfEnemyType.Item1 <= Time.time
            && firstSpawnOfEnemyType.Item2 >= Time.time)) {
          active = true;
        } else if (firstSpawnOfEnemyType.Item1 < (Time.time + leadWarnTime)
            && firstSpawnOfEnemyType.Item2 >= Time.time) {
          warning = true;
        }
        break;
      }
      if (active) {
        SetSpawningIndicator(i);
      } else if (warning) {
        SetWarningIndicator(i);
      } else {
        SetSafeIndicator(i);
      }
    }
  }

  private void SetWarningIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", Color.yellow);
  }

  private void SetSpawningIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", Color.yellow);
  }

  private void SetSafeIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", road);
  }
}
