#nullable enable
using System.Collections.Generic;
using UnityEngine;

using EnemyKey = System.Tuple<EnemyData.Type, int>;

public class ObjectPool : MonoBehaviour {
#pragma warning disable 8618
  static public ObjectPool Instance;
#pragma warning restore 8618

  [SerializeField] private int startingSize = 20;
  [SerializeField] private int infectionLevels = 3;
  //readonly private Dictionary<EnemyData.Type, Queue<GameObject>> objectPools = new();
  readonly private Dictionary<EnemyKey, Queue<GameObject>> objectPools = new();
  readonly private HashSet<Enemy> activeEnemies = new();
  readonly private Dictionary<EnemyData.Type, string> enemyMap = new() {
    { EnemyData.Type.ANT, "Enemies/Ant/Ant" },
    { EnemyData.Type.APHID, "Enemies/Aphid/Aphid" },
    { EnemyData.Type.BEETLE, "Enemies/Beetle/Beetle" },
    { EnemyData.Type.HERCULES_BEETLE, "Enemies/Hercules Beetle/Hercules Beetle" },
    { EnemyData.Type.LEAF_BUG, "Enemies/Leaf Bug/Leaf Bug" },
    { EnemyData.Type.SLUG, "Enemies/Slug/Slug" },
    { EnemyData.Type.SNAIL, "Enemies/Snail/Snail"},
    { EnemyData.Type.STINK_BUG, "Enemies/Stink Bug/Stink Bug" },
    { EnemyData.Type.TARANTULA, "Enemies/Tarantula/Tarantula" },
    { EnemyData.Type.TERMITE, "Enemies/Termite/Termite" },
    { EnemyData.Type.WOLF_SPIDER, "Enemies/Wolf Spider/Wolf Spider" },
  };
  readonly private Dictionary<EnemyData.Type, GameObject> prefabMap = new();

  // InitializeObjectPool should be called before this class is used,
  // otherwise the initial pool sizes will not be setup.
  void Awake() {
    Instance = this;
  }

  // Returns an enemy with the given data and position.  If the cooresponding pool is
  // not empty then a pre-created gameObject is returned, otherwise a new one is instantiated.
  public GameObject InstantiateEnemy(EnemyData data, Waypoint start, Transform? parent = null) {
    // TODO(emonzon): Adjust the resource load to include adding different prefabs depending on
    //                infection level. Something like:
    //                Resources.Load<GameObject>(string.Concat(enemyMap[data.type], data.infectionLevel)
    EnemyKey enemyKey = new(data.type, data.infectionLevel);
    if (!objectPools.ContainsKey(enemyKey)) {
      //prefabMap.Add(data.type, Resources.Load<GameObject>(enemyMap[data.type]));
      prefabMap.Add(data.type, Resources.Load<GameObject>(GetResourceLoadPath(data)));
      objectPools.Add(enemyKey, new Queue<GameObject>());
      Debug.Log("WARNING: missing object pool for type: " + data.type.ToString());
    }
    var pool = objectPools[enemyKey];

    GameObject gameObject;
    if (pool.Count != 0) {
      gameObject = pool.Dequeue();
      gameObject.SetActive(true);
    } else {
      gameObject = Instantiate(prefabMap[data.type]);
    }
    if (parent == null) {
      gameObject.transform.position = start.transform.position;
    } else {
      gameObject.transform.position = parent.transform.position;
    }

    Enemy enemy = gameObject.GetComponent<Enemy>();
    enemy.Data = data;
    enemy.PrevWaypoint = start;

    float variance = enemy.Data.spawnVariance;
    Vector3 position = gameObject.transform.position;
    float xVariance = UnityEngine.Random.Range(-variance, variance);
    float zVariance = UnityEngine.Random.Range(-variance, variance);
    position.x += xVariance;
    position.z += zVariance;
    enemy.transform.position = position;
    enemy.SetVariance(xVariance, zVariance);

    enemy.Initialize(start);

    activeEnemies.Add(enemy);

    return gameObject;
  }

  // Deactivates an enemy and enqueues it back on the correct objectPool.
  public void DestroyEnemy(GameObject gameObject) {
    gameObject.SetActive(false);
    Enemy enemy = gameObject.GetComponent<Enemy>();
    activeEnemies.Remove(enemy);
    EnemyKey enemyKey = new(enemy.Type, enemy.InfectionLevel);
    if (!objectPools.ContainsKey(enemyKey)) {
      prefabMap.Add(enemy.Type, Resources.Load<GameObject>(GetResourceLoadPath(enemy.Data)));
      objectPools.Add(enemyKey, new Queue<GameObject>());
      Debug.Log("WARNING: Missing type from ObjectPool.");
    }
    objectPools[enemyKey].Enqueue(gameObject);
  }

  // Deactivates all enemies and enqueues them back on the correct objectPool.
  public void DestroyAllEnemies() {
    foreach (Enemy enemy in activeEnemies) {
      GameObject enemyObject = enemy.gameObject;
      enemyObject.SetActive(false);
      EnemyKey enemyKey = new(enemy.Type, enemy.InfectionLevel);
      if (!objectPools.ContainsKey(enemyKey)) {
        prefabMap.Add(enemy.Type, Resources.Load<GameObject>(GetResourceLoadPath(enemy.Data)));
        objectPools.Add(enemyKey, new Queue<GameObject>());
        Debug.Log("WARNING: Missing type from ObjectPool.");
      }
      objectPools[enemyKey].Enqueue(enemyObject);
    }
    activeEnemies.Clear();
  }

  // This returns the active enemies. Individual enemies may be modified but the
  // list istself should never be modified outside the ObjectPool class.
  public HashSet<Enemy> GetActiveEnemies() {
    return activeEnemies;
  }

  // Creates startingSize enemies for each prefab, populating the objectPools map.
  public void InitializeObjectPool(HashSet<EnemyData.Type> enemyTypes) {
    foreach (var kvp in enemyMap) {
      prefabMap.Add(kvp.Key, Resources.Load<GameObject>(kvp.Value));
    }

    foreach (var type in enemyTypes) {
      for (int infection = 0; infection < infectionLevels; infection++) {
        EnemyKey key = new(type, infection);
        objectPools[key] = new Queue<GameObject>();
        for (int i = 0; i < startingSize; i++) {
          GameObject gameObject = Instantiate(prefabMap[type]);
          gameObject.SetActive(false);
        
          objectPools[key].Enqueue(gameObject);
        }

      }
    }
  }

  private string GetResourceLoadPath(EnemyData data) {
    return string.Concat(enemyMap[data.type], "_", data.infectionLevel);
  }
}
