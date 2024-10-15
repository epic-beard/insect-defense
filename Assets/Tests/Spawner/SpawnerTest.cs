using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using static Spawner;
using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class SpawnerTest {

  #region PopulateSpawnTimes

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
    PopulateSpawnTimes(
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
    PopulateSpawnTimes(
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
    PopulateSpawnTimes(
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
    PopulateSpawnTimes(
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
    PopulateSpawnTimes(
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
    PopulateSpawnTimes(
        ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, waveStartTime, enemyType);

    float endTime = (repetitions * repeatDelay) + waveStartTime;
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(dummyStart, dummyEnd, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(projectedStartTime, endTime));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  #endregion PopulateSpawnTimes

  #region MergeSpawnTimes

  [Test]
  public void MergeSpawnTimesEmptyList() {
    EnemySpawnTimes spawnTimes = new();

    MergeSpawnTimes(ref spawnTimes);

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

    MergeSpawnTimes(ref spawnTimes);

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

    MergeSpawnTimes(ref spawnTimes);

    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(start1, end2, spawnLocation, enemyType);
    expectedSpawnTimes[spawnLocation][enemyType].Add(Tuple.Create(start3, end3));

    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  #endregion MergeSpawnTimes

  // Each wave with a GetSpawnTimes has a test in this region.
  #region GetSpawnTimesTests

  [Test]
  public void GetSpawnTimesCannedEnemyWave() {
    float projectedStartTime = Wave.NondeterministicTimeAddition;
    EnemySpawnTimes spawnTimes = new();
    CannedEnemyWave wave = new();
    wave.spawnLocation = 0;
    wave.repetitions = 1;
    wave.repeatDelay = 5.0f;
    wave.enemyDataKey = "Aphid_IL0";

    float expectedStartTimeND = projectedStartTime;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    EnemyDataManager enemyDataManager = new GameObject().AddComponent<EnemyDataManager>();
    enemyDataManager.SetFileName("data.enemies");
    enemyDataManager.InvokeAwake();

    projectedStartTime = 10.0f;
    float expectedStartTime = projectedStartTime + (wave.repetitions * wave.repeatDelay);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, expectedStartTime, wave.spawnLocation, EnemyData.Type.APHID);

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));
    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void GetSpawnTimesConcurrentWave() {
    SpacerWave spacerWave1 = new();
    spacerWave1.delay = 5.0f;
    SpacerWave spacerWave2 = new();
    spacerWave2.delay = 8.0f;
    Wave[] spacerWaves = {spacerWave1, spacerWave2 };
    // Setting up 2 child waves with different delays.
    ConcurrentWave concurrentWave = new(spacerWaves);

    float projectedStartTime = Wave.NondeterministicTimeAddition;
    float expectedStartTimeND = projectedStartTime;
    EnemySpawnTimes spawnTimes = new();

    concurrentWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Ensure that if the wave hasn't started, no processing is done.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    projectedStartTime = 10.0f;
    float expectedStartTime = projectedStartTime + spacerWave2.delay;

    concurrentWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Make sure that the largest of the two delays is taken from the child waves.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));
  }

  [Test]
  public void GetSpawnTimesDelayedWave() {
    DelayedWave delayedWave = new();
    delayedWave.WaveStartTime = 0.0f;
    delayedWave.warmup = 10.0f;
    delayedWave.cooldown = 10.0f;
    float projectedStartTime = Wave.NondeterministicTimeAddition;
    EnemySpawnTimes spawnTimes = new();
    float expectedStartTimeND = projectedStartTime;

    delayedWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Ensure that if the wave hasn't started, no processing is done.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    SpacerWave spacerWave = new();
    spacerWave.delay = 0.0f;
    delayedWave.wave = spacerWave;
    projectedStartTime = 50.0f;
    float expectedStartTime = projectedStartTime + delayedWave.warmup + delayedWave.cooldown;

    delayedWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Check for correct processing if WaveStartTime == 0.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));

    delayedWave.WaveStartTime = 30.0f;
    projectedStartTime = 30.0f;
    expectedStartTime = projectedStartTime + delayedWave.warmup + delayedWave.cooldown;

    delayedWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Check for correct processing if WaveStartTime != 0.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));
  }

  [Test]
  public void GetSpawnTimesDialogueBoxWave() {
    float projectedStartTime = 10.0f;
    float expectedStartTimeND = Wave.NondeterministicTimeAddition + projectedStartTime;
    EnemySpawnTimes spawnTimes = new();
    DialogueBoxWave wave = new();

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Detect that the wave hasn't started and add nondeterministic addition
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Make sure that the nondeterministic addition is added at most once.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    wave.WaveStartTime = Time.time - 0.05f;
    projectedStartTime = wave.WaveStartTime;
    float expectedStartTimeDeterministic = projectedStartTime + wave.delay;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // A DialogueBoxWave doesn't "start" until after the dialoguebox itself is dismissed.
    // Thus, if it is started, the projectedStartTime should equal WaveStartTime + delay.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeDeterministic));

    projectedStartTime = 10.0f;
    float expectedStartTimeWaveStarted = 10.0f;
    wave.Finished = true;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // If the wave is done, it shouldn't modify projectedStartTime at all.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeWaveStarted));
  }

  [Test]
  public void GetSpawnTimesEnemyWave() {
    EnemyWave wave = new();
    wave.repeatDelay = 10.0f;
    wave.repetitions = 1;
    wave.spawnLocation = 0;
    EnemyData data = new();
    data.type = EnemyData.Type.APHID;
    wave.data = data;
    EnemySpawnTimes spawnTimes = new();
    
    float projectedStartTime = Wave.NondeterministicTimeAddition;
    float expectedStartTimeND = projectedStartTime;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Ensure that if the wave hasn't started, no processing is done.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    projectedStartTime = 10.0f;
    float expectedEndTime = projectedStartTime + (wave.repeatDelay * wave.repetitions);
    EnemySpawnTimes expectedSpawnTimes =
        CreateEnemySpawnTimes(projectedStartTime, expectedEndTime, wave.spawnLocation, EnemyData.Type.APHID);

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    Assert.That(projectedStartTime, Is.EqualTo(expectedEndTime));
    CollectionAssert.AreEqual(expectedSpawnTimes, spawnTimes);
  }

  [Test]
  public void GetSpawnTimesSequentialWave() {
    SpacerWave spacerWave = new();
    spacerWave.delay = 5.0f;
    SequentialWave sequentialWave = new(spacerWave);
    float projectedStartTime = Wave.NondeterministicTimeAddition;
    float expectedStartTimeND = projectedStartTime;
    EnemySpawnTimes spawnTimes = new();

    sequentialWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Ensure that if the wave hasn't started, no processing is done.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    projectedStartTime = 10.0f;
    float expectedStartTime = projectedStartTime + spacerWave.delay;

    sequentialWave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Make sure that the delays from the child wave is added as expected.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));
  }

  [Test]
  public void GetSpawnTimesSpacerWave() {
    float projectedStartTime = Wave.NondeterministicTimeAddition + 10.0f;
    float expectedStartTimeND = projectedStartTime;
    EnemySpawnTimes spawnTimes = new();
    SpacerWave wave = new();
    wave.delay = 5.0f;
    
    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Ensure that processing only happens if projectedStartTime is less than NondeterministicTimeAddition.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeND));

    projectedStartTime = 10.0f;
    float expectedStartTimeNotStarted = 15.0f;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // If the wave hasn't started yet, add the delay to the projectedStartTime.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeNotStarted));

    // If the wave has started, add delay to start time.
    projectedStartTime = 10.0f;
    wave.WaveStartTime = Time.time - 0.05f;
    float expectedStartTimeStarted = wave.WaveStartTime + wave.delay;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // If the wave has started, projectedStartTime should equal the wave's start time plus delay.
    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTimeStarted));
  }

  [Test]
  public void GetSpawnTimesWaitUntilDeadWave() {
    float projectedStartTime = 10.0f;
    float expectedNDStartTime = Wave.NondeterministicTimeAddition + projectedStartTime;
    float expectedDeterministicStartTime = projectedStartTime;
    EnemySpawnTimes spawnTimes = new();
    WaitUntilDeadWave wave = new();

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Detect that the wave hasn't started and add nondeterministic addition
    Assert.That(projectedStartTime, Is.EqualTo(expectedNDStartTime));

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Make sure that the nondeterministic addition is added at most once.
    Assert.That(projectedStartTime, Is.EqualTo(expectedNDStartTime));

    wave.Finished = true;
    projectedStartTime = 10.0f;

    wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // If the wave is done, it shouldn't modify projectedStartTime at all.
    Assert.That(projectedStartTime, Is.EqualTo(expectedDeterministicStartTime));
  }

  [Test]
  public void GetSpawnTimesWaves() {
    SpacerWave spacerWave = new();
    spacerWave.delay = 10.0f;
    SpacerWave nestedSpacerWave = new();
    nestedSpacerWave.delay = 5.0f;
    DelayedWave delayedWave = new();
    delayedWave.wave = nestedSpacerWave;

    Waves waves = new();
    waves.AddWave(0, spacerWave);
    waves.AddWave(1, delayedWave);

    float projectedStartTime = Wave.NondeterministicTimeAddition;
    float expectedNDStartTime = projectedStartTime;
    EnemySpawnTimes spawnTimes = new();

    waves.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    // Detect that the wave hasn't started and add nondeterministic addition
    Assert.That(projectedStartTime, Is.EqualTo(expectedNDStartTime));

    projectedStartTime = 10.0f;
    float expectedStartTime = projectedStartTime + spacerWave.delay + nestedSpacerWave.delay;

    waves.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);

    Assert.That(projectedStartTime, Is.EqualTo(expectedStartTime));
  }

  #endregion GetSpawnTimesTests

  #region TestHelperMethods

  public static void PrintEnemySpawnTimes(String lead, EnemySpawnTimes spawnTimes) {
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
