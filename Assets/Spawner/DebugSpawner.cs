using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour {
  [SerializeField] private Waypoint start;
  [SerializeField] private EnemyData data;

  private void Start() {
    Enum.GetValues(typeof(EnemyData.Type));
    // TODO: Figure out why the hercules beetle and wolf spider cause crashes.
    var types = new HashSet<EnemyData.Type>() {
      EnemyData.Type.ANT,
      EnemyData.Type.APHID,
      EnemyData.Type.BEETLE,
      //EnemyData.Type.HERCULES_BEETLE,
      EnemyData.Type.LEAF_BUG,
      EnemyData.Type.SLUG,
      EnemyData.Type.SNAIL,
      EnemyData.Type.STINK_BUG,
      EnemyData.Type.TARANTULA,
      EnemyData.Type.TERMITE,
      //EnemyData.Type.WOLF_SPIDER,
    };

    // Use this code once the crash issue has been solved. Don't forget linq.
    //HashSet<EnemyData.Type> allEnemyTypes =
    //  new HashSet<EnemyData.Type>(Enum.GetValues(typeof(EnemyData.Type)).OfType<EnemyData.Type>().ToList());

    ObjectPool.Instance.InitializeObjectPool(types);
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.F)) {
      Spawn();
    }
  }

  private void Spawn() {
    ObjectPool.Instance.InstantiateEnemy(data, start);
  }
}
