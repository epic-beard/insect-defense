using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumTowerSelectionUI : MonoBehaviour {
  readonly private string towerSelectionListviewName = "tower_selection__listview";

  [SerializeField] List<GameObject> prefabs;

  private UIDocument terrariumScreen;
  private ListView towerSelectionListView;
  private Dictionary<string, GameObject> towerNameToPrefab = new();

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
    towerSelectionListView.makeItem = () => new Button();
    towerSelectionListView.bindItem = (e, i) => {
      Button towerButton = (Button)e;
      Tower tower = prefabs[i].GetComponent<Tower>();
      string towerName = tower.TowerType.ToString();
      int cost = GameStateManager.Instance.GetTowerCost(tower.TowerType, tower.Cost);
      if (cost > GameStateManager.Instance.Nu) {
        towerButton.SetEnabled(false);
      }

      towerButton.text = towerName;
      if (!towerNameToPrefab.ContainsKey(towerName)) {
        towerNameToPrefab.Add(towerName, prefabs[i]);
      }
      towerButton.RegisterCallback<ClickEvent>(TowerClickEvent);
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    Button buttonPressed = evt.target as Button;
    GameStateManager.SelectedTowerType = towerNameToPrefab[buttonPressed.text];
    TerrariumContextUI.Instance.SetTowerContextPanel();
    TerrariumContextUI.Instance.SetContextTowerName(GameStateManager.SelectedTowerType.name);
  }

  public void UpdateAffordableTowers(int nu) {
    for (int i = 0; i < prefabs.Count; i++) { 
      Tower tower = prefabs[i].GetComponent<Tower>();
      int cost = GameStateManager.Instance.GetTowerCost(tower.TowerType, tower.Cost);
      var item = towerSelectionListView.ElementAt(i);
      if (item == null) {
        continue;
      }
      item.SetEnabled(cost <= GameStateManager.Instance.Nu);
    }
  }
}
