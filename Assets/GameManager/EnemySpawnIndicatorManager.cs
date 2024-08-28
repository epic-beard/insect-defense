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

  // Update is called once per frame
  void Update() {
    if (EnemySpawnTimes == null) return;
    // Process EnemySpawnTimes and set indicator status appropriately.
  }

  // TODO(emonzon): Current plan
  //   For each wave's OnComplete method, call an update method in this class.
  //   This class will require a merge tool for the EnemySpawnTimes. This class
  //   will only remember present and future waves, it has no memory of the past.

  private void SetWarningIndicator(int spawnPoint) { }

  private void SetSpawningIndicator(int spawnPoint) { }

  private void SetSafeIndicator(int spawnPoint) { }
}
