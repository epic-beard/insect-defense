using UnityEngine;

public class TowerSelectHandler : MonoBehaviour {

  Tower tower;

  private void Start() {
    tower = GetComponentInParent<Tower>();
  }

  private void OnMouseUp() {
    if (GameStateManager.Instance.IsMouseOverUI) return;
    GameStateManager.Instance.SetNewSelectedTower(tower);
  }
}
