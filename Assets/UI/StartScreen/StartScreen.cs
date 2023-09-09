using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour {
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
  #endregion

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

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    quitButton.RegisterCallback<ClickEvent>(QuitCallback);
    newGameButton.RegisterCallback<ClickEvent>(NewGameCallback);
  }

  private void NewGameCallback(ClickEvent evt) {
    Button newGameButton = evt.target as Button;
    if (newGameButton == null) { return; }

    PlayerState.Instance = new PlayerState();
    SaveManager.Instance.Save(PlayerState.Instance);
    SceneManager.LoadScene(labSceneName);
  }

  private void QuitCallback(ClickEvent evt) {
    Button quitButton = evt.target as Button;
    if (quitButton == null) { return; }

    Application.Quit();
  }
}
