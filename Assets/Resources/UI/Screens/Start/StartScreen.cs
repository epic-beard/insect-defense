using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StartScreen : MonoBehaviour {
  public static StartScreen Instance;
  #region PrivateMembers
  readonly private string continueButtonName = "continue-button";
  readonly private string loadGameButtonName = "load-game-button";
  readonly private string newGameButtonName = "new-game-button";
  readonly private string quitButtonName = "quit-button";
  readonly private string settingsButtonName = "settings-button";

  readonly private string labSceneName = "Lab";

  private UIDocument uiDocument;

  private Button continueButtonVE;
  private Button loadGameButtonVE;
  private Button newGameButtonVE;
  private Button quitButtonVE;
  private Button settingsButtonVE;

  private PlayerState continuePlayerState;
  #endregion

  private void Awake() {
    SetVisualElements();
    Instance = this;

    PlayerState newPlayer = new PlayerState();
    PlayerState.Instance = newPlayer;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    continueButtonVE = rootElement.Q<Button>(continueButtonName);
    loadGameButtonVE = rootElement.Q<Button>(loadGameButtonName);
    newGameButtonVE = rootElement.Q<Button>(newGameButtonName);
    quitButtonVE = rootElement.Q<Button>(quitButtonName);
    settingsButtonVE = rootElement.Q<Button>(settingsButtonName);
  }

  private void Start() {
    RegisterCallbacks();
    SetContinueButtion();
  }

  private void RegisterCallbacks() {
    loadGameButtonVE.RegisterCallback<ClickEvent>(LoadGameCallback);
    newGameButtonVE.RegisterCallback<ClickEvent>(NewGameCallback);
    quitButtonVE.RegisterCallback<ClickEvent>(QuitCallback);
    settingsButtonVE.RegisterCallback<ClickEvent>(SettingsCallback);
  }

  private void LoadGameCallback(ClickEvent evt) {
    Button loadGameButtonVE = evt.target as Button;
    if (loadGameButtonVE == null) { return; }

    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    StartScreenInputManager.Instance.OpenLoadScreen();
  }

  private void NewGameCallback(ClickEvent evt) {
    Button newGameButtonVE = evt.target as Button;
    if (newGameButtonVE == null) { return; }

    SaveManager.Instance.Save(PlayerState.Instance);
    SceneManager.LoadScene(labSceneName);
  }

  private void QuitCallback(ClickEvent evt) {
    Button quitButtonVE = evt.target as Button;
    if (quitButtonVE == null) { return; }

    Application.Quit();
  }

  private void SettingsCallback(ClickEvent evt) {
    Button settingsButtonVE = evt.target as Button;
    if (settingsButtonVE == null) { return; }

    StartScreenInputManager.Instance.OpenSettings();
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetContinueButtion() {
    List<PlayerState> playerStates = SaveManager.Instance.GetSaves();
    if (playerStates.Count > 0) {
      playerStates.Sort((PlayerState one, PlayerState two) => one.lastSavedTime.CompareTo(two.lastSavedTime));
      continuePlayerState = playerStates[0];
      continueButtonVE.style.display = DisplayStyle.Flex;
      continueButtonVE.RegisterCallback<ClickEvent>(ContinueCallback);
    } else {
      continueButtonVE.style.display = DisplayStyle.None;
    }
  }

  private void ContinueCallback(ClickEvent evt) {
    PlayerState.Instance = continuePlayerState;
    SceneManager.LoadScene(labSceneName);

  }

  public void ShowStartScreen() {
    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
