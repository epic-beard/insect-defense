using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerTest {}

#region TowerUtils

public static class TowerUtils {
  public static void SetTargetingIndicator(this Tower tower, Transform transform) {
    typeof(Tower)
        .GetField("targetingIndicator", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(tower, transform);
  }
}

#endregion