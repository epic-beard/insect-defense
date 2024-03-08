using Assets;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextPanel : MonoBehaviour {
  public static ContextPanel Instance;

  readonly private string enemyArmorCurrentArmorLabelName = "enemy_current_armor__label";
  readonly private string enemyArmorMaxArmorLabelName = "enemy_max_armor__label";
  readonly private string enemyContextVisualElementName = "enemy_context__visualelement";
  readonly private string enemyDamageLabelName = "enemy_damage__label";
  readonly private string enemyHpCurrentHpLabelName = "enemy_current_hp__label";
  readonly private string enemyHpMaxHpLabelName = "enemy_max_hp__label";
  readonly private string enemyNameLabelName = "enemy_name__label";
  readonly private string enemyNuLabelName = "enemy_nu__label";
  readonly private string enemySizeLabelName = "enemy_size__label";
  readonly private string enemySpeedLabelName = "enemy_speed__label";
  readonly private string noContextVisualElementName = "no_context__visualelement";
  readonly private string towerBehaviorDropdownName = "tower_behavior__dropdown";
  readonly private string towerContextVisualElementName = "tower_context__visualelement";
  readonly private string towerNameLabelName = "tower_name__label";
  readonly private string towerPriorityDropdownName = "tower_priority__dropdown";
  readonly private string towerUpgradeButtonNameTemplate = "tree_X_upgrade_Y__button";
  readonly private string towerUpgradeLabelNameTemplate = "tower_upgrade_tree_X__label";
  readonly private string sellTowerButtonName = "sell_tower__button";

  private UIDocument terrariumScreen;

  private Label enemyArmorCurrentArmorLabel;
  private Label enemyArmorMaxArmorLabel;
  private VisualElement enemyContextVisualElement;
  private Label enemyDamageLabel;
  private Label enemyHpCurrentHpLabel;
  private Label enemyHpMaxHpLabel;
  private Label enemyNameLabel;
  private Label enemyNuLabel;
  private Label enemySizeLabel;
  private Label enemySpeedLabel;
  private Button sellTowerButton;

  private VisualElement noContextVisualElement;

  private DropdownField towerBehaviorDropdown;
  private VisualElement towerContextVisualElement;
  private Label towerNameLabel;
  private DropdownField towerPriorityDropdown;
  private ButtonWithTooltip[,] towerUpgradeButtons = new ButtonWithTooltip[3, 5];
  private Label[] towerUpgradeTreeLabels = new Label[3];

  private void Awake() {
    SetVisualElements();
    RegisterCallbacks();
    ConstructDropdownChoices();

    Instance = this;
  }

  private void Start() {
    RegisterCallbacks();
    SetNoContextPanel();
  }

  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    enemyArmorCurrentArmorLabel = rootElement.Q<Label>(enemyArmorCurrentArmorLabelName);
    enemyArmorMaxArmorLabel = rootElement.Q<Label>(enemyArmorMaxArmorLabelName);
    enemyContextVisualElement = rootElement.Q<VisualElement>(enemyContextVisualElementName);
    enemyDamageLabel = rootElement.Q<Label>(enemyDamageLabelName);
    enemyHpCurrentHpLabel = rootElement.Q<Label>(enemyHpCurrentHpLabelName);
    enemyHpMaxHpLabel = rootElement.Q<Label>(enemyHpMaxHpLabelName);
    enemyNameLabel = rootElement.Q<Label>(enemyNameLabelName);
    enemyNuLabel = rootElement.Q<Label>(enemyNuLabelName);
    enemySizeLabel = rootElement.Q<Label>(enemySizeLabelName);
    enemySpeedLabel = rootElement.Q<Label>(enemySpeedLabelName);
    sellTowerButton = rootElement.Q<Button>(sellTowerButtonName);

    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);

    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerContextVisualElement = rootElement.Q<VisualElement>(towerContextVisualElementName);
    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);
  }

  private void RegisterCallbacks() {
    towerBehaviorDropdown.RegisterCallback<ChangeEvent<string>>(BehaviorCallback);
    towerPriorityDropdown.RegisterCallback<ChangeEvent<string>>(PriorityCallback);

    sellTowerButton.RegisterCallback<ClickEvent>(OnSellTowerClick);

    VisualElement rootElement = terrariumScreen.rootVisualElement;
    for (int i = 0; i < 3; i++) {
      for (int j = 0; j < 5; j++) {
        string buttonName = towerUpgradeButtonNameTemplate.Replace("X", i.ToString()).Replace("Y", j.ToString());
        towerUpgradeButtons[i, j] = rootElement.Q<ButtonWithTooltip>(buttonName);
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
      towerBehaviorDropdown.choices.Add(ToTitleCase(behavior.ToString()));
    }

    towerPriorityDropdown.choices.RemoveAt(0);
    foreach (var priority in Enum.GetValues(typeof(Targeting.Priority))) {
      towerPriorityDropdown.choices.Add(FormatPriorityEnumString(priority.ToString()));
    }
  }

  private string FormatPriorityEnumString(string toModify) {
    string[] words = toModify.Split('_');
    for (int i = 0; i < words.Length; i++) {
      words[i] = ToTitleCase(words[i]);
    }
    return string.Join(" ", words);
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  private void HandleTowerUpgradeCallback(ClickEvent evt) {
    ButtonWithTooltip button = Utilities.GetAncestor<ButtonWithTooltip>(evt.target as VisualElement);
    if (button == null) return;

    TowerAbility upgrade = GetUpgradeFromButtonName(button.name);
    if (GameStateManager.Instance.Nu < upgrade.cost) {
      return;
    }
    GameStateManager.Instance.Nu -= upgrade.cost;
    GameStateManager.Instance.SelectedTower.Upgrade(upgrade);
    SetContextForTower(GameStateManager.Instance.SelectedTower);
  }

  private void OnSellTowerClick(ClickEvent evt) {
    GameStateManager.Instance.RefundSelectedTower();
  }

  private TowerAbility GetUpgradeFromButtonName(string buttonName) {
    // tree_X_upgrade_Y__button
    // 0____5_________15
    int upgradePath = (int)Char.GetNumericValue(buttonName[5]);
    int upgradeNum = (int)Char.GetNumericValue(buttonName[15]);

    return GameStateManager.Instance.SelectedTower.GetUpgradePath(upgradePath)[upgradeNum];
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

  // Set all appripriate text, pictures, and miscellaneous information for a specific enemy.
  public void SetContextForEnemy(Enemy enemy) {
    enemyNameLabel.text = ToTitleCase(enemy.Type.ToString());
    enemySizeLabel.text = ToTitleCase(enemy.Size.ToString());
    enemyHpCurrentHpLabel.text = enemy.HP.ToString();
    enemyHpMaxHpLabel.text = enemy.MaxHp.ToString();
    enemyArmorCurrentArmorLabel.text = enemy.Armor.ToString();
    enemyArmorMaxArmorLabel.text = enemy.MaxArmor.ToString();
    enemySpeedLabel.text = enemy.Speed.ToString();
    enemyDamageLabel.text = enemy.Damage.ToString();
    enemyNuLabel.text = enemy.Nu.ToString();
}

  // Set all appropriate text, pictures, and miscellaneous information for a specific tower.
  public void SetContextForTower(Tower tower) {
    towerNameLabel.text = tower.Name;
    towerBehaviorDropdown.index = ((int)tower.Behavior);
    towerPriorityDropdown.index = ((int)tower.Priority);

    // Ensure only the correct button is enabled for clicking.
    for (int i = 0; i < 3; i++) {
      towerUpgradeTreeLabels[i].text = tower.GetUpgradePathName(i);

      for (int j = 0; j < 5; j++) {
        towerUpgradeButtons[i, j].SetEnabled(false);
        towerUpgradeButtons[i, j].SetButtonText(
            tower.GetUpgradePath(i)[j].name + "\n" + Constants.nu + " " + tower.GetUpgradePath(i)[j].cost);
        towerUpgradeButtons[i, j].TooltipText = tower.GetUpgradePath(i)[j].description;
      }

      for (int j = 0; j <= tower.UpgradeLevels[i] - 1; j++) {
        // TODO: There is probably a better way to notify the player that an upgrade has been purchased.
        towerUpgradeButtons[i, j].SetButtonText(
            towerUpgradeButtons[i, j].Button.text + " Bought.");
      }
      if (tower.UpgradeLevels[i] < 5) {
        towerUpgradeButtons[i, tower.UpgradeLevels[i]].SetEnabled(true);
      }
    }
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }

  public void SubscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent += SetContextForEnemy;
  }

  public void DesbuscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent -= SetContextForEnemy;
  }
}