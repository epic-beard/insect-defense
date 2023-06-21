using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static event Action GameOver;
  public static GameObject SelectedTower;

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

  void Start () {
    Health = maxHealth;
    nu = startingNu;
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }
}
