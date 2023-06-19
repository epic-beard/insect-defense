using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static event Action GameOver;
  public static GameObject SelectedTower;

  [SerializeField] private int startingHealth = 100;
  [SerializeField] private int startingNu = 100;
  public int nu;
  private int health;
  public int Health {
    get { return health; }
    private set {
      // Update the ui health label.
      TerrariumUI.instance.SetHpLabelText(value);
      health = value;
    }
  }

  void Start () {
    Health = startingHealth;
    nu = startingNu;
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }
}
