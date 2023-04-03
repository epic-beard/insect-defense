using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  private ObjectPool pool;
  [SerializeField] private EnemyData data;
  void Awake() {
    pool = FindObjectOfType<ObjectPool>();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.A)) {
      Spawn();
    }
  }

  private void Spawn() {
    pool.InstantiateEnemy(data, start);
  }
}
