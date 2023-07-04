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
  }

  private void ConstructTowerSelectionListView() {
    towerSelectionListView.makeItem = () => new Button();
    towerSelectionListView.bindItem = (e, i) => {
      Button tower = (Button)e;
      string towerName = prefabs[i].GetComponent<Tower>().TowerType.ToString();

      tower.text = towerName;
      towerNameToPrefab.Add(towerName, prefabs[i]);
      tower.RegisterCallback<ClickEvent>(TowerClickEvent);
    };
    towerSelectionListView.itemsSource = prefabs;
  }

  private void TowerClickEvent(ClickEvent evt) {
    Button buttonPressed = evt.target as Button;
    GameStateManager.SelectedTowerType = towerNameToPrefab[buttonPressed.text];
    TerrariumContextUI.Instance.SetTowerContextPanel();
    TerrariumContextUI.Instance.SetContextTowerName(GameStateManager.SelectedTowerType.name);
  }
}
