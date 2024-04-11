using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Start() {
    Enum.GetValues(typeof(EnemyData.Type));

    HashSet<EnemyData.Type> allEnemyTypes =
      new HashSet<EnemyData.Type>(Enum.GetValues(typeof(EnemyData.Type)).OfType<EnemyData.Type>().ToList());
    ObjectPool.Instance.InitializeObjectPool(allEnemyTypes);
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
