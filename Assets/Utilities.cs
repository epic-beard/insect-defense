using UnityEngine.UIElements;

namespace Assets {

  public static class Utilities {
    // The element passed in may be nested arbitrarily deep with respect to the ancestor we
    // registered an event to. If there is no ancestor of type T, this will return null.
    public static T GetAncestor<T>(VisualElement ve) where T : VisualElement {
      while (ve != null && ve as T == null) {
        ve = ve.parent;
      }
      return ve as T;
    }

    // Sets the full player context for a specific tower.
    public static void SetSelectedTower(Tower tower) {
      TowerManager.Instance.SetNewSelectedTower(tower);
      ContextPanel.Instance.SetTowerContextPanel();
      TowerDetail.Instance.SetContextForTower(TowerManager.SelectedTower);
    }
  }

  public static class MyExtensions {
    public static string TabMultiLine(this string str) {
      return "\t" + string.Join("\n\t", str.Split("\n"));
    }
  }
}
