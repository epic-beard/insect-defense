using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class TerrariumUI : MonoBehaviour {
  public static TerrariumUI Instance;

  // Contextual Panel
  readonly private string enemyContextVisualElementName = "enemy_context__visualelement";
  readonly private string noContextVisualElementName = "no_context__visualelement";
  readonly private string towerBehaviorDropdownName = "tower_behavior__dropdown";
  readonly private string towerContextVisualElementName = "tower_context__visualelement";
  readonly private string towerNameLabelName = "tower_name__label";
  readonly private string towerPriorityDropdownName = "tower_priority__dropdown";

  // Bottom Panel
  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";

  // Status Panel
  readonly private string hpLabelName = "hp__label";

  // Game View
  readonly private string gameViewVisualElementName = "game_view__visualelement";

  // Tower Select Panel
  readonly private string towerSelectionListviewName = "tower_selection__listview";

  // The list of 'authorized' tower prefabs for this map. The tower select menu is built off this list.
  [SerializeField] List<GameObject> prefabs;

  private UIDocument terrariumScreen;

  // Contextual Panel
  private VisualElement enemyContextVisualElement;
  private VisualElement noContextVisualElement;
  private DropdownField towerBehaviorDropdown;
  private VisualElement towerContextVisualElement;
  private Label towerNameLabel;
  private DropdownField towerPriorityDropdown;

  // Bottom Panel
  private Button playPauseButton;
  private Button settingsButton;

  // Status Panel
  private Label hpLabel;

  // Game View
  private VisualElement gameView;

  // Tower Select Panel
  private ListView towerSelectionListView;

  private Dictionary<string, GameObject> towerNameToPrefab = new();

  private void Awake() {
    SetVisualElements();
    ConstructTowerSelectionListView();
    //SetNoContextPanel();
    SetTowerContextPanel();

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

    enemyContextVisualElement = rootElement.Q<VisualElement>(enemyContextVisualElementName);
    gameView = rootElement.Q<VisualElement>(gameViewVisualElementName);
    hpLabel = rootElement.Q<Label>(hpLabelName);
    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);
    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
    towerContextVisualElement = rootElement.Q<VisualElement>(towerContextVisualElementName);
    towerSelectionListView = rootElement.Q<ListView>(towerSelectionListviewName);

    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);
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
    GameStateManager.SelectedTowerType = towerNameToPrefab[buttonPressed.text];
    SetTowerContextPanel();
    SetContextTowerName(GameStateManager.SelectedTowerType.name);
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(PauseManager.Instance.HandlePauseCallback);
    playPauseButton.RegisterCallback<ClickEvent>(KeepPlayPauseButtonNameCorrect);
    settingsButton.RegisterCallback<ClickEvent>(SettingsMenu.Instance.ToggleSettingsCallback);

    towerBehaviorDropdown.RegisterCallback<ChangeEvent<string>>(BehaviorCallback);
    towerPriorityDropdown.RegisterCallback<ChangeEvent<string>>(PriorityCallback);
  }

  private void BehaviorCallback(ChangeEvent<string> evt) {
    if (GameStateManager.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but behavior change attempted.");
    }
    Targeting.Behavior behavior =
        (Targeting.Behavior)System.Enum.Parse(
            typeof(Targeting.Behavior), towerBehaviorDropdown.value.ToUpper());
    GameStateManager.SelectedTower.Behavior = behavior;
  }

  private void PriorityCallback(ChangeEvent<string> evt) {
    if (GameStateManager.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but priority change attempted.");
    }
    Targeting.Priority priority =
        (Targeting.Priority)System.Enum.Parse(
            typeof(Targeting.Priority), towerPriorityDropdown.value.ToUpper().Replace(" ", ""));
    GameStateManager.SelectedTower.Priority = priority;
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

  public void SetNoContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.None;
    towerContextVisualElement.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.Flex;
  }

  public void SetTowerContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.None;
    towerContextVisualElement.style.display = DisplayStyle.Flex;
    noContextVisualElement.style.display = DisplayStyle.None;
  }

  public void SetEnemyContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.Flex;
    towerContextVisualElement.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.None;
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }
}
