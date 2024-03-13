using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemFocusScreen : MonoBehaviour {
  public static ItemFocusScreen Instance;
  readonly private string backButtonName = "item-focus-back-button";
  
  private UIDocument uiDocument;
  
  private Button backButtonVE;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    backButtonVE.clicked += CloseScreen;
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    backButtonVE = rootElement.Q<Button>(backButtonName);
  }

  public void OpenScreen() {
    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  public void CloseScreen() {
    LabCamera.Instance.ReturnCamera();
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
  }
}
