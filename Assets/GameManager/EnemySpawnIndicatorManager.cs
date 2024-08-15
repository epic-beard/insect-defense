using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class EnemySpawnIndicatorManager : MonoBehaviour {
  public static EnemySpawnIndicatorManager Instance;

  private void Awake() {
    Instance = this;
  }

  // Update is called once per frame
  void Update() {
    List<Dictionary<EnemyData.Type, List<Tuple<float, float>>>> spawnTimes =
        Spawner.Instance.GetSpawnTimes();
    // TODO(emonzon): Write the code to process this data into warning lights.
  }
}
