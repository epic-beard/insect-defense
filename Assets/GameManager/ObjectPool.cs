using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

  // An ugly artifact, just a pair class.
  // [TODO: nnewsom] get rid of this with Addressables or a SerializableDictionary.
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

  // Returns an enemy with the given data and position.  If the cooresponding pool is
  // not empty then a pre-created gameObject is returned, otherwise a new one is instantiated.
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

  // Deactivates an enemy and enqueues it back on the correct objectPool.
  public void DestroyEnemy(GameObject gameObject) {
    EnemyData.Type type = gameObject.GetComponent<Enemy>().data.type;
    gameObject.SetActive(false);
    objectPools[type].Enqueue(gameObject);
  }

  // Creates startingSize enemies for each prefab, populating the objectPools map.
  private void InitializeObjectPool() {
    foreach (var (type, prefab) in prefabs) {
      objectPools[type] = new Queue<GameObject>();
      for (int i = 0; i < startingSize; i++) {
        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        objectPools[type].Enqueue(gameObject);
      }
    }
  }
}
