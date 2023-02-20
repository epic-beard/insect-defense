using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour {
  private ISubwave subwave;
  public ObjectPool pool;
  void Start() {
    EnemySubwave enemyWave = new() {
      data = new EnemyData() {
        type = EnemyData.Type.BEETLE,
        speed = 1.0f
      },
      repetitions = 2,
      repeatDelay = 1,
      spawnLocation = 0,
    };
    EnemySubwave slowWave = new() {
      data = new EnemyData() {
        type = EnemyData.Type.BEETLE,
        speed = 0.5f
      },
      repetitions = 2,
      repeatDelay = 1,
      spawnLocation = 0,
    };
    EnemySubwave fastWave = new() {
      data = new EnemyData() {
        type = EnemyData.Type.BEETLE,
        speed = 2.0f
      },
      repetitions = 2,
      repeatDelay = 1,
      spawnLocation = 0,
    };
    ConcurrentSubwave conWave = new();
    conWave.Subwaves.Add(fastWave);
    conWave.Subwaves.Add(slowWave);

    SequentialSubwave seqWave = new();
    seqWave.Subwaves.Add(enemyWave);
    seqWave.Subwaves.Add(conWave);
    Wave wave = new();
    wave.Subwaves.Add(seqWave);
    wave.Subwaves.Add(enemyWave);
    pool = FindObjectOfType<ObjectPool>();
    StartCoroutine(wave.Start(this));
  }

  void Update() {
  }

  public class Wave {
    public List<ISubwave> Subwaves = new();
    public IEnumerator Start(Spawner spawner) { 
      yield return new WaitUntil(() => Input.GetKey(KeyCode.N));
      foreach (var subwave in Subwaves) {
        yield return subwave.Run(spawner);
        yield return new WaitUntil(() => Input.GetKey(KeyCode.N));
      }      
    }
    public void Serialize(Stream s) { }
    public Wave Deserialize(Stream s) {
      return new Wave();
    }
  }

  public interface ISubwave {
    IEnumerator Run(Spawner spawner);
    bool Finished { get; set; }
  }

  public class SequentialSubwave : ISubwave {
    public List<ISubwave> Subwaves = new();
    public bool Finished { get; set; }
    public IEnumerator Run(Spawner spawner) {
      foreach (var subwave in Subwaves) {
        yield return spawner.StartCoroutine(subwave.Run(spawner));
      }
      Finished = true;
    }
  }

  public class ConcurrentSubwave : ISubwave {
    public List<ISubwave> Subwaves = new();
    public bool Finished { get; set; }
    public IEnumerator Run(Spawner spawner) {
      foreach (var subwave in Subwaves) {
        spawner.StartCoroutine(subwave.Run(spawner));
      }
      yield return new WaitUntil(() => Subwaves.All((s) => s.Finished));
      Finished = true;
    }
  }

  public class EnemySubwave : ISubwave {
    public int repetitions;
    public float repeatDelay;
    public EnemyData data;
    public int spawnLocation;
    public bool Finished { get; set; }

    public IEnumerator Run(Spawner spawner) {
      for (int i = 0; i < repetitions; i++) {
        spawner.pool.InstantiateEnemy(data, spawnLocation);
        yield return new WaitForSeconds(repeatDelay);
      }
      Finished = true;
    }
  }

  public class SpacerSubwave : ISubwave {
    public float delay;
    public bool Finished { get; set; }

    public IEnumerator Run(Spawner spawner) {
      yield return new WaitForSeconds(delay);
      Finished = true;
    }
  }
}