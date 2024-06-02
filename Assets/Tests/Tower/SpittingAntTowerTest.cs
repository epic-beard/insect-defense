using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpittingAntTowerTest {

  SpittingAntTower spittingAntTower;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    spittingAntTower = gameObject.AddComponent<SpittingAntTower>();

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();

    ProjectileHandler projectileHandler = new(splash, spittingAntTower.ProjectileSpeed, Tower.hitRange);
    spittingAntTower.SetProjectileHandler(projectileHandler);
    Time.captureDeltaTime = 1;
  }

  #region SpecialAbilityUpgradeTests

  // Test setting ArmorTearStun.
  [Test]
  public void SpecialAbilityUpgradeAcidStun() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_3_ACIDIC_SYNERGY);

    Assert.That(true, Is.EqualTo(spittingAntTower.AcidicSynergy));
    Assert.That(false, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
  }

  // Test setting ArmorTearExplosion.
  [Test]
  public void SpecialAbilityUpgradeArmorTearExplosion() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_5_VENOM_CORSEPLOSION);

    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.AcidBuildupBonus));
  }

  // Test setting DotSlow.
  [Test]
  public void SpecialAbilityUpgradeDotSlow() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_3_ACID_BUILDUP_BONUS);

    Assert.That(true, Is.EqualTo(spittingAntTower.AcidBuildupBonus));
    Assert.That(false, Is.EqualTo(spittingAntTower.AcidEnhancement));
  }

  // Test setting DotExplosion.
  [Test]
  public void SpecialAbilityUpgradeDotExplosion() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_5_DOT_ENHANCEMENT);

    Assert.That(true, Is.EqualTo(spittingAntTower.AcidEnhancement));
    Assert.That(false, Is.EqualTo(spittingAntTower.AntiAir));
  }

  // Test setting AntiAir.
  [Test]
  public void SpecialAbilityUpgradeAntiAir() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_3_ANTI_AIR);

    Assert.That(true, Is.EqualTo(spittingAntTower.AntiAir));
    Assert.That(false, Is.EqualTo(spittingAntTower.ContinuousAttack));
  }

  // Test setting continuous fire, this requires setting the splash particle system on the tower.
  [Test]
  public void SpecialAbilityUpgradeConstantFire() {
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplash(splash);

    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);

    Assert.That(true, Is.EqualTo(spittingAntTower.ContinuousAttack));
  }

  #endregion

  #region GetEnemiesInExplosionRangeTest

  // Test that GetEnemiesInExplosionRange functions as expected
  [Test]
  public void GetEnemiesInExplosionRange() {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    spittingAntTower.SetAreaofEffect(10.0f);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);

    List<Enemy> expectedEnemiesInRange = new() { enemyInRange };

    List<Enemy> enemiesInRange =
        spittingAntTower.InvokeGetEnemiesInExplosionRange(objectPool.GetActiveEnemies(), target, 10.0f);

    Assert.That(enemiesInRange, Is.EqualTo(expectedEnemiesInRange));
  }

  #endregion

  #region ProcessDamageAndEffectsTests

  // Test HandleArmorTearExplosion.
  [Test]
  public void HandleArmorTearExplosion() {
    float enemyArmor = 1000.0f;
    Enemy target = CreateEnemy(Vector3.zero, hp: 10000.0f, armor: enemyArmor);
    Enemy enemyInRange = CreateEnemy(Vector3.zero, hp: 10000.0f, armor: enemyArmor);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0), hp: 10000.0f, armor: enemyArmor);

    spittingAntTower.SetAreaofEffect(10.0f);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);

    ObjectPool objectPool = CreateObjectPool();
    ObjectPool.Instance = objectPool;
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);

    float expectedArmorTear = 5.0f;
    spittingAntTower.ArmorTear = expectedArmorTear;

    spittingAntTower.InvokeHandleArmorTearExplosion(target, expectedArmorTear);

    // Ensure that the splash damage is applied appropriately regardless of ArmorTearExplosion.
    Assert.That(target.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor - expectedArmorTear));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));
  }

  // Test continuous fire on an unarmored target.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffectsContinuousFireNoArmor() {
    Enemy target = CreateEnemy(Vector3.zero, armor: 0.0f, hp: 10.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplash(splash);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;

    float expectedHp = target.HP;

    spittingAntTower.InvokeProcessDamageAndEffects(target);

    Assert.That(target.HP, Is.LessThan(expectedHp));
    Assert.That(target.Armor, Is.EqualTo(0.0f));

    return null;
  }

  // Test continuous fire on an armored target.
  [UnityTest]
  public IEnumerator ProcessDamageAndEffectsContinuousFireWithArmor() {
    Enemy target = CreateEnemy(Vector3.zero, armor: 2.0f, hp: 10.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplash(splash);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;

    float expectedArmor = target.Armor - spittingAntTower.ArmorTear;
    float expectedHp = target.HP - (Mathf.Max(spittingAntTower.Damage * (100 - expectedArmor) / 100, 0.0f));

    spittingAntTower.InvokeProcessDamageAndEffects(target);

    Assert.That(target.HP, Is.EqualTo(expectedHp));
    Assert.That(target.Armor, Is.EqualTo(expectedArmor));

    return null;
  }

  // Test with splash fire on targets of varying armor.
  [Test]
  public void ProcessDamageAndEffectsSplashShot([Values(1.0f, 10.0f)] float enemyArmor) {
    Enemy target = CreateEnemy(Vector3.zero, armor: enemyArmor, hp: 10.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();

    spittingAntTower.SetSplash(splash);
    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);

    ObjectPool objectPool = CreateObjectPool();
    ObjectPool.Instance = objectPool;
    HashSet<Enemy> activeEnemies = new() { target };
    objectPool.SetActiveEnemies(activeEnemies);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;

    float expectedArmor = target.Armor - spittingAntTower.ArmorTear;
    float expectedHP = target.HP - (Mathf.Max(spittingAntTower.Damage * (100 - expectedArmor) / 100, 0.0f));

    spittingAntTower.InvokeProcessDamageAndEffects(target);

    Assert.That(target.HP, Is.EqualTo(expectedHP));
    Assert.That(target.Armor, Is.EqualTo(expectedArmor));
  }

  #endregion

  #region TestHelperMethods

  // Create and return an enemy with optional args.
  private Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 1.0f) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    gameObject.SetActive(false);
    EnemyData data = new() {
      maxArmor = armor,
      maxHP = hp,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.SetTarget(enemy.transform);
    enemy.Data = data;
    gameObject.SetActive(true);
    return enemy;
  }

  private ObjectPool CreateObjectPool() {
    return new GameObject().AddComponent<ObjectPool>();
  }

  #endregion
}

