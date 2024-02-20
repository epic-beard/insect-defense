using Assets;
using UnityEngine;

public class TowerSelectHandler : MonoBehaviour {

  Tower tower;

  private void Start() {
    tower = GetComponentInParent<Tower>();
  }

  private void OnMouseUp() {
    Utilities.SetSelectedTower(tower);
  }
}
