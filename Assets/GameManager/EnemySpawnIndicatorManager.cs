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

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    Spawner.Instance.UpdateSpawnIndicatorData += UpdateData;
  }

  public void UpdateData() {
    EnemySpawnTimes = Spawner.Instance.GetSpawnTimes();
  }

  // TODO(emonzon): Complete this class to realize the enemy spawn warning indicator.
  void Update() {
    if (EnemySpawnTimes == null) return;
  }

  private void SetWarningIndicator(int spawnPoint) { }

  private void SetSpawningIndicator(int spawnPoint) { }

  private void SetSafeIndicator(int spawnPoint) { }
}
