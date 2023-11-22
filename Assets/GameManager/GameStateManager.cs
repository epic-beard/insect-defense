#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class is a catch-all for gamestate management that doesn't have a better home elsewhere.
public class GameStateManager : MonoBehaviour {
#pragma warning disable 8618
  public static GameStateManager Instance;
#pragma warning restore 8618
  public static event Action GameOver = delegate { };
  public static event Action<int> HealthChanged = delegate { };
  // The type of tower currently selected by the user for construction.
  public static GameObject? SelectedTowerType;

  // The specific tower the user clicked on in the map.
  public Tower? SelectedTower;
  public Dictionary<Vector2Int, Tower> activeTowerMap = new();

  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;
  private int nu;
  public int Nu { 
    get {
      return nu;
    } 
    set {
      nu = value;
      TerrariumTowerSelectionUI.Instance?.UpdateAffordableTowers();
      TerrariumBottomUI.Instance?.UpdateNu();

      if (SelectedTowerType != null) {
        Tower tower = SelectedTowerType.GetComponent<Tower>();
        if (Nu < GetTowerCost(tower.TowerType, tower.Cost)) {
          SelectedTowerType = null;
        } 
      }
    }
  }
  private int health;
  private Dictionary<TowerData.Type, int> towerCounts = new();
  private readonly float towerScalingFactor = 1.2f;

  private void Awake() {
    Instance = this;
    Nu = startingNu;
  }

  public int Health {
    get { return health; }
    private set {
      health = value;
      // Update the ui health label.
      HealthChanged?.Invoke(health);
    }
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

    TowerData.Type towerType = SelectedTowerType.GetComponent<Tower>().TowerType;
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

    AddTower(waypoint.GetCoordinates(), tower);

    Nu -= cost;
    return true;
  }

  private void AddTower(Vector2Int coordinates, Tower tower) {
    activeTowerMap.Add(coordinates, tower);
    if (!towerCounts.ContainsKey(tower.TowerType)) {
      towerCounts[tower.TowerType] = 0;
    }
    towerCounts[tower.TowerType]++;
  }

  public List<Tower> GetTowersInRange(float range, Vector3 pos) {
    return activeTowerMap.Values.Where(
      (tower) => Vector3.Distance(pos, tower.transform.position) <= range).ToList();
  }

  public Tower GetTower(Vector2Int coordinates) {
    return activeTowerMap[coordinates];
  }

  public void ClearSelection() {
    SelectedTowerType = null;
    SelectedTower = null;
  }

  public int GetTowerCost(TowerData.Type type, float cost) {
    if (!towerCounts.ContainsKey(type)) {
      towerCounts[type] = 0;
    }

    return Mathf.RoundToInt(
      cost * Mathf.Pow(towerScalingFactor, towerCounts[type]));
  }
}
