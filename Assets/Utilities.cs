using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets {
  public static class MyExtensions {
    public static string TabMultiLine(this string str) {
      return "\t" + string.Join("\n\t", str.Split("\n"));
    }
  }
}
