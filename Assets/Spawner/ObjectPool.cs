#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
#pragma warning disable 8618
  static public ObjectPool Instance;
#pragma warning restore 8618

  // An ugly artifact, just a pair class.
  // [TODO: nnewsom] get rid of this with Addressables or a SerializableDictionary.
  [Serializable]
  public class EnemyEntry {
    [SerializeField] public EnemyData.Type type;
#pragma warning disable 8618
    [SerializeField] public GameObject prefab;
#pragma warning restore 8618
  }

  [SerializeField] private int startingSize = 20;
  [SerializeField] private List<EnemyEntry> entries = new();
  readonly private Dictionary<EnemyData.Type, Queue<GameObject>> objectPools = new();
  readonly private Dictionary<EnemyData.Type, GameObject> prefabs = new();
  readonly private HashSet<Enemy> activeEnemies = new();

  void Awake() {
    Instance = this;

    foreach (var entry in entries) {
      prefabs[entry.type] = entry.prefab;
    }
    InitializeObjectPool();
  }

  // Returns an enemy with the given data and position.  If the cooresponding pool is
  // not empty then a pre-created gameObject is returned, otherwise a new one is instantiated.
  public GameObject InstantiateEnemy(EnemyData data, Waypoint start, Transform? parent = null) {
    var pool = objectPools[data.type];
    GameObject gameObject;
    if (pool.Count != 0) {
      gameObject = pool.Dequeue();
      gameObject.SetActive(true);
    } else {
      if (parent == null) {
        gameObject = GameObject.Instantiate(prefabs[data.type]);
      } else {
        gameObject = GameObject.Instantiate(prefabs[data.type], parent);
      }
    }
    Enemy enemy = gameObject.GetComponent<Enemy>();
    enemy.Data = data;
    enemy.PrevWaypoint = start;
    enemy.enabled = true;
    activeEnemies.Add(enemy);

    return gameObject;
  }

  // Deactivates an enemy and enqueues it back on the correct objectPool.
  public void DestroyEnemy(GameObject gameObject) {
    gameObject.SetActive(false);
    Enemy enemy = gameObject.GetComponent<Enemy>();
    activeEnemies.Remove(enemy);
    EnemyData.Type type = enemy.Data.type;
    enemy.enabled = false;
    objectPools[type].Enqueue(gameObject);
  }

  // This returns the active enemies. Individual enemies may be modified but the
  // list istself should never be modified outside the ObjectPool class.
  public HashSet<Enemy> GetActiveEnemies() {
    return activeEnemies;
  }

  // Creates startingSize enemies for each prefab, populating the objectPools map.
  private void InitializeObjectPool() {
    foreach (var (type, prefab) in prefabs) {
      objectPools[type] = new Queue<GameObject>();
      for (int i = 0; i < startingSize; i++) {
        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.enabled = false;
        objectPools[type].Enqueue(gameObject);
      }
    }
  }
}
