using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour {
  readonly private string loadOptionsButtonName = "load-options-button";
  readonly private string quitToLabButtonName = "quit-to-lab-button";
  readonly private string restartLevelButtonName = "restart-level-button";
  readonly private string soundSettingsButtonName = "sound-settings-button";
  readonly private string loadOptionsPanelName = "load-options-panel";
  readonly private string soundSettingsPanelName = "sound-settings-panel";
  readonly private string terrariumSettingsPanelName = "terrarium-settings-panel";

  public static SettingsScreen Instance;

  private UIDocument uiDocument;
  private Button loadOptionsButtonVE;
  private Button quitToLabButtonVE;
  private Button restartLevelButtonVE;
  private Button soundSettingsButtonVE;
  private VisualElement loadOptionsPanelVE;
  private VisualElement soundSettingsPanelVE;
  private VisualElement terrariumSettingsPanelVE;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    terrariumSettingsPanelVE.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    loadOptionsButtonVE = rootElement.Q<Button>(loadOptionsButtonName);
    quitToLabButtonVE = rootElement.Q<Button>(quitToLabButtonName);
    restartLevelButtonVE = rootElement.Q<Button>(restartLevelButtonName);
    soundSettingsButtonVE = rootElement.Q<Button>(soundSettingsButtonName);
    loadOptionsPanelVE = rootElement.Q<VisualElement>(loadOptionsPanelName);
    soundSettingsPanelVE = rootElement.Q<VisualElement>(soundSettingsPanelName);
    terrariumSettingsPanelVE = rootElement.Q<VisualElement>(terrariumSettingsPanelName);
  }

  public bool IsOpen() {
    return uiDocument.rootVisualElement.style.display == DisplayStyle.Flex;
  }

  private void RegisterCallbacks() {
    loadOptionsButtonVE.RegisterCallback<ClickEvent>(
      (evt) => { OpenLoadOptions(); });
    quitToLabButtonVE.RegisterCallback<ClickEvent>(
      (evt) => { QuitToLab(); });
    restartLevelButtonVE.RegisterCallback<ClickEvent>(
      (evt) => { RestartLevel(); });
    soundSettingsButtonVE.RegisterCallback<ClickEvent>(
      (evt) => { OpenSoundSettings(); });
  }

  public void OpenSettings(bool inGame = false) {
    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    soundSettingsPanelVE.style.display = DisplayStyle.Flex;
    loadOptionsPanelVE.style.display = DisplayStyle.None;
    if (inGame) terrariumSettingsPanelVE.style.display = DisplayStyle.Flex;
  }

  public void CloseSettings() {
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    terrariumSettingsPanelVE.style.display = DisplayStyle.None;
  }

  public void OpenSoundSettings() {
    UiSfx.PlaySfx(UiSfx.settings_tab_active);
    soundSettingsPanelVE.style.display = DisplayStyle.Flex;
    loadOptionsPanelVE.style.display = DisplayStyle.None;
    soundSettingsButtonVE.AddToClassList("button-array-button-active");
    soundSettingsButtonVE.RemoveFromClassList("button-array-button-inactive");
    loadOptionsButtonVE.AddToClassList("button-array-button-inactive");
    loadOptionsButtonVE.RemoveFromClassList("button-array-button-active");
  }

  public void OpenLoadOptions() {
    UiSfx.PlaySfx(UiSfx.settings_tab_active);
    soundSettingsPanelVE.style.display = DisplayStyle.None;
    loadOptionsPanelVE.style.display = DisplayStyle.Flex;
    loadOptionsButtonVE.AddToClassList("button-array-button-active");
    loadOptionsButtonVE.RemoveFromClassList("button-array-button-inactive");
    soundSettingsButtonVE.AddToClassList("button-array-button-inactive");
    soundSettingsButtonVE.RemoveFromClassList("button-array-button-active");
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
