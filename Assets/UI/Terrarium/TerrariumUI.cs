using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {

  UIDocument terrariumScreen;

  private void OnEnable() {
    SetVisualElement();
  }

  private void SetVisualElement() {
    terrariumScreen = GetComponent<UIDocument>();
  }
}
