using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartScreen : MonoBehaviour {
  readonly private string continueButtonName = "continue__button";
  readonly private string loadGameButtonName = "load_game__button";
  readonly private string newGameButtonName = "new_game__button";
  readonly private string quitButtonName = "quit__button";
  readonly private string settingsButtonName = "settings__button";

  private UIDocument startScreen;

  private Button continueButton;
  private Button loadGameButton;
  private Button newGameButton;
  private Button quitButton;
  private Button settingsButton;

  private void Awake() {
    SetVisualElements();
  }

  private void SetVisualElements() {
    startScreen = GetComponent<UIDocument>();
    VisualElement rootElement = startScreen.rootVisualElement;

    continueButton = rootElement.Q<Button>(continueButtonName);
    loadGameButton = rootElement.Q<Button>(loadGameButtonName);
    newGameButton = rootElement.Q<Button>(newGameButtonName);
    quitButton = rootElement.Q<Button>(quitButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
  }
}
