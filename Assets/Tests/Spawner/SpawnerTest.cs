using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawnerTest {

  //[Test]
  //public void SpacerSubwaveTest() {
  //  Spawner spawner = new();
  //  Spawner.SpacerSubwave subwave = new() { delay = 10 };
  //  var enumerator = subwave.Run(spawner);
  //  enumerator.MoveNext();
  //  Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(WaitForSeconds)));
  //  enumerator.MoveNext();
  //  Assert.That(enumerator.Current, Is.Null);
  //  Assert.That(subwave.Finished);
  //}

  [UnityTest]
  public IEnumerator SpacerSubwaveTest2() {
    Spawner spawner = new();
    Spawner.SpacerSubwave subwave = new() { delay = 10 };
    var enumerator = subwave.Run(spawner);
    enumerator.MoveNext();
    Assert.That(enumerator.Current.GetType(), Is.EqualTo(typeof(WaitForSeconds)));
    //while (Time.time < 10) {
    //  yield return null;
    //}
    yield return new WaitForSeconds(10);
    enumerator.MoveNext();
    Assert.That(enumerator.Current, Is.Null);
    Assert.That(subwave.Finished);
    yield return null;
  }
}
