#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

using static EpicBeardLib.XmlSerializationHelpers;

public class Spawner : MonoBehaviour {
  public static event Action OnLevelComplete;
  static public Spawner Instance;

  [SerializeField] private List<Waypoint> spawnLocations = new();
  [SerializeField] private string filename = "";

  private void Awake() {
    Instance = this;
  }

  void Start() {
    //----------------------------------------
    //EnemyData testData1 = new() {
    //  maxArmor = 1,
    //  maxHP = 20,
    //  size = EnemyData.Size.NORMAL,
    //  nu = 20,
    //  type = EnemyData.Type.ANT,
    //  speed = 1.0f
    //  damage = 1
    //};

    //EnemyWave enemyWave1 = new() {
    //  data = testData1,
    //  repetitions = 2,
    //  spawnLocation = 0,
    //  spawnAmmount = 2,
    //};

    //EnemyData testData2 = new() {
    //  maxArmor = 5,
    //  maxHP = 5,
    //  size = EnemyData.Size.TINY,
    //  nu = 10,
    //  type = EnemyData.Type.BEETLE,
    //  speed = 0.5f
    //  damage = 2
    //};

    //EnemyWave enemyWave2 = new() {
    //  data = testData2,
    //  repetitions = 1,
    //  spawnLocation = 0,
    //  spawnAmmount = 1,
    //};

    //SequentialWave seqWave1 = new();
    //seqWave1.Subwaves.Add(enemyWave1);
    //seqWave1.Subwaves.Add(new SpacerWave() { delay = 1.0f });
    //seqWave1.Subwaves.Add(enemyWave2);

    //CannedEnemyWave cannedEnemyWave1 = new() {
    //  repetitions = 3,
    //  spawnLocation = 0,
    //  spawnAmmount = 2,
    //  enemyDataKey = "ant"
    //};

    //CannedEnemyWave cannedEnemyWave2 = new() {
    //  repetitions = 3,
    //  spawnLocation = 0,
    //  spawnAmmount = 1,
    //  enemyDataKey = "beetle"
    //};

    //ConcurrentWave conWave1 = new();
    //conWave1.Subwaves.Add(cannedEnemyWave1);
    //conWave1.Subwaves.Add(cannedEnemyWave2);

    //Waves testWave = new();
    //testWave.waves.Add(seqWave1);
    //testWave.waves.Add(conWave1);

    //Serialize<Wave>(testWave, filename);
    //----------------------------------------
    if (filename.Length == 0) return;
    Wave? wave = Deserialize<Wave>(filename);
    if (wave != null)  SpawnWave(wave);
  }

  public void SpawnWave(Wave wave) {
    ClearWave();
    StartCoroutine(wave.Start());
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
  public abstract class Wave {
    // Starts the wave.  Meant to be called as a Coroutine.
    public abstract IEnumerator Start();
    // Whether or not this wave has completed.
    [XmlIgnore]
    public bool Finished { get; set; }
  }

  // The top level of the subwave heirarchy, describing a level.
  public class Waves : Wave {
    // Each wave represents one round of combat.
    readonly public List<Wave> waves = new();
    // Starts the level logic.
    public override IEnumerator Start() {
      foreach (var wave in waves) {
        // Wait for the signal to start the next level.
        yield return new WaitUntil(() => Input.GetKey(KeyCode.N));
        // Run the wave and wait till it is complete.
        yield return wave.Start();
      }

      // Sanity check, make sure all the waves have completed.
      yield return new WaitUntil(() => waves.All<Wave>((wave) => wave.Finished));

      // The level ends once all the enemies have been spawned and destroyed.
      yield return new WaitUntil(() => ObjectPool.Instance.GetActiveEnemies().Count() == 0);

      // Make sure the players health didn't drop to zero getting rid of the last enemy.
      if (GameStateManager.Instance.Health > 0) {
        OnLevelComplete.Invoke();
      }
      Finished = true;
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
  }
}