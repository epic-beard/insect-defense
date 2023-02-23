using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour {
  [SerializeField] private List<Waypoint> spawnLocations = new();
  private ObjectPool pool;

  void Start() {
    pool = FindObjectOfType<ObjectPool>();
    // TODO: Deserialize a Waves.
    // StartCoroutine(wave.Start(this));
  }

  // A thin wrapper around InstantiateEnemy, using spawnLocation as an
  // index into spawnLocations.
  public GameObject Spawn(EnemyData data, int spawnLocation) {
    return pool.InstantiateEnemy(data, spawnLocations[spawnLocation]);
  }

  // The interface for the Waves system.
  public interface IWave {
    // Starts the wave.  Meant to be called as a Coroutine.
    IEnumerator Start(Spawner spawner);
    // Whether or not this wave has completed.
    bool Finished { get; set; }
  }

  // The top level of the subwave heirarchy, describing a level.
  public class Waves : IWave{
    // Each wave represents one round of combat.
    readonly private List<IWave> waves = new();
    // Whether this wave has completed.
    public bool Finished { get; set; }
    // Starts the level logic.
    public IEnumerator Start(Spawner spawner) {
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
  public class SequentialWave : IWave {
    readonly public List<IWave> Subwaves = new();
    public bool Finished { get; set; }
    public IEnumerator Start(Spawner spawner) {
      foreach (var subwave in Subwaves) {
        // Start the subwave and wait till it's finished.
        yield return spawner.StartCoroutine(subwave.Start(spawner));
      }
      Finished = true;
    }
  }

  // A wave that calls its subwaves concurrently.
  public class ConcurrentWave : IWave {
    readonly public List<IWave> Subwaves = new();
    public bool Finished { get; set; }
    public IEnumerator Start(Spawner spawner) {
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
  public class EnemyWave : IWave {
    public int repetitions;
    public float repeatDelay;
    public EnemyData data;
    public int spawnLocation;
    public bool Finished { get; set; }

    public IEnumerator Start(Spawner spawner) {
      for (int i = 0; i < repetitions; i++) {
        // Create the enemy.
        spawner.Spawn(data, spawnLocation);
        // Wait for repeat delay seconds.
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
    }
  }

  // A wave that just waits for a given delay then finishes.
  // Used to pause between waves.
  public class SpacerWave : IWave {
    public float delay;
    public bool Finished { get; set; }

    public IEnumerator Start(Spawner spawner) {
      // Wait for delay seconds.
      yield return new WaitForSeconds(delay);
      Finished = true;
    }
  }
}