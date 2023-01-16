#nullable enable
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Targeting : MonoBehaviour {
  public enum Priotiy {
    first,  // Enemy closest to the defensive goal.
    last,  // Enemy farthest from the defensive goal.
    mostHP,
    leaseHP,
    mostArmor,
    leastArmor,
  }

  public enum Behavior {
    stubborn,  // Don't change targets until the target dies or moves out of range.
    flier,  // Always prioritize flying enemies.
    camo,  // Always prioritize camo enemies
    none,
  }

  Priority priority;
  Behavior behavior;
  Enemy stubbornTarget;

  public Enemy? findTarget() {
    Enemy[] enemies = FindObjectsOfType<Enemy>();

    switch (behavior) {
      case Behavior.stubborn:
        if (stubbornTarget == null) {
          return stubbornTarget;
        }
        break;
      case Behavior.flier:
        // Narrow enemies to fliers only.
        break;
      case Behavior.camo:
        // Narrow enemies to camo only.
      case Behavior.none:
      default:
        break;
    }

    switch (priority) {
      case Priority.first;
      default:
        break;
    }

    // Order of operations:
    //  1. Resolve Behavior first:
    //    a. If stubborn is set, check to see if an enemy has been selected and is within range. If so, return that enemy.
    //    b. Otherwise use flier and camo to narrow possible enemy targets.
    //  2. Search through the remaining enemies according to priority.

    return new Enemy();
  }
}