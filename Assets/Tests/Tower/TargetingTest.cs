#nullable enable
using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class TargetingTest {
  readonly float TOWER_RANGE = 10.0f;
  
  //    --- Tests focusing on stubborn behavior. ---
  // Stubborn target test with a viable alternative in range. It should return oldTarget.
  [Test]
  public void StubbornWithAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, Vector3.zero);
    Enemy newTarget = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { oldTarget, newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(oldTarget));
  }
  
  // Stubborn target test without a stubborn target. Should return newTarget.
  [Test]
  public void StubbornNotPresentWithAlternative() {
    Enemy newTarget = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(newTarget));
  }
 
  // Stubborn target test with the stubborn target out of range and without a viable alternative. Should return null.
  [Test]
  public void StubbornNotInRangeWithoutAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, new Vector3(100, 0, 0));
    Enemy[] enemies = new Enemy[] { oldTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(null));
  }

  // Stubborn target test with the stubborn target out of range and with a viable alternative. Should return newTarget.
  [Test]
  public void StubbornNotInRangeWithAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, new Vector3(100, 0, 0));
    Enemy newTarget = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { oldTarget, newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(newTarget));
  }

  //    --- Test for isFlier behavior ---
  // Should return flier.
  [Test]
  public void IsFlierWithAlternative() {
    Enemy flier = CreateEnemy(0.5f, 0.0f, true, false, Vector3.zero);
    Enemy nonFlier = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { flier, nonFlier };
    Targeting targeting = CreateTargeting(Targeting.Behavior.FLIER, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, true);
    Assert.That(target, Is.EqualTo(flier));
  }

  //    --- Test for isCamo behavior. ---
  // Should return camo.
  [Test]
  public void IsCamoWithAlternative() {
    Enemy camo = CreateEnemy(0.5f, 0.0f, false, true, Vector3.zero);
    Enemy nonCamo= CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { camo, nonCamo };
    Targeting targeting = CreateTargeting(Targeting.Behavior.CAMO, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, true, false);
    Assert.That(target, Is.EqualTo(camo));
  }

  //    --- Behavior fallthrough test ---
  // Should return nonCamo, since camo is out of range.
  [Test]
  public void IsCamoOutOfRangeWithAlternative() {
    Enemy camo = CreateEnemy(0.5f, 0.0f, false, true, new Vector3(100, 0, 0));
    Enemy nonCamo = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { camo, nonCamo };
    Targeting targeting = CreateTargeting(Targeting.Behavior.CAMO, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, true, false);
    Assert.That(target, Is.EqualTo(nonCamo));
  }

  //    --- Priority tests ---

  // Test both the first and last priority. The setup for this test was nontrivial and integrating them together
  // was trivial, so this test does both.
  [Test]
  public void FirstAndLast() {
    Waypoint waypoint1 = CreateWaypoint(Vector3.right);
    Waypoint waypoint2 = CreateWaypoint(waypoint1.transform.position + Vector3.right);
    Waypoint waypoint3 = CreateWaypoint(waypoint2.transform.position + Vector3.right);
    Waypoint waypoint4 = CreateWaypoint(waypoint3.transform.position + Vector3.right);
    waypoint1.nextWaypoints = new() { waypoint2 };
    waypoint2.nextWaypoints = new() { waypoint3 };
    waypoint3.nextWaypoints = new() { waypoint4 };

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    InvokeGetDistanceToEnd(pathManager, new Waypoint[] {
      waypoint1, waypoint2, waypoint3, waypoint4 });

    Enemy first = CreateEnemy(0.0f, 0.0f, false, false, waypoint3.transform.position + Vector3.left * 0.5f);
    Enemy last = CreateEnemy(0.0f, 0.0f, false, false, waypoint2.transform.position + Vector3.left * 0.25f);
    first.NextWaypoint = waypoint3;
    last.NextWaypoint = waypoint2;
    Enemy[] enemies = new Enemy[] { first, last };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.FIRST);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(first));

    targeting.priority = Targeting.Priority.LAST;

    target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(last));
  }

  // Should return the enemy with the lowest armor value, leastArmor
  [Test]
  public void LeastArmor() {
    Enemy leastArmor = CreateEnemy(0.5f, 0.0f, false, false, Vector3.zero);
    Enemy mostArmor = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { leastArmor, mostArmor };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.LEASTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(leastArmor));
  }
  
  // Should return the enemy with the highest armor value, mostArmor.
  [Test]
  public void MostArmor() {
    Enemy leastArmor = CreateEnemy(0.5f, 0.0f, false, false, Vector3.zero);
    Enemy mostArmor = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { leastArmor, mostArmor };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(mostArmor));
  }

  // Should return the enemy with the lowest HP value, leastHP.
  [Test]
  public void LeastHP() {
    Enemy leastHP = CreateEnemy(0.0f, 1.0f, false, false, Vector3.zero);
    Enemy mostHP = CreateEnemy(0.0f, 10.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { leastHP, mostHP };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.LEASTHP);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(leastHP));
  }

  // Should return the enemy with the highest HP value, mostHP.
  [Test]
  public void MostHP() {
    Enemy leastHP = CreateEnemy(0.0f, 1.0f, false, false, Vector3.zero);
    Enemy mostHP = CreateEnemy(0.0f, 10.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { leastHP, mostHP };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.MOSTHP);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(mostHP));
  }

  //    --- No viable target ---
  // Should return null, as there are no viable targets.
  [Test]
  public void NoViableTargetTest() {
    Enemy tooFar = CreateEnemy(0.0f, 1.0f, false, false, new Vector3(100, 0, 0));
    Enemy flier = CreateEnemy(0.0f, 10.0f, false, true, Vector3.zero);
    Enemy camo = CreateEnemy(0.0f, 10.0f, true, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { tooFar, flier, camo };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(null));
  }

  //    --- Test the floating point comparison ---
  // Should return 1.
  [Test]
  public void FloatComparisonTestFirstGreater() {
    float first = 100.0f;
    float second = 10.0f;

    float result = Targeting.CompareFloats(first, second);
    Assert.That(result, Is.EqualTo(1));
  }

  // Should return -1.
  [Test]
  public void FloatComparisonTestFirstLesser() {
    float first = 1.0f;
    float second = 10.0f;

    float result = Targeting.CompareFloats(first, second);
    Assert.That(result, Is.EqualTo(-1));
  }

  // Should always return -1.
  [Test]
  public void FloatComparisonTestFirstEqualToSecond() {
    float first = 1.0f;
    float second = 1.0f;

    float result = Targeting.CompareFloats(first, second);
    Assert.That(result, Is.EqualTo(-1));

    result = Targeting.CompareFloats(second, first);
    Assert.That(result, Is.EqualTo(-1));
  }

  // Create an Enemy and set various properties for testing.
  Enemy CreateEnemy(float armor, float hp, bool isFlier, bool isCamo, Vector3 position) {
    GameObject gameObject = new();
    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.armor = armor;
    enemy.hp = hp;
    enemy.isFlier = isFlier;
    enemy.isCamo = isCamo;
    enemy.transform.position = position;
    return enemy;
  }

  // Create a Targeting script with the appropriate behavior and priority.
  Targeting CreateTargeting(Targeting.Behavior behavior, Targeting.Priority priority) {
    Targeting targeting = new() {
      behavior = behavior,
      priority = priority
    };
    return targeting;
  }

  Waypoint CreateWaypoint(Vector3 position) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    return gameObject.AddComponent<Waypoint>();
  }

  void InvokeGetDistanceToEnd(PathManager pathManager, Waypoint[] waypoints) {
    object[] args = { waypoints };
    Type[] argTypes = { typeof(Waypoint[]) };
    MethodInfo getDistanceToEnd = typeof(PathManager).GetMethod(
      "GetDistanceToEnd",
       BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    getDistanceToEnd.Invoke(pathManager, args);
  }
}
