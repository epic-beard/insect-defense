using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
  public static PauseManager Instance;

  private bool locked = false;

  private void Awake() {
    Instance = this;
  }
  void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      HandlePause();
    }
  }

  void HandlePause() {
    if (locked) return;

    Time.timeScale = Time.timeScale == 0 ? 1 : 0;
  }

  public void PauseAndLock() {
    locked = true;
    Time.timeScale = 0;
  }

  public void UnpauseAndUnlock() {
    locked = false;
    Time.timeScale = 1;
  }
}
