using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {
  public static TerrariumUI Instance;

  readonly private string messageBoxLabelName = "message_box__label";

  UIDocument terrariumUI;
  private Label messageBoxLabel;

  private void Awake() {
    Instance = this;
    SetVisualElements();
  }

  void SetVisualElements() {
    terrariumUI = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumUI.rootVisualElement;
    messageBoxLabel = rootElement.Q<Label>(messageBoxLabelName);
  }

  public void HideUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.None;
  }

  public void ShowUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  public void ShowDialogue(string message) {
    PauseManager.Instance.HandleScreenPause();
    messageBoxLabel.text = message;
    messageBoxLabel.style.display = DisplayStyle.Flex;
  }
}
