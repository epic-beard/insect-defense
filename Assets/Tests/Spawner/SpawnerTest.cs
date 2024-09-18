using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class SpawnerTest {

  [Test]
  public void PopulateAndMergeSpawnTimesEmptyList() {
    EnemySpawnTimes spawnTimes = new();
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, endTime, spawnLocation, enemyType);

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateAndMergeSpawnTimesNoOverlapList() {
    EnemySpawnTimes spawnTimes = new();
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = 100.0f;
    float dummyEnd = 110.0f;
    int dummySpawnLocation = 1;

    spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, dummySpawnLocation, enemyType);

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart, dummyEnd, dummySpawnLocation, enemyType);
    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    expectedSpawnTimes[spawnLocation].Add(
        enemyType,
        new List<Tuple<float, float>> { Tuple.Create(projectedStartTime, endTime) });

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateAndMergeSpawnTimesNoOverLapDifferentEnemyTypes() {
    EnemySpawnTimes spawnTimes = new();
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = Time.time + 1000.0f;
    float dummyEnd = Time.time + 1010.0f;
    EnemyData.Type dummyEnemyType = EnemyData.Type.APHID;

    spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, dummyEnemyType);

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, dummyEnemyType);
    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    expectedSpawnTimes[spawnLocation].Add(
        enemyType,
        new List<Tuple<float, float>> { Tuple.Create(projectedStartTime, endTime) });

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateAndMergeSpawnTimesNoIntersection() {
    float projectedStartTime = Time.time + 10.0f;
    int spawnLocation = 0;
    int repetitions = 1;
    float repeatDelay = 5.0f;
    float waveStartTime = projectedStartTime;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = Time.time + 1000.0f;
    float dummyEnd = Time.time + 1010.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, endTime, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart, dummyEnd));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void PopulateAndMergeSpawnTimesLeadIntersection() {
    float projectedStartTime = Time.time + 5.0f;
    int spawnLocation = 0;
    int repetitions = 2;
    float repeatDelay = 3.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart = Time.time + 10.0f;
    float dummyEnd = Time.time + 15.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    
    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, dummyEnd, spawnLocation, enemyType);

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  // Two intervals were used to start with to further differentiate this test from PopulateAndMergeSpawnTimesLeadIntersection.
  [Test]
  public void PopulateAndMergeSpawnTimesTrailIntersection() {
    float projectedStartTime = Time.time + 25.0f;
    int spawnLocation = 0;
    int repetitions = 9;
    float repeatDelay = 3.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart1 = Time.time + 5.0f;
    float dummyEnd1 = Time.time + 10.0f;
    float dummyStart2 = Time.time + 20.0f;
    float dummyEnd2 = Time.time + 30.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart2, dummyEnd2));

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart2, endTime));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  // This test posits a situation with three pre-existing intervals. The desired addition will bridge
  // the gap between the first two, so the result should be two intervals.
  [Test]
  public void PopulateAndMergeSpawnTimesOmniIntersection() {
    float projectedStartTime = Time.time + 2.0f;
    int spawnLocation = 0;
    int repetitions = 7;
    float repeatDelay = 3.0f;
    float waveStartTime = 0.0f;
    EnemyData.Type enemyType = EnemyData.Type.ANT;

    float dummyStart1 = Time.time + 5.0f;
    float dummyEnd1 = Time.time + 10.0f;
    float dummyStart2 = Time.time + 20.0f;
    float dummyEnd2 = Time.time + 30.0f;
    float dummyStart3 = Time.time + 55.0f;
    float dummyEnd3 = Time.time + 70.0f;

    EnemySpawnTimes spawnTimes = CreateEnemySpawnTimes(dummyStart1, dummyEnd1, spawnLocation, enemyType);
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart2, dummyEnd2));
    spawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart3, dummyEnd3));

    Spawner.PopulateAndMergeSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = projectedStartTime + (repetitions * repeatDelay) - (Time.time - waveStartTime);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, dummyEnd2, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(dummyStart3, dummyEnd3));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

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
