using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets {

  public static class Utilities {
    // A very careful description of this.
    public static T GetAncestor<T>(VisualElement ve) where T : VisualElement {
      while (ve != null && ve as T == null) {
        ve = ve.parent;
      }
      return ve as T;
    }
  }

  public static class MyExtensions {
    public static string TabMultiLine(this string str) {
      return "\t" + string.Join("\n\t", str.Split("\n"));
    }
  }
}
