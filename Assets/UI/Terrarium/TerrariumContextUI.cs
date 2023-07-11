using System;
using UnityEngine;
using UnityEngine.UIElements;
using static Targeting;

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
    ConstructDropdownChoices();
    
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
        towerUpgradeButtons[i, j].RegisterCallback<ClickEvent>(HandleTowerUpgradeCallback);
        towerUpgradeButtons[i, j].SetEnabled(false);
      }
      string labelName = towerUpgradeLabelNameTemplate.Replace("X", i.ToString());
      towerUpgradeTreeLabels[i] = rootElement.Q<Label>(labelName);
    }
  }

  // Construct the dropdown choices for the Behavior and Priorty options.
  private void ConstructDropdownChoices() {
    towerBehaviorDropdown.choices.RemoveAt(0);
    foreach (var behavior in Enum.GetValues(typeof(Targeting.Behavior))) {
      towerBehaviorDropdown.choices.Add(FormatTargetingEnumsForUser(behavior.ToString()));
    }

    towerPriorityDropdown.choices.RemoveAt(0);
    foreach (var priority in Enum.GetValues(typeof(Targeting.Priority))) {
      towerPriorityDropdown.choices.Add(FormatTargetingEnumsForUser(priority.ToString()));
    }
  }

  // Recursively process Targeting's enum strings for presentation to the user in the context UI.
  private string FormatTargetingEnumsForUser(string toModify) {
    if (toModify.Contains('_')) {
      string[] words = toModify.Split('_');
      for (int i = 0; i < words.Length; i++) {
        words[i] = FormatTargetingEnumsForUser(words[i]);
      }
      return string.Join(" ", words);
    }
    return string.Concat(toModify[..1].ToUpper(), toModify[1..].ToLower());
  }

  private void HandleTowerUpgradeCallback(ClickEvent evt) {
    // TODO:
    //   Fire a call to the Upgrade method with the appropriate info.
    //   Adjust which button is enabled.
    Debug.Log("You clicked a button! Button clicked: " + evt.target);
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
    if (GameStateManager.Instance.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but behavior change attempted.");
      return;
    }
    Targeting.Behavior behavior =
        (Targeting.Behavior)System.Enum.Parse(
            typeof(Targeting.Behavior), towerBehaviorDropdown.value.ToUpper());
    GameStateManager.Instance.SelectedTower.Behavior = behavior;
  }

  private void PriorityCallback(ChangeEvent<string> evt) {
    if (GameStateManager.Instance.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but priority change attempted.");
      return;
    }
    Targeting.Priority priority =
        (Targeting.Priority)System.Enum.Parse(
            typeof(Targeting.Priority), towerPriorityDropdown.value.ToUpper().Replace(" ", "_"));
    GameStateManager.Instance.SelectedTower.Priority = priority;
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
    towerBehaviorDropdown.index = ((int)tower.Behavior);
    towerPriorityDropdown.index = ((int)tower.Priority);

    // Ensure only the correct button is enabled for clicking.
    for (int i = 0; i < 3; i++) {
      for (int j= 0; j <= tower.UpgradeLevels[i] - 1; j++) {
        // TODO: There is probably a better way to notify the player that an upgrade has been purchased.
        towerUpgradeButtons[i, j].text = "Bought";
      }
      towerUpgradeButtons[i, tower.UpgradeLevels[i]].SetEnabled(true);
    }
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }
}
