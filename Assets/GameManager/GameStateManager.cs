using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour {
  public static GameStateManager Instance;
  public static event Action GameOver; 

  [SerializeField] private GameObject gameOverScreen;
  [SerializeField] private int startingHealth = 10;
  [SerializeField] private int startingNu = 100;
  public int nu;
  public int Health { get; private set; }
  private void Awake() {
    Instance = this;
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
