using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Start() {
    var types = new HashSet<EnemyData.Type>() {
      EnemyData.Type.ANT,
      EnemyData.Type.APHID,
      EnemyData.Type.BEETLE,
      EnemyData.Type.LEAF_BUG,
      EnemyData.Type.TARANTULA
    };
    ObjectPool.Instance.InitializeObjectPool(types);
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
