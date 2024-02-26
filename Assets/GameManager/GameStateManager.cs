#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class is a catch-all for gamestate management that doesn't have a better home elsewhere.
public class GameStateManager : MonoBehaviour {
  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;
  private readonly float towerScalingFactor = 1.2f;

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
    TowerData data = TowerDataManager.Instance.GetTowerData(towerType);
    
    return BuildTower(waypoint, SelectedTowerType, data);
  }

  public bool BuildTower(Waypoint waypoint, GameObject prefab, TowerData data) {
    int cost = GetTowerCost(data.type, data.cost);
    if (Nu < cost) { return false; }
    GameObject towerObj = Instantiate(
      prefab,
      waypoint.transform.position,
      Quaternion.identity);
    Tower tower = towerObj.GetComponent<Tower>();
    tower.SetTowerData(data);
    tower.Tile = waypoint.GetComponent<Tile>();

    ActiveTowerMap.Add(waypoint.GetCoordinates(), tower);
    if (!TowerPrices.ContainsKey(data.type)) {
      TowerPrices.Add(data.type, new());
    }
    TowerPrices[data.type].Push(cost);

    Nu -= cost;
    return true;
  }

  public List<Tower> GetTowersInRange(float range, Vector3 pos) {
    return ActiveTowerMap.Values.Where(
      (tower) => Vector3.Distance(pos, tower.transform.position) <= range).ToList();
  }

  public Tower GetTower(Vector2Int coordinates) {
    return ActiveTowerMap[coordinates];
  }

  // Refund the tower's full cost (including upgrades) and remove the tower from the map.
  public void RefundSelectedTower() {
    if (SelectedTower == null) return;
    int cost = SelectedTower.Value;
    TowerPrices[SelectedTower.Type].Pop();
    Nu += cost;
    ActiveTowerMap.Remove(SelectedTower.Tile.GetCoordinates());
    Destroy(SelectedTower.gameObject);
    ClearSelection();
  }

  public void ClearSelection() {
    SelectedTowerType = null;
    if (SelectedTower != null) {
      SelectedTower.Tile.SetUnselected();
    }
    SelectedTower = null;
    TerrariumContextUI.Instance.SetNoContextPanel();
  }

  public int GetTowerCost(TowerData.Type type, float cost) {
    if (!TowerPrices.ContainsKey(type) || TowerPrices[type].Count == 0) {
      return Mathf.RoundToInt(cost);
    }

    return Mathf.RoundToInt(GetPreviousTowerCost(type) * towerScalingFactor);
  }

  public int GetPreviousTowerCost(TowerData.Type type) {
    if (!TowerPrices.ContainsKey(type) || TowerPrices[type].Count == 0) {
      return Mathf.RoundToInt(TowerDataManager.Instance.GetTowerData(type).cost);
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
