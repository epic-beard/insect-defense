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

public class Spawner : MonoBehaviour {
  public static event Action<int> WavesStarted = delegate { };
  public static event Action<int, int> WaveComplete = delegate { };
  public static event Action LevelComplete = delegate { };
#pragma warning disable 8618
  static public Spawner Instance;
#pragma warning restore 8618
  [SerializeField] private List<Waypoint> spawnLocations = new();
  [SerializeField] private string filename = "";

  public int CurrWave { get; set; } = 1;
  public int NumWaves { get; set; }

  private void Awake() {
    Instance = this;
  }

  void Start() {
    if (filename.Length == 0) return;
    Waves? waves = Deserialize<Waves>(filename);
    if (waves != null) {
      ObjectPool.Instance.InitializeObjectPool(waves.GetEnemyKeys());
      SpawnWaves(waves);
    } else {
      // TODO: Make this an error.
      Debug.Log("ERROR: Waves is null.");
    }
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
      ObjectPool.Instance.DestroyEnemy(enemy.gameObject);
    }
  }

  // A thin wrapper around InstantiateEnemy, using spawnLocation as an
  // index into spawnLocations.
  public GameObject Spawn(EnemyData data, int spawnLocation) {
    return Spawn(data, spawnLocations[spawnLocation]);
  }
  // Same as above but looks up the enemy stats in the EnemyManager.
  public GameObject Spawn(string enemyDataKey, int spawnLocation) {
    return Spawn(enemyDataKey, spawnLocations[spawnLocation]);
  }
  // Same as above but includes a Transform at which to spawn the enemy.
  public GameObject Spawn(string enemyDataKey, Waypoint startWaypoint, Transform? parent = null) {
    EnemyData data = EnemyDataManager.Instance.GetEnemyData(enemyDataKey);
    return Spawn(data, startWaypoint, parent);
  }

  public GameObject Spawn(EnemyData data, Waypoint startWaypoint, Transform? parent = null) {
    return ObjectPool.Instance.InstantiateEnemy(data, startWaypoint, parent);
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

  public abstract class Wave {
    public static Action WaveChanged = delegate { };
    // Starts the wave.  Meant to be called as a Coroutine.
    public abstract IEnumerator Start();
    // Whether or not this wave has completed.
    [XmlIgnore]
    public bool Finished { get; set; }

    public abstract HashSet<EnemyKey> GetEnemyKeys();

    public abstract void BindData(VisualElement ve);

    public abstract IList<Wave> GetChildren();

    public abstract bool AddWave(int index, Wave wave);

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
  }

  // The top level of the subwave heirarchy, describing a level.
  public class Waves : Wave {
    public int NumWaves { get { return waves.Count(); } }
    // Each wave represents one round of combat.
    readonly public List<Wave> waves = new();
    // Starts the level logic.
    public override IEnumerator Start() {
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
        LevelComplete.Invoke();
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
  }

  public class DelayedWave : Wave {
    public float warmup = 0.0f;
    public Wave wave;
    public float cooldown = 0.0f;

    public override IEnumerator Start() {
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
        warmup = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetFloatField("Cooldown:", cooldown, evt => {
        cooldown = evt.newValue; WaveChanged.Invoke(); }));

      ve.Add(foldout);
    }
    public override IList<Wave> GetChildren() {
      return new List<Wave>() { wave };
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }
  }

  // A wave that calls its subwaves sequentially.
  public class SequentialWave : Wave {
    readonly public List<Wave> Subwaves = new();
    public override IEnumerator Start() {
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
  }

  // A wave that calls its subwaves concurrently.
  public class ConcurrentWave : Wave {
    readonly public List<Wave> Subwaves = new();
    public override IEnumerator Start() {
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
  }

  // A wave that creates an enemy at regular intervals.
  public class EnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public int? WaveTag;
    public EnemyData data;

    public override IEnumerator Start() {
      for (int i = 0; i < repetitions; i++) {
        for (int j = 0; j < spawnAmmount; j++) {
          // Create the enemy.
          GameObject obj = Spawner.Instance.Spawn(data, spawnLocation);
          Enemy enemy = obj.GetComponent<Enemy>();
          enemy.WaveTag = WaveTag;
        }
        // Wait for repeat delay seconds.
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
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
      return new HashSet<EnemyKey>() { new (data.type, data.infectionLevel) };
    }

    public override void BindData(VisualElement ve) {
      ve.Clear();
      Foldout foldout = GetFoldout("Enemy");

      foldout.Add(GetIntegerField("Repetitions:", repetitions, evt => {
        repetitions = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetFloatField("Repeat Delay:", repeatDelay, evt => {
        repeatDelay = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnLocation, evt => {
        spawnLocation = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnAmmount, evt => {
        spawnAmmount = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetNullableIntegerField("Wave Tag:", WaveTag, i => {
        WaveTag = i; WaveChanged.Invoke(); }));

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

    // The following method keeps waveTag from serializing when null.
    public bool ShouldSerializeWaveTag() { return WaveTag.HasValue; }
  }

  // Same as the EnemyWave but used canned stats looked up by a
  // string key.
  public class CannedEnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public string enemyDataKey = "";
    public int? WaveTag;
    public EnemyStatOverrides Overrides = new();
    public EnemyData.Properties? Properties;
    public EnemyData.CarrierProperties? CarrierOverride;
    public EnemyData.SpawnerProperties? SpawnerOverride;
    public EnemyData.DazzleProperties? DazzleOverride;
    public EnemyData.SlimeProperties? SlimeOverride;

    public override IEnumerator Start() {
      for (int i = 0; i < repetitions; i++) {
        for (int j = 0; j < spawnAmmount; j++) {
          // Create the enemy.
          GameObject obj = Spawner.Instance.Spawn(enemyDataKey, spawnLocation);
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
        }
        // Wait for repeat delay seconds.
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
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
        repetitions = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetFloatField("Repeat Delay:", repeatDelay, evt => {
        repeatDelay = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetIntegerField("Spawn Location:", spawnLocation, evt => {
        spawnLocation = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetIntegerField("Spawn Ammount:", spawnAmmount, evt => {
        spawnAmmount = evt.newValue; WaveChanged.Invoke(); }));

      foldout.Add(GetStringField("Enemy Data Key:", enemyDataKey, evt => {
        enemyDataKey = evt.newValue; WaveChanged.Invoke(); }));

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
      // WaveTag: we dont have a solution for nullables yet.
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }

    // The following methods keep the various overrides from serializing when they are unset.
    public bool ShouldSerializeOverrides() { return Overrides != null && Overrides.Count > 0; }
    public bool ShouldSerializeProperties() { return Properties.HasValue; }
    public bool ShouldSerializeCarrierOverride() { return CarrierOverride.HasValue; }
    public bool ShouldSerializeSpawnerOverride() { return SpawnerOverride.HasValue; }
    public bool ShouldSerializeDazzleOverride() { return DazzleOverride.HasValue; }
    public bool ShouldSerializeSlimeOverride() { return SlimeOverride.HasValue; }
    public bool ShouldSerializeWaveTag() { return WaveTag.HasValue; }
  }

  // A wave that just waits for a given delay then finishes.
  // Used to pause between waves.
  public class SpacerWave : Wave {
    public float delay;

    public override IEnumerator Start() {
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
        delay = evt.newValue; WaveChanged.Invoke(); }));

      ve.Add(foldout);
    }

    public override IList<Wave> GetChildren() {
      return new List<Wave>();
    }

    public override bool AddWave(int index, Wave wave) {
      return false;
    }
  }

  public class DialogueBoxWave : Wave {
    public List<string> messages = new();
    public float delay = 0.5f;

    public override IEnumerator Start() {
      TowerManager.Instance.ClearSelection();
      MessageBox.Instance.ShowDialogue(messages);
      Finished = true;
      yield return new WaitUntil(() => !MessageBox.Instance.IsOpen());
      yield return new WaitForSeconds(delay);
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
        delay = evt.newValue; WaveChanged.Invoke(); }));

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
  }

  public class WaitUntilDeadWave : Wave {
    public int? WaveTag;
    public override IEnumerator Start() {
      if (WaveTag == null) {
        yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count == 0);
      } else {
        yield return new WaitUntil(() =>
            !ObjectPool.Instance.GetActiveEnemies().Any(
                (e) => e.WaveTag == WaveTag));
      }
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
  }
}