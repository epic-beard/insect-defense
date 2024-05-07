using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerSelector : MonoBehaviour {
  readonly private string towerSelectionListviewName = "select-tower-list";

  [SerializeField] List<GameObject> prefabs;

  private UIDocument uiDocument;
  private ListView towerSelectionListViewVE;
  private Dictionary<TowerData.Type, GameObject> towerTypeToPrefab = new();
  private Dictionary<TowerData.Type, SelectTowerButton> towerButtons = new();

  private void Awake() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    towerSelectionListViewVE = rootElement.Q<ListView>(towerSelectionListviewName);
  }

  private void Start() {
    ConstructTowerSelectionListView();
    GameStateManager.OnNuChanged += UpdateAffordableTowers;
  }

  private void ConstructTowerSelectionListView() {
    towerSelectionListViewVE.Clear();
    towerTypeToPrefab.Clear();
    towerButtons.Clear();
    towerSelectionListViewVE.makeItem = () => new SelectTowerButton();
    towerSelectionListViewVE.bindItem = (e, i) => {
      SelectTowerButton towerButton = (SelectTowerButton)e;
      Tower tower = prefabs[i].GetComponent<Tower>();
      string towerTypeName = tower.Type.ToString();
      int cost = TowerManager.Instance.GetTowerCost(TowerManager.Instance.GetTowerData(tower.Type));
      if (cost > GameStateManager.Instance.Nu) {
        towerButton.SetEnabled(false);
      }
      
      towerButton.Cost = cost;
      
      TowerData towerData = TowerManager.Instance.GetTowerData(tower.Type);
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
    towerSelectionListViewVE.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    VisualElement ve = evt.target as VisualElement;
    if (ve == null) return;
    SelectTowerButton button = Utilities.GetAncestor<SelectTowerButton>(ve);

    TowerManager.Instance.SetSelectedTowerType(button.TowerType);
    ContextPanel.Instance.SetTowerContextPanel();
    TowerDetail.Instance.SetContextForTower(TowerManager.Instance.previewTowers[button.TowerType]);
  }

  public void UpdateAffordableTowers(int nu) {
    foreach (var entry in towerButtons) {
      TowerData towerData = TowerManager.Instance.GetTowerData(entry.Key);
      int cost = TowerManager.Instance.GetTowerCost(towerData);

      SelectTowerButton button = entry.Value;
      button.Cost = cost;
      button.SetEnabled(cost <= GameStateManager.Instance.Nu);
    }
  }
}
