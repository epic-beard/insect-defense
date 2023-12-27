using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StartScreen : MonoBehaviour {
  public static StartScreen Instance;
  #region PrivateMembers
  readonly private string continueButtonName = "continue__button";
  readonly private string loadGameButtonName = "load_game__button";
  readonly private string newGameButtonName = "new_game__button";
  readonly private string quitButtonName = "quit__button";
  readonly private string settingsButtonName = "settings__button";

  readonly private string labSceneName = "Lab";

  private UIDocument startScreen;

  private Button continueButton;
  private Button loadGameButton;
  private Button newGameButton;
  private Button quitButton;
  private Button settingsButton;

  private PlayerState continuePlayerState;
  #endregion

  private void Awake() {
    SetVisualElements();
    Instance = this;

    PlayerState newPlayer = new PlayerState();
    PlayerState.Instance = newPlayer;
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

  private void Start() {
    RegisterCallbacks();
    SetContinueButtion();
  }

  private void RegisterCallbacks() {
    loadGameButton.RegisterCallback<ClickEvent>(LoadGameCallback);
    newGameButton.RegisterCallback<ClickEvent>(NewGameCallback);
    quitButton.RegisterCallback<ClickEvent>(QuitCallback);
    settingsButton.RegisterCallback<ClickEvent>(SettingsCallback);
  }

  private void LoadGameCallback(ClickEvent evt) {
    Button loadGameButton = evt.target as Button;
    if (loadGameButton == null) { return; }

    LoadScreen.Instance.OpenMenu();
    startScreen.rootVisualElement.style.display = DisplayStyle.None;
    StartScreenInputManager.Instance.OpenLoadScreen();
  }

  private void NewGameCallback(ClickEvent evt) {
    Button newGameButton = evt.target as Button;
    if (newGameButton == null) { return; }

    SaveManager.Instance.Save(PlayerState.Instance);
    SceneManager.LoadScene(labSceneName);
  }

  private void QuitCallback(ClickEvent evt) {
    Button quitButton = evt.target as Button;
    if (quitButton == null) { return; }

    Application.Quit();
  }

  private void SettingsCallback(ClickEvent evt) {
    Button settingsButton = evt.target as Button;
    if (settingsButton == null) { return; }

    StartScreenInputManager.Instance.OpenSettings();
    startScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetContinueButtion() {
    List<PlayerState> playerStates = SaveManager.Instance.GetSaves();
    if (playerStates.Count > 0) {
      playerStates.Sort((PlayerState one, PlayerState two) => one.lastSavedTime.CompareTo(two.lastSavedTime));
      continuePlayerState = playerStates[0];
      continueButton.style.display = DisplayStyle.Flex;
      continueButton.RegisterCallback<ClickEvent>(ContinueCallback);
    } else {
      continueButton.style.display = DisplayStyle.None;
    }
  }

  private void ContinueCallback(ClickEvent evt) {
    PlayerState.Instance = continuePlayerState;
    SceneManager.LoadScene(labSceneName);

  }

  public void ShowStartScreen() {
    startScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
