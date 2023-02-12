using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  private ObjectPool pool;
  [SerializeField] private EnemyData data;
  void Awake() {
    pool = FindObjectOfType<ObjectPool>();
    data = new() {
      type = EnemyData.Type.BEETLE,
      speed = 1.0f
    };
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
