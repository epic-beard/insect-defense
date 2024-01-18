using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {
  public static TerrariumUI Instance;
  
  UIDocument terrariumUI;

  private void Awake() {
    Instance = this;
    SetVisualElements();
  }

  void SetVisualElements() {
    terrariumUI = GetComponent<UIDocument>();
  }

  public void HideUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.None;
  }
  public void ShowUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
