using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {
  public static TerrariumUI Instance;

  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";
  readonly private string towerSelectionListviewName = "tower_selection__listview";
  readonly private string gameViewVisualElementName = "game_view__visualelement";
  readonly private string contextWindowLabelName = "placeholder__label";
  readonly private string hpLabelName = "hp__label";

  // The list of 'authorized' tower prefabs for this map. The tower select menu is built off this list.
  [SerializeField] List<GameObject> prefabs;

  private UIDocument terrariumScreen;

  private Label contextLabel;
  private VisualElement gameView;
  private Label hpLabel;
  private Button playPauseButton;
  private Button settingsButton;
  private Dictionary<string, GameObject> towerNameToPrefab = new();
  private ListView towerSelectionListView;

  private void Awake() {
    SetVisualElements();
    ConstructTowerSelectionListView();

    Instance = this;
  }

  private void Start() {
    RegisterCallbacks();
  }

  // This method must be called before any other in this class. It fetches visual elements that need styling
  // and stores them into local variables.
  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    contextLabel = rootElement.Q<Label>(contextWindowLabelName);
    gameView = rootElement.Q<VisualElement>(gameViewVisualElementName);
    hpLabel = rootElement.Q<Label>(hpLabelName);
    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
    towerSelectionListView = rootElement.Q<ListView>(towerSelectionListviewName);
  }

  private void ConstructTowerSelectionListView() {
    towerSelectionListView.makeItem = () => new Button();
    towerSelectionListView.bindItem = (e, i) => {
      Button tower = (Button)e;
      string towerName = prefabs[i].GetComponent<Tower>().TowerType.ToString();

      tower.text = towerName;
      towerNameToPrefab.Add(towerName, prefabs[i]);
      tower.RegisterCallback<ClickEvent>(TowerClickEvent);
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    Button buttonPressed = evt.target as Button;
    contextLabel.text = buttonPressed.text;
    GameStateManager.SelectedTower = towerNameToPrefab[buttonPressed.text];
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

  public void SetHpLabelText(int hp) {
    hpLabel.text = hp.ToString();
  }
}
