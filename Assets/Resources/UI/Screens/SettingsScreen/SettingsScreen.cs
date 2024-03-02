using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour {
  readonly private string loadOptionsButtonName = "load-options--button";
  readonly private string quitToLabButtonName = "quit-to-lab--button";
  readonly private string restartLevelButtonName = "restart-level--button";
  readonly private string soundSettingsButtonName = "sound-settings--button";
  readonly private string loadOptionsPanelName = "load-options--panel";
  readonly private string soundSettingsPanelName = "sound-settings--panel";
  readonly private string terrariumSpecificPanelName = "terrarium-specific--panel";

  public static SettingsScreen Instance;

  private UIDocument settingsScreen;
  private Button loadOptionsButton;
  private Button quitToLabButton;
  private Button restartLevelButton;
  private Button soundSettingsButton;
  private VisualElement loadOptionsPanel;
  private VisualElement soundSettingsPanel;
  private VisualElement terrariumSpecificPanel;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
    terrariumSpecificPanel.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    settingsScreen = GetComponent<UIDocument>();
    VisualElement rootElement = settingsScreen.rootVisualElement;

    loadOptionsButton = rootElement.Q<Button>(loadOptionsButtonName);
    quitToLabButton = rootElement.Q<Button>(quitToLabButtonName);
    restartLevelButton = rootElement.Q<Button>(restartLevelButtonName);
    soundSettingsButton = rootElement.Q<Button>(soundSettingsButtonName);
    loadOptionsPanel = rootElement.Q<VisualElement>(loadOptionsPanelName);
    soundSettingsPanel = rootElement.Q<VisualElement>(soundSettingsPanelName);
    terrariumSpecificPanel = rootElement.Q<VisualElement>(terrariumSpecificPanelName);
  }

  public bool IsOpen() {
    return settingsScreen.rootVisualElement.style.display == DisplayStyle.Flex;
  }

  private void RegisterCallbacks() {
    loadOptionsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenLoadOptions(); });
    quitToLabButton.RegisterCallback<ClickEvent>(
      (evt) => { QuitToLab(); });
    restartLevelButton.RegisterCallback<ClickEvent>(
      (evt) => { RestartLevel(); });
    soundSettingsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenSoundSettings(); });
  }

  public void OpenSettings(bool inGame = false) {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.Flex;
    soundSettingsPanel.style.display = DisplayStyle.Flex;
    loadOptionsPanel.style.display = DisplayStyle.None;
    if (inGame) terrariumSpecificPanel.style.display = DisplayStyle.Flex;
  }

  public void CloseSettings() {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
    terrariumSpecificPanel.style.display = DisplayStyle.None;
  }

  public void OpenSoundSettings() {
    soundSettingsPanel.style.display = DisplayStyle.Flex;
    loadOptionsPanel.style.display = DisplayStyle.None;
  }

  public void OpenLoadOptions() {
    soundSettingsPanel.style.display = DisplayStyle.None;
    loadOptionsPanel.style.display = DisplayStyle.Flex;
  }

  // Note: This doesn't unpause the game, but the pause manager handles ensuring new scenes
  // have timescale of 1.
  private void QuitToLab() {
    SceneManager.LoadScene(Constants.labSceneName);
  }

  private void RestartLevel() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
