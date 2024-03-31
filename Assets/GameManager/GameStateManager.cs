#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class is a catch-all for gamestate management that doesn't have a better home elsewhere.
public class GameStateManager : MonoBehaviour {
  [SerializeField] private int maxHealth = 100;
  [SerializeField] private int startingNu = 100;

#pragma warning disable 8618
  public static GameStateManager Instance;
#pragma warning restore 8618

  public static event Action GameOver = delegate { };
  public static event Action<int> HealthChanged = delegate { };
  public static event Action<int> OnNuChanged = delegate { };

  public bool IsMouseOverUI = false;

  private int nu;
  public int Nu { 
    get {
      return nu;
    } 
    set {
      nu = value;
      OnNuChanged?.Invoke(nu);
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

  public bool CanClickGameScreen() {
    return !IsMouseOverUI || !SettingsScreen.Instance.IsOpen();
  }
}
