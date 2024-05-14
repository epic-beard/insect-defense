using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Start() {
    Enum.GetValues(typeof(EnemyData.Type));

    ObjectPool.Instance.InitializeObjectPool(new HashSet<Tuple<EnemyData.Type, int>> { });
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.F)) {
      Spawn();
    }
  }

  private void Spawn() {
    ObjectPool.Instance.InstantiateEnemy(data, start);
  }
}
