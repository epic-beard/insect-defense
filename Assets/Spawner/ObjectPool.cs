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

  [SerializeField] private int startingSize = 20;
  [SerializeField] private List<EnemyEntry> entries = new();
  readonly private Dictionary<EnemyData.Type, Queue<GameObject>> objectPools = new();
  readonly private Dictionary<EnemyData.Type, GameObject> prefabs = new();
  [SerializeField] private List<Waypoint> startingPoints = new();

  void Awake() {
    foreach (var entry in entries) {
      prefabs[entry.type] = entry.prefab;
    }
    InitializeObjectPool();
  }

  // Returns an enemy with the given data and position.  If the cooresponding pool is
  // not empty then a pre-created gameObject is returned, otherwise a new one is instantiated.
  public GameObject InstantiateEnemy(EnemyData data, Waypoint start) {
    var pool = objectPools[data.type];
    GameObject gameObject;
    if (pool.Count != 0) {
      gameObject = pool.Dequeue();
      gameObject.SetActive(true);
    } else {
      gameObject = GameObject.Instantiate(prefabs[data.type]);
    }
    Enemy enemy = gameObject.GetComponent<Enemy>();
    enemy.data = data;
    enemy.PrevWaypoint = start;
    enemy.enabled = true;

    return gameObject;
  }

  public GameObject InstantiateEnemy(EnemyData data, int startingPoint) {
    return InstantiateEnemy(data, startingPoints[startingPoint]);
  }

  // Deactivates an enemy and enqueues it back on the correct objectPool.
  public void DestroyEnemy(GameObject gameObject) {
    EnemyData.Type type = gameObject.GetComponent<Enemy>().data.type;
    gameObject.SetActive(false);
    gameObject.GetComponent<Enemy>().enabled = false;
    objectPools[type].Enqueue(gameObject);
  }

  // Creates startingSize enemies for each prefab, populating the objectPools map.
  private void InitializeObjectPool() {
    foreach (var (type, prefab) in prefabs) {
      objectPools[type] = new Queue<GameObject>();
      prefab.GetComponent<Enemy>().pool = this;
      for (int i = 0; i < startingSize; i++) {
        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        gameObject.GetComponent<Enemy>().enabled = false;
        objectPools[type].Enqueue(gameObject);
      }
    }
  }
}
