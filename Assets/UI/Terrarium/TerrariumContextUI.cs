using System.Collections;
using System.Collections.Generic;
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

  private UIDocument terrariumScreen;

  private VisualElement enemyContextVisualElement;
  private VisualElement noContextVisualElement;
  private DropdownField towerBehaviorDropdown;
  private VisualElement towerContextVisualElement;
  private Label towerNameLabel;
  private DropdownField towerPriorityDropdown;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    enemyContextVisualElement = rootElement.Q<VisualElement>(enemyContextVisualElementName);
    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);
    towerBehaviorDropdown = rootElement.Q<DropdownField>(towerBehaviorDropdownName);
    towerContextVisualElement = rootElement.Q<VisualElement>(towerContextVisualElementName);
    towerNameLabel = rootElement.Q<Label>(towerNameLabelName);
    towerPriorityDropdown = rootElement.Q<DropdownField>(towerPriorityDropdownName);

    Instance = this;
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
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
