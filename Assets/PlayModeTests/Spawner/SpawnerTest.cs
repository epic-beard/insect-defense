using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawnerTest {
  [UnityTest]
  public IEnumerator ConcurrentSubwaveTest() {
    // Creates a concurrent subwave with two spacer subwabves.
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerWave spacer1 = new() { delay = 0.1f };
    Spawner.SpacerWave spacer2 = new() { delay = 0.1f };
    Spawner.ConcurrentWave wave = new();
    wave.Subwaves.Add(spacer1);
    wave.Subwaves.Add(spacer2);

    // We capture the enumerator to manually advance the wave.
    // This is normally handled under the hood by Unity.
    var enumerator = wave.Start(spawner);
    Assert.True(enumerator.MoveNext());
    yield return new WaitForSeconds(0.11f);
    Assert.False(enumerator.MoveNext());
    Assert.True(spacer1.Finished);
    Assert.True(spacer2.Finished);
    Assert.That(wave.Finished);
    yield return null;
  }

  [UnityTest]
  public IEnumerator SequentialSubwaveTest() {
    // Creates a sequential subwave with two spacer subwaves.
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerWave spacer1 = new() { delay = 0.1f };
    Spawner.SpacerWave spacer2 = new() { delay = 0.1f };
    Spawner.SequentialWave wave = new();
    wave.Subwaves.Add(spacer1);
    wave.Subwaves.Add(spacer2);

    var enumerator = wave.Start(spawner);
    Assert.True(enumerator.MoveNext());
    yield return new WaitForSeconds(0.11f);
    Assert.True(enumerator.MoveNext());
    Assert.True(spacer1.Finished);
    Assert.False(spacer2.Finished);
    yield return new WaitForSeconds(0.11f);
    Assert.False(enumerator.MoveNext());
    Assert.True(spacer2.Finished);
    Assert.That(wave.Finished);
    yield return null;
  }

  [UnityTest]
  public IEnumerator SpacerSubwaveTest() {
    // Create a spacer subwave with a short delay.
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerWave wave = new() { delay = 0.1f };

    var enumerator = wave.Start(spawner);
    Assert.True(enumerator.MoveNext());
    yield return new WaitForSeconds(0.11f);
    Assert.False(enumerator.MoveNext());
    Assert.That(wave.Finished);
    yield return null;
  }
}

public class EnemySubwaveTest {
  private ObjectPool objectPool;
  private EnemyData enemyData;
  private Spawner spawner;

  [SetUp]
  public void SetUp() {
    GameObject prefab = new();
    prefab.SetActive(false);
    prefab.AddComponent<Enemy>();

    objectPool = CreateObjectPool();
    AddPrefab(objectPool, EnemyData.Type.BEETLE, prefab);
    InvokeInitializeObjectPool(objectPool);

    spawner = new GameObject().AddComponent<Spawner>();
    Waypoint spawnLocation = new GameObject().AddComponent<Waypoint>();
    spawnLocation.transform.position = Vector3.zero;
    AddStartingLocation(spawner, spawnLocation);

    spawnLocation = new GameObject().AddComponent<Waypoint>();
    spawnLocation.transform.position = Vector3.left;
    AddStartingLocation(spawner, spawnLocation);

    SetObjectPool(spawner, objectPool);

    enemyData = new EnemyData() {
      type = EnemyData.Type.BEETLE
    };
  }

  [UnityTest]
  public IEnumerator EnemySubwaveWorks() {
    Spawner.EnemyWave wave = new() {
      repetitions = 2,
      repeatDelay = 0.1f,
      spawnLocation = 1,
      data = enemyData,
    };

    var enumerator = wave.Start(spawner);
    Assert.True(enumerator.MoveNext());
    Assert.That(objectPool.GetActiveEnemies().Count, Is.EqualTo(1));
    foreach (var enemy in objectPool.GetActiveEnemies()) {
      Assert.That(enemy.data, Is.EqualTo(enemyData));
      Assert.That(enemy.transform.position, Is.EqualTo(Vector3.left));
    }
    yield return new WaitForSeconds(0.11f);
    Assert.True(enumerator.MoveNext());
    Assert.That(objectPool.GetActiveEnemies().Count, Is.EqualTo(2));
    foreach (var enemy in objectPool.GetActiveEnemies()) {
      Assert.That(enemy.data, Is.EqualTo(enemyData));
      Assert.That(enemy.transform.position, Is.EqualTo(Vector3.left));
    }
    yield return new WaitForSeconds(0.11f);
    Assert.False(enumerator.MoveNext());
    Assert.True(wave.Finished);
    yield return null;
  }
  private void AddStartingLocation(Spawner spawner, Waypoint waypoint) {
    var startingLocations = (List<Waypoint>)typeof(Spawner)
      .GetField("spawnLocations", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(spawner);
    startingLocations.Add(waypoint);
  }

  // Adds a prefab to the ObjectPool's list of prefabs.
  private void AddPrefab(ObjectPool objectPool, EnemyData.Type type, GameObject prefab) {
    var prefabs = (Dictionary<EnemyData.Type, GameObject>)typeof(ObjectPool)
      .GetField("prefabs", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(objectPool);
    prefabs.Add(type, prefab);
  }

  // Adds a prefab to the ObjectPool's list of prefabs.
  private void SetObjectPool(Spawner spawner, ObjectPool objectPool) {
    typeof(Spawner)
      .GetField("pool", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(spawner, objectPool);
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

  private ObjectPool CreateObjectPool() {
    return new GameObject().AddComponent<ObjectPool>();
  }
}