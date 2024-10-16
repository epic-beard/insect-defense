#nullable enable
using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using static EpicBeardLib.XmlSerializationHelpers;

using EnemyStatOverrides = EpicBeardLib.Containers.SerializableDictionary<EnemyData.Stat, float>;
using NullableIntegerField = WDNullableField<int, UnityEngine.UIElements.IntegerField>;
using EnemyKey = System.Tuple<EnemyData.Type, int>;
using EnemySpawnTimes = System.Collections.Generic.List<
    System.Collections.Generic.Dictionary<
        EnemyData.Type, System.Collections.Generic.List<System.Tuple<float, float>>>>;

public class Spawner : MonoBehaviour {
  public static event Action<int> WavesStarted = delegate { };
  public static event Action<int, int> WaveComplete = delegate { };
  public static event Action<int> LevelComplete = delegate { };
  public static event Action<EnemySpawnTimes?> UpdateSpawnIndicatorData = delegate { };
#pragma warning disable 8618
  static public Spawner Instance;
#pragma warning restore 8618

  [SerializeField] private List<Waypoint> spawnLocations = new();
  [SerializeField] private string filename = "";

  private Waves? waves;

  public int CurrWave { get; set; } = 1;
  public int NumWaves { get; set; }
  public List<Waypoint> SpawnLocations { get { return spawnLocations; } }

  private void Awake() {
    Instance = this;
  }

  void Start() {
    if (filename.Length == 0) return;
    waves = Deserialize<Waves>(filename);
    if (waves != null) {
      ObjectPool.Instance.InitializeObjectPool(waves.GetEnemyKeys());
      SpawnWaves(waves);
    } else {
      // TODO: Make this an error.
      Debug.Log("ERROR: Waves is null.");
    }
    // Send initial timing data to EnemySpawnIndiciatorManager.
    UpdateSpawnIndicatorData.Invoke(GetSpawnTimes());
  }

  // This can return null.
  public EnemySpawnTimes? GetSpawnTimes() {
    EnemySpawnTimes result = new();
    float startTime = Time.time;
    waves?.GetSpawnTimes(ref result, ref startTime);
    MergeSpawnTimes(ref result);
    return result;
  }

  public void SpawnWaves(Waves waves) {
    ClearWave();
    NumWaves = waves.NumWaves;
    WavesStarted.Invoke(NumWaves);
    StartCoroutine(waves.Start());
  }

  public void ClearWave() {
    StopAllCoroutines();
    var enemies = ObjectPool.Instance.GetActiveEnemies().ToList();
    foreach (var enemy in enemies) {
      ObjectPool.Instance.DestroyEnemy(enemy);
    }
  }

  // A thin wrapper around InstantiateEnemy, using spawnLocation as an
  // index into spawnLocations.
  public GameObject Spawn(EnemyData data, int spawnLocation, Vector2? pos = null) {
    return Spawn(data, spawnLocations[spawnLocation], pos);
  }
  // Same as above but looks up the enemy stats in the EnemyManager.
  public GameObject Spawn(string enemyDataKey, int spawnLocation, Vector2? pos = null) {
    return Spawn(enemyDataKey, spawnLocations[spawnLocation], pos);
  }
  // Same as above but includes a Transform at which to spawn the enemy.
  public GameObject Spawn(string enemyDataKey, Waypoint startWaypoint, Vector2? pos = null, Transform? parent = null) {
    EnemyData data = EnemyDataManager.Instance.GetEnemyData(enemyDataKey);
    return Spawn(data, startWaypoint, pos, parent);
  }

  public GameObject Spawn(EnemyData data, Waypoint startWaypoint, Vector2? pos = null, Transform? parent = null) {
    return ObjectPool.Instance.InstantiateEnemy(data, startWaypoint, pos, parent).gameObject;
  }

  // Merge an existing EnemySpawnTimes with new data derived from the other parameters.
  // Params:
  //   spawnTimes - Existing data about enemy spawn times to merge into.
  //   projectedStartTime - When the current enemy will start spawning.
  //                        If waveStartTime is nonzero, they should be equal.
  //   spawnLocation - 0 based index of where the enemy will spawn.
  //   repetitions - How many times the current enemy will spawn.
  //   repeatDelay - The wait between spawn instances.
  //   waveStartTime - When the current wave started, if it has.
  //   enemyType - The type of enemy that will spawn.
  public static void PopulateSpawnTimes(
      ref EnemySpawnTimes spawnTimes,
      float projectedStartTime,
      int spawnLocation,
      int repetitions,
      float repeatDelay,
      float waveStartTime,
      EnemyData.Type enemyType) {
    // Ensure spawnTimes is big enough to reference the spawnlocation we are using.
    while (spawnTimes.Count < (spawnLocation + 1)) {
      spawnTimes.Add(new());
    }

    float endTime = repetitions * repeatDelay;
    if (waveStartTime == 0.0f) {
      // The wave has yet to start so
      endTime += projectedStartTime;
    } else {
      endTime += waveStartTime;
    }

    if (!spawnTimes[spawnLocation].ContainsKey(enemyType)) {
      spawnTimes[spawnLocation].Add(
          enemyType,
          new List<Tuple<float, float>> { Tuple.Create(projectedStartTime, endTime) });
    } else {
      List<Tuple<float, float>> enemySpawns = spawnTimes[spawnLocation][enemyType];
      // Make sure the new interval is inserted in the correct place.
      for (int i = 0; i < enemySpawns.Count; i++) {
        if (projectedStartTime < enemySpawns[i].Item1) {
          enemySpawns.Insert(i, Tuple.Create(projectedStartTime, endTime));
          break;
        }
        if (enemySpawns.Count == i + 1) {
          enemySpawns.Add(Tuple.Create(projectedStartTime, endTime));
          break;
        }
      }
    }
  }

  // Process spawnTimes and merge all overlapping intervals.
  public static void MergeSpawnTimes(ref EnemySpawnTimes spawnTimes) {
    foreach (var enemyTypeDict in spawnTimes) {
      for (int index = 0; index < enemyTypeDict.Count; index++) {
        var enemnyValueList = enemyTypeDict.ElementAt(index).Value;

        // No need to process if there's 1 or 0 entries in enemnyValueList.
        if (enemnyValueList.Count < 2) continue;

        List<Tuple<float, float>> updatedSpawns = new();

        for (int i = 0; i < enemnyValueList.Count; i++) {
          // Check for overlap.
          if (i + 1 < enemnyValueList.Count && enemnyValueList[i + 1].Item1 <= enemnyValueList[i].Item2) {

            float startTime = enemnyValueList[i].Item1;
            float endTime = Math.Max(enemnyValueList[i].Item2, enemnyValueList[i + 1].Item2);
            i++;

            // We need to find out how far this match goes.
            for (int j = i + 1; j < enemnyValueList.Count; j++) {
              // Check to see if the overlap is continuing.
              if (enemnyValueList[j].Item1 <= endTime) {
                endTime = Math.Max(endTime, enemnyValueList[j].Item2);
                // i needs to be adjusted so that index j is the next one examined by the outer loop.
                i = j;
              }
            }

            updatedSpawns.Add(Tuple.Create<float, float>(startTime, endTime));
          } else {
            updatedSpawns.Add(enemnyValueList[i]);
          }
        }

        enemyTypeDict[enemyTypeDict.ElementAt(index).Key] = updatedSpawns;
      }
    }
  }

  public interface IWaveOrMetric {
    public abstract Wave GetWaveWithDefaults(WaveMetrics defaults);
  }

  // The interface for the Waves system.
  [XmlInclude(typeof(Waves))]
  [XmlInclude(typeof(SequentialWave))]
  [XmlInclude(typeof(ConcurrentWave))]
  [XmlInclude(typeof(EnemyWave))]
  [XmlInclude(typeof(CannedEnemyWave))]
  [XmlInclude(typeof(SpacerWave))]
  [XmlInclude(typeof(DialogueBoxWave))]
  [XmlInclude(typeof(WaitUntilDeadWave))]
  [XmlInclude(typeof(DelayedWave))]
  public abstract class Wave : IWaveOrMetric {
    public static Action WaveChanged = delegate { };
    // Starts the wave.  Meant to be called as a Coroutine.
    public abstract IEnumerator Start();
    // Whether or not this wave has completed.
    [XmlIgnore]
    public bool Finished { get; set; }
    [XmlIgnore]
    public float WaveStartTime { get; set; }
    [XmlIgnore]
    public static float NondeterministicTimeAddition { get; } = 1000000.0f;

    public abstract HashSet<EnemyKey> GetEnemyKeys();

    public abstract void BindData(VisualElement ve);

    public abstract IList<Wave> GetChildren();

    public abstract bool AddWave(int index, Wave wave);

    // Construct the appropriate spawnTimes during the method call and return it through spawnTimes.
    // modifier is used as a way of passing data to child calls pertaining to any necessary delays;
    // for example, a DelayedWave can have a cooldown, which can adjust the next wave's start time.
    public abstract void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime);

    protected Foldout GetFoldout(string name) {
      Foldout foldout = new();
      foldout.text = name;
      foldout.AddToClassList("wave-designer-foldout");
      return foldout;
    }

    protected IntegerField GetIntegerField(string name, int value, EventCallback<ChangeEvent<int>> callback) {
      IntegerField field = new(name);
      field.value = value;
      field.AddToClassList("wave-designer-field");
      field.RegisterValueChangedCallback<int>(callback);
      return field;
    }

    protected FloatField GetFloatField(string name, float value, EventCallback<ChangeEvent<float>> callback) {
      FloatField field = new(name);
      field.value = value;
      field.AddToClassList("wave-designer-field");
      field.RegisterValueChangedCallback<float>(callback);
      return field;
    }

    protected TextField GetStringField(string name, string value, EventCallback<ChangeEvent<string>> callback) {
      TextField field = new(name);
      field.value = value;
      field.AddToClassList("wave-designer-field");
      field.RegisterValueChangedCallback<string>(callback);
      field.focusable = true;
      return field;
    }

    protected Label GetLabel(string text) {
      Label label = new(text);
      label.AddToClassList("wave-designer-label");
      return label;
    }

    protected NullableIntegerField GetNullableIntegerField(string name, int? value, Action<int?> callback) {
      NullableIntegerField field = new(name);
      field.Value = value;
      field.AddToClassList("wave-designer-nullable-field");
      field.OnValueChanged += callback;
      return field;
    }

    protected WDStringList GetStringList(string name, List<string> items, Action<List<string>> callback) {
      WDStringList list = new(items, name);
      list.AddToClassList("wave-designer-string-list");
      list.OnItemsChanged += callback;
      return list;
    }

