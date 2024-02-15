using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumTowerSelectionUI : MonoBehaviour {
  readonly private string towerSelectionListviewName = "tower_selection__listview";

  [SerializeField] List<GameObject> prefabs;

  private UIDocument terrariumScreen;
  private ListView towerSelectionListView;
  private Dictionary<TowerData.Type, GameObject> towerTypeToPrefab = new();
  private Dictionary<TowerData.Type, TowerButton> towerButtons = new();

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    towerSelectionListView = rootElement.Q<ListView>(towerSelectionListviewName);
  }

  private void Start() {
    ConstructTowerSelectionListView();
    GameStateManager.OnNuChanged += UpdateAffordableTowers;
  }

  private void ConstructTowerSelectionListView() {
    towerSelectionListView.Clear();
    towerTypeToPrefab.Clear();
    towerButtons.Clear();
    towerSelectionListView.makeItem = () => new TowerButton();
    towerSelectionListView.bindItem = (e, i) => {
      TowerButton towerButton = (TowerButton)e;
      Tower tower = prefabs[i].GetComponent<Tower>();
      string towerTypeName = tower.TowerType.ToString();
      int cost = GameStateManager.Instance.GetTowerCost(
          tower.TowerType,
          TowerDataManager.Instance.GetTowerData(tower.TowerType).cost);
      if (cost > GameStateManager.Instance.Nu) {
        towerButton.SetEnabled(false);
      }
      
      towerButton.Cost = cost;
      
      TowerData towerData = TowerDataManager.Instance.GetTowerData(tower.TowerType);
      towerButton.Name = towerData.name;
      towerButton.TooltipText = towerData.tooltip.tooltipText;
      towerButton.Image = towerData.icon_path;

      // Preserve information so the clickhandler knows what type of tower was selected.
      towerButton.TowerType = tower.TowerType;
      if (!towerTypeToPrefab.ContainsKey(tower.TowerType)) {
        towerTypeToPrefab.Add(tower.TowerType, prefabs[i]);
      }
      towerButton.RegisterCallback<ClickEvent>(TowerClickEvent);
      if (!towerButtons.ContainsKey(tower.TowerType)) {
        towerButtons.Add(tower.TowerType, towerButton);
      }
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    VisualElement ve = evt.target as VisualElement;
    if (ve == null) return;
    TowerButton button = GetTowerButtonParent(ve);

    GameStateManager.SelectedTowerType = towerTypeToPrefab[button.TowerType];
    TerrariumContextUI.Instance.SetTowerContextPanel();
    TerrariumContextUI.Instance.SetContextTowerName(GameStateManager.SelectedTowerType.name);
  }

  public void UpdateAffordableTowers(int nu) {
    foreach (var entry in towerButtons) {
      TowerData towerData = TowerDataManager.Instance.GetTowerData(entry.Key);
      int cost = GameStateManager.Instance.GetTowerCost(towerData.type, towerData.cost);

      TowerButton button = entry.Value;
      button.Cost = cost;
      button.SetEnabled(cost <= GameStateManager.Instance.Nu);
    }
  }

  public TowerButton GetTowerButtonParent(VisualElement ve) {
    while (ve != null && ve as TowerButton == null) {
      ve = ve.parent;
    }
    return ve as TowerButton;
  }
}
