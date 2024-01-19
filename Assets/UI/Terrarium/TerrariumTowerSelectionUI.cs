using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumTowerSelectionUI : MonoBehaviour {
  readonly private string towerSelectionListviewName = "tower_selection__listview";

  [SerializeField] List<GameObject> prefabs;

  private UIDocument terrariumScreen;
  private ListView towerSelectionListView;
  private Dictionary<string, GameObject> towerNameToPrefab = new();
  private Dictionary<TowerData.Type, Button> towerButtons = new();

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
    towerNameToPrefab.Clear();
    towerButtons.Clear();
    towerSelectionListView.makeItem = () => new Button();
    towerSelectionListView.bindItem = (e, i) => {
      Button towerButton = (Button)e;
      Tower tower = prefabs[i].GetComponent<Tower>();
      string towerTypeName = tower.TowerType.ToString();
      int cost = GameStateManager.Instance.GetTowerCost(
          tower.TowerType,
          TowerDataManager.Instance.GetTowerData(tower.TowerType).cost);
      if (cost > GameStateManager.Instance.Nu) {
        towerButton.SetEnabled(false);
      }
      
      SetTowerButtonText(towerButton, tower.name, cost);
      towerButton.tooltip = towerTypeName;
      if (!towerNameToPrefab.ContainsKey(towerTypeName)) {
        towerNameToPrefab.Add(towerTypeName, prefabs[i]);
      }
      towerButton.RegisterCallback<ClickEvent>(TowerClickEvent);
      if (!towerButtons.ContainsKey(tower.TowerType)) {
        towerButtons.Add(tower.TowerType, towerButton);
      }
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    Button buttonPressed = evt.target as Button;
    GameStateManager.SelectedTowerType = towerNameToPrefab[buttonPressed.tooltip];
    TerrariumContextUI.Instance.SetTowerContextPanel();
    TerrariumContextUI.Instance.SetContextTowerName(GameStateManager.SelectedTowerType.name);
  }

  public void UpdateAffordableTowers(int nu) {
    foreach (var entry in towerButtons) {
      TowerData towerData = TowerDataManager.Instance.GetTowerData(entry.Key);
      int cost = GameStateManager.Instance.GetTowerCost(towerData.type, towerData.cost);

      Button button = entry.Value;
      SetTowerButtonText(button, towerData.name, cost);
      button.SetEnabled(cost <= GameStateManager.Instance.Nu);
    }
  }

  public void SetTowerButtonText(Button button, string towerName, int cost) {
    button.text = towerName + " - " + Constants.nu + " " + cost;
  }
}
