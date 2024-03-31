#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets;

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
    Vector2 towerVector2Position = towerPosition.DropY();
    // Ensure all enemies are viable targets.
    List<Enemy> targets =
        GetAllValidEnemiesInRange(enemies, towerVector2Position, towerRange, camoSight, antiAir);

    // Ensure the old target is within range of the tower.
    if (behavior == Behavior.STUBBORN
        && oldTarget != null
        && oldTarget.enabled
        && (Vector2.Distance(towerVector2Position, oldTarget.transform.position.DropY()) <= towerRange)
        && (!oldTarget.Camo || camoSight)
        && (!oldTarget.Flying || antiAir)) {
      return oldTarget;
    }

    List<Enemy> workingTargets = targets.Where(e => behaviorPredicates[behavior](e)).ToList();
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

  // Find and return all enemies within the given tower's range and given the twoer's limitations.
  public List<Enemy> GetAllValidEnemiesInRange(
      HashSet<Enemy> enemies,
      Vector2 towerPosition,
      float towerRange,
      bool camoSight,
      bool antiAir) {
    return enemies
        .Where(e => e.enabled)
        .Where(e => Vector2.Distance(towerPosition, e.transform.position.DropY()) <= towerRange)
        .Where(e => !e.Flying || antiAir)
        .Where(e => !e.Camo || camoSight)
        .ToList();
  }

  // Compare two floats in the following manner:
  //  return 1 if first is greater than second,
  //  return -1 if first is less than second.
  // Note that this algorithm will never produce a result describing equality. This is intentional.
  public static int CompareFloats(float first, float second) {
    if (Mathf.Abs(first - second) <= Mathf.Epsilon) return 0;
    return (first > second) ? 1 : -1;
  }
}
