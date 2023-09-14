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

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }

  // Add a tower to the active tower tracking map.
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
