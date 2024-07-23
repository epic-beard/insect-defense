using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

using EnemyKey = System.Tuple<EnemyData.Type, int>;

[TestFixture]
public class ObjectPoolTest {
  private ObjectPool objectPool;
  private EnemyData enemyData;
  private Waypoint waypoint;
  private HashSet<EnemyKey> keys;
  [SetUp]
  public void SetUp() {
    keys = new HashSet<EnemyKey>() { new(EnemyData.Type.BEETLE, 0) };

    enemyData = new EnemyData() {
      maxHP = 10,
      type = EnemyData.Type.BEETLE, 
      infectionLevel = 0,
    };

    waypoint = new GameObject().AddComponent<Waypoint>();
  }

  // Creates an ObjecPool of a starting size n. Checks that each pool in
  // the ObjectPool has Count == n.
  [Test]
  public void InitializeObjectPoolWorks([Values(0, 1, 10)] int startingSize) {
    objectPool = new GameObject().AddComponent<ObjectPool>();
    objectPool.SetStartingSize(startingSize);
    objectPool.InitializeObjectPool(keys);
    var objectPools = objectPool.GetObjectPools();

    Assert.That(objectPools.Count, Is.EqualTo(objectPool.NumInfectionLevels));
    foreach (var (_, pool) in objectPools) {
      Assert.That(pool.Count, Is.EqualTo(startingSize));
    }
  }

  // Instantiate an enemy and check that it is active and has the correct position and
  // enemy data.
  [Test]
  public void InstantiateEnemyWorks() {
    objectPool = new GameObject().AddComponent<ObjectPool>();
    objectPool.SetStartingSize(1);
    objectPool.InitializeObjectPool(keys);
    Enemy enemy = objectPool.InstantiateEnemy(enemyData, waypoint);

    Assert.That(enemy.enabled);
    Assert.That(enemy.PrevWaypoint, Is.SameAs(waypoint));
    Assert.That(enemy.Data, Is.EqualTo(enemyData));
  }

  // Set starting size to 1 so there is exactly one enemey of each type.  Create an enemy
  // and save a reference to it, destroy that enemy and create a new one and save a reference to it.
  // Both references should point to the same GameObject. 
  [Test]
  public void DestroyEnemyWorks() {
    objectPool = new GameObject().AddComponent<ObjectPool>();
    objectPool.SetStartingSize(1);
    objectPool.InitializeObjectPool(keys);
    Enemy enemy1 = objectPool.InstantiateEnemy(enemyData, waypoint);
    GameObject gameObject1 = enemy1.gameObject;
    objectPool.DestroyEnemy(enemy1);
    Enemy enemy2 = objectPool.InstantiateEnemy(enemyData, waypoint);
    GameObject gameObject2 = enemy2.gameObject;

    Assert.That(gameObject1, Is.SameAs(gameObject2));
  }

  // Create an ObjectPool with starting size 1 and create two enemies.  ObjectPool should
  // return a new enemy for each all.
  [Test]
  public void ObjectPoolResizeWorks() {
    objectPool = new GameObject().AddComponent<ObjectPool>();
    objectPool.SetStartingSize(1);
    objectPool.InitializeObjectPool(keys);
    Enemy enemy1 = objectPool.InstantiateEnemy(enemyData, waypoint);
    Enemy enemy2 = objectPool.InstantiateEnemy(enemyData, waypoint);

    Assert.That(enemy1, Is.Not.SameAs(enemy2));
  }
}

// Extension methods to hold reflection-based calls to access private fields, properties, or methods of
// ObjectPool.
public static class ObjectPoolUtils {
  public static void SetActiveEnemies(this ObjectPool objectPool, HashSet<Enemy> enemies) {
    typeof(ObjectPool)
        .GetField("activeEnemies", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(objectPool, enemies);
  }

  public static Dictionary<EnemyKey, Queue<Enemy>> GetObjectPools(this ObjectPool objectPool) {
    return (Dictionary<EnemyKey, Queue<Enemy>>)typeof(ObjectPool)
      .GetField("objectPools", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(objectPool);
  }

  public static void SetStartingSize(this ObjectPool objectPool, int size) {
    typeof(ObjectPool)
      .GetField("startingSize", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(objectPool, size);
  }
}
