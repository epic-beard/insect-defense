using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour {
  readonly private string soundSettingsButtonName = "sound-settings--button";
  readonly private string loadOptionsButtonName = "load-options--button";
  readonly private string quitToLabButtonName = "quit-to-lab--button";
  readonly private string soundSettingsPanelName = "sound-settings--panel";
  readonly private string loadOptionsPanelName = "load-options--panel";
  readonly private string terrariumSpecificPanelName = "terrarium-specific--panel";

  public static SettingsScreen Instance;

  private UIDocument settingsScreen;
  private Button soundSettingsButton;
  private Button loadOptionsButton;
  private Button quitToLabButton;
  private VisualElement soundSettingsPanel;
  private VisualElement loadOptionsPanel;
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

    soundSettingsButton = rootElement.Q<Button>(soundSettingsButtonName);
    loadOptionsButton = rootElement.Q<Button>(loadOptionsButtonName);
    quitToLabButton = rootElement.Q<Button>(quitToLabButtonName);
    soundSettingsPanel = rootElement.Q<VisualElement>(soundSettingsPanelName);
    loadOptionsPanel = rootElement.Q<VisualElement>(loadOptionsPanelName);
    terrariumSpecificPanel = rootElement.Q<VisualElement>(terrariumSpecificPanelName);
  }

  private void RegisterCallbacks() {
    soundSettingsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenSoundSettings(); });
    loadOptionsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenLoadOptions(); });
    quitToLabButton.RegisterCallback<ClickEvent>(
      (evt) => { QuitToLab(); });
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

  // Note, this doesn't unpause the game, but the pause manager handles ensuring new scenes
  // have timescale of 1.
  private void QuitToLab() {
    SceneManager.LoadScene(Constants.labSceneName);
  }
}
