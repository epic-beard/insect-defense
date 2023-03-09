using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class WebShootingSpiderTowerTest {

  WebShootingSpiderTower wssTower;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    wssTower = gameObject.AddComponent<WebShootingSpiderTower>();

    ParticleSystem web = new GameObject().AddComponent<ParticleSystem>();

    ProjectileHandler projectileHandler = new(web, wssTower.ProjectileSpeed, Tower.hitRange);
    wssTower.SetProjectileHandler(projectileHandler);
  }

  #region SpecialAbilityUpgradeTests

  // Test setting SlowStun.
  [Test]
  public void SpecialAbilityUpgradeSlowStun() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_3_SLOW_STUN);

    Assert.That(true, Is.EqualTo(wssTower.SlowStun));
    Assert.That(false, Is.EqualTo(wssTower.PermanentSlow));
  }

  // Test setting PermanentSlow.
  [Test]
  public void SpecialAbilityUpgradePermanentSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW);

    Assert.That(true, Is.EqualTo(wssTower.PermanentSlow));
    Assert.That(false, Is.EqualTo(wssTower.LingeringSlow));
  }

  // Test setting LingeringSlow.
  [Test]
  public void SpecialAbilityUpgradeLingeringSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_2_5_LINGERING_SLOW);

    Assert.That(true, Is.EqualTo(wssTower.LingeringSlow));
    Assert.That(false, Is.EqualTo(wssTower.AntiAir));
  }

  // Test setting AntiAir.
  [Test]
  public void SpecialAbilityUpgradeAntiAir() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_3_ANTI_AIR);

    Assert.That(true, Is.EqualTo(wssTower.AntiAir));
    Assert.That(false, Is.EqualTo(wssTower.AAAssist));
  }

  // Test setting AAAssist.
  [Test]
  public void SpecialAbilityUpgradeAAAssist() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_5_AA_ASSIST);

    Assert.That(true, Is.EqualTo(wssTower.AAAssist));
  }

  #endregion

  #region AoESlowTests

  [Test]
  public void EnemieshitBySlow() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();

    Assert.That(0, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(1, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(2, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(3, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(3, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(3, Is.EqualTo(wssTower.EnemiesHitBySlow));
  }

  [Test]
  public void SlowAppliedToSecondaryTargets() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();

    Assert.That(0.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.5f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.5f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.75f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(1.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(1.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));
  }

  #endregion

  // Test to make sure the AoE range testing capability of SlowNearbyEnemies doesn't apply slows to
  // enemies out of range.
  [Test]
  public void SlowNearbyEnemiesOutOfAoERange() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();
    wssTower.Upgrade(ability);
    wssTower.AreaOfEffect = 10.0f;

    Enemy target = CreateEnemy(Vector3.zero);
    // Create an enemy out of the tower's AoE range.
    Enemy outOfRange = CreateEnemy(Vector3.right * 100);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { target, outOfRange };
    objectPool.SetActiveEnemies(activeEnemies);
    wssTower.SetObjectPool(objectPool);

    // Establish a baseline before invoking SlowNearbyEnemies
    Assert.That(0.0f, Is.EqualTo(target.SlowPower));
    Assert.That(0.0f, Is.EqualTo(outOfRange.SlowPower));

    wssTower.InvokeSlowNearbyEnemies(target);

    Assert.That(0.0f, Is.EqualTo(target.SlowPower));
    Assert.That(0.0f, Is.EqualTo(outOfRange.SlowPower));
  }

  // Check to ensure that the proper number of in-range enemies are affected by the tower's AoE slow.
  [Test]
  public void SlowNearbyEnemies() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();
    wssTower.Upgrade(ability);
    wssTower.Upgrade(ability);
    wssTower.AreaOfEffect = 10.0f;
    wssTower.SlowPower = 0.25f;
    wssTower.SlowDuration = 1.0f;
    float secondaryTowerSlowPower = wssTower.SlowPower * wssTower.SlowAppliedToSecondaryTargets;
    float secondaryTowerSlowDuration = wssTower.SlowDuration * wssTower.SlowAppliedToSecondaryTargets;

    Enemy target = CreateEnemy(Vector3.zero);
    Enemy firstClosestEnemy = CreateEnemy(Vector3.right);
    Enemy secondClosestEnemy = CreateEnemy(Vector3.right * 2);
    Enemy thirdClosestEnemy = CreateEnemy(Vector3.right * 3);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { target, firstClosestEnemy, secondClosestEnemy, thirdClosestEnemy };
    objectPool.SetActiveEnemies(activeEnemies);
    wssTower.SetObjectPool(objectPool);

    // Establish a baseline before invoking SlowNearbyEnemies
    Assert.That(0.0f, Is.EqualTo(target.SlowPower));
    Assert.That(0.0f, Is.EqualTo(target.SlowDuration));
    Assert.That(0.0f, Is.EqualTo(firstClosestEnemy.SlowPower));
    Assert.That(0.0f, Is.EqualTo(firstClosestEnemy.SlowDuration));
    Assert.That(0.0f, Is.EqualTo(secondClosestEnemy.SlowPower));
    Assert.That(0.0f, Is.EqualTo(secondClosestEnemy.SlowDuration));
    Assert.That(0.0f, Is.EqualTo(thirdClosestEnemy.SlowPower));
    Assert.That(0.0f, Is.EqualTo(thirdClosestEnemy.SlowDuration));

    wssTower.InvokeSlowNearbyEnemies(target);

    // Make sure that the slows and durations were applied appropriately.
    Assert.That(0.0f, Is.EqualTo(target.SlowPower));
    Assert.That(0.0f, Is.EqualTo(target.SlowDuration));
    Assert.That(secondaryTowerSlowPower, Is.EqualTo(firstClosestEnemy.SlowPower));
    Assert.That(secondaryTowerSlowDuration, Is.EqualTo(firstClosestEnemy.SlowDuration));
    Assert.That(secondaryTowerSlowPower, Is.EqualTo(secondClosestEnemy.SlowPower));
    Assert.That(secondaryTowerSlowDuration, Is.EqualTo(secondClosestEnemy.SlowDuration));
    Assert.That(0.0f, Is.EqualTo(thirdClosestEnemy.SlowPower));
    Assert.That(0.0f, Is.EqualTo(thirdClosestEnemy.SlowDuration));
  }

  #region TestHelperMethods

  // Create and return an enemy with optional args.
  private Enemy CreateEnemy(
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

  // Create a TowerAbility for use in testing the secondary slow numbers and duration.
  private TowerAbility CreateTowerAbilityForAoESlowTests() {
    TowerAbility ability = new();
    ability.mode = TowerAbility.Mode.MULTIPLICATIVE;
    ability.attributeModifiers = new TowerAbility.AttributeModifier[0];
    ability.upgradePath = 1;

    return ability;
  }

  #endregion
}

#region WebShootingSpiderTowerUtils

public static class WebShootingSpiderTowerUtils {
  public static void SetProjectileHandler(this WebShootingSpiderTower wssTower, ProjectileHandler projectileHandler) {
    typeof(WebShootingSpiderTower)
        .GetField("projectileHandler", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, projectileHandler);
  }

  public static void SetObjectPool(this WebShootingSpiderTower wssTower, ObjectPool pool) {
    typeof(WebShootingSpiderTower)
        .GetField("objectPool", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, pool);
  }

  public static void InvokeSlowNearbyEnemies(this WebShootingSpiderTower wssTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo slowNearbyEnemies = typeof(WebShootingSpiderTower).GetMethod(
        "SlowNearbyEnemies",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    slowNearbyEnemies.Invoke(wssTower, args);
  }
}

#endregion
