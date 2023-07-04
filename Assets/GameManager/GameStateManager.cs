using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static GameStateManager Instance;
  public static event Action GameOver;
  // The type of tower currently selected by the user for construction.
  public static GameObject SelectedTowerType;
  // The specific tower the user clicked on in the map.
  public static Tower SelectedTower;

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
      // Update the ui health label.
      TerrariumHealthUI.Instance.SetHpLabelText(value);
      health = value;
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

  public Tower GetTower(Vector2Int coordinates) {
    return activeTowerMap[coordinates];
  }

  public void ClearSelection() {
    SelectedTowerType = null;
    SelectedTower = null;
  }
}
