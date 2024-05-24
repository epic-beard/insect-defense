using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationWindow : MonoBehaviour {
  public static ConfirmationWindow Instance;

  private static readonly string textLabelName = "confirmation_window_label";
  private static readonly string yesButtonName = "yes_button";
  private static readonly string noButtonName = "no_button";

  private UIDocument uiDocument;
  private Label textLabel;
  private Button yesButton;
  private Button noButton;

  private Action action;
  private bool hasRun;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    textLabel = rootElement.Q<Label>(textLabelName);
    yesButton = rootElement.Q<Button>(yesButtonName);
    noButton = rootElement.Q<Button>(noButtonName);
  }

  private void RegisterCallbacks() {
    yesButton.RegisterCallback<ClickEvent>(
      (evt) => {
        if (hasRun) return;
        action();
        hasRun = true;
        CloseWindow();
      });
    noButton.RegisterCallback<ClickEvent>(
      (evt) => {
        CloseWindow(); 
      });
  }

  public void ShowWindow(string text, Action action) {
    textLabel.text = text;
    this.action = action;
    hasRun = false;
    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    uiDocument.rootVisualElement.MarkDirtyRepaint();
  }

  public void CloseWindow() {
    textLabel.text = "";
    this.action = () => { };
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    uiDocument.rootVisualElement.MarkDirtyRepaint();
  }
}
