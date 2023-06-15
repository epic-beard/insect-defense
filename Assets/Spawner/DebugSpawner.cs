using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.A)) {
      Spawn();
    }
  }

  private void Spawn() {
    ObjectPool.Instance.InstantiateEnemy(data, start);
  }
}
