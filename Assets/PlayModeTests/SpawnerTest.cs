using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawnerTest {
  [UnityTest]
  public IEnumerator EnemySubwaveTest() {
    ObjectPool pool = new GameObject().AddComponent<ObjectPool>();
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.EnemySubwave subwave = new() {
      repetitions = 2,
      repeatDelay = 0.1f,
      spawnLocation = 1,
      data = new() {
        type = EnemyData.Type.BEETLE,
      },
    };

    var enumerator = subwave.Run(spawner);
    Assert.True(enumerator.MoveNext());
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(WaitUntil)));
    yield return null;
  }

  [UnityTest]
  public IEnumerator ConcurrentSubwaveTest() {
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerSubwave spacer1 = new() { delay = 0.1f };
    Spawner.SpacerSubwave spacer2 = new() { delay = 0.1f };
    Spawner.ConcurrentSubwave subwave = new();
    subwave.Subwaves.Add(spacer1);
    subwave.Subwaves.Add(spacer2);

    var enumerator = subwave.Run(spawner);
    Assert.True(enumerator.MoveNext());
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(WaitUntil)));
    yield return new WaitForSeconds(0.11f);
    Assert.True(spacer1.Finished);
    Assert.True(spacer2.Finished);
    Assert.False(enumerator.MoveNext());
    Assert.That(subwave.Finished);
    yield return null;
  }

  [UnityTest]
  public IEnumerator SequentialSubwaveTest() {
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerSubwave spacer1 = new() { delay = 0.1f };
    Spawner.SpacerSubwave spacer2 = new() { delay = 0.1f };
    Spawner.SequentialSubwave subwave = new();
    subwave.Subwaves.Add(spacer1);
    subwave.Subwaves.Add(spacer2);

    var enumerator = subwave.Run(spawner);
    Assert.True(enumerator.MoveNext());
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(Coroutine)));
    yield return new WaitForSeconds(0.11f);
    Assert.True(spacer1.Finished);
    Assert.False(spacer2.Finished);
    Assert.True(enumerator.MoveNext());
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(Coroutine)));
    yield return new WaitForSeconds(0.11f);
    Assert.True(spacer2.Finished);
    Assert.False(enumerator.MoveNext());
    Assert.That(subwave.Finished);
    yield return null;
  }

  [UnityTest]
  public IEnumerator SpacerSubwaveTest() {
    Spawner spawner = new GameObject().AddComponent<Spawner>();
    Spawner.SpacerSubwave subwave = new() { delay = 0.1f };
    var enumerator = subwave.Run(spawner);
    Assert.True(enumerator.MoveNext());
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(WaitForSeconds)));
    yield return new WaitForSeconds(0.1f);
    Assert.False(enumerator.MoveNext());
    Assert.That(subwave.Finished);
    yield return null;
  }
}
