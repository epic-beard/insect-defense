#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class EnemySpawnIndicatorManager : MonoBehaviour {
  public static EnemySpawnIndicatorManager Instance;

  public EnemySpawnTimes? EnemySpawnTimes;

  private float leadWarnTime = 5.0f;
  private List<Material> spawnMaterials = new();
  private Color road;

  private void Awake() {
    Instance = this;
    Spawner.UpdateSpawnIndicatorData += UpdateData;
  }

  private void Start() {
    // TODO(emonzon): Get this from resources (move Tile to resources directory).
    road = new Color(0.7169812f, 0.4447915f, 0.3273763f);

    foreach (Waypoint spawnPoint in Spawner.Instance.SpawnLocations) {
      spawnMaterials.Add(spawnPoint.transform.Find("Cube").GetComponent<Renderer>().material);
    }
  }

  public void UpdateData(EnemySpawnTimes? spawnTimes) {
    EnemySpawnTimes = spawnTimes;
  }

  // TODO(emonzon): Complete this class to realize the enemy spawn warning indicator.
  void Update() {
    if (EnemySpawnTimes == null) return;


    for (int i = 0; i < EnemySpawnTimes.Count; i++) {
      var spawnPointSpawns = EnemySpawnTimes[i];
      bool warning = false;
      bool active = false;
      // TODO(emonzon): Upgrade this to fire per-enemy per-spawn point.
      foreach (var enemyType in spawnPointSpawns.Keys) {
        float now = Time.time;

        var spawnsAtPointAndType = spawnPointSpawns[enemyType];
        if (spawnsAtPointAndType.Count == 0) continue;

        var firstSpawnOfEnemyType = spawnsAtPointAndType[0];
        // Remove any old intervals.
        while (firstSpawnOfEnemyType.Item2 < now) {
          spawnsAtPointAndType.RemoveAt(0);
          if (spawnsAtPointAndType.Count > 0) {
            firstSpawnOfEnemyType = spawnsAtPointAndType[0];
          } else {
            break;
          }
        }
        // Check for spawn indiciator changes.
        if (firstSpawnOfEnemyType.Item1 <= now
            && firstSpawnOfEnemyType.Item2 >= now) {
          //Debug.Log("Found an active interval");
          active = true;
        } else if (firstSpawnOfEnemyType.Item1 < (now + leadWarnTime)
            && firstSpawnOfEnemyType.Item2 >= now) {
          //Debug.Log("Found a soon-to-be active interval");
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

  void PrintEnemySpawnTimes(String lead, EnemySpawnTimes? spawnTimes) {
    string result = "";
    if (spawnTimes == null) {
      Debug.Log(lead + result);
      return;
    }
    for (int i = 0; i < spawnTimes.Count; i++) {
      result += "Spawn point: " + i + "\n";
      foreach (var enemyType in spawnTimes[i].Keys) {
        result += enemyType.ToString() + ": { ";
        foreach (var spawnInterval in spawnTimes[i][enemyType]) {
          result += "(" + spawnInterval.Item1 + ", " + spawnInterval.Item2 + " ), ";
        }
        result += "}\n";
      }
    }
    Debug.Log(lead + result);
  }

  private void SetWarningIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", Color.yellow);
  }

  private void SetSpawningIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", Color.red);
  }

  private void SetSafeIndicator(int spawnPoint) {
    spawnMaterials[spawnPoint].SetColor("_Color", road);
  }
}
