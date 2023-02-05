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
    Enemy oldTarget = CreateEnemy(Vector3.zero, armor: 0.5f);
    Enemy newTarget = CreateEnemy(Vector3.zero, armor: 1.0f); ;
    Enemy[] enemies = new Enemy[] { oldTarget, newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(oldTarget));
  }
  
  // Stubborn target test without a stubborn target. Should return newTarget.
  [Test]
  public void StubbornNotPresentWithAlternative() {
    Enemy newTarget = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(newTarget));
  }
 
  // Stubborn target test with the stubborn target out of range and without a viable alternative. Should return null.
  [Test]
  public void StubbornNotInRangeWithoutAlternative() {
    Enemy oldTarget = CreateEnemy(new Vector3(100, 0, 0), armor: 0.5f);
    Enemy[] enemies = new Enemy[] { oldTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(null));
  }

  // Stubborn target test with the stubborn target out of range and with a viable alternative. Should return newTarget.
  [Test]
  public void StubbornNotInRangeWithAlternative() {
    Enemy oldTarget = CreateEnemy(new Vector3(100, 0, 0), armor: 0.5f);
    Enemy newTarget = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { oldTarget, newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(newTarget));
  }

  //    --- Test for isFlier behavior ---

  // Should return flier.
  [Test]
  public void IsFlierWithAlternative() {
    Enemy flier = CreateEnemy(Vector3.zero, armor: 0.5f, properties: EnemyData.Properties.FLYING);
    Enemy nonFlier = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { flier, nonFlier };
    Targeting targeting = CreateTargeting(Targeting.Behavior.FLIER, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, true);
    Assert.That(target, Is.EqualTo(flier));
  }

  //    --- Test for isCamo behavior. ---

  // Should return camo.
  [Test]
  public void IsCamoWithAlternative() {
    Enemy camo = CreateEnemy(Vector3.zero, armor: 0.5f, properties: EnemyData.Properties.CAMO);
    Enemy nonCamo= CreateEnemy(Vector3.zero, armor: 0.5f);
    Enemy[] enemies = new Enemy[] { camo, nonCamo };
    Targeting targeting = CreateTargeting(Targeting.Behavior.CAMO, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, true, false);
    Assert.That(target, Is.EqualTo(camo));
  }

  //    --- Behavior fallthrough test ---

  // Should return nonCamo, since camo is out of range.
  [Test]
  public void IsCamoOutOfRangeWithAlternative() {
    Enemy camo = CreateEnemy(new Vector3(100, 0, 0), armor: 0.5f, properties: EnemyData.Properties.CAMO);
    Enemy nonCamo = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { camo, nonCamo };
    Targeting targeting = CreateTargeting(Targeting.Behavior.CAMO, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, true, false);
    Assert.That(target, Is.EqualTo(nonCamo));
  }

  //    --- Priority tests ---

  // Test both the first and last priority. The setup for this test was nontrivial and integrating them together
  // was trivial, so this test does both.
  [Test]
  public void FirstAndLast([Values(Targeting.Priority.FIRST, Targeting.Priority.LAST)] Targeting.Priority priority) {
    Waypoint waypoint1 = CreateWaypoint(Vector3.right);
    Waypoint waypoint2 = CreateWaypoint(waypoint1.transform.position + Vector3.right);
    Waypoint waypoint3 = CreateWaypoint(waypoint2.transform.position + Vector3.right);
    Waypoint waypoint4 = CreateWaypoint(waypoint3.transform.position + Vector3.right);
    waypoint1.nextWaypoints.Add(waypoint2);
    waypoint2.nextWaypoints.Add(waypoint3);
    waypoint3.nextWaypoints.Add(waypoint4);

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    InvokeGetDistanceToEnd(pathManager, new Waypoint[] {
      waypoint1, waypoint2, waypoint3, waypoint4 });

    Enemy first = CreateEnemy(waypoint3.transform.position + Vector3.left * 0.5f);
    Enemy last = CreateEnemy(waypoint2.transform.position + Vector3.left * 0.25f);
    first.NextWaypoint = waypoint3;
    last.NextWaypoint = waypoint2;
    Enemy[] enemies = new Enemy[] { first, last };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, priority);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);

    Enemy expected = priority == Targeting.Priority.FIRST ? first : last;
    Assert.That(target, Is.EqualTo(expected));
  }

  // Should return the enemy with the lowest armor value, leastArmor
  [Test]
  public void LeastArmor() {
    Enemy leastArmor = CreateEnemy(Vector3.zero.normalized, armor: 0.5f);
    Enemy mostArmor = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { leastArmor, mostArmor };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.LEASTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(leastArmor));
  }
  
  // Should return the enemy with the highest armor value, mostArmor.
  [Test]
  public void MostArmor() {
    Enemy leastArmor = CreateEnemy(Vector3.zero, armor: 0.5f);
    Enemy mostArmor = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { leastArmor, mostArmor };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.MOSTARMOR);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(mostArmor));
  }

  // Should return the enemy with the lowest HP value, leastHP.
  [Test]
  public void LeastHP() {
    Enemy leastHP = CreateEnemy(Vector3.zero, armor: 0.5f);
    Enemy mostHP = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy[] enemies = new Enemy[] { leastHP, mostHP };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.LEASTHP);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(leastHP));
  }

  // Should return the enemy with the highest HP value, mostHP.
  [Test]
  public void MostHP() {
    Enemy leastHP = CreateEnemy(Vector3.zero, hp: 1.0f);
    Enemy mostHP = CreateEnemy(Vector3.zero, hp: 10.0f);
    Enemy[] enemies = new Enemy[] { leastHP, mostHP };
    Targeting targeting = CreateTargeting(Targeting.Behavior.NONE, Targeting.Priority.MOSTHP);

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, TOWER_RANGE, false, false);
    Assert.That(target, Is.EqualTo(mostHP));
  }

  //    --- No viable target ---

  // Should return null, as there are no viable targets.
  [Test]
  public void NoViableTargetTest() {
    Enemy tooFar = CreateEnemy(new Vector3(100, 0, 0));
    Enemy flier = CreateEnemy(Vector3.zero, properties: EnemyData.Properties.FLYING);
    Enemy camo = CreateEnemy(Vector3.zero, properties: EnemyData.Properties.CAMO);
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
  Enemy CreateEnemy(Vector3 position, float armor = 0.0f, float hp = 0.0f, EnemyData.Properties properties = EnemyData.Properties.NONE) {
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      maxArmor = armor,
      maxHP = hp,
      properties = properties,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.data = data;
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
