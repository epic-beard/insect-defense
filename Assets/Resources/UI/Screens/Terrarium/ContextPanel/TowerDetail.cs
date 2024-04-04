using Assets;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static Targeting;

public class TowerDetail : MonoBehaviour {
  public static TowerDetail Instance;

  readonly private string towerNameLabelName = "tower-name";
  readonly private string towerIconElementName = "tower-icon";
  readonly private string towerDescriptionLabelName = "tower-description";
  readonly private string sellTowerButtonName = "sell-tower";

  readonly private string towerBehaviorDropdownName = "tower-behavior";
  readonly private string towerPriorityDropdownName = "tower-priority";
  readonly private string towerUpgradeButtonNameTemplate = "tree_X_upgrade_Y__button";
  readonly private string towerUpgradeLabelNameTemplate = "tower_upgrade_tree_X__label";

  readonly private string towerStatDamageName = "tower-stat-damage";
  readonly private string towerStatAttackSpeedName = "tower-stat-attack-speed";
  readonly private string towerStatRangeName = "tower-stat-range";
  readonly private string towerStatArmorPierceName = "tower-stat-armor-pierce";
  readonly private string towerStatAreaOfEffectName = "tower-stat-area-of-effect";
  readonly private string towerStatDamageOverTimeName = "tower-stat-damage-over-time";

  private UIDocument uiDocument;
  private VisualElement rootElement;

  private Label towerNameLabel;
  private VisualElement towerIconElement;
  private Label towerDescriptionLabel;
  private Button sellTowerButton;

  private DropdownField towerBehaviorDropdown;
  private DropdownField towerPriorityDropdown;

  private Label towerStatDamage;
  private Label towerStatAttackSpeed;
  private Label towerStatRange;
  private Label towerStatArmorPierce;
  private Label towerStatAreaOfEffect;
  private Label towerStatDamageOverTime;
  
  private Button[,] towerUpgradeButtons = new Button[3, 5];
  private Label[] towerUpgradeTreeLabels = new Label[3];

  private void Awake() {
    SetVisualElements();
    RegisterCallbacks();
    ConstructDropdownChoices();
    Instance = this;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    rootElement = uiDocument.rootVisualElement;

    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerIconElement = rootElement.Q<VisualElement>(className: towerIconElementName);
    towerDescriptionLabel = rootElement.Q<Label>(className: towerDescriptionLabelName);
    sellTowerButton = rootElement.Q<Button>(sellTowerButtonName);

    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);

    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerStatDamage = rootElement.Q<Label>(towerStatDamageName);
    towerStatAttackSpeed = rootElement.Q<Label>(towerStatAttackSpeedName);
    towerStatRange = rootElement.Q<Label>(towerStatRangeName);
    towerStatArmorPierce = rootElement.Q<Label>(towerStatArmorPierceName);
    towerStatAreaOfEffect = rootElement.Q<Label>(towerStatAreaOfEffectName);
    towerStatDamageOverTime = rootElement.Q<Label>(towerStatDamageOverTimeName);
  }

  private void RegisterCallbacks() {
    towerBehaviorDropdown.RegisterCallback<ChangeEvent<string>>(BehaviorCallback);
    towerPriorityDropdown.RegisterCallback<ChangeEvent<string>>(PriorityCallback);

    sellTowerButton.RegisterCallback<ClickEvent>(OnSellTowerClick);

    for (int i = 0; i < 3; i++)
    {
      for (int j = 0; j < 5; j++)
      {
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
    foreach (var behavior in Enum.GetValues(typeof(Targeting.Behavior)))
    {
      towerBehaviorDropdown.choices.Add(ToTitleCase(behavior.ToString()));
    }

    towerPriorityDropdown.choices.RemoveAt(0);
    foreach (var priority in Enum.GetValues(typeof(Targeting.Priority)))
    {
      towerPriorityDropdown.choices.Add(FormatPriorityEnumString(priority.ToString()));
    }
  }

  private string FormatPriorityEnumString(string toModify) {
    string[] words = toModify.Split('_');
    for (int i = 0; i < words.Length; i++)
    {
      words[i] = ToTitleCase(words[i]);
    }
    return string.Join(" ", words);
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  private void HandleTowerUpgradeCallback(ClickEvent evt) {
    Button button = Utilities.GetAncestor<Button>(evt.target as VisualElement);
    if (button == null) return;

    TowerAbility upgrade = GetUpgradeFromButtonName(button.name);
    if (GameStateManager.Instance.Nu < upgrade.cost)
    {
      return;
    }
    GameStateManager.Instance.Nu -= upgrade.cost;
    TowerManager.SelectedTower.Upgrade(upgrade);
    SetContextForTower(TowerManager.SelectedTower);
  }

  private void OnSellTowerClick(ClickEvent evt) {
    TowerManager.Instance.RefundSelectedTower();
  }

  private TowerAbility GetUpgradeFromButtonName(string buttonName) {
    // tree_X_upgrade_Y__button
    // 0____5_________15
    int upgradePath = (int)Char.GetNumericValue(buttonName[5]);
    int upgradeNum = (int)Char.GetNumericValue(buttonName[15]);

    return TowerManager.SelectedTower.GetUpgradePath(upgradePath)[upgradeNum];
  }

  private void BehaviorCallback(ChangeEvent<string> evt) {
    if (TowerManager.SelectedTower == null)
    {
      Debug.Log("[ERROR] No tower selected, but behavior change attempted.");
      return;
    }
    Targeting.Behavior behavior =
        (Targeting.Behavior)System.Enum.Parse(
            typeof(Targeting.Behavior), towerBehaviorDropdown.value.ToUpper());
    TowerManager.SelectedTower.Behavior = behavior;
  }

  private void PriorityCallback(ChangeEvent<string> evt) {
    if (TowerManager.SelectedTower == null)
    {
      Debug.Log("[ERROR] No tower selected, but priority change attempted.");
      return;
    }
    Targeting.Priority priority =
        (Targeting.Priority)System.Enum.Parse(
            typeof(Targeting.Priority), towerPriorityDropdown.value.ToUpper().Replace(" ", "_"));
    TowerManager.SelectedTower.Priority = priority;
  }

  // Set all appropriate text, pictures, and miscellaneous information for a specific tower.
  public void SetContextForTower(Tower tower) {
    towerNameLabel.text = tower.Name;
    towerIconElement.style.backgroundImage = Resources.Load<Texture2D>("Icons/test");
    towerDescriptionLabel.text = "placeholder";

    towerBehaviorDropdown.index = ((int)tower.Behavior);
    towerPriorityDropdown.index = ((int)tower.Priority);

    // Ensure only the correct button is enabled for clicking.
    for (int i = 0; i < 3; i++)
    {
      towerUpgradeTreeLabels[i].text = tower.GetUpgradePathName(i);

      for (int j = 0; j < 5; j++)
      {
        //towerUpgradeButtons[i, j].SetEnabled(false);
        towerUpgradeButtons[i, j].text = 
            tower.GetUpgradePath(i)[j].name + "\n" + Constants.nu + " " + tower.GetUpgradePath(i)[j].cost;
      }

      for (int j = 0; j <= tower.UpgradeLevels[i] - 1; j++)
      {
        // TODO: There is probably a better way to notify the player that an upgrade has been purchased.
        //towerUpgradeButtons[i, j].text = towerUpgradeButtons[i, j].text + " Bought.";
      }
      if (tower.UpgradeLevels[i] < 5)
      {
        towerUpgradeButtons[i, tower.UpgradeLevels[i]].SetEnabled(true);
      }
    }
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }
}
