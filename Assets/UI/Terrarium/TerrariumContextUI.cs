using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumContextUI : MonoBehaviour {
  public static TerrariumContextUI Instance;

  readonly private string enemyContextVisualElementName = "enemy_context__visualelement";
  readonly private string noContextVisualElementName = "no_context__visualelement";
  readonly private string towerBehaviorDropdownName = "tower_behavior__dropdown";
  readonly private string towerContextVisualElementName = "tower_context__visualelement";
  readonly private string towerNameLabelName = "tower_name__label";
  readonly private string towerPriorityDropdownName = "tower_priority__dropdown";
  readonly private string towerUpgradeButtonNameTemplate = "tree_X_upgrade_Y__button";
  readonly private string towerUpgradeLabelNameTemplate = "tower_upgrade_tree_X__label";

  private UIDocument terrariumScreen;

  private VisualElement enemyContextVisualElement;
  private VisualElement noContextVisualElement;
  private DropdownField towerBehaviorDropdown;
  private VisualElement towerContextVisualElement;
  private Label towerNameLabel;
  private DropdownField towerPriorityDropdown;
  private Button[,] towerUpgradeButtons = new Button[3, 5];
  private Label[] towerUpgradeTreeLabels = new Label[3];

  private void Awake() {
    SetVisualElements();

    Instance = this;
  }

  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    enemyContextVisualElement = rootElement.Q<VisualElement>(enemyContextVisualElementName);
    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);
    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerContextVisualElement = rootElement.Q<VisualElement>(towerContextVisualElementName);
    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);

    for (int i = 0; i < 3; i++) {
      for (int j = 0; j < 5; j++) {
        string buttonName = towerUpgradeButtonNameTemplate.Replace("X", i.ToString()).Replace("Y", j.ToString());
        towerUpgradeButtons[i, j] = rootElement.Q<Button>(buttonName);
      }
      string labelName = towerUpgradeLabelNameTemplate.Replace("X", i.ToString());
      towerUpgradeTreeLabels[i] = rootElement.Q<Label>(labelName);
    }
  }

  private void Start() {
    RegisterCallbacks();
    SetNoContextPanel();
  }

  private void RegisterCallbacks() {
    towerBehaviorDropdown.RegisterCallback<ChangeEvent<string>>(BehaviorCallback);
    towerPriorityDropdown.RegisterCallback<ChangeEvent<string>>(PriorityCallback);
  }

  private void BehaviorCallback(ChangeEvent<string> evt) {
    if (GameStateManager.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but behavior change attempted.");
      return;
    }
    Targeting.Behavior behavior =
        (Targeting.Behavior)System.Enum.Parse(
            typeof(Targeting.Behavior), towerBehaviorDropdown.value.ToUpper());

    // TODO - What is going on with the behavior storage? Figure it out.
    Debug.Log("Selected Behavior: " + behavior + ", index: " + ((int)behavior));
    GameStateManager.SelectedTower.Behavior = behavior;
  }

  private void PriorityCallback(ChangeEvent<string> evt) {
    if (GameStateManager.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but priority change attempted.");
      return;
    }
    Targeting.Priority priority =
        (Targeting.Priority)System.Enum.Parse(
            typeof(Targeting.Priority), towerPriorityDropdown.value.ToUpper().Replace(" ", ""));
    GameStateManager.SelectedTower.Priority = priority;
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

  // Set all appropriate text, pictures, and miscellaneous information for a specific tower
  public void SetContextForTower(Tower tower) {
    towerNameLabel.text = tower.Name;
    Debug.Log("Behavior: " + tower.Behavior + ", index: " + ((int)tower.Behavior));
    //towerBehaviorDropdown.index = ((int)tower.Behavior);
    // set the behavior
    // set the priority
    // set the upgrades
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }
}
