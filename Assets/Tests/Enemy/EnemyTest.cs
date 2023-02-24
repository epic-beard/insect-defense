using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Networking.UnityWebRequest;

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
    Assert.That(remainigHP, Is.EqualTo(resultHP));
  }

  // Test armor tear to make sure it works as expected and with overflow.
  [Test, Sequential]
  public void ArmorTear(
      [Values(3.0f, 10.0f)] float tear,
      [Values(5.0f, 5.0f)] float armor,
      [Values(2.0f, 0.0f)] float resultArmor) {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: armor);
    float remainingArmor = enemy.TearArmor(tear);
    Assert.That(remainingArmor, Is.EqualTo(resultArmor));
  }

  // Test AddAcidStacks to make sure that it returns true when at max stacks and false when it doesn't.
  [Test, Sequential]
  public void AddAcidStackMax(
      [Values(1.0f, 10000.0f)] float acidStacks,
      [Values(false, true)] bool isMaxStacks) {
    Enemy enemy = CreateEnemy(Vector3.zero);
    
    bool currStacks = enemy.AddAcidStacks(acidStacks);
    Assert.That(currStacks, Is.EqualTo(isMaxStacks));
  }

  // Make sure ResetAcidStacks performs as expected.
  [Test]
  public void ResetAcidStacks() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    bool currStacks = enemy.AddAcidStacks(enemy.MaxAcidStacks);
    Assert.That(currStacks, Is.EqualTo(true));
    enemy.ResetAcidStacks();
    Assert.That(enemy.AcidStacks, Is.EqualTo(0.0f));
  }

  // Test AddStunTime.
  [Test]
  public void AddStunTime([Values(0.0f, 1.0f, 1000.0f)] float stunTime) {
    Enemy enemy = CreateEnemy(Vector3.zero);

    float actualStunTime = enemy.AddStunTime(stunTime);
    Assert.That(actualStunTime, Is.EqualTo(stunTime));
  }

  // Test slows.
  [Test]
  public void ApplySlowSecondSlowIsBigger() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    enemy.ApplySlow(10.0f, 10.0f);
    Assert.That(10.0f, Is.EqualTo(enemy.SlowPower));
    Assert.That(10.0f, Is.EqualTo(enemy.SlowDuration));

    enemy.ApplySlow(20.0f, 7.0f);
    Assert.That(20.0f, Is.EqualTo(enemy.SlowPower));
    Assert.That(12.0f, Is.EqualTo(enemy.SlowDuration));
  }

  [Test]
  public void ApplySlowFirstSlowIsBigger() {
    Enemy enemy = CreateEnemy(Vector3.zero);

    enemy.ApplySlow(25.0f, 10.0f);
    Assert.That(25.0f, Is.EqualTo(enemy.SlowPower));
    Assert.That(10.0f, Is.EqualTo(enemy.SlowDuration));

    enemy.ApplySlow(5.0f, 15.0f);
    Assert.That(25.0f, Is.EqualTo(enemy.SlowPower));
    Assert.That(13.0f, Is.EqualTo(enemy.SlowDuration));
  }

  #endregion

  // Confirm that GetDistanceToEnd calculates the distance correctly.
  [Test]
  public void GetDistanceToEndTest() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    enemy.PrevWaypoint = CreateWaypoint(Vector3.zero);
    enemy.NextWaypoint = CreateWaypoint(Vector3.right);

    Assert.That(enemy.GetDistanceToEnd(), Is.EqualTo(1.0f));

    enemy.NextWaypoint.DistanceToEnd += 1.0f;

    Assert.That(enemy.GetDistanceToEnd(), Is.EqualTo(2.0f));
  }

  #region TestHelperMethods

  // Create and return an enemy with optional args.
  Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 1.0f) {
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      currArmor = armor,
      currHP = hp,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.data = data;
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
