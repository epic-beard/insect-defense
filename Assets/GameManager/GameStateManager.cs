using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  public static event Action GameOver;
  public static GameObject SelectedTower;

  [SerializeField] private int startingHealth = 10;
  [SerializeField] private int startingNu = 100;
  public int nu;
  public int Health { get; private set; }

  void Start () {
    Health = startingHealth;
    nu = startingNu;
  }

  public void DealDamage(int damage) {
    Health -= damage;
    if (Health <= 0) GameOver?.Invoke();
  }
}
