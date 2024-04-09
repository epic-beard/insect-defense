using UnityEngine.UIElements;
using UnityEngine;

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

    // Intended for use stripping the height out of positions.
    public static Vector2 DropY(this Vector3 vector) {
      return new Vector2(vector.x, vector.z);
    }

    public static void ToOpaqueMode(this Material material) {
      material.SetOverrideTag("RenderType", "");
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
      material.SetInt("_ZWrite", 1);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = -1;
    }

    public static void ToFadeMode(this Material material) {
      material.SetOverrideTag("RenderType", "Transparent");
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      material.SetInt("_ZWrite", 0);
      material.DisableKeyword("_ALPHATEST_ON");
      material.EnableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
  }
}
