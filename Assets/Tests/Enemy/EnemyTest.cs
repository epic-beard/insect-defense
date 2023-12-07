using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class EnemyTest {

  #region BasicAttributeAlterationTests

  // Damage test for the enemy. Focuses on varying amounts of damage and armor piercing
  // to make sure that armor pierce cannot overflow and that damage is handled appropriately.
  [Test, Sequential]
  public void DamageEnemy(
      [Values(5.0f, 5.0f, 5.0f)] float damage,
      [Values(0.0f, 1.0f, 8.0f)] float armorPierce,
      [Values(8.0f, 7.0f, 5.0f)] float resultHP) {
    Enemy enemy = CreateEnemy(Vector3.zero, hp: 10.0f, armor: 3.0f);
    float remainigHP = enemy.DamageEnemy(damage, armorPierce);
    Assert.That(resultHP, Is.EqualTo(remainigHP));
  }

  // Test armor tear to make sure it works as expected and with overflow.
  [Test, Sequential]
  public void ArmorTear(
      [Values(3.0f, 10.0f)] float tear,
      [Values(5.0f, 5.0f)] float armor,
      [Values(2.0f, 0.0f)] float resultArmor) {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: armor);
    float remainingArmor = enemy.TearArmor(tear);
    Assert.That(resultArmor, Is.EqualTo(remainingArmor));
  }

  // Test AddAcidStacks to make sure that it returns true when at max stacks and false when it doesn't.
  [Test, Sequential]
  public void AddAcidStackMax(
      [Values(1.0f, 10000.0f)] float acidStacks,
      [Values(false, true)] bool isMaxStacks) {
    Enemy enemy = CreateEnemy(Vector3.zero);

    bool isCurrentMaxStacks = enemy.AddAcidStacks(acidStacks);
    Assert.That(isCurrentMaxStacks, Is.EqualTo(isMaxStacks));
  }

  // Make sure ResetAcidStacks performs as expected.
  [Test]
  public void ResetAcidStacks() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    bool isCurrentMaxStacks = enemy.AddAcidStacks(enemy.MaxAcidStacks);
    Assert.That(isCurrentMaxStacks, Is.EqualTo(true));
    enemy.ResetAcidStacks();
    Assert.That(0.0f, Is.EqualTo(enemy.AcidStacks));
  }

  // Test AddStunTime.
  [Test]
  public void AddStunTime([Values(0.0f, 1.0f, 1000.0f)] float stunTime) {
    Enemy enemy = CreateEnemy(Vector3.zero);

    float actualStunTime = enemy.AddStunTime(stunTime);
    Assert.That(stunTime, Is.EqualTo(actualStunTime));
  }

  // Test slows when the second slow applied is larger than the first.
  [Test]
  public void ApplySlowSecondSlowIsBigger() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    enemy.ApplySlow(10.0f, 10.0f);
    Assert.That(enemy.SlowPower, Is.EqualTo(10.0f));
    Assert.That(enemy.SlowDuration, Is.EqualTo(10.0f));

    enemy.ApplySlow(20.0f, 7.0f);
    Assert.That(enemy.SlowPower, Is.EqualTo(20.0f));
    Assert.That(enemy.SlowDuration, Is.EqualTo(12.0f));
  }

  // Test slows when the first slow spplied is larger than the second.
  [Test]
  public void ApplySlowFirstSlowIsBigger() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    enemy.ApplySlow(25.0f, 10.0f);
    Assert.That(enemy.SlowPower, Is.EqualTo(25.0f));
    Assert.That(enemy.SlowDuration, Is.EqualTo(10.0f));

    enemy.ApplySlow(5.0f, 15.0f);
    Assert.That(enemy.SlowPower, Is.EqualTo(25.0f));
    Assert.That(enemy.SlowDuration, Is.EqualTo(13.0f));
  }

  // Test the permanent slow.
  [Test]
  public void ApplyPermanentSlow() {
    Enemy enemy = CreateEnemy(Vector3.zero, speed: 10.0f);
    float permanentSlowPercent = 0.3f;
    float expectedNewSpeed = enemy.Speed * permanentSlowPercent;

    enemy.ApplyPermanentSlow(permanentSlowPercent);

    Assert.That(enemy.Speed, Is.EqualTo(expectedNewSpeed));
  }

  // Test crippling.
  [Test]
  public void ApplyCripple() {
    float enemySpeed = 10.0f;
    Enemy enemy = CreateEnemy(Vector3.zero, speed: enemySpeed);

    enemy.ApplyCripple();

    Assert.That(enemySpeed * enemy.CrippleSlow, Is.EqualTo(enemy.Speed));
  }

  [Test]
  public void ApplyCrippleToImmuneEnemy() {
    float enemySpeed = 10.0f;
    Enemy enemy = CreateEnemy(Vector3.zero, speed: enemySpeed);
    enemy.SetCrippleImmunity(true);

    enemy.ApplyCripple();

    Assert.That(enemySpeed, Is.EqualTo(enemy.Speed));
  }

  #endregion

  // Test GetClosestWaypoint in the situation where PrevWaypoint is closest.
  [Test]
  public void GetClosestWaypointPrevIsClosest() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    Waypoint prevWaypoint = CreateWaypoint(Vector3.zero);
    Waypoint nextWaypoint = CreateWaypoint(Vector3.right);
    enemy.PrevWaypoint = prevWaypoint;
    enemy.NextWaypoint = nextWaypoint;

    Waypoint closestWaypoint = enemy.GetClosestWaypoint();

    Assert.That(closestWaypoint, Is.EqualTo(prevWaypoint));
  }

  // Test GetClosestWaypoint in the situation where NextWaypoint is closest.
  [Test]
  public void GetClosestWaypointNextIsClosest() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    Waypoint prevWaypoint = CreateWaypoint(Vector3.right);
    Waypoint nextWaypoint = CreateWaypoint(Vector3.zero);
    enemy.PrevWaypoint = prevWaypoint;
    enemy.NextWaypoint = nextWaypoint;

    Waypoint closestWaypoint = enemy.GetClosestWaypoint();

    Assert.That(closestWaypoint, Is.EqualTo(nextWaypoint));
  }

  // Confirm that GetDistanceToEnd calculates the distance correctly.
  [Test]
  public void GetDistanceToEndTest() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    enemy.PrevWaypoint = CreateWaypoint(Vector3.zero);
    enemy.NextWaypoint = CreateWaypoint(Vector3.right);

    Assert.That(1.0f, Is.EqualTo(enemy.GetDistanceToEnd()));

    enemy.NextWaypoint.DistanceToEnd += 1.0f;

    Assert.That(2.0f, Is.EqualTo(enemy.GetDistanceToEnd()));
  }

  #region TestHelperMethods

  // Create and return an enemy with optional args.
  Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 1.0f,
      float speed = 0.0f) {
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      maxArmor = armor,
      maxHP = hp,
      speed = speed,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.Data = data;
    return enemy;
  }

  // Creates and returns a Waypoint.
  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  #endregion
}

#region EnemyUtils

// Extension methods to hold reflection-based calls to access private fields, properties, or methods
// of Enemy.
public static class EnemyUtils {
  public static void SetCrippleImmunity(this Enemy enemy,  bool isImmunity) {
    typeof(Enemy)
        .GetProperty("CrippleImmunity")
        .SetValue(enemy, isImmunity);
  }
  public static void SetTarget(this Enemy enemy, Transform target) {
    typeof(Enemy)
        .GetField("target", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(enemy, target);
  }
}

#endregion
