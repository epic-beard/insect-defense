using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MessageBox : MonoBehaviour {
  public static MessageBox Instance;
  readonly private string messageBoxLabelName = "message_box__label";
  readonly private string advanceButtonName = "advance__button";

  UIDocument messageBox;
  private Label messageBoxLabel;
  private Button advanceButton;

  private List<string> messages;
  private int index;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    Hide();
  }

  private void SetVisualElements() {
    messageBox = GetComponent<UIDocument>();
    VisualElement rootElement = messageBox.rootVisualElement;
    messageBoxLabel = rootElement.Q<Label>(messageBoxLabelName);
    advanceButton = rootElement.Q<Button>(advanceButtonName);
  }

  private void Show() {
    messageBox.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  private void Hide() {
    messageBox.rootVisualElement.style.display = DisplayStyle.None;
  }

  private string GetAdvanceButtonText() {
    return (messages.Count == index + 1) ? "Close" : "Next";
  }

  public void ShowDialogue(List<string> m) {
    messages = m;
    index = 0;

    PauseManager.Instance.HandlePause(PauseToken.MESSAGE_BOX);
    messageBoxLabel.text = messages[0];
    advanceButton.text = GetAdvanceButtonText();
    Show();
  }

}
