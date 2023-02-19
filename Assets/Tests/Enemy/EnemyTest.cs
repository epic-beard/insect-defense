using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Networking.UnityWebRequest;

public class EnemyTest {

  //    --- Tests for basic attribute alteration ---

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
    Assert.That(remainingArmor, Is.EqualTo(remainingArmor));
  }

  // Test AddAcidStacks to make sure that it returns true when at max stacks and false when it doesn't.
  [Test]
  public void AddAcidStackMax() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    
    bool maxStacks = enemy.AddAcidStacks(1.0f);
    Assert.That(maxStacks, Is.EqualTo(false));
    
    maxStacks = enemy.AddAcidStacks(10000.0f);
    Assert.That(maxStacks, Is.EqualTo(true));
  }

  // Confirm that GetDistanceToEnd calculates the distance correctly.
  [Test]
  public void GetDistanceToEndTest() {
    Enemy enemy = CreateEnemy(Vector3.zero);
    enemy.PrevWaypoint = GetWaypoint(Vector3.zero);
    enemy.NextWaypoint = GetWaypoint(Vector3.right);

    float distanceToEnd = enemy.GetDistanceToEnd();
    Assert.That(distanceToEnd, Is.EqualTo(1.0f));

    enemy.NextWaypoint.DistanceToEnd += 1.0f;

    distanceToEnd = enemy.GetDistanceToEnd();
    Assert.That(distanceToEnd, Is.EqualTo(2.0f));
  }

  // Create and return an enemy with optional args.
  Enemy CreateEnemy(
      Vector3 position,
      EnemyData.Size size = EnemyData.Size.NORMAL,
      float acidStacks = 0.0f,
      float armor = 0.0f,
      float hp = 1.0f,
      float speed = 1.0f) {
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      acidStacks = acidStacks,
      currArmor = armor,
      currHP = hp,
      size = size,
      speed = speed,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.data = data;
    return enemy;
  }

  // Creates and returns a Waypoint.
  Waypoint GetWaypoint(Vector3 position) {
    GameObject gameObject = new();
    Waypoint waypoint = gameObject.AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }
}
