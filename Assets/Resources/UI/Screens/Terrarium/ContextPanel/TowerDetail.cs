using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Targeting;

public class TowerDetail : MonoBehaviour {
  public static TowerDetail Instance;

  readonly private string towerNameLabelName = "tower-name";
  readonly private string towerIconElementName = "tower-icon";
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
  readonly private string towerStatDotStacksName = "tower-stat-dot-stacks";

  private UIDocument uiDocument;
  private VisualElement rootElement;

  private Label towerNameLabel;
  private VisualElement towerIconElement;
  private Button sellTowerButton;

  private DropdownField towerBehaviorDropdown;
  private DropdownField towerPriorityDropdown;

  private ButtonWithTooltip towerStatDamageIcon;
  private ButtonWithTooltip towerStatAttackSpeedIcon;
  private ButtonWithTooltip towerStatRangeIcon;
  private ButtonWithTooltip towerStatArmorPierceIcon;
  private ButtonWithTooltip towerStatAreaOfEffectIcon;
  private ButtonWithTooltip towerStatDotStacksIcon;

  private Label towerStatDamage;
  private Label towerStatAttackSpeed;
  private Label towerStatRange;
  private Label towerStatArmorPierce;
  private Label towerStatAreaOfEffect;
  private Label towerStatDotStacks;

  private ButtonWithTooltip[,] towerUpgradeIcons = new ButtonWithTooltip[3, 5];
  private Button[,] towerUpgradeSwitches = new Button[3, 5];
  private Label[] towerUpgradeTreeLabels = new Label[3];
  private Dictionary<Behavior, int> behaviorIndexMap;
  private Dictionary<Priority, int> priorityIndexMap;

  private void Awake() {
    SetVisualElements();
    RegisterCallbacks();
    Instance = this;
    GameStateManager.OnNuChanged += UpdateAffordableTowerUpgrades;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    rootElement = uiDocument.rootVisualElement;

    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerIconElement = rootElement.Q<VisualElement>(className: towerIconElementName);
    sellTowerButton = rootElement.Q<Button>(sellTowerButtonName);

    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);

    towerStatDamageIcon = rootElement.Q<ButtonWithTooltip>(towerStatDamageName);
    towerStatAttackSpeedIcon = rootElement.Q<ButtonWithTooltip>(towerStatAttackSpeedName);
    towerStatRangeIcon = rootElement.Q<ButtonWithTooltip>(towerStatRangeName);
    towerStatArmorPierceIcon = rootElement.Q<ButtonWithTooltip>(towerStatArmorPierceName);
    towerStatAreaOfEffectIcon = rootElement.Q<ButtonWithTooltip>(towerStatAreaOfEffectName);
    towerStatDotStacksIcon = rootElement.Q<ButtonWithTooltip>(towerStatDotStacksName);

    towerStatDamage = rootElement.Q<Label>(towerStatDamageName);
    towerStatAttackSpeed = rootElement.Q<Label>(towerStatAttackSpeedName);
    towerStatRange = rootElement.Q<Label>(towerStatRangeName);
    towerStatArmorPierce = rootElement.Q<Label>(towerStatArmorPierceName);
    towerStatAreaOfEffect = rootElement.Q<Label>(towerStatAreaOfEffectName);
    towerStatDotStacks = rootElement.Q<Label>(towerStatDotStacksName);
  }

  private void RegisterCallbacks() {
    towerBehaviorDropdown.RegisterCallback<ChangeEvent<string>>(BehaviorCallback);
    towerPriorityDropdown.RegisterCallback<ChangeEvent<string>>(PriorityCallback);
    towerBehaviorDropdown.RegisterCallback<MouseDownEvent>(evt => {
      UiSfx.PlaySfx(UiSfx.dropdown_active);
    });
    towerPriorityDropdown.RegisterCallback<MouseDownEvent>(evt => {
      UiSfx.PlaySfx(UiSfx.dropdown_active);
    });

    sellTowerButton.RegisterCallback<ClickEvent>(OnSellTowerClick);

    for (int i = 0; i < 3; i++) {
      for (int j = 0; j < 5; j++) {
        string buttonName = towerUpgradeButtonNameTemplate.Replace("X", i.ToString()).Replace("Y", j.ToString());

        towerUpgradeIcons[i, j] = rootElement.Q<ButtonWithTooltip>(buttonName);

        towerUpgradeSwitches[i, j] = rootElement.Q<Button>(buttonName);
        towerUpgradeSwitches[i, j].RegisterCallback<ClickEvent>(HandleTowerUpgradeCallback);
        towerUpgradeSwitches[i, j].SetEnabled(true);
      }
      string labelName = towerUpgradeLabelNameTemplate.Replace("X", i.ToString());
      towerUpgradeTreeLabels[i] = rootElement.Q<Label>(labelName);
    }
  }



  // Construct the dropdown choices for the Behavior and Priorty options.
  private void ConstructDropdownChoices(Tower tower) {
    towerBehaviorDropdown.choices.Clear();
    foreach (var obj in Enum.GetValues(typeof(Targeting.Behavior))) {
      Behavior behavior = (Behavior)obj;
      if (tower.Type != TowerData.Type.WEB_SHOOTING_SPIDER_TOWER && behavior == Behavior.SLOW_EM_ALL) {
        continue;
      }
      String choice = ToTitleCase(behavior.ToString());
      towerBehaviorDropdown.choices.Add(choice);
      if (behavior == tower.Behavior) {
        towerBehaviorDropdown.SetValueWithoutNotify(choice);
      }
    }

    towerPriorityDropdown.choices.Clear();
    foreach (var obj in Enum.GetValues(typeof(Targeting.Priority))) {
      Priority priority = (Priority)obj;
      string choice = FormatPriorityEnumString(priority.ToString());
      towerPriorityDropdown.choices.Add(choice);
      if (priority == tower.Priority) {
        towerPriorityDropdown.SetValueWithoutNotify(choice);
      }
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
    Button rockerSwitch = Utilities.GetAncestor<Button>(evt.target as VisualElement);
    if (rockerSwitch == null) return;

    ButtonWithTooltip icon = rootElement.Q<ButtonWithTooltip>(rockerSwitch.name);
    if (icon == null) return;

    Tower tower = TowerManager.Instance.SelectedTower;
    if (tower == null) return;

    // tree_X_upgrade_Y__button
    // 0____5_________15
    int upgradePath = (int)Char.GetNumericValue(rockerSwitch.name[5]);
    int upgradeNum = (int)Char.GetNumericValue(rockerSwitch.name[15]);

    // check that current upgrade is not already owned
    if (tower.UpgradeIndex[upgradePath] >= upgradeNum) {
      ConfirmationWindow.Instance.ShowWindow(
        "Sell Upgrades?",
        () => {
          SellUpgrades(tower, upgradePath, upgradeNum);
          if (!tower.IsMutatingUpgrades) {
            tower.IsMutatingUpgrades = true;
            TowerManager.Instance.DisableTowerAfterSellingUpgrade(tower);
          }
        });
      return;
    }

    // Enforce upgrade limits 1 - 3 - 5. That is, one tree at 1, one tree at 3, one tree at 5.
    if (!tower.IsLegalUpgrade(upgradePath, upgradeNum)) {
      return;
    }

    // check that player can afford the upgrades
    int totalCost = 0;
    for (int i = tower.UpgradeIndex[upgradePath] + 1; i <= upgradeNum; i++) {
      totalCost += tower.GetUpgradePath(upgradePath)[i].cost;
    }
    if (GameStateManager.Instance.Nu < totalCost) {
      UiSfx.PlaySfx(UiSfx.rocker_switch_fail);
      return;
    }

    if (TowerManager.Instance.SelectedTower.IsPreviewTower) {
      return;
    }

    // all checks passed, continue with upgrade
    UiSfx.PlaySfx(UiSfx.rocker_switch);
    for (int i = tower.UpgradeIndex[upgradePath] + 1; i <= upgradeNum; i++) {
      SetRockerSwitchState(towerUpgradeSwitches[upgradePath, i], true);
      TowerAbility upgrade = tower.GetUpgradePath(upgradePath)[i];
      GameStateManager.Instance.Nu -= upgrade.cost;
      tower.Upgrade(upgrade);
    }

    SetContextForTower(tower);
  }

  private void SellUpgrades(Tower tower, int upgradePath, int upgradeNum) {
    UiSfx.PlaySfx(UiSfx.rocker_switch);

    int level = tower.UpgradeIndex[upgradePath];
    while (level >= upgradeNum) {
      TowerAbility upgrade = tower.GetUpgradePath(upgradePath)[level];
      GameStateManager.Instance.Nu += upgrade.cost;
      Button rockerSwitch = towerUpgradeSwitches[upgradePath, level];
      SetRockerSwitchState(rockerSwitch, false);
      level--;
    }

    List<int> levels = tower.UpgradeIndex.ToList();
    levels[upgradePath] = level;
    tower.SetTowerData(TowerManager.Instance.GetTowerData(tower.Type));
    tower.ResetTransientState();

    for (int path = 0; path < 3; path++) {
      level = -1;
      while (level < levels[path]) {
        level++;
        TowerAbility upgrade = tower.GetUpgradePath(path)[level];
        tower.Upgrade(upgrade);
      }
    }

    SetContextForTower(tower);
  }

  private void OnSellTowerClick(ClickEvent evt) {
    UiSfx.PlaySfx(UiSfx.sell_click);
    ConfirmationWindow.Instance.ShowWindow("Sell Tower?",
      () => {
        TowerManager.Instance.RefundSelectedTower();
      });
  }

  private void BehaviorCallback(ChangeEvent<string> evt) {
    if (TowerManager.Instance.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but behavior change attempted.");
      return;
    }
    UiSfx.PlaySfx(UiSfx.dropdown_event_changed);
    Targeting.Behavior behavior =
        (Targeting.Behavior)System.Enum.Parse(
            typeof(Targeting.Behavior), towerBehaviorDropdown.value.ToUpper());
    TowerManager.Instance.SelectedTower.Behavior = behavior;
  }

  private void PriorityCallback(ChangeEvent<string> evt) {
    if (TowerManager.Instance.SelectedTower == null) {
      Debug.Log("[ERROR] No tower selected, but priority change attempted.");
      return;
    }
    UiSfx.PlaySfx(UiSfx.dropdown_event_changed);
    Targeting.Priority priority =
        (Targeting.Priority)System.Enum.Parse(
            typeof(Targeting.Priority), towerPriorityDropdown.value.ToUpper().Replace(" ", "_"));
    TowerManager.Instance.SelectedTower.Priority = priority;
  }

  // Set all appropriate text, pictures, and miscellaneous information for a specific tower.
  public void SetContextForTower(Tower tower) {
    towerNameLabel.text = tower.Name;
    towerIconElement.style.backgroundImage = Resources.Load<Texture2D>("Icons/test");

    ConstructDropdownChoices(tower);

    towerStatDamageIcon.TooltipText = "Damage";
    towerStatAttackSpeedIcon.TooltipText = "Attack Speed";
    towerStatRangeIcon.TooltipText = "Range";
    towerStatArmorPierceIcon.TooltipText = "Armor Pierce";
    towerStatAreaOfEffectIcon.TooltipText = "Area of Effect";
    towerStatDotStacksIcon.TooltipText = "Damage Over Time Stacks";

    towerStatDamage.text = tower.Damage.ToString();
    towerStatAttackSpeed.text = tower.AttackSpeed.ToString();
    towerStatRange.text = tower.Range.ToString();
    towerStatArmorPierce.text = tower.ArmorPierce.ToString();
    towerStatAreaOfEffect.text = tower.AreaOfEffect.ToString();
    towerStatDotStacks.text = tower.AcidStacks.ToString();

    for (int i = 0; i < 3; i++) {
      string upgradePathName = tower.GetUpgradePathName(i);
      towerUpgradeTreeLabels[i].text = upgradePathName;
      for (int j = 0; j < 5; j++) {
        bool isOwned = j <= tower.UpgradeIndex[i];
        SetRockerSwitchState(towerUpgradeSwitches[i, j], isOwned);

        ButtonWithTooltip icon = towerUpgradeIcons[i, j];
        TowerAbility towerUpgrade = tower.GetUpgradePath(i)[j];

        icon.TooltipText = Constants.nu + towerUpgrade.cost + " \u21d2 " + towerUpgrade.description;

        string imgPath = "UI/images/tower upgrades/" + tower.Name + "/" + upgradePathName.ToLower() + " " + j;
        icon.style.backgroundImage = Resources.Load<Texture2D>(imgPath);

        UpdateUpgradeIconState(icon, i, j, GameStateManager.Instance.Nu);
      }
    }
  }

  private void SetRockerSwitchState(Button rockerSwitch, bool isOn) {
    if (isOn) {
      rockerSwitch.AddToClassList("rocker-switch-on");
      rockerSwitch.RemoveFromClassList("rocker-switch-off");
    } else {
      rockerSwitch.AddToClassList("rocker-switch-off");
      rockerSwitch.RemoveFromClassList("rocker-switch-on");
    }
  }

  private void UpdateUpgradeIconState(ButtonWithTooltip icon, int path, int level, int nu) {
    Tower tower = TowerManager.Instance.SelectedTower;
    if (tower == null) return;

    bool isOwned = level <= tower.UpgradeIndex[path];

    bool isPreviousUpgradeOwned = level - 1 == tower.UpgradeIndex[path];
    bool isAffordable = nu >= tower.GetUpgradePath(path)[level].cost;
    bool isPurchasable = isPreviousUpgradeOwned && isAffordable;

    SetUpgradeIconState(icon, isOwned, isPurchasable);
  }

  private void SetUpgradeIconState(ButtonWithTooltip icon, bool isOwned, bool isPurchasable) {
    icon.RemoveFromClassList("tower-upgrade-icon-purchased");
    icon.RemoveFromClassList("tower-upgrade-icon-purchasable");
    icon.RemoveFromClassList("tower-upgrade-icon-not-purchasable");

    if (isOwned) {
      icon.AddToClassList("tower-upgrade-icon-purchased");
    } else if (isPurchasable) {
      icon.AddToClassList("tower-upgrade-icon-purchasable");
    } else {
      icon.AddToClassList("tower-upgrade-icon-not-purchasable");
    }
  }

  private void UpdateAffordableTowerUpgrades(int nu) {
    for (int i = 0; i < 3; i++) {
      for (int j = 0; j < 5; j++) {
        ButtonWithTooltip icon = towerUpgradeIcons[i, j];
        UpdateUpgradeIconState(icon, i, j, nu);
      }
    }
  }

  public void SetContextTowerName(string name) {
    towerNameLabel.text = name;
  }
}
