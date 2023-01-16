#nullable enable
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Targeting {

  public Enemy? FindTarget() {
    GameObject gameObject = new();
    return gameObject.AddComponent<Enemy>();
  }
}
