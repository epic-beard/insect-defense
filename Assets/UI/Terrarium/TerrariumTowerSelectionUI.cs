using Assets;
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
      string towerTypeName = tower.Type.ToString();
      int cost = GameStateManager.Instance.GetTowerCost(
          tower.Type,
          TowerDataManager.Instance.GetTowerData(tower.Type).cost);
      if (cost > GameStateManager.Instance.Nu) {
        towerButton.SetEnabled(false);
      }
      
      towerButton.Cost = cost;
      
      TowerData towerData = TowerDataManager.Instance.GetTowerData(tower.Type);
      towerButton.Name = towerData.name;
      towerButton.TooltipText = towerData.tooltip.tooltipText;
      towerButton.ImagePath = towerData.icon_path;

      // Preserve information so the clickhandler knows what type of tower was selected.
      towerButton.TowerType = tower.Type;
      if (!towerTypeToPrefab.ContainsKey(tower.Type)) {
        towerTypeToPrefab.Add(tower.Type, prefabs[i]);
      }
      towerButton.RegisterCallback<ClickEvent>(TowerClickEvent);
      if (!towerButtons.ContainsKey(tower.Type)) {
        towerButtons.Add(tower.Type, towerButton);
      }
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    VisualElement ve = evt.target as VisualElement;
    if (ve == null) return;
    TowerButton button = Utilities.GetAncestor<TowerButton>(ve);

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
}
