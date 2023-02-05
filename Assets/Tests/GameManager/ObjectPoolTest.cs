using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class ObjectPoolTest {
  private ObjectPool objectPool;
  private EnemyData enemyData;

  [SetUp]
  public void SetUp() {
    objectPool = CreateObjectPool();
    GameObject prefab = new();
    prefab.AddComponent<Enemy>();

    AddPrefab(objectPool, EnemyData.Type.BEETLE, prefab);
    SetStartingSize(objectPool, 1);

    enemyData = new EnemyData() {
      type = EnemyData.Type.BEETLE
    };
  }

  // Creates an ObjecPool of a starting size n. Checks that each pool in
  // the ObjectPool has Count == n.
  [Test]
  public void InitializeObjectPoolWorks([Values(0, 1, 10)] int startingSize) {
    SetStartingSize(objectPool, startingSize);
    InvokeInitializeObjectPool(objectPool);
    var objectPools = GetObjectPools(objectPool);

    Assert.That(objectPools.Count, Is.EqualTo(1));
    foreach (var (_, pool) in objectPools) {
      Assert.That(pool.Count, Is.EqualTo(startingSize));
    }
  }

  // Instantiate an enemy and check that it is active and has the correct position and
  // enemy data.
  [Test]
  public void InstantiateEnemyWorks() {
    InvokeInitializeObjectPool(objectPool);
    GameObject gameObject = objectPool.InstantiateEnemy(enemyData, Vector3.left);

    Assert.That(gameObject.activeSelf);
    Assert.That(gameObject.transform.position, Is.EqualTo(Vector3.left));
    Assert.That(gameObject.GetComponent<Enemy>().data, Is.EqualTo(enemyData));
  }

  // Set starting size to 1 so there is exactly one enemey of each type.  Create an enemy
  // and save a reference to it, destroy that enemy and create a new one and save a reference to it.
  // Both references should point to the same GameObject. 
  [Test]
  public void DestroyEnemyWorks() {
    SetStartingSize(objectPool, 1);
    InvokeInitializeObjectPool(objectPool);
    GameObject gameObject1 = objectPool.InstantiateEnemy(enemyData, Vector3.left);
    objectPool.DestroyEnemy(gameObject1);
    GameObject gameObject2 = objectPool.InstantiateEnemy(enemyData, Vector3.right);

    Assert.That(gameObject1, Is.SameAs(gameObject2));
  }

  // Create an ObjectPool with starting size 1 and create two enemies.  ObjectPool should
  // return a new enemy for each all.
  [Test]
  public void ObjectPoolResizeWorks() {
    SetStartingSize(objectPool, 1);
    InvokeInitializeObjectPool(objectPool);
    GameObject gameObject1 = objectPool.InstantiateEnemy(enemyData, Vector3.left);
    GameObject gameObject2 = objectPool.InstantiateEnemy(enemyData, Vector3.right);

    Assert.That(gameObject1, Is.Not.SameAs(gameObject2));
  }

  private ObjectPool CreateObjectPool() {
    return new GameObject().AddComponent<ObjectPool>();
  }

  private Dictionary<EnemyData.Type, Queue<GameObject>> GetObjectPools(ObjectPool objectPool) {
    return (Dictionary<EnemyData.Type, Queue<GameObject>>)typeof(ObjectPool)
      .GetField("objectPools", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(objectPool);
  }

  private void SetStartingSize(ObjectPool objectPool, int size) {
    typeof(ObjectPool)
      .GetField("startingSize", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(objectPool, size);
  }

  // Adds a prefab to the ObjectPool's list of prefabs.
  private void AddPrefab(ObjectPool objectPool, EnemyData.Type type, GameObject prefab) {
    var prefabs = (Dictionary<EnemyData.Type, GameObject>)typeof(ObjectPool)
      .GetField("prefabs", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(objectPool);
    prefabs.Add(type, prefab);
  }

  private void InvokeInitializeObjectPool(ObjectPool objectPool) {
    MethodInfo initializeObjectPool = typeof(ObjectPool).GetMethod(
      name: "InitializeObjectPool",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      binder: null,
      callConvention: CallingConventions.Standard,
      types: new Type[0],
      modifiers: null);
    initializeObjectPool.Invoke(objectPool, new object[0]);
  }
}
