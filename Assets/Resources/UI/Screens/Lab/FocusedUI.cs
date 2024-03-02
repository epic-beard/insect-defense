using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FocusedUI : MonoBehaviour {
  public static FocusedUI Instance;
  readonly private string backButtonName = "back__button";
  
  private UIDocument focusedScreen;
  
  private Button backButton;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    backButton.clicked += CloseScreen;
    focusedScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    focusedScreen = GetComponent<UIDocument>();
    VisualElement rootElement = focusedScreen.rootVisualElement;

    backButton = rootElement.Q<Button>(backButtonName);
  }

  public void OpenScreen() {
    focusedScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  public void CloseScreen() {
    LabCamera.Instance.ReturnCamera();
    focusedScreen.rootVisualElement.style.display = DisplayStyle.None;
  }
}
