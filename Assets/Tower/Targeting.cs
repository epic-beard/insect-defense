#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Targeting {

  public enum Behavior {
    ALL,
    CAMO,  // Always prioritize camo enemies
    FLIER,  // Always prioritize flying enemies.
    STUBBORN,  // Don't change targets until the target dies or moves out of range.
  }

  public enum Priority {
    FIRST,  // Enemy closest to the defensive goal.
    LAST,  // Enemy farthest from the defensive goal.
    LEAST_ARMOR,
    LEAST_HP,
    MOST_ARMOR,
    MOST_HP,
  }

  public delegate bool BehaviorPredicate(Enemy enemy);
  readonly Dictionary<Behavior, BehaviorPredicate> behaviorPredicates = new() {
    { Behavior.ALL, (enemy => true) },
    { Behavior.CAMO, (enemy) => enemy.Camo },
    { Behavior.FLIER, (enemy) => enemy.Flying },
    { Behavior.STUBBORN, (enemy) => true },  // This has an entry in case of stubborn fail-through.
  };
  readonly Dictionary<Priority, Comparison<Enemy>> priorityPredicates = new() {
    { Priority.FIRST, (enemy1, enemy2) => CompareFloats(enemy1.GetDistanceToEnd(), enemy2.GetDistanceToEnd()) },
    { Priority.LAST, (enemy1, enemy2) => CompareFloats(enemy2.GetDistanceToEnd(), enemy1.GetDistanceToEnd()) },
    { Priority.LEAST_ARMOR, (enemy1, enemy2) => CompareFloats(enemy1.Armor, enemy2.Armor) },
    { Priority.LEAST_HP, (enemy1, enemy2) => CompareFloats(enemy1.HP, enemy2.HP) },
    { Priority.MOST_ARMOR, (enemy1, enemy2) => CompareFloats(enemy2.Armor, enemy1.Armor) },
    { Priority.MOST_HP, (enemy1, enemy2) => CompareFloats(enemy2.HP, enemy1.HP) },
  };

  public Priority priority;
  public Behavior behavior;

  // Find and return a target. Searches through available enemies for appropriate targets. This should ignore
  // inappropriate targets (a tower that cannot hit airborne targets should not get an airborne enemy), and
  // will only examine targets within the tower's given range. It also takes stubborn targeting into account.
  public Enemy? FindTarget(
      Enemy? oldTarget,
      HashSet<Enemy> enemies,
      Vector3 towerPosition,
      float towerRange,
      bool camoSight,
      bool antiAir) {
    // Ensure all enemies are viable targets.
    List<Enemy> targets = enemies
        .Where(e => e.enabled)
        .Where(e => Vector3.Distance(towerPosition, e.transform.position) <= towerRange)
        .Where(e => !e.Flying || antiAir)
        .Where(e => !e.Camo || camoSight)
        .ToList();
    // This is a working copy to avoid extra work later in the case of no enemies found with the behavior filter.
    List<Enemy> workingTargets = new();

    // Ensure the old target is within range of the tower.
    if (behavior == Behavior.STUBBORN
        && oldTarget != null
        && (Vector3.Distance(towerPosition, oldTarget.transform.position) < towerRange)
        && (!oldTarget.Camo || camoSight)
        && (!oldTarget.Flying || antiAir)) {
      return oldTarget;
    }

    // Apply the behavior predicate to all potential targets.
    workingTargets = targets.Where(e => behaviorPredicates[behavior](e)).ToList();
    // Only get the unfiltered list if we have used filters and there are no filtered results.
    if (workingTargets.Count == 0 && behavior != Behavior.ALL) {
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
    return (first > second) ? 1 : -1;
  }
}
