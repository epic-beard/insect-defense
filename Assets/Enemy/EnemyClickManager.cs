using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickManager : MonoBehaviour {
  private Enemy enemy;

  private void Start() {
    enemy = GetComponentInParent<Enemy>();
  }
  private void OnMouseDown() {
    GameStateManager.Instance.SelectEnemy(enemy);
    TerrariumContextUI.Instance.SetContextForEnemy(enemy);
    TerrariumContextUI.Instance.SetEnemyContextPanel();
  }
}
