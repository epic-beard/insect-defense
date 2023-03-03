using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class Spawner : MonoBehaviour {
  [SerializeField] private List<Waypoint> spawnLocations = new();
  private ObjectPool pool;
  private EnemyDataManager enemyDataManager;

  void Start() {
    pool = FindObjectOfType<ObjectPool>();
    enemyDataManager = FindObjectOfType<EnemyDataManager>();
    // TODO: Deserialize a Waves.
    // StartCoroutine(wave.Start(this));
  }

  // A thin wrapper around InstantiateEnemy, using spawnLocation as an
  // index into spawnLocations.
  public GameObject Spawn(EnemyData data, int spawnLocation) {
    return pool.InstantiateEnemy(data, spawnLocations[spawnLocation]);
  }
  // Same as above but looks up the enemy stats in the EnemyManager.
  public GameObject Spawn(string enemyDataKey, int spawnLocation) {
    EnemyData data = enemyDataManager.GetEnemyData(enemyDataKey);
    return Spawn(data, spawnLocation);
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
    public abstract IEnumerator Start(Spawner spawner);
    // Whether or not this wave has completed.
    [XmlIgnore]
    public bool Finished { get; set; }
  }

  // The top level of the subwave heirarchy, describing a level.
  public class Waves : Wave {
    // Each wave represents one round of combat.
    readonly public List<Wave> waves = new();
    // Starts the level logic.
    public override IEnumerator Start(Spawner spawner) {
      foreach (var wave in waves) {
        // Wait for the signal to start the next level.
        yield return new WaitUntil(() => Input.GetKey(KeyCode.N));
        // Run the wave and wait till it is complete.
        yield return wave.Start(spawner);
      }
      Finished = true;
    }
    public void Serialize(Stream s) { }
    public Waves Deserialize(Stream s) {
      return new Waves();
    }
  }

  // A wave that calls its subwaves sequentially.
  public class SequentialWave : Wave {
    readonly public List<Wave> Subwaves = new();
    public override IEnumerator Start(Spawner spawner) {
      foreach (var subwave in Subwaves) {
        // Start the subwave and wait till it's finished.
        yield return spawner.StartCoroutine(subwave.Start(spawner));
      }
      Finished = true;
    }
  }

  // A wave that calls its subwaves concurrently.
  public class ConcurrentWave : Wave {
    readonly public List<Wave> Subwaves = new();
    public override IEnumerator Start(Spawner spawner) {
      // Start all the subwaves.
      foreach (var subwave in Subwaves) {
        spawner.StartCoroutine(subwave.Start(spawner));
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
    public EnemyData data;

    public override IEnumerator Start(Spawner spawner) {
      for (int i = 0; i < repetitions; i++) {
        // Create the enemy.
        spawner.Spawn(data, spawnLocation);
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
    public string enemyDataKey;

    public override IEnumerator Start(Spawner spawner) {
      for (int i = 0; i < repetitions; i++) {
        // Create the enemy.
        spawner.Spawn(enemyDataKey, spawnLocation);
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

    public override IEnumerator Start(Spawner spawner) {
      // Wait for delay seconds.
      yield return new WaitForSeconds(delay);
      Finished = true;
    }
  }
}