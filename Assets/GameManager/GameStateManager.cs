#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class is a catch-all for gamestate management that doesn't have a better home elsewhere.
public class GameStateManager : MonoBehaviour {
  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;
  private readonly float towerScalingFactor = 1.2f;
  private readonly float buildDelay = 2.0f;
  private readonly float sellDelay = 2.0f;

#pragma warning disable 8618
  public static GameStateManager Instance;
#pragma warning restore 8618

  public static event Action GameOver = delegate { };
  public static event Action<int> HealthChanged = delegate { };
  public static event Action<int> OnNuChanged = delegate { };

  // The type of tower currently selected by the user for construction.
  public static GameObject? SelectedTowerType;
  // The specific tower the user clicked on in the map.
  public Tower? SelectedTower;
  // Each tower, keyed by its waypoint coordinates.
  public Dictionary<Vector2Int, Tower> ActiveTowerMap = new();
  public Dictionary<TowerData.Type, Stack<int>> TowerPrices = new();

  public bool IsMouseOverUI = false;

  private int nu;
  public int Nu { 
    get {
      return nu;
    } 
    set {
      nu = value;
      OnNuChanged?.Invoke(nu);
      //TODO:nnewsom this isn't working.
      // If we can no longer afford the selected tower, deselect it.
      if (SelectedTowerType != null) {
        Tower tower = SelectedTowerType.GetComponent<Tower>();
        if (Nu < GetTowerCost(tower.Type, tower.Cost)) {
          SelectedTowerType = null;
        } 
      }
    }
  }

  private int health;
  public int Health {
    get { return health; }
    private set {
      health = value;
      // Update the ui health label.
      HealthChanged?.Invoke(health);
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    Health = maxHealth;
    Nu = startingNu;
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }

  // Builds a tower at the given waypoint, after checking that the player has
  // enough Nu.  Returns true if the tower was constructed.
  public bool BuildTower(Waypoint waypoint) {
    if (SelectedTowerType == null) { return false; }

    TowerData.Type towerType = SelectedTowerType.GetComponent<Tower>().Type;
    TowerData data = TowerManager.Instance.GetTowerData(towerType);
    int cost = GetTowerCost(data.type, data.cost);
    if (Nu < cost) { return false; }
    //Nu -= cost;
    //AddTowerPrice(data.type, cost);
    StartCoroutine(BuildTower(waypoint, data));
    return true;
  }

  private void AddTowerPrice(TowerData.Type type, int cost) {
    if (!TowerPrices.ContainsKey(type)) {
      TowerPrices.Add(type, new());
    }
    TowerPrices[type].Push(cost);
  }

  public IEnumerator BuildTower(Waypoint waypoint, TowerData data) {
    int cost = GetTowerCost(data.type, data.cost);
    Nu -= cost;
    AddTowerPrice(data.type, cost);

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
    yield return new WaitForSeconds(buildDelay);
    tower.enabled = true;
  }

  public List<Tower> GetTowersInRange(float range, Vector3 pos) {
    return ActiveTowerMap.Values.Where(
      (tower) => Vector3.Distance(pos, tower.transform.position) <= range).ToList();
  }

  public Tower GetTower(Vector2Int coordinates) {
    return ActiveTowerMap[coordinates];
  }

  public bool HasTower(Vector2Int coordinates) {
    return ActiveTowerMap.ContainsKey(coordinates);
  }

  // Refund the tower's full cost (including upgrades) and remove the tower from the map.
  public void RefundSelectedTower() {
    if (SelectedTower == null) return;
    ActiveTowerMap.Remove(SelectedTower.Tile.GetCoordinates());
    StartCoroutine(DestroyTower(SelectedTower));
    ClearSelection();
  }

  private  IEnumerator DestroyTower(Tower tower) {
    tower.enabled = false;
    yield return new WaitForSeconds(sellDelay);
    Destroy(tower.gameObject);
    int cost = tower.Value;
    TowerPrices[tower.Type].Pop();
    Nu += cost;
  }

  public void ClearSelection() {
    SelectedTowerType = null;
    if (SelectedTower != null) {
      SelectedTower.Tile.SetUnselected();
    }
    SelectedTower = null;
    ContextPanel.Instance.SetNoContextPanel();
  }

  public int GetTowerCost(TowerData.Type type, float cost) {
    if (!TowerPrices.ContainsKey(type) || TowerPrices[type].Count == 0) {
      return Mathf.RoundToInt(cost);
    }

    return Mathf.RoundToInt(GetPreviousTowerCost(type) * towerScalingFactor);
  }

  public int GetPreviousTowerCost(TowerData.Type type) {
    if (!TowerPrices.ContainsKey(type) || TowerPrices[type].Count == 0) {
      return Mathf.RoundToInt(TowerManager.Instance.GetTowerData(type).cost);
    }

    return TowerPrices[type].Peek();
  }

  // Set a new tower as the selected tower. Adjust the tile color to indicate selection as necessary.
  public void SetNewSelectedTower(Tower tower) {
    if (SelectedTower != null) {
      SelectedTower.Tile.SetUnselected();
    }
    tower.Tile.SetSelected();
    SelectedTower = tower;
  }

  public bool CanClickGameScreen() {
    return !IsMouseOverUI || !SettingsScreen.Instance.IsOpen();
  }
}
