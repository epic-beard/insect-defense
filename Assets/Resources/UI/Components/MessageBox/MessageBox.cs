using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MessageBox : MonoBehaviour {
  public static MessageBox Instance;
  readonly private string containerName = "message-box-container";
  readonly private string labelName = "message-box-label";
  readonly private string buttonName = "message-box-button";

  private VisualElement containerVE;
  private Label labelVE;
  private Button buttonVE;

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
    VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
    containerVE = rootElement.Q<VisualElement>(containerName);
    labelVE = rootElement.Q<Label>(labelName);
    buttonVE = rootElement.Q<Button>(buttonName);
  }

  private void RegisterCallbacks() {
    buttonVE.RegisterCallback<ClickEvent>(AdvanceMessageBox);
  }

  public bool IsOpen() {
    return containerVE.style.display == DisplayStyle.Flex;
  }

  private void Show() {
    containerVE.style.display = DisplayStyle.Flex;
  }

  private void Hide() {
    containerVE.style.display = DisplayStyle.None;
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
      labelVE.text = messages[index++].ToString();
      buttonVE.text = GetAdvanceButtonText();
    }
  }

  public void ShowDialogue(List<string> m) {
    messages = m;
    index = 0;

    TerrariumInputManager.Instance.EnableMessageBoxActionMap();
    PauseManager.Instance.HandlePause(PauseToken.MESSAGE_BOX);
    labelVE.text = messages[index++];
    buttonVE.text = GetAdvanceButtonText();
    Show();
  }

}
