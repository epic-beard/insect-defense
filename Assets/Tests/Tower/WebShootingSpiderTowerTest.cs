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

    Assert.That(wssTower.SlowStun, Is.EqualTo(true));
    Assert.That(wssTower.PermanentSlow, Is.EqualTo(false));
  }

  // Test setting PermanentSlow.
  [Test]
  public void SpecialAbilityUpgradePermanentSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW);

    Assert.That(wssTower.PermanentSlow, Is.EqualTo(true));
    Assert.That(wssTower.LingeringSlow, Is.EqualTo(false));
  }

  // Test setting LingeringSlow.
  [Test]
  public void SpecialAbilityUpgradeLingeringSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_2_5_LINGERING_SLOW);

    Assert.That(wssTower.LingeringSlow, Is.EqualTo(true));
    Assert.That(wssTower.AntiAir, Is.EqualTo(false));
  }

  // Test setting AntiAir.
  [Test]
  public void SpecialAbilityUpgradeAntiAir() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_3_ANTI_AIR);

    Assert.That(wssTower.AntiAir, Is.EqualTo(true));
    Assert.That(wssTower.GroundingShot, Is.EqualTo(false));
  }

  // Test setting AAAssist.
  [Test]
  public void SpecialAbilityUpgradeAAAssist() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_5_GROUNDING_SHOT);

    Assert.That(wssTower.GroundingShot, Is.EqualTo(true));
  }

  #endregion

  #region AoESlowTests

  [Test]
  public void EnemieshitBySlow() {
    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(0));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 1.0f));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(1));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 2.0f));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(2));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 3.0f));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(3));
  }

  [Test]
  public void SlowAppliedToSecondaryTargets() {
    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.0f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.5f, 0.0f));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.5f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.75f, 0.0f));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.75f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(1.0f, 0.0f));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(1.0f));
  }

  #endregion

  // Test to make sure the AoE range testing capability of SlowNearbyEnemies doesn't apply slows to
  // enemies out of range.
  [Test]
  public void SlowNearbyEnemiesOutOfAoERange() {
    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.5f, 1.0f));
    wssTower.AreaOfEffect = 10.0f;

    Enemy target = CreateEnemy(Vector3.zero);
    // Create an enemy out of the tower's AoE range.
    Enemy outOfRange = CreateEnemy(Vector3.right * 100);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { target, outOfRange };
    objectPool.SetActiveEnemies(activeEnemies);
    wssTower.SetObjectPool(objectPool);

    // Establish a baseline before invoking SlowNearbyEnemies
    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(outOfRange.SlowPower, Is.EqualTo(0.0f));

    wssTower.InvokeSlowNearbyEnemies(target);

    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(outOfRange.SlowPower, Is.EqualTo(0.0f));
  }

  // Check to ensure that the proper number of in-range enemies are affected by the tower's AoE slow.
  [Test]
  public void SlowNearbyEnemies() {
    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.5f, 2.0f));
    wssTower.AreaOfEffect = 10.0f;
    wssTower.SlowPower = 0.25f;
    wssTower.SlowDuration = 1.0f;
    float secondaryTowerSlowPower = wssTower.SlowPower * wssTower.SecondarySlowPotency;
    float secondaryTowerSlowDuration = wssTower.SlowDuration * wssTower.SecondarySlowPotency;

    Enemy target = CreateEnemy(Vector3.zero);
    Enemy firstClosestEnemy = CreateEnemy(Vector3.right);
    Enemy secondClosestEnemy = CreateEnemy(Vector3.right * 2);
    Enemy thirdClosestEnemy = CreateEnemy(Vector3.right * 3);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { target, firstClosestEnemy, secondClosestEnemy, thirdClosestEnemy };
    objectPool.SetActiveEnemies(activeEnemies);
    wssTower.SetObjectPool(objectPool);

    // Establish a baseline before invoking SlowNearbyEnemies
    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(firstClosestEnemy.SlowPower, Is.EqualTo(0.0f));
    Assert.That(firstClosestEnemy.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(secondClosestEnemy.SlowPower, Is.EqualTo(0.0f));
    Assert.That(secondClosestEnemy.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(thirdClosestEnemy.SlowPower, Is.EqualTo(0.0f));
    Assert.That(thirdClosestEnemy.SlowDuration, Is.EqualTo(0.0f));

    wssTower.InvokeSlowNearbyEnemies(target);

    // Make sure that the slows and durations were applied appropriately.
    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(firstClosestEnemy.SlowPower, Is.EqualTo(secondaryTowerSlowPower));
    Assert.That(firstClosestEnemy.SlowDuration, Is.EqualTo(secondaryTowerSlowDuration));
    Assert.That(secondClosestEnemy.SlowPower, Is.EqualTo(secondaryTowerSlowPower));
    Assert.That(secondClosestEnemy.SlowDuration, Is.EqualTo(secondaryTowerSlowDuration));
    Assert.That(thirdClosestEnemy.SlowPower, Is.EqualTo(0.0f));
    Assert.That(thirdClosestEnemy.SlowDuration, Is.EqualTo(0.0f));
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
  private TowerAbility CreateTowerAbilityForAoESlowTests(float secondarySlow, float secondaryTargets) {
    TowerAbility ability = new();
    ability.attributeModifiers = new TowerAbility.AttributeModifier[2];
    ability.attributeModifiers[0].attribute = TowerData.Stat.SECDONARY_SLOW_POTENCY;
    ability.attributeModifiers[0].mode = TowerAbility.Mode.SET;
    ability.attributeModifiers[0].mod = secondarySlow;
    ability.attributeModifiers[1].attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS;
    ability.attributeModifiers[1].mode = TowerAbility.Mode.SET;
    ability.attributeModifiers[1].mod = secondaryTargets;
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

  public static void SetUpperMesh(this WebShootingSpiderTower wssTower, MeshRenderer meshRenderer) {
    typeof(WebShootingSpiderTower)
        .GetField("upperMesh", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, meshRenderer.transform);
  }

  public static void SetWebShot(this WebShootingSpiderTower wssTower, ParticleSystem particleSystem) {
    typeof(WebShootingSpiderTower)
        .GetField("webShot", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, particleSystem);
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
