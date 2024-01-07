using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour {
  readonly private string soundSettingsButtonName = "sound-settings--button";
  readonly private string loadOptionsButtonName = "load-options--button";
  readonly private string soundSettingsPanelName = "sound-settings--panel";
  readonly private string loadOptionsPanelName = "load-options--panel";

  public static SettingsScreen Instance;

  private UIDocument settingsScreen;
  private Button soundSettingsButton;
  private Button loadOptionsButton;
  private VisualElement soundSettingsPanel;
  private VisualElement loadOptionsPanel;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    settingsScreen = GetComponent<UIDocument>();
    VisualElement rootElement = settingsScreen.rootVisualElement;

    soundSettingsButton = rootElement.Q<Button>(soundSettingsButtonName);
    loadOptionsButton = rootElement.Q<Button>(loadOptionsButtonName);
    soundSettingsPanel = rootElement.Q<VisualElement>(soundSettingsPanelName);
    loadOptionsPanel = rootElement.Q<VisualElement>(loadOptionsPanelName);
  }

  private void RegisterCallbacks() {
    soundSettingsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenSoundSettings(); });
    loadOptionsButton.RegisterCallback<ClickEvent>(
      (evt) => { OpenLoadOptions(); });
  }

  public void OpenSettings() {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.Flex;
    soundSettingsPanel.style.display = DisplayStyle.Flex;
    loadOptionsPanel.style.display = DisplayStyle.None;
  }

  public void CloseSettings() {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  public void OpenSoundSettings() {
    soundSettingsPanel.style.display = DisplayStyle.Flex;
    loadOptionsPanel.style.display = DisplayStyle.None;
  }

  public void OpenLoadOptions() {
    soundSettingsPanel.style.display = DisplayStyle.None;
    loadOptionsPanel.style.display = DisplayStyle.Flex;
  }
}
