using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickManager : MonoBehaviour {
  public static Enemy SelectedEnemy;

  private Enemy enemy;

  private void Start() {
    enemy = GetComponentInParent<Enemy>();
  }
  private void OnMouseDown() {
    if (SelectedEnemy != null) {
      TerrariumContextUI.Instance.DesbuscribeToEnemyStateBroadcast(SelectedEnemy);
    }
    SelectedEnemy = enemy;
    TerrariumContextUI.Instance.SubscribeToEnemyStateBroadcast(enemy);
    TerrariumContextUI.Instance.SetContextForEnemy(enemy);
    TerrariumContextUI.Instance.SetEnemyContextPanel();
  }
}
