using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickManager : MonoBehaviour {
  public static Enemy SelectedEnemy;

  private Enemy enemy;

  private void Start() {
    enemy = GetComponentInParent<Enemy>();
  }
  private void OnMouseUp() {
    if (GameStateManager.Instance.IsMouseOverUI) return;
    if (SelectedEnemy != null) {
      ContextPanel.Instance.DesbuscribeToEnemyStateBroadcast(SelectedEnemy);
    }
    SelectedEnemy = enemy;
    ContextPanel.Instance.SubscribeToEnemyStateBroadcast(enemy);
    ContextPanel.Instance.SetContextForEnemy(enemy);
    ContextPanel.Instance.SetEnemyContextPanel();
  }
}