    public Wave GetWaveWithDefaults(WaveMetrics defaults) { return this; }
  }

  // The top level of the subwave heirarchy, describing a level.
  public class Waves : Wave {
    public int NumWaves { get { return waves.Count(); } }
    public int Level;
    // Each wave represents one round of combat.
    readonly public List<Wave> waves = new();

    // Starts the level logic.
    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      foreach (var wave in waves) {
        // Start the level immediately.
        GameSpeedManager.Instance.Pause();
        yield return new WaitForSeconds(0.1f);
        // Run the wave and wait till it is complete.
        yield return wave.Start();

        yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count == 0);
        if (++Instance.CurrWave != Instance.NumWaves) {
          WaveComplete.Invoke(Instance.CurrWave, Instance.NumWaves);
        }
        // Wait long enough for the "Wave Complete" text to appear and disappear.
        yield return new WaitForSeconds(3);
      }

      // Sanity check, make sure all the waves have completed.
      yield return new WaitUntil(() => waves.All<Wave>((wave) => wave.Finished));

      // The level ends once all the enemies have been spawned and destroyed.
      yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count() == 0);

      // Make sure the players health didn't drop to zero getting rid of the last enemy.
      if (GameStateManager.Instance.Health > 0) {
        LevelComplete.Invoke(Level);
      }
      Finished = true;
    }

    public override string ToString() {
      return "Waves\n" + string.Join("\n", waves.ConvertAll(x => x.ToString().TabMultiLine()));
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      HashSet<EnemyKey> types = new();
      foreach (var wave in waves) {
        types.UnionWith(wave.GetEnemyKeys());
      }
      return types;
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      ve.Add(GetLabel("Waves"));
    }

    public override IList<Wave> GetChildren() {
      return waves;
    }

    public override bool AddWave(int index, Wave wave) {
      waves.Insert(index, wave);
      WaveChanged.Invoke();
      return true;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      foreach (var wave in waves) {
        wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);
      }
    }
  }

  public class DelayedWave : Wave {
    public float warmup = 0.0f;
    public Wave wave;
    public float cooldown = 0.0f;

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      if (warmup > 0.0f) yield return new WaitForSeconds(warmup);

      Spawner.Instance.StartCoroutine(wave.Start());
      yield return new WaitUntil(() => wave.Finished);

      if (cooldown > 0.0f) yield return new WaitForSeconds(cooldown);
      Finished = true;
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      return wave.GetEnemyKeys();
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Delayed");

      foldout.Add(GetFloatField("Warmup:", warmup, evt => {
        warmup = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetFloatField("Cooldown:", cooldown, evt => {
        cooldown = evt.newValue; WaveChanged.Invoke();
      }));

      ve.Add(foldout);
    }
    public override IList<Wave> GetChildren() {
      return new List<Wave>() { wave };
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      projectedStartTime = WaveStartTime != 0.0f ? WaveStartTime + warmup : projectedStartTime + warmup;

      wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);
      projectedStartTime += cooldown;
    }
  }

  // A wave that calls its subwaves sequentially.
  public class SequentialWave : Wave {
    readonly public List<Wave> Subwaves = new();

    public SequentialWave() { }

    public SequentialWave(params Wave[] waves) {
      Subwaves.AddRange(waves);
    }

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      foreach (var subwave in Subwaves) {
        // Start the subwave and wait till it's finished.
        yield return Spawner.Instance.StartCoroutine(subwave.Start());
      }
      Finished = true;
    }
    public override string ToString() {
      return "SequentialWave\n"
          + string.Join("\n", Subwaves.ConvertAll(x => x.ToString().TabMultiLine()));
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      HashSet<EnemyKey> types = new();
      foreach (var wave in Subwaves) {
        types.UnionWith(wave.GetEnemyKeys());
      }
      return types;
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      ve.Add(GetLabel("Sequential"));
    }
    public override IList<Wave> GetChildren() {
      return Subwaves;
    }

    public override bool AddWave(int index, Wave wave) {
      Subwaves.Insert(index, wave);
      WaveChanged.Invoke();
      return true;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      foreach (var wave in Subwaves) {
        wave.GetSpawnTimes(ref spawnTimes, ref projectedStartTime);
      }
    }
  }

  // A wave that calls its subwaves concurrently.
  public class ConcurrentWave : Wave {
    readonly public List<Wave> Subwaves = new();

    public ConcurrentWave() { }

    public ConcurrentWave(params Wave[] waves) {
      Subwaves.AddRange(waves);
    }

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      // Start all the subwaves.
      foreach (var subwave in Subwaves) {
        Spawner.Instance.StartCoroutine(subwave.Start());
      }
      // Wait until all the subwaves have finished.
      yield return new WaitUntil(() => Subwaves.All((s) => s.Finished));
      Finished = true;
    }

    public override string ToString() {
      return "ConcurrentWave\n"
          + string.Join("\n", Subwaves.ConvertAll(x => x.ToString().TabMultiLine()));
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      HashSet<EnemyKey> types = new();
      foreach (var wave in Subwaves) {
        types.UnionWith(wave.GetEnemyKeys());
      }
      return types;
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      ve.Add(GetLabel("Concurrent"));
    }

    public override IList<Wave> GetChildren() {
      return Subwaves;
    }

    public override bool AddWave(int index, Wave wave) {
      Subwaves.Insert(index, wave);
      WaveChanged.Invoke();
      return true;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      // We need to 'pass on' the furthest future projectedStartTime we can. Simultaneously,
      //  we need to ensure each wave gets a clean modifier to use.
      float largestModifier = 0.0f;
      foreach (var wave in Subwaves) {
        float tempModifier = projectedStartTime;
        wave.GetSpawnTimes(ref spawnTimes, ref tempModifier);
        largestModifier = Math.Max(largestModifier, tempModifier);
      }
      projectedStartTime = largestModifier;
    }
  }

  // A wave that creates an enemy at regular intervals.
  public class EnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public int? WaveTag;
    public List<Vector2> Positions = new();
    public EnemyData data;

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      for (int i = 0; i < repetitions; i++) {
        if (Positions.Count > 0) {
          foreach (Vector2 pos in Positions) {
            SpawnEnemy(data, spawnLocation, pos);
          }
        } else {
          for (int j = 0; j < spawnAmmount; j++) {
            SpawnEnemy(data, spawnLocation);
          }
        }
        // Wait for repeat delay seconds.
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
    }

    private Enemy SpawnEnemy(EnemyData data, int spawnLocation, Vector2? pos = null) {
      GameObject obj = Spawner.Instance.Spawn(data, spawnLocation, pos);
      Enemy enemy = obj.GetComponent<Enemy>();
      enemy.WaveTag = WaveTag;
      return enemy;
    }

    public override string ToString() {
      return "EnemyWave"
        + "\n\tRepetitions " + repetitions
        + "\n\tRepeat Delay: " + repeatDelay
        + "\n\tSpawn Location: " + spawnLocation
        + "\n\tSpawn Ammount: " + spawnAmmount
        + "\n\tEnemy Data: " + data;
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      return new HashSet<EnemyKey>() { new(data.type, data.infectionLevel) };
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Enemy");

      foldout.Add(GetIntegerField("Repetitions:", repetitions, evt => {
        repetitions = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetFloatField("Repeat Delay:", repeatDelay, evt => {
        repeatDelay = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnLocation, evt => {
        spawnLocation = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnAmmount, evt => {
        spawnAmmount = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetNullableIntegerField("Wave Tag:", WaveTag, i => {
        WaveTag = i; WaveChanged.Invoke();
      }));

      ve.Add(foldout);

      // TODO(nnewsom) implement:
      // WaveTag: we dont have a solution for nullables yet.
      // Data: need a solution for this if we're going to support this type of wave.
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      PopulateSpawnTimes(
          ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, WaveStartTime, data.type);

      projectedStartTime += repetitions * repeatDelay;
    }

    // The following method keeps waveTag from serializing when null.
    public bool ShouldSerializeWaveTag() { return WaveTag.HasValue; }
    public bool ShouldSerializePositions() { return Positions.Count > 0; }
  }

  // Same as the EnemyWave but used canned stats looked up by a
  // string key.
  public class CannedEnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public string enemyDataKey = "";
    public List<Vector2> Positions = new();
    public int? WaveTag;
    public EnemyStatOverrides Overrides = new();
    public EnemyData.Properties? Properties;
    public EnemyData.CarrierProperties? CarrierOverride;
    public EnemyData.SpawnerProperties? SpawnerOverride;
    public EnemyData.DazzleProperties? DazzleOverride;
    public EnemyData.SlimeProperties? SlimeOverride;

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      for (int i = 0; i < repetitions; i++) {
        if (Positions.Count > 0) {
          foreach (Vector2 pos in Positions) {
            SpawnEnemy(enemyDataKey, spawnLocation, pos);

          }
        } else {
          for (int j = 0; j < spawnAmmount; j++) {
            SpawnEnemy(enemyDataKey, spawnLocation);
          }
        }
        // Wait for repeat delay seconds.
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
    }

    private Enemy SpawnEnemy(string key, int spawnLocation, Vector2? pos = null) {
      GameObject obj = Spawner.Instance.Spawn(key, spawnLocation, pos);
      Enemy enemy = obj.GetComponent<Enemy>();
      if (Overrides != null) {
        foreach (var kvp in Overrides) {
          enemy.SetStat(kvp.Key, kvp.Value);
        }
      }
      if (Properties.HasValue) {
        enemy.SetProperties(Properties.Value);
      }
      if (CarrierOverride.HasValue) {
        enemy.SetCarrier(CarrierOverride.Value);
      }
      if (SpawnerOverride.HasValue) {
        enemy.SetSpawner(SpawnerOverride.Value);
      }
      if (DazzleOverride.HasValue) {
        enemy.SetDazzle(DazzleOverride.Value);
      }
      if (SlimeOverride.HasValue) {
        enemy.SetSlime(SlimeOverride.Value);
      }

      enemy.WaveTag = WaveTag;

      return enemy;
    }

    public override string ToString() {
      return "CannedEnemyWave"
        + "\n\tRepetitions " + repetitions
        + "\n\tRepeat Delay: " + repeatDelay
        + "\n\tSpawn Location: " + spawnLocation
        + "\n\tSpawn Ammount: " + spawnAmmount
        + "\n\tEnemy Data Key: " + enemyDataKey;
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      EnemyData data = EnemyDataManager.Instance.GetEnemyData(enemyDataKey);

      return new HashSet<EnemyKey>() { new(data.type, data.infectionLevel) };
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Canned Enemy");

      foldout.Add(GetIntegerField("Repetitions:", repetitions, evt => {
        repetitions = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetFloatField("Repeat Delay:", repeatDelay, evt => {
        repeatDelay = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnLocation, evt => {
        spawnLocation = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetIntegerField("Spawn Ammount:", spawnAmmount, evt => {
        spawnAmmount = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetStringField("Enemy Data Key:", enemyDataKey, evt => {
        enemyDataKey = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetNullableIntegerField("Wave Tag:", WaveTag, i => {
        WaveTag = i; WaveChanged.Invoke();
      }));

      ve.Add(foldout);

      // TODO(nnewsom) implement:
      // EnemyStatOverrides Overrides
      // EnemyData.Properties? Properties;
      // EnemyData.CarrierProperties? CarrierOverride;
      // EnemyData.SpawnerProperties? SpawnerOverride;
      // EnemyData.DazzleProperties? DazzleOverride;
      // EnemyData.SlimeProperties? SlimeOverride;
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) {
        return;
      }

      PopulateSpawnTimes(
          ref spawnTimes, projectedStartTime, spawnLocation, repetitions, repeatDelay, WaveStartTime,
          EnemyDataManager.Instance.GetEnemyData(enemyDataKey).type);

      projectedStartTime += repetitions * repeatDelay;
    }

    // The following methods keep the various overrides from serializing when they are unset.
    public bool ShouldSerializeOverrides() { return Overrides != null && Overrides.Count > 0; }
    public bool ShouldSerializeProperties() { return Properties.HasValue; }
    public bool ShouldSerializeCarrierOverride() { return CarrierOverride.HasValue; }
    public bool ShouldSerializeSpawnerOverride() { return SpawnerOverride.HasValue; }
    public bool ShouldSerializeDazzleOverride() { return DazzleOverride.HasValue; }
    public bool ShouldSerializeSlimeOverride() { return SlimeOverride.HasValue; }
    public bool ShouldSerializeWaveTag() { return WaveTag.HasValue; }

    public bool ShouldSerializePositions() { return Positions.Count > 0; }
  }

  // A wave that just waits for a given delay then finishes.
  // Used to pause between waves.
  public class SpacerWave : Wave {
    public float delay;

    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      // Wait for delay seconds.
      yield return new WaitForSeconds(delay);
      Finished = true;
    }
    public override string ToString() {
      return "SpacerWave\n\tDelay: " + delay;
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      return new HashSet<EnemyKey>();
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Spacer");

      foldout.Add(GetFloatField("Delay:", delay, evt => {
        delay = evt.newValue; WaveChanged.Invoke();
      }));

      ve.Add(foldout);
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      projectedStartTime = WaveStartTime == 0.0f ? projectedStartTime + delay : WaveStartTime + delay;
    }
  }

  public class DialogueBoxWave : Wave {
    public List<string> messages = new();
    public float delay = 0.5f;

    public override IEnumerator Start() {
      TowerManager.Instance.ClearSelection();
      MessageBox.Instance.ShowDialogue(messages);
      yield return new WaitUntil(() => !MessageBox.Instance.IsOpen());
      // WaveStasrtTime is set here instead of earlier to make internal logic using WaveStartTime to function.
      WaveStartTime = Time.time;
      yield return new WaitForSeconds(delay);
      Finished = true;
      Spawner.UpdateSpawnIndicatorData.Invoke(Spawner.Instance.GetSpawnTimes());
    }

    public override string ToString() {
      return "DialogueBoxWave\n\t" + string.Join("\n\t", messages);
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      return new HashSet<EnemyKey>();
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Dialogue Box");

      foldout.Add(GetFloatField("Delay:", delay, evt => {
        delay = evt.newValue; WaveChanged.Invoke();
      }));

      foldout.Add(GetStringList("Messages", messages, items => {
        messages = items; WaveChanged.Invoke();
      }));
      ve.Add(foldout);
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      projectedStartTime = WaveStartTime == 0.0f ?
          projectedStartTime + NondeterministicTimeAddition : WaveStartTime + delay;
    }
  }

  public class WaitUntilDeadWave : Wave {
    public int? WaveTag;
    public override IEnumerator Start() {
      WaveStartTime = Time.time;
      if (WaveTag == null) {
        yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count == 0);
      } else {
        yield return new WaitUntil(() =>
            !ObjectPool.Instance.GetActiveEnemies().Any(
                (e) => e.WaveTag == WaveTag));
      }
      Finished = true;
      Spawner.UpdateSpawnIndicatorData.Invoke(Spawner.Instance.GetSpawnTimes());
    }

    // The following method keeps waveTag from serializing when null.
    public bool ShouldSerializeWaveTag() { return WaveTag.HasValue; }

    public override string ToString() {
      return "WaitUntilDeadWave\n";
    }

    public override HashSet<EnemyKey> GetEnemyKeys() {
      return new HashSet<EnemyKey>();
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      ve.Add(GetLabel("Wait Until Dead"));
      // TODO(nnewsom) implement:
      // WaveTag
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    public override void GetSpawnTimes(ref EnemySpawnTimes spawnTimes, ref float projectedStartTime) {
      if (Finished || projectedStartTime >= NondeterministicTimeAddition) return;

      projectedStartTime += NondeterministicTimeAddition;  // Set delay time for future waves to far from now.
    }
  }

  public class WaveMetrics : IWaveOrMetric {
    public float? warmup = null;
    public float? repeatDelay = null;
    public float? cooldown = null;
    public float? duration = null;
    public int? repetitions = null;
    public int? spawnLocation = null;
    public int? spawnAmount = null;
    public string? enemyDataKey = null;
    public List<Vector2>? Positions = null;
    public int? WaveTag = null;
    public EnemyStatOverrides? Overrides = null;
    public EnemyData.Properties? Properties = null;
    public EnemyData.CarrierProperties? CarrierOverride = null;
    public EnemyData.SpawnerProperties? SpawnerOverride = null;
    public EnemyData.DazzleProperties? DazzleOverride = null;
    public EnemyData.SlimeProperties? SlimeOverride = null;

    public WaveMetrics() { }

    // Creates an enemy wave from a metric, defaulting to the values in defaults.
    // Throws if default, metric combination does not result in a valid wave.
    // The resulting enemy wave will wait for cooldown seconds and spawn for duration - cooldown - warmup.
    public Wave GetWaveWithDefaults(WaveMetrics defaults) {
      // Throws if any two of the following are null:
      // repetitions, duration, repeatDelay.
      int? repetitions = this.repetitions ?? defaults.repetitions;
      float? duration = this.duration ?? defaults.duration;
      float? repeatDelay = this.repeatDelay ?? defaults.repeatDelay;
      float? warmup = this.warmup ?? defaults.warmup ?? 0.0f;
      float? cooldown = this.cooldown ?? defaults.cooldown ?? 0.0f;
      duration -= warmup + cooldown;
      if (repetitions == null) {
        if (duration == null || repeatDelay == null) {
          throw new ArgumentNullException(nameof(repetitions) + " " + nameof(duration) + " " + nameof(repeatDelay));
        }
        repetitions = (int)Math.Floor(duration / repeatDelay ?? 1);
      } else if (repeatDelay == null) {
        if (duration == null || repetitions == null) {
          throw new ArgumentNullException(nameof(repetitions) + " " + nameof(duration) + " " + nameof(repeatDelay));
        }
        repeatDelay = duration / repetitions ?? 1;
      }

      // Set the fields while performing the defaulting.
      // SpawnAmount defaults to 1.
      // SpawnLocation and EnemyDataKey throw if left unset as this is likely uninentional.
      // Repetitions and repeatDelay should be non-null after the above logic so that exception is just a sanity check.
      CannedEnemyWave enemy = new() {
        repetitions = repetitions ?? throw new ArgumentNullException(nameof(repetitions)),
        repeatDelay = repeatDelay ?? throw new ArgumentNullException(nameof(repeatDelay)),
        spawnLocation = this.spawnLocation ?? defaults.spawnLocation ?? throw new ArgumentNullException("spawnLocation null"),
        spawnAmmount = this.spawnAmount ?? defaults.spawnAmount ?? 1,
        enemyDataKey = this.enemyDataKey ?? defaults.enemyDataKey ?? throw new ArgumentNullException("enemyDataKey null"),
        WaveTag = this.WaveTag ?? defaults.WaveTag,
        Overrides = this.Overrides ?? defaults.Overrides ?? new(),
        Properties = this.Properties ?? defaults.Properties,
        CarrierOverride = this.CarrierOverride ?? defaults.CarrierOverride,
        SpawnerOverride = this.SpawnerOverride ?? defaults.SpawnerOverride,
        DazzleOverride = this.DazzleOverride ?? defaults.DazzleOverride,
        SlimeOverride = this.SlimeOverride ?? defaults.SlimeOverride,
        Positions = this.Positions ?? defaults.Positions ?? new(),
      };
      return new DelayedWave() {
        wave = enemy,
        warmup = this.warmup ?? defaults.warmup ?? 0.0f,
        cooldown = this.cooldown ?? defaults.cooldown ?? 0.0f,
      };
    }
  }

  // Used to break up patterns when sending single enemy waves.  Concurently spawns enemies at the given delays.
  // relatively prime delays give best results. The List of Tuples is expected to be in this order:
  // <Warmup delay, repition depay>.
  public static Wave GetConcurrentWaveWithDefaults(string defaultEnemyDataKey, List<WaveMetrics> metrics, int defaultSpawnLocation = 0) {
    WaveMetrics defaults = new() {
      enemyDataKey = defaultEnemyDataKey,
      spawnLocation = defaultSpawnLocation,
    };
    return GetConcurrentWaveWithDefaults(defaults, metrics.ToArray());
  }

  public static SequentialWave GetSequentialWaveWithDefaults(WaveMetrics defaults, params IWaveOrMetric[] waveOrMetrics) {
    SequentialWave wave = new();
    foreach (IWaveOrMetric waveOrMetric in waveOrMetrics) {
      wave.Subwaves.Add(waveOrMetric.GetWaveWithDefaults(defaults));
    }
    return wave;
  }

  public static ConcurrentWave GetConcurrentWaveWithDefaults(WaveMetrics defaults, params IWaveOrMetric[] waveOrMetrics) {
    ConcurrentWave wave = new();
    foreach (IWaveOrMetric waveOrMetric in waveOrMetrics) {
      wave.Subwaves.Add(waveOrMetric.GetWaveWithDefaults(defaults));
    }
    return wave;
  }
}