using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {

  UIDocument terrariumScreen;

  private void OnEnable() {
    SetVisualElements();
  }

  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
  }
}
