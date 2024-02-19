#nullable enable
using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

using static EpicBeardLib.XmlSerializationHelpers;

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
  public GameObject Spawn(string enemyDataKey, Waypoint nextWaypoint, Transform? parent = null) {
    EnemyData data = EnemyDataManager.Instance.GetEnemyData(enemyDataKey);
    return Spawn(data, nextWaypoint, parent);
  }

  public GameObject Spawn(EnemyData data, Waypoint nextWaypoint, Transform? parent = null) {
    return ObjectPool.Instance.InstantiateEnemy(data, nextWaypoint, parent);
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

  public abstract class Wave {
    // Starts the wave.  Meant to be called as a Coroutine.
    public abstract IEnumerator Start();
    // Whether or not this wave has completed.
    [XmlIgnore]
    public bool Finished { get; set; }
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
        PauseManager.Instance.HandlePause();
        yield return new WaitForSeconds(0.1f);
        // Run the wave and wait till it is complete.
        yield return wave.Start();

        yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count == 0);
        WaveComplete.Invoke(++Instance.CurrWave, Instance.NumWaves);
        //Wait long enough for the "Wave Complete" text to appear and disappear.
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
  }

  // A wave that creates an enemy at regular intervals.
  public class EnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public EnemyData data;

    public override IEnumerator Start() {
      for (int i = 0; i < repetitions; i++) {
        for (int j = 0; j < spawnAmmount; j++) {
          // Create the enemy.
          Spawner.Instance.Spawn(data, spawnLocation);
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
  }

  // Same as the EnemyWave but used canned stats looked up by a
  // string key.
  public class CannedEnemyWave : Wave {
    public int repetitions;
    public float repeatDelay;
    public int spawnLocation;
    public int spawnAmmount;
    public string enemyDataKey = "";

    public override IEnumerator Start() {
      for (int i = 0; i < repetitions; i++) {
        for (int j = 0; j < spawnAmmount; j++) {
          // Create the enemy.
          Spawner.Instance.Spawn(enemyDataKey, spawnLocation);
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
  }

  public class DialogueBoxWave : Wave {
    public List<string> messages = new();

    public override IEnumerator Start() {
      MessageBox.Instance.ShowDialogue(messages);
      Finished = true;
      yield return new WaitUntil(() => !MessageBox.Instance.IsOpen());
    }

    public override string ToString() {
      return "DialogueBoxWave\n\t" + string.Join("\n\t", messages);
    }
  }

  public class WaitUntilDeadWave : Wave {
    public override IEnumerator Start() {
      yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count == 0);
    }

    public override string ToString() {
      return "WaitUntilDeadWave\n";
    }
  }
}