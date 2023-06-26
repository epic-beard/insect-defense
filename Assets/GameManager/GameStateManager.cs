using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static GameStateManager Instance;
  public static event Action GameOver;
  public static GameObject SelectedTower;

  public enum ContextType {
    TOWER,
    ENEMY,
    NONE,
  }

  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;
  public int nu;
  private int health;
  public int Health {
    get { return health; }
    private set {
      // Update the ui health label.
      TerrariumUI.Instance.SetHpLabelText(value);
      health = value;
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start () {
    Health = maxHealth;
    nu = startingNu;
  }

  private void Update() {
    // Right click should clear selected context.
    if (Input.GetMouseButton(1)) {
      SelectedTower = null;
      TerrariumUI.Instance.SetNoContextPanel();
    }
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }
}
