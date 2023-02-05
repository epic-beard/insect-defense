using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
  [Serializable]
  public class EnemyEntry {
    [SerializeField] public EnemyData.Type type;
    [SerializeField] public GameObject prefab;
  }

  readonly private int startingSize = 20;
  [SerializeField] private List<EnemyEntry> entries = new();
  private Dictionary<EnemyData.Type, Queue<GameObject>> objectPools = new();
  private Dictionary<EnemyData.Type, GameObject> prefabs = new();

  void Awake() {
    foreach (var entry in entries) {
      prefabs[entry.type] = entry.prefab;
    }

    InitializeObjectPool();
  }

  public GameObject InstantiateEnemy(EnemyData data, Vector3 position) {
    var pool = objectPools[data.type];
    GameObject gameObject;
    if (pool.Count != 0) {
      gameObject = pool.Dequeue();
      gameObject.SetActive(true);
    } else {
      gameObject = GameObject.Instantiate(prefabs[data.type]);
    }
    gameObject.transform.position = position;
    gameObject.GetComponent<Enemy>().data = data;

    return gameObject;
  }

  public void DestroyEnemy(GameObject gameObject) {
    EnemyData.Type type = gameObject.GetComponent<Enemy>().data.type;
    gameObject.SetActive(false);
    objectPools[type].Enqueue(gameObject);
  }

  private void InitializeObjectPool() {
    foreach (var (type, _) in prefabs) {
      objectPools[type] = new Queue<GameObject>();
    }

    foreach (var (type, prefab) in prefabs) {
      for (int i = 0; i < startingSize; i++) {
        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        objectPools[type].Enqueue(gameObject);
      }
    }
  }
}
