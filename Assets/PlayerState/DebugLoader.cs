using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLoader : MonoBehaviour {

  [SerializeField] PlayerState State;
  void Awake() {
    PlayerState.Instance ??= State;
  }
}
