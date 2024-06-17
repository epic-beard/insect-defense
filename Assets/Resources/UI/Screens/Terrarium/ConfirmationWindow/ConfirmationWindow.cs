using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationWindow : MonoBehaviour {
  public static ConfirmationWindow Instance;

  private static readonly string windowName = "confirmation-window";
  private static readonly string textLabelName = "confirmation-window-label";
  private static readonly string yesButtonName = "yes-button";
  private static readonly string noButtonName = "no-button";

  private UIDocument uiDocument;
  private VisualElement window;
  private Label textLabel;
  private Button yesButton;
  private Button noButton;

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
    
    window = rootElement.Q<VisualElement>(windowName);
    textLabel = rootElement.Q<Label>(textLabelName);
    yesButton = rootElement.Q<Button>(yesButtonName);
    noButton = rootElement.Q<Button>(noButtonName);
  }

  private void RegisterCallbacks() {
    window.RegisterCallback<MouseLeaveEvent>(
      (evt) => {
        GameStateManager.Instance.IsMouseOverUI = false;
      });

    window.RegisterCallback<MouseEnterEvent>(
      (evt) => {
        GameStateManager.Instance.IsMouseOverUI = true;
      });

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
