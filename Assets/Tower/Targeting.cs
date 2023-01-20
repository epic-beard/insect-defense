#nullable enable
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System;
using System.Runtime.ExceptionServices;

public class Targeting {

  public enum Behavior {
    NONE,
    CAMO,  // Always prioritize camo enemies
    FLIER,  // Always prioritize flying enemies.
    STUBBORN,  // Don't change targets until the target dies or moves out of range.
  }

  public enum Priority {
    FIRST,  // Enemy closest to the defensive goal.
    LAST,  // Enemy farthest from the defensive goal.
    LEASTARMOR,
    LEASTHP,
    MOSTARMOR,
    MOSTHP,
  }

  public delegate bool BehaviorPredicate(Enemy enemy);
  readonly Dictionary<Behavior, BehaviorPredicate> behaviorPredicates = new() {
    { Behavior.NONE, (enemy => true) },
    { Behavior.CAMO, (enemy) => enemy.IsCamo() },
    { Behavior.FLIER, (enemy) => enemy.IsFlier() },
    { Behavior.STUBBORN, (enemy) => true },  // This has an entry in case of stubborn fail-through.
  };
  readonly Dictionary<Priority, Comparison<Enemy>> priorityPredicates = new() {
    // TODO - These two need additional infrastructure to do properly.
    { Priority.FIRST, (enemy1, enemy2) => 0 },
    { Priority.LAST, (enemy1, enemy2) => 0 },
    { Priority.LEASTARMOR, (enemy1, enemy2) => CompareFloats(enemy2.GetArmor(), enemy1.GetArmor()) },
    { Priority.LEASTHP, (enemy1, enemy2) => CompareFloats(enemy2.GetHP(), enemy1.GetHP()) },
    { Priority.MOSTARMOR, (enemy1, enemy2) => CompareFloats(enemy1.GetArmor(), enemy2.GetArmor()) },
    { Priority.MOSTHP, (enemy1, enemy2) => CompareFloats(enemy1.GetHP(), enemy2.GetHP()) },
  };

  public Priority priority;
  public Behavior behavior;

  public Enemy? FindTarget(Enemy? oldTarget, Enemy[] enemies, Vector3 towerPosition, float towerRange, bool camoSight, bool antiAir) {
    // Ensure all enemies are viable targets.
    List<Enemy> targets = enemies.ToList()
        .Where(e => Vector3.Distance(towerPosition, e.getPosition()) < towerRange)
        .Where(e => e.IsFlier() ? antiAir : true)
        .Where(e => e.IsCamo() ? camoSight : true)
        .ToList();
    // This is a working copy to avoid extra work later in the case of no enemies found with the behavior filter.
    List<Enemy> workingTargets = new();

    // Ensure the old target is within range of the tower.
    if (behavior == Behavior.STUBBORN
        && oldTarget != null
        && (Vector3.Distance(towerPosition, oldTarget.getPosition()) < towerRange)
        && (oldTarget.IsCamo() ? camoSight : true)
        && (oldTarget.IsFlier() ? antiAir : true)) {
      return oldTarget;
    }

    // Apply the behavior predicate to all potential targets.
    workingTargets = targets.Where(e => behaviorPredicates[behavior](e)).ToList();
    // Only get the unfiltered list if we have used filters and there are no filtered results.
    if (workingTargets.Count == 0 && behavior != Behavior.NONE) {
      workingTargets = targets;
    }

    if (workingTargets.Count == 0) {
      return null;
    }

    // Sort the working list of targets according to the appropriate targeting priority.
    workingTargets.Sort(priorityPredicates[priority]);
    return workingTargets[0];
  }

  // Compare two floats in the following manner:
  //  return 1 if first is greater than second,
  //  return -1 if first is less than second.
  // Note that this algorithm will never produce a result describing equality. This is intentional.
  public static int CompareFloats(float first, float second) {
    return (first > second) ? -1 : 1;
  }
}
