using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickManager : MonoBehaviour {
  private Enemy enemy;

  private void Start() {
    enemy = GetComponentInParent<Enemy>();
  }
  private void OnMouseDown() {
    Debug.Log("I've been clicked!");
    TerrariumContextUI.Instance.SetContextForEnemy(enemy);
  }
}