#region SpittingAntTowerUtils

// Extension methods to hold reflection-based calls to access private fields, properties, or methods of
// SpittingAntTower.
public static class SpittingAntTowerUtils {

  public static void SetSplash(this SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("splash", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  public static void SetSplashExplosion(this SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("splashExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  public static void SetBeam(this SpittingAntTower spittingAntTower, LineRenderer lineRenderer) {
    typeof(SpittingAntTower)
        .GetField("beam", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, lineRenderer);
  }

  public static void SetUpperMesh(this SpittingAntTower spittingAntTower, MeshRenderer meshRenderer) {
    typeof(SpittingAntTower)
        .GetField("upperMesh", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, meshRenderer.transform);
  }

  public static void SetProjectileHandler(this SpittingAntTower spittingAntTower, ProjectileHandler projectileHandler) {
    typeof(SpittingAntTower)
        .GetField("projectileHandler", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, projectileHandler);
  }

  public static void SetAreaofEffect(this SpittingAntTower spittingAntTower, float aoe) {
    typeof(SpittingAntTower)
        .GetProperty("AreaOfEffect")
        .SetValue(spittingAntTower, aoe);
  }

  public static void InvokeHandleArmorTearExplosion(
      this SpittingAntTower spittingAntTower, Enemy enemy, float armorTear) {
    object[] args = { enemy, armorTear };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo handleSplashEffects = typeof(SpittingAntTower).GetMethod(
        "HandleArmorTearExplosion",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleSplashEffects.Invoke(spittingAntTower, args);
  }

  public static void InvokeProcessDamageAndEffects(this SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo processDamageAndEffects = typeof(SpittingAntTower).GetMethod(
        "ProcessDamageAndEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    processDamageAndEffects.Invoke(spittingAntTower, args);
  }

  public static List<Enemy> InvokeGetEnemiesInExplosionRange(
      this SpittingAntTower spittingAntTower, HashSet<Enemy> enemiesInRange, Enemy enemy, float explosionRange) {
    object[] args = { enemiesInRange, enemy, explosionRange };
    Type[] argTypes = { typeof(HashSet<Enemy>), typeof(Enemy), typeof(float) };
    MethodInfo getEnemiesInExplosionRange = typeof(SpittingAntTower).GetMethod(
        "GetEnemiesInExplosionRange",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    return (List<Enemy>)getEnemiesInExplosionRange.Invoke(spittingAntTower, args);
  }
}

#endregion
