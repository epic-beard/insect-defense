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
    wssTower.SetPrimaryProjectileHandler(projectileHandler);

    ParticleSystem secondaryWeb = new GameObject().AddComponent<ParticleSystem>();
    wssTower.SetSecondaryWebShot(secondaryWeb);
    ProjectileHandler secondaryProjectileHandler = new(secondaryWeb, wssTower.ProjectileSpeed, Tower.hitRange);
    wssTower.SetSecondaryProjectileHandler(secondaryProjectileHandler);
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

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 1));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(1));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 2));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(2));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.0f, 3));

    Assert.That(wssTower.SecondarySlowTargets, Is.EqualTo(3));
  }

  [Test]
  public void SlowAppliedToSecondaryTargets() {
    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.0f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.5f, 0));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.5f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.75f, 0));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(0.75f));

    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(1.0f, 0));

    Assert.That(wssTower.SecondarySlowPotency, Is.EqualTo(1.0f));
  }

  #endregion

  // Test to make sure the AoE range testing capability of SlowNearbyEnemies doesn't apply slows to
  // enemies out of range.
  [Test]
  public void SlowNearbyEnemiesOutOfAoERange() {
    wssTower.Upgrade(CreateTowerAbilityForAoESlowTests(0.5f, 1));
    wssTower.AreaOfEffect = 10.0f;

    Enemy target = CreateEnemy(Vector3.zero);
    // Create an enemy out of the tower's AoE range.
    Enemy outOfRange = CreateEnemy(Vector3.right * 100);

    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = objectPool;
    HashSet<Enemy> activeEnemies = new() { target, outOfRange };
    objectPool.SetActiveEnemies(activeEnemies);

    // Establish a baseline before invoking SlowNearbyEnemies
    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(outOfRange.SlowPower, Is.EqualTo(0.0f));

    wssTower.InvokeSlowNearbyEnemies(target);

    Assert.That(target.SlowPower, Is.EqualTo(0.0f));
    Assert.That(outOfRange.SlowPower, Is.EqualTo(0.0f));
  }

  #region TestHelperMethods

  // Create and return an enemy with optional args.
  private Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 1.0f) {
    GameObject gameObject = new();
    gameObject.SetActive(false);
    gameObject.transform.position = position;

    EnemyData data = new() {
      maxArmor = armor,
      maxHP = hp,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.Data = data;
    gameObject.SetActive(true);
    return enemy;
  }

  // Create a TowerAbility for use in testing the secondary slow numbers and duration.
  private TowerAbility CreateTowerAbilityForAoESlowTests(float secondarySlow, int secondaryTargets) {
    TowerAbility ability = new();
    ability.floatAttributeModifiers = new TowerAbility.AttributeModifier<float>[1];
    ability.floatAttributeModifiers[0].attribute = TowerData.Stat.SECONDARY_SLOW_POTENCY;
    ability.floatAttributeModifiers[0].mode = TowerAbility.Mode.SET;
    ability.floatAttributeModifiers[0].mod = secondarySlow;
    ability.intAttributeModifiers = new TowerAbility.AttributeModifier<int>[1];
    ability.intAttributeModifiers[0].attribute = TowerData.Stat.SECONDARY_SLOW_TARGETS;
    ability.intAttributeModifiers[0].mode = TowerAbility.Mode.SET;
    ability.intAttributeModifiers[0].mod = secondaryTargets;
    ability.upgradePath = 1;

    return ability;
  }

  #endregion
}

#region WebShootingSpiderTowerUtils

public static class WebShootingSpiderTowerUtils {
  public static void SetPrimaryProjectileHandler(
      this WebShootingSpiderTower wssTower, ProjectileHandler projectileHandler) {
    typeof(WebShootingSpiderTower)
        .GetField("primaryProjectileHandler", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, projectileHandler);
  }

  public static void SetSecondaryProjectileHandler(
      this WebShootingSpiderTower wssTower, ProjectileHandler projectileHandler) {
    typeof(WebShootingSpiderTower)
        .GetField("secondaryProjectileHandler", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, projectileHandler);
  }

  public static void SetMesh(this WebShootingSpiderTower wssTower, MeshRenderer meshRenderer) {
    typeof(WebShootingSpiderTower)
        .GetField("mesh", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, meshRenderer.transform);
  }

  public static void SetSecondaryWebShot(this WebShootingSpiderTower wssTower, ParticleSystem particleSystem) {
    typeof(WebShootingSpiderTower)
        .GetField("secondaryWebShot", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, particleSystem);
  }

  public static void SetPrimaryWebShot(this WebShootingSpiderTower wssTower, ParticleSystem particleSystem) {
    typeof(WebShootingSpiderTower)
        .GetField("primaryWebShot", BindingFlags.Instance | BindingFlags.NonPublic)
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
