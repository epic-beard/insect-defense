#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

  // Create an Enemy and set various properties for testing.
  Enemy CreateEnemy(float armor, float hp, bool isFlier, bool isCamo, Vector3 position) {
    GameObject gameObject = new();
    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.armor = armor;
    enemy.hp = hp;
    enemy.isFlier = isFlier;
    enemy.isCamo = isCamo;
    enemy.position = position;
    return enemy;
  }

  // Create a Targeting script with the appropriate behavior and priority.
  Targeting CreateTargeting(Targeting.Behavior behavior, Targeting.Priority priority) {
    Targeting targeting = new();
    targeting.behavior = behavior;
    targeting.priority = priority;
    return targeting;
  }
}
