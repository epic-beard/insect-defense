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
  private static readonly string windowName = "confirmation_window";

  private UIDocument uiDocument;
  private Label textLabel;
  private Button yesButton;
  private Button noButton;
  private VisualElement window;

  private Action action;
  private bool hasRun;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    window.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;
    
    textLabel = rootElement.Q<Label>(textLabelName);
    yesButton = rootElement.Q<Button>(yesButtonName);
    noButton = rootElement.Q<Button>(noButtonName);
    window = rootElement.Q<VisualElement>(windowName);
  }

  private void RegisterCallbacks() {
    yesButton.RegisterCallback<ClickEvent>(
      (evt) => {
        if (!hasRun) {
          action();
          hasRun = true;
        }
        CloseWindow();
      });
    noButton.RegisterCallback<ClickEvent>(
      (evt) => {
        CloseWindow(); 
      });
    window.RegisterCallback<MouseEnterEvent>(
      (evt) => {
        GameStateManager.Instance.IsMouseOverUI = true;
      });

    window.RegisterCallback<MouseLeaveEvent>(
      (evt) => {
        GameStateManager.Instance.IsMouseOverUI = false;
      });
  }

  public void ShowWindow(string text, Action action) {
    textLabel.text = text;
    this.action = action;
    hasRun = false;
    window.style.display = DisplayStyle.Flex;
  }

  public void CloseWindow() {
    textLabel.text = "";
    this.action = () => { };
    window.style.display = DisplayStyle.None;
    window.MarkDirtyRepaint();
  }
}
