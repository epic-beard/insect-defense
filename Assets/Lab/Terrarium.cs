#nullable enable
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Terrarium : MonoBehaviour {
  public static event Action<Terrarium> TerrariumClicked = delegate { };
  public static Terrarium? Selected;
  public float cameraOffsetZ;
  public int level;

  private void Start() {
    if (PlayerState.Instance.CurrentLevel < level) {
      var mat = GetComponent<Renderer>().material;
      mat.SetColor("_Color", Color.gray);
    }
  }

  private void OnMouseUp() {
    if (!LabState.Instance.CanClickGameScreen()) return;
    lab.TerrariumUI.Instance.OpenScreen(this);
  }
}
