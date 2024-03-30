using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;

public class TowerManager : MonoBehaviour {
  public static TowerManager Instance;

  // The type of tower currently selected by the user for construction.
  public static Tower? SelectedTower;
  // The specific tower the user clicked on in the map.
  public static TowerData.Type? SelectedTowerType;
  public Dictionary<Vector2Int, Tower> ActiveTowerMap = new();
  // A First In Last Out container of historical prices of a tower.
  public Dictionary<TowerData.Type, Stack<int>> TowerPrices = new();

  [SerializeField] private string towerDataFilename = "data.towers";
  private readonly float towerScalingFactor = 1.2f;
  private readonly float buildDelay = 2.0f;
  private readonly float sellDelay = 2.0f;
  private TowerDictionary towers = new();
  private readonly Dictionary<TowerData.Type, string> prefabMap = new() {
    { TowerData.Type.SPITTING_ANT_TOWER, "Towers/SpittingAntTower/Spitting Ant Tower" },
    { TowerData.Type.MANTIS_TOWER, "Towers/MantisTower/Mantis Tower" },
    { TowerData.Type.ASSASSIN_BUG_TOWER, "Towers/AssassinBugTower/Assassin Bug Tower" },
    { TowerData.Type.WEB_SHOOTING_SPIDER_TOWER, "Towers/WebShootingSpiderTower/Web Shooting Spider Tower" },
  };

  private void Awake() {
    Instance = this;
    towers = Deserialize<TowerDictionary>(towerDataFilename);
  }

  public Tower GetTower(Vector2Int coordinates) {
    return ActiveTowerMap[coordinates];
  }

  public bool HasTower(Vector2Int coordinates) {
    return ActiveTowerMap.ContainsKey(coordinates);
  }

  public int GetTowerCost(TowerData data) {
    if (!TowerPrices.ContainsKey(data.type) || TowerPrices[data.type].Count == 0) {
      return Mathf.RoundToInt(data.cost);
    }

    return Mathf.RoundToInt(GetPreviousTowerCost(data.type) * towerScalingFactor);
  }

  public int GetPreviousTowerCost(TowerData.Type type) {
    if (!TowerPrices.ContainsKey(type) || TowerPrices[type].Count == 0) {
      return Mathf.RoundToInt(TowerManager.Instance.GetTowerData(type).cost);
    }

    return TowerPrices[type].Peek();
  }

  public TowerData GetTowerData(TowerData.Type type) {
    return towers[type];
  }

  public Tower ConstructTower(Waypoint waypoint, TowerData.Type type) {
    return ConstructTower(waypoint, GetTowerData(type));
  }

  // Builds a tower at the given waypoint, after checking that the player has
  // enough Nu.  Returns true if the tower was constructed.
  public bool BuildTower(Waypoint waypoint) {
    if (SelectedTowerType == null) { return false; }

    TowerData data = GetTowerData(SelectedTowerType??TowerData.Type.NONE);
    int cost = GetTowerCost(data);
    if (GameStateManager.Instance.Nu < cost) { return false; }
    GameStateManager.Instance.Nu -= cost;
    AddTowerPrice(data.type, cost);
    StartCoroutine(BuildTower(waypoint, data));
    return true;
  }

  public IEnumerator BuildTower(Waypoint waypoint, TowerData data) {
    Tower tower = ConstructTower(waypoint, data);

    yield return new WaitForSeconds(buildDelay);
    tower.enabled = true;
  }

  public List<Tower> GetTowersInRange(float range, Vector3 pos) {
    return ActiveTowerMap.Values.Where(
      (tower) => Vector3.Distance(pos, tower.transform.position) <= range).ToList();
  }

  // Set a new tower as the selected tower. Adjust the tile color to indicate selection as necessary.
  public void SetNewSelectedTower(Tower tower) {
    if (SelectedTower != null) {
      SelectedTower.Tile.SetUnselected();
    }
    tower.Tile.SetSelected();
    SelectedTower = tower;
  }

  public string GetTowerPrefabPath(TowerData.Type type) {  return prefabMap[type]; }


  public void RefundSelectedTower() {
    RefundTower(SelectedTower);
  }
  // Refund the tower's full cost (including upgrades) and remove the tower from the map.
  public void RefundTower(Tower tower) {
    if (tower == null) return;
    ActiveTowerMap.Remove(tower.Tile.GetCoordinates());
    StartCoroutine(DestroyTower(tower));
    ClearSelection();
  }

  private IEnumerator DestroyTower(Tower tower) {
    tower.enabled = false;
    yield return new WaitForSeconds(sellDelay);
    Destroy(tower.gameObject);
    int cost = tower.Value;
    TowerPrices[tower.Type].Pop();
    GameStateManager.Instance.Nu += cost;
  }

  public void ClearSelection() {
    SelectedTowerType = null;
    if (SelectedTower != null) {
      SelectedTower.Tile.SetUnselected();
    }
    SelectedTower = null;
    ContextPanel.Instance.SetNoContextPanel();
  }

  private Tower ConstructTower(Waypoint waypoint, TowerData data) {
    string towerDataPath = TowerManager.Instance.GetTowerPrefabPath(data.type);
    GameObject prefab = Resources.Load<GameObject>(towerDataPath);
    GameObject towerObj = Instantiate(
        prefab,
        waypoint.transform.position,
        Quaternion.identity);
    Tower tower = towerObj.GetComponent<Tower>();
    tower.SetTowerData(data);
    tower.Tile = waypoint.GetComponent<Tile>();
    tower.enabled = false;

    ActiveTowerMap.Add(waypoint.GetCoordinates(), tower);
    return tower;
  }

  private void AddTowerPrice(TowerData.Type type, int cost) {
    if (!TowerPrices.ContainsKey(type)) {
      TowerPrices.Add(type, new());
    }
    TowerPrices[type].Push(cost);
  }
}
