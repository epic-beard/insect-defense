using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Networking.UnityWebRequest;

public class EnemyTest {

  //    --- Tests for basic attribute alteration ---

  // No damage overflow test for the enemy. Focuses on varying amounts of damage and armor piercing
  // to make sure that armor pierce cannot overflow and that damage is handled appropriately.
  [Test, Sequential]
  public void DamageEnemyNoHpOverflow(
      [Values(5.0f, 5.0f, 5.0f)] float damage,
      [Values(0.0f, 1.0f, 8.0f)] float armorPierce,
      [Values(8.0f, 7.0f, 5.0f)] float resultHP) {
    Enemy enemy = CreateEnemy(Vector3.zero, hp: 10.0f, armor: 3.0f);
    float remainigHP = enemy.DamageEnemy(damage, armorPierce);
    Assert.That(remainigHP, Is.EqualTo(resultHP));
  }

  // Test damage overflow and make sure an enemy does not drop below 0 hp.
  [Test]
  public void DamageEnemyHpOverflow() {
    Enemy enemy = CreateEnemy(Vector3.zero, hp: 10.0f);
    float remainigHP = enemy.DamageEnemy(50.0f, 0.0f);
    Assert.That(remainigHP, Is.EqualTo(0.0f));
  }

  Enemy CreateEnemy(
      Vector3 position,
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
      speed = speed,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.data = data;
    return enemy;
  }
}
