#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static GameStateManager Instance;
  public static event Action GameOver;
  public static event Action<int> HealthChanged;
  // The type of tower currently selected by the user for construction.
  public static GameObject? SelectedTowerType;

  // The specific tower the user clicked on in the map.
  public Tower? SelectedTower;
  public Dictionary<Vector2Int, Tower> activeTowerMap = new();

  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;
  public int nu;
  private int health;

  private void Awake() {
    Instance = this;
  }
  public int Health {
    get { return health; }
    private set {
      health = value;
      // Update the ui health label.
      HealthChanged?.Invoke(health);
    }
  }

  private void Start () {
    Health = maxHealth;
    nu = startingNu;
  }

  private void Update() {
    // Right click should clear selected context.
    if (Input.GetMouseButton(1)) {
      ClearSelection();
      TerrariumContextUI.Instance.SetNoContextPanel();
    }
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }

  public void AddTower(Vector2Int coordinates, Tower tower) {
    activeTowerMap.Add(coordinates, tower);
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
}
