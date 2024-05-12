using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ObjectPoolTest {
  private ObjectPool objectPool;
  private EnemyData enemyData;
  private Waypoint waypoint;
  private HashSet<EnemyData.Type> types;
  [SetUp]
  public void SetUp() {
    objectPool = new GameObject().AddComponent<ObjectPool>();
    types = new HashSet<EnemyData.Type>() { EnemyData.Type.BEETLE };
    objectPool.SetStartingSize(1);

    enemyData = new EnemyData() {
      maxHP = 10,
      type = EnemyData.Type.BEETLE
    };

    waypoint = new GameObject().AddComponent<Waypoint>();
  }

  // Creates an ObjecPool of a starting size n. Checks that each pool in
  // the ObjectPool has Count == n.
  [Test]
  public void InitializeObjectPoolWorks([Values(0, 1, 10)] int startingSize) {
    objectPool.SetStartingSize(startingSize);
    objectPool.InitializeObjectPool(types);
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
    objectPool.InitializeObjectPool(types);
    GameObject gameObject = objectPool.InstantiateEnemy(enemyData, waypoint);

    Assert.That(gameObject.activeSelf);
    Assert.That(gameObject.GetComponent<Enemy>().PrevWaypoint, Is.SameAs(waypoint));
    Assert.That(gameObject.GetComponent<Enemy>().Data, Is.EqualTo(enemyData));
  }

  // Set starting size to 1 so there is exactly one enemey of each type.  Create an enemy
  // and save a reference to it, destroy that enemy and create a new one and save a reference to it.
  // Both references should point to the same GameObject. 
  [Test]
  public void DestroyEnemyWorks() {
    objectPool.SetStartingSize(1);
    objectPool.InitializeObjectPool(types);
    GameObject gameObject1 = objectPool.InstantiateEnemy(enemyData, waypoint);
    objectPool.DestroyEnemy(gameObject1);
    GameObject gameObject2 = objectPool.InstantiateEnemy(enemyData, waypoint);

    Assert.That(gameObject1, Is.SameAs(gameObject2));
  }

  // Create an ObjectPool with starting size 1 and create two enemies.  ObjectPool should
  // return a new enemy for each all.
  [Test]
  public void ObjectPoolResizeWorks() {
    objectPool.SetStartingSize(1);
    objectPool.InitializeObjectPool(types);
    GameObject gameObject1 = objectPool.InstantiateEnemy(enemyData, waypoint);
    GameObject gameObject2 = objectPool.InstantiateEnemy(enemyData, waypoint);

    Assert.That(gameObject1, Is.Not.SameAs(gameObject2));
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

  public static Dictionary<Tuple<EnemyData.Type, int>, Queue<GameObject>> GetObjectPools(this ObjectPool objectPool) {
    return (Dictionary<Tuple<EnemyData.Type, int>, Queue<GameObject>>)typeof(ObjectPool)
      .GetField("objectPools", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(objectPool);
  }

  public static void SetStartingSize(this ObjectPool objectPool, int size) {
    typeof(ObjectPool)
      .GetField("startingSize", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(objectPool, size);
  }
}
