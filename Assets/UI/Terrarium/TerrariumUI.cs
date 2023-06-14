using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {
  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";
  readonly private string towerSelectionListviewName = "tower_selection__listview";
  readonly private string gameViewVisualElementName = "game_view__visualelement";

  UIDocument terrariumScreen;

  Button playPauseButton;
  Button settingsButton;
  ListView towerSelectionListView;
  VisualElement gameView;
  List<string> towerNames;

  private void Awake() {
    SetVisualElements();
    ConstructTowerSelectionListView();
    SetupGameView();
  }

  private void Start() {
    RegisterCallbacks();
  }

  // This method must be called before any other in this class. It fetches visual elements that need styling
  // and stores them into local variables.
  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
    towerSelectionListView = rootElement.Q<ListView>(towerSelectionListviewName);
    gameView = rootElement.Q<VisualElement>(gameViewVisualElementName);
  }

  private void ConstructTowerSelectionListView() {
    towerNames = new();
    towerNames.Add("Ant Farm");
    towerNames.Add("Ant Pheremone Tower");
    towerNames.Add("Assassin Bug");
    towerNames.Add("Bee Hive");
    towerNames.Add("Mantis");
    towerNames.Add("Noxious Beetle");
    towerNames.Add("Spitting Ant Tower");
    towerNames.Add("Trapdoor Spider");
    towerNames.Add("Web Shooting Spider Tower");

    towerSelectionListView.makeItem = () => new Button();
    towerSelectionListView.bindItem = (e, i) => { (e as Button).text = towerNames[i]; };
    towerSelectionListView.itemsSource = towerNames;
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(PauseManager.Instance.HandlePauseCallback);
    playPauseButton.RegisterCallback<ClickEvent>(KeepPlayPauseButtonNameCorrect);
    settingsButton.RegisterCallback<ClickEvent>(SettingsMenu.Instance.ToggleSettingsCallback);
  }

  private void KeepPlayPauseButtonNameCorrect(ClickEvent evt) {
    if (Time.timeScale == 0) {
      playPauseButton.text = "Play";
    } else {
      playPauseButton.text = "Pause";
    }
  }

  private void SetupGameView() {
    //gameView.style.height = Screen.safeArea.height;
  }
}
