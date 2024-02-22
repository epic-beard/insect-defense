using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.F)) {
      Spawn();
    }
  }

  private void Spawn() {
    ObjectPool.Instance.InstantiateEnemy(data, start);
  }
}
