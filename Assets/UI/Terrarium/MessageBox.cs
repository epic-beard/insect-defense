using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MessageBox : MonoBehaviour {
  public static MessageBox Instance;
  readonly private string messageBoxVEName = "i_message_box__ve";
  readonly private string messageBoxLabelName = "i_message_box__label";
  readonly private string advanceButtonName = "i_advance__button";

  UIDocument messageBox;
  private VisualElement messageBoxVE;
  private Label messageBoxLabel;
  private Button advanceButton;

  private List<string> messages;
  private int index;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    TerrariumInputManager.OnAdvance += AdvanceMessageBox;
    Hide();
  }

  private void SetVisualElements() {
    messageBox = GetComponent<UIDocument>();
    VisualElement rootElement = messageBox.rootVisualElement;
    messageBoxVE = rootElement.Q<VisualElement>(messageBoxVEName);
    messageBoxLabel = rootElement.Q<Label>(messageBoxLabelName);
    advanceButton = rootElement.Q<Button>(advanceButtonName);
  }

  private void RegisterCallbacks() {
    advanceButton.RegisterCallback<ClickEvent>(AdvanceMessageBox);
  }

  public bool IsOpen() {
    return messageBoxVE.style.display == DisplayStyle.Flex;
  }

  private void Show() {
    messageBoxVE.style.display = DisplayStyle.Flex;
  }

  private void Hide() {
    messageBoxVE.style.display = DisplayStyle.None;
  }

  private string GetAdvanceButtonText() {
    return (messages.Count == index) ? "Close" : "Next";
  }
  private void AdvanceMessageBox(ClickEvent evt) {
    AdvanceMessageBox();
  }

  private void AdvanceMessageBox() {
    if (index == messages.Count) {
      messages = new();
      index = 0;
      Hide();
      PauseManager.Instance.HandlePause(PauseToken.MESSAGE_BOX);
      TerrariumInputManager.Instance.EnablePlayerActionMap();
    } else {
      messageBoxLabel.text = messages[index++].ToString();
      advanceButton.text = GetAdvanceButtonText();
    }
  }

  public void ShowDialogue(List<string> m) {
    messages = m;
    index = 0;

    TerrariumInputManager.Instance.EnableMessageBoxActionMap();
    PauseManager.Instance.HandlePause(PauseToken.MESSAGE_BOX);
    messageBoxLabel.text = messages[index++];
    advanceButton.text = GetAdvanceButtonText();
    Show();
  }

}
