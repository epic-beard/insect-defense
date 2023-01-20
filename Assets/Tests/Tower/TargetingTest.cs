#nullable enable
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TargetingTest {

  // Tests focusing on stubborn behavior.
  [Test]
  public void StubbornWithAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] {
      oldTarget,
      CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero)
    };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);
    float towerRange = 10.0f;

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, towerRange);
    Assert.That(target, Is.EqualTo(oldTarget));
  }

  public void StubbornNotPresentWithAlternative() {
    Enemy newTarget = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);
    float towerRange = 10.0f;

    Enemy? target = targeting.FindTarget(null, enemies, Vector3.right, towerRange);
    Assert.That(target, Is.EqualTo(newTarget));
  }

  public void StubbornNotInRangeWithoutAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, new Vector3(100, 0, 0));
    Enemy[] enemies = new Enemy[] { oldTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);
    float towerRange = 10.0f;

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, towerRange);
    Assert.That(target, Is.EqualTo(null));
  }

  public void StubbornNotInRangeWithAlternative() {
    Enemy oldTarget = CreateEnemy(0.5f, 0.0f, false, false, new Vector3(100, 0, 0));
    Enemy newTarget = CreateEnemy(1.0f, 0.0f, false, false, Vector3.zero);
    Enemy[] enemies = new Enemy[] { oldTarget, newTarget };
    Targeting targeting = CreateTargeting(Targeting.Behavior.STUBBORN, Targeting.Priority.MOSTARMOR);
    float towerRange = 10.0f;

    Enemy? target = targeting.FindTarget(oldTarget, enemies, Vector3.right, towerRange);
    Assert.That(target, Is.EqualTo(newTarget));
  }

  // Tests focusing on non-stubborn behaviors and priority testing.
  // Planned:
  // - isFlying
  // - isCamo
  // - default to non-behavior target (non-flier for isFlying is true)
  // - 1 for each implemented priority
  // - 1 for no viable targets

  Enemy CreateEnemy(float armor, float hp, bool isFlying, bool isCamo, Vector3 position) {
    GameObject gameObject = new();
    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.armor = armor;
    enemy.hp = hp;
    enemy.isFlying = isFlying;
    enemy.isCamo = isCamo;
    enemy.position = position;
    return enemy;
  }

  Targeting CreateTargeting(Targeting.Behavior behavior, Targeting.Priority priority) {
    Targeting targeting = new();
    targeting.behavior = behavior;
    targeting.priority = priority;
    return targeting;
  }
}
