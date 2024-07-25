using Assets;
using UnityEngine;

// This class manages mouse interaction with the game map only. It does not touch the overlaid UI.
public class MouseManager : MonoBehaviour {
  public static Enemy SelectedEnemy;

  void Update() {
    // Only do stuff if we aren't overlapping the UI.
    if (GameStateManager.Instance.IsMouseOverUI) return;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
      GameObject hitObject = hitInfo.transform.root.gameObject;
      if (hitObject == null) {
        Debug.Log("[ERROR] Mouse raycast found null Game Object.");
      }

      // Do mouseover stuff.
      Tile tile = null;
      if (hitObject.name.Equals("Map")) {
        tile = hitInfo.transform.gameObject.GetComponentInParent<Tile>();
        if (tile.isTowerPlaceable && !TowerManager.Instance.HasTower(tile.GetCoordinates())) {
          TowerManager.Instance.SetPreviewTowerPosition(tile);
        } else {
          TowerManager.Instance.ClearTowerPreview();
        }
      }

      // When the left mouse button is released.
      if (Input.GetMouseButtonUp(0)) {
        // The player selected an area of the map instead of an enemy or tower. Check to see if building
        // a tower is appropriate.
        if (GameStateManager.Instance.IsMouseOverUI) return;
        if (hitObject.name.Equals("Map")) {
          tile.BuildTowerIfPossible();
          TowerManager.Instance.ClearSelection();
          if (TowerManager.Instance.ActiveTowerMap.ContainsKey(tile.GetCoordinates())) {
            Tower tower = TowerManager.Instance.ActiveTowerMap[tile.GetCoordinates()];
            SetSelectedTower(tower);
          }
        } else {
          Enemy enemy = hitObject.GetComponent<Enemy>();
          if (enemy != null) {
            SetSelectedEnemy(enemy);
          }

          Tower tower = hitObject.GetComponent<Tower>();
          if (tower != null && TowerManager.Instance.ActiveTowerMap.ContainsKey(tower.Tile.GetCoordinates())) {
            SetSelectedTower(tower);
          }
        }
      }
    }
  }

  private void SetSelectedEnemy(Enemy enemy) {
    if (SelectedEnemy != null) {
      EnemyDetail.Instance.DesbuscribeToEnemyStateBroadcast(SelectedEnemy);
    }
    SelectedEnemy = enemy;
    EnemyDetail.Instance.SubscribeToEnemyStateBroadcast(enemy);
    EnemyDetail.Instance.SetContextForEnemy(enemy);
    ContextPanel.Instance.SetEnemyContextPanel();
    TowerManager.Instance.ClearTowerSelectionAndType();
  }

  private void SetSelectedTower(Tower tower) {
    Utilities.SetSelectedTower(tower);
  }
}
