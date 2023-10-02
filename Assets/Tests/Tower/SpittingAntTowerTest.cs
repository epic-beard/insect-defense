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
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN);

    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearStun));
    Assert.That(false, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
  }

  // Test setting ArmorTearExplosion.
  [Test]
  public void SpecialAbilityUpgradeArmorTearExplosion() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION);

    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotSlow));
  }

  // Test setting DotSlow.
  [Test]
  public void SpecialAbilityUpgradeDotSlow() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_3_DOT_SLOW);

    Assert.That(true, Is.EqualTo(spittingAntTower.DotSlow));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotExplosion));
  }

  // Test setting DotExplosion.
  [Test]
  public void SpecialAbilityUpgradeDotExplosion() {
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_5_DOT_EXPLOSION);

    Assert.That(true, Is.EqualTo(spittingAntTower.DotExplosion));
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

  #region ApplyArmorTearAndCheckForAcidStunTests

  // Make sure acid damage doesn't stun doesn't apply if AcidStun isn't set.
  [Test]
  public void ApplyArmorTearAndCheckForAcidStunDoesNotStunWithoutAcidStun() {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: 1.0f);

    bool isStunned = spittingAntTower.InvokeApplyArmorTearAndCheckForArmorTearStun(enemy, 2.0f);

    Assert.That(isStunned, Is.EqualTo(false));
  }

  // Confirm acid stun behavior with a variety of preconditions.
  [Test, Sequential]
  public void ApplyArmorTearAndCheckForArmorTearStun(
      [Values(0.0f, 1.0f, 2.0f)] float enemyArmor,
      [Values(1.0f, 1.0f, 1.0f)] float armorTear,
      [Values(false, true, false)] bool expectedStun) {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: enemyArmor);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN);

    bool isStunned = spittingAntTower.InvokeApplyArmorTearAndCheckForArmorTearStun(enemy, armorTear);

    Assert.That(isStunned, Is.EqualTo(expectedStun));
  }

  #endregion

  #region GetEnemiesInExplosionRangeTest

  // Test that GetEnemiesInExplosionRange functions as expected
  [Test]
  public void GetEnemiesInExplosionRange() {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    spittingAntTower.SetSplashExplosionRangeMultiplier(10.0f);

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

  // Test the slow aspect of acid effects.
  [Test]
  public void HandleAcidEffectsSlows() {
    spittingAntTower.SetDotSlow(true);
    spittingAntTower.SetDotExplosion(false);
    float slowPower = 0.5f;
    float slowDuration = 10.0f;
    spittingAntTower.SlowPower = slowPower;
    spittingAntTower.SlowDuration = slowDuration;

    Enemy target = CreateEnemy(Vector3.zero);

    spittingAntTower.InvokeHandleMaxAcidStackEffects(target);

    // Make sure the target is slowed.
    Assert.That(target.SlowDuration, Is.EqualTo(slowDuration));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));

    spittingAntTower.InvokeHandleMaxAcidStackEffects(target);

    // Make sure the slow is not applied more than once.
    Assert.That(target.SlowDuration, Is.EqualTo(slowDuration));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));
  }

  // Test that acid explosions have the desired effects.
  [Test]
  public void HandleAcidEffectsAcidExplosions() {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    float enemyHP = 10000.0f;
    target.HP = enemyHP;
    enemyInRange.HP = enemyHP;
    enemyOutOfRange.HP = enemyHP;

    spittingAntTower.AreaOfEffect = 10.0f;
    spittingAntTower.SetAcidExplosionRangeMultiplier(1.0f);
    spittingAntTower.DamageOverTime = 1000.0f;
    spittingAntTower.SetDotSlow(false);
    spittingAntTower.SetDotExplosion(true);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN);
    ParticleSystem acidExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetAcidExplosion(acidExplosion);

    ObjectPool objectPool = CreateObjectPool();
    ObjectPool.Instance = objectPool;
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);

    float expectedEnemyHP = enemyHP - (target.MaxAcidStacks * target.AcidDamagePerStackPerSecond);

    spittingAntTower.InvokeHandleMaxAcidStackEffects(target);

    Assert.That(target.HP, Is.EqualTo(expectedEnemyHP));
    Assert.That(enemyInRange.HP, Is.EqualTo(expectedEnemyHP));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
  }

  // Test HandleSplashEffects with combinatorial arguments.
  [Test]
  public void HandleSplashEffects(
      [Values(true, false)] bool armorTearExplosion,
      [Values(true, false)] bool acidStun) {
    Enemy target = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy enemyInRange = CreateEnemy(Vector3.zero, armor: 1.0f);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0), armor: 1.0f);
    float enemyHp = 10000.0f;
    target.HP = enemyHp;
    enemyInRange.HP = enemyHp;
    enemyOutOfRange.HP = enemyHp;

    spittingAntTower.SetSplashExplosionRangeMultiplier(10.0f);
    spittingAntTower.SetArmorTearExplosion(armorTearExplosion);
    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);
    float expectedStunTime = 0.0f;
    if (acidStun) {
      spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_3_ARMOR_TEAR_STUN);
      expectedStunTime = spittingAntTower.StunTime;
    }

    ObjectPool objectPool = CreateObjectPool();
    ObjectPool.Instance = objectPool;
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);

    float expectedDamage = spittingAntTower.Damage;

    spittingAntTower.InvokeHandleSplashEffects(target, expectedDamage);

    // Ensure that the splash damage is applied appropriately regardless of ArmorTearExplosion.
    Assert.That(target.HP, Is.EqualTo(enemyHp - expectedDamage));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHp - expectedDamage));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));

    // Ensure that armor tear is applied appropriately only if ArmorTearExplosion is set.
    Assert.That(target.StunTime, Is.EqualTo(expectedStunTime));
    Assert.That(enemyInRange.StunTime, Is.EqualTo(expectedStunTime));
    Assert.That(enemyOutOfRange.StunTime, Is.EqualTo(0.0f));
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

    float expectedHp = target.HP;
    float expectedArmor = target.Armor;

    spittingAntTower.InvokeProcessDamageAndEffects(target);

    Assert.That(target.HP, Is.EqualTo(expectedHp));
    Assert.That(target.Armor, Is.LessThan(expectedArmor));

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
    float expectedHP = target.HP - (Mathf.Max(spittingAntTower.Damage - expectedArmor, 0.0f));

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
  public static void SetDotSlow(this SpittingAntTower spittingAntTower, bool dotSlow) {
    typeof(SpittingAntTower)
        .GetProperty("DotSlow")
        .SetValue(spittingAntTower, dotSlow);
  }

  public static void SetDotExplosion(this SpittingAntTower spittingAntTower, bool dotExplosion) {
    typeof(SpittingAntTower)
        .GetProperty("DotExplosion")
        .SetValue(spittingAntTower, dotExplosion);
  }

  public static void SetArmorTearExplosion(this SpittingAntTower spittingAntTower, bool tearExplosion) {
    typeof(SpittingAntTower)
        .GetProperty("ArmorTearExplosion")
        .SetValue(spittingAntTower, tearExplosion);
  }

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

  public static void SetSplashExplosionRangeMultiplier(this SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("splashExplosionMultiplier", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, range);
  }

  public static void SetAcidExplosion(this SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("acidExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  public static void SetAcidExplosionRangeMultiplier(this SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("acidExplosionMultiplier", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, range);
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

  public static void InvokeProcessDamageAndEffects(this SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo processDamageAndEffects = typeof(SpittingAntTower).GetMethod(
        "ProcessDamageAndEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    processDamageAndEffects.Invoke(spittingAntTower, args);
  }

  public static void InvokeHandleMaxAcidStackEffects(this SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo handleMaxAcidStackEffects = typeof(SpittingAntTower).GetMethod(
        "HandleMaxAcidStackEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleMaxAcidStackEffects.Invoke(spittingAntTower, args);
  }

  public static void InvokeHandleSplashEffects(
      this SpittingAntTower spittingAntTower, Enemy enemy, float onHitDamage) {
    object[] args = { enemy, onHitDamage };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo handleSplashEffects = typeof(SpittingAntTower).GetMethod(
        "HandleSplashEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleSplashEffects.Invoke(spittingAntTower, args);
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

  public static bool InvokeApplyArmorTearAndCheckForArmorTearStun(
      this SpittingAntTower spittingAntTower, Enemy enemy, float armorTear) {
    object[] args = { enemy, armorTear };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo applyArmorTearAndCheckForAcidStun = typeof(SpittingAntTower).GetMethod(
        "ApplyArmorTearAndCheckForArmorTearStun",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    return (bool)applyArmorTearAndCheckForAcidStun.Invoke(spittingAntTower, args);
  }
}

#endregion
