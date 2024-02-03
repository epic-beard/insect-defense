using UnityEngine;

public class TowerSelectHandler : MonoBehaviour {

  Tower tower;

  private void Start() {
    tower = GetComponentInParent<Tower>();
  }

  private void OnMouseUp() {
    GameStateManager.Instance.SetNewSelectedTower(tower);
  }
}
