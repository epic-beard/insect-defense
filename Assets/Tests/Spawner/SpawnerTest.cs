using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class SpawnerTest {

  // Tests to add:
  //   - For each subwave ensure GetSpawnTimes works as expected.

  [Test]
  public void PopulateSpawnTimesEmptyInsert() {
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart1 = Time.time + 5.0f;
    float dummyEnd1 = Time.time + 10.0f;

    EnemySpawnTimes spawnTimes = new();
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, endTime, spawnLocation, enemyType);

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateSpawnTimesInsertAfter() {
    float projectedStartTime = Time.time + 30.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = 10.0f;
    float dummyEnd = 15.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(projectedStartTime, endTime));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateSpawnTimesInsertBefore() {
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = 100.0f;
    float dummyEnd = 110.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, endTime, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart, dummyEnd));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateSpawnTimesInsertBetween() {
    float projectedStartTime = Time.time + 40.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart1 = 10.0f;
    float dummyEnd1 = 10.0f;
    float dummyStart2 = 100.0f;
    float dummyEnd2 = 110.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart2, dummyEnd2));
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(projectedStartTime, endTime));
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart2, dummyEnd2));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateSpawnTimesInsertDifferentEnemyTypes() {
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyTypeAnt = EnemyData.Type.ANT;
    EnemyData.Type enemyTypeBeetle = EnemyData.Type.BEETLE;

    float dummyStart1 = Time.time + 5.0f;
    float dummyEnd1 = Time.time + 10.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyTypeBeetle);
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyTypeAnt);

    float endTime = projectedStartTime + (repetitions * repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyTypeBeetle);
    expectedSpawnTimes[spawnLocation].Add(
        enemyTypeAnt,
        new List<Tuple<float, float>> { Tuple.Create(projectedStartTime, endTime) });

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateSpawnTimesNonzeroWaveStartTime() {
    float projectedStartTime = Time.time + 30.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = projectedStartTime;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = 10.0f;
    float dummyEnd = 15.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    Spawner.PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = (repetitions * repeatDelay) + waveStartTime;
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(projectedStartTime, endTime));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void MergeSpawnTimesEmptyList() {
    EnemySpawnTimes spawnTimes = new();

    Spawner.MergeSpawnTimes(ref spawnTimes);

    EnemySpawnTimes expectedSpawnTimes = new();

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void MergeSpawnTimesNoCollapse() {
    int spawnLocation = 0;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float start1 = 0.0f;
    float end1 = 5.0f;
    float start2 = 10.0f;
    float end2 = 20.0f;
    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(start1, end1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start2, end2));

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(start1, end1, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start2, end2));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void MergeSpawnTimesNoCollapseMultiCollapseNoBreak() {
    int spawnLocation = 0;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float start1 = 0.0f;
    float end1 = 15.0f;
    float start2 = 10.0f;
    float end2 = 25.0f;
    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(start1, end1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start2, end2));

    Spawner.MergeSpawnTimes(ref spawnTimes);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(start1, end2, spawnLocation, enemyType);

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void MergeSpawnTimesNoCollapseMultiCollapseWithBreak() {
    int spawnLocation = 0;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float start1 = 0.0f;
    float end1 = 15.0f;
    float start2 = 10.0f;
    float end2 = 25.0f;
    float start3 = 40.0f;
    float end3 = 50.0f;
    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(start1, end1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start2, end2));
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start3, end3));

    Spawner.MergeSpawnTimes(ref spawnTimes);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(start1, end2, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start3, end3));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  // Tests to add:
  //   - For each subwave ensure GetSpawnTimes works as expected.

  #region GetSpawnTimesTests

  [Test]
  public void GetSpawnTimesWaitUntilDeadWave() {
    float projectedStartTime = 10.0f;


  }

  #endregion GetSpawnTimesTests

  #region TestHelperMethods

  void PrintEnemySpawnTimes(String lead, EnemySpawnTimes spawnTimes) {
    string result = "";
    for (int i = 0; i < spawnTimes.Count; i++) {
      result += "Spawn point: " + i + "\n";
      foreach (var enemyType in spawnTimes[i].Keys) {
        result += enemyType.ToString() + ": { ";
        foreach (var spawnInterval in spawnTimes[i][enemyType]) {
          result += "(" + spawnInterval.Item1 + ", " + spawnInterval.Item2 + " ), ";
        }
        result += "}\n";
      }
    }
    Debug.Log(lead + result);
  }

  EnemySpawnTimes CreateEnemySpawnTimes(float startTime, float endTime, int index, EnemyData.Type enemyType) {
    EnemySpawnTimes result = new();

    while (result.Count < (index + 1)) {
      result.Add(new());
    }

    result[index].Add(
        enemyType,
        new List<Tuple<float, float>> { Tuple.Create(startTime, endTime) });

    return result;
  }

  #endregion TestHelperMethods
}


public static class SpawnerUtils {
  public static void AddStartingLocation(this Spawner spawner, Waypoint waypoint) {
    var startingLocations = (List<Waypoint>)typeof(Spawner)
      .GetField("spawnLocations", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(spawner);
    startingLocations.Add(waypoint);
  }
}
