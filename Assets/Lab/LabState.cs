using UnityEngine;

public class LabState : MonoBehaviour {
  public static LabState Instance;
  public bool isFocused = false;

  private void Awake() {
    Instance = this;
  }
}
