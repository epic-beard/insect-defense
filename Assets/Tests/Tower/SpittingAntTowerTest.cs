using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.EventSystems.EventTrigger;

public class SpittingAntTowerTest {

  #region SpecialAbilityUpgradeTests

  // Test setting ArmorTearStun.
  [Test]
  public void SpecialAbilityUpgradeAcidStun() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ARMOR_TEAR_STUN);

    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearStun));
    Assert.That(false, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
  }

  // Test setting ArmorTearExplosion.
  [Test]
  public void SpecialAbilityUpgradeArmorTearExplosion() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_5_ARMOR_TEAR_EXPLOSION);

    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotSlow));
  }

  // Test setting DotSlow.
  [Test]
  public void SpecialAbilityUpgradeDotSlow() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_3_DOT_SLOW);

    Assert.That(true, Is.EqualTo(spittingAntTower.DotSlow));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotExplosion));
  }

  // Test setting DotExplosion.
  [Test]
  public void SpecialAbilityUpgradeDotExplosion() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_5_DOT_EXPLOSION);

    Assert.That(true, Is.EqualTo(spittingAntTower.DotExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.CamoSight));
  }

  // Test setting continuous fire, this requires setting the splash particle system on the tower.
  [Test]
  public void SpecialAbilityUpgradeConstantFire() {
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplash(spittingAntTower, splash);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE);

    Assert.That(true, Is.EqualTo(spittingAntTower.ContinuousAttack));
  }

  #endregion

  #region ApplyArmorTearAndCheckForAcidStunTests

  // Make sure acid damage doesn't stun doesn't apply if AcidStun isn't set.
  [Test]
  public void ApplyArmorTearAndCheckForAcidStunDoesNotStunWithoutAcidStun() {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: 1.0f);
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);

    bool isStunned = InvokeApplyArmorTearAndCheckForArmorTearStun(spittingAntTower, enemy, 2.0f);

    Assert.That(isStunned, Is.EqualTo(false));
  }

  // Confirm acid stun behavior with a variety of preconditions.
  [Test, Sequential]
  public void ApplyArmorTearAndCheckForArmorTearStun(
      [Values(0.0f, 1.0f, 2.0f)] float enemyArmor,
      [Values(1.0f, 1.0f, 1.0f)] float armorTear,
      [Values(false, true, false)] bool expectedStun) {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: enemyArmor);
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ARMOR_TEAR_STUN);

    bool isStunned = InvokeApplyArmorTearAndCheckForArmorTearStun(spittingAntTower, enemy, armorTear);

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
    
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplashExplosionRangeMultiplier(spittingAntTower, 10.0f);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    List<Enemy> expectedEnemiesInRange = new() { enemyInRange };

    List<Enemy> enemiesInRange = InvokeGetEnemiesInExplosionRange(spittingAntTower, target, 10.0f);

    Assert.That(enemiesInRange, Is.EqualTo(expectedEnemiesInRange));
  }

  #endregion

  #region ProcessDamageAndEffectsTests

  // Test the slow aspect of acid effects.
  [Test]
  public void HandleAcidEffectsSlows() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerDotSlow(spittingAntTower, true);
    SetSpittingAntTowerDotExplosion(spittingAntTower, false);
    float slowPower = 0.5f;
    float slowDuration = 10.0f;
    spittingAntTower.SlowPower = slowPower;
    spittingAntTower.SlowDuration = slowDuration;

    Enemy target = CreateEnemy(Vector3.zero);
    target.data.Initialize();

    InvokeHandleAcidEffects(spittingAntTower, target);

    // Make sure the target is slowed.
    Assert.That(target.SlowDuration, Is.EqualTo(slowDuration));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));

    InvokeHandleAcidEffects(spittingAntTower, target);

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
    target.data.currHP = enemyHP;
    enemyInRange.data.currHP = enemyHP;
    enemyOutOfRange.data.currHP = enemyHP;

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.AreaOfEffect = 10.0f;
    SetSpittingAntTowerAcidExplosionRangeMultiplier(spittingAntTower, 1.0f);
    spittingAntTower.DamageOverTime = 1000.0f;
    SetSpittingAntTowerDotSlow(spittingAntTower, false);
    SetSpittingAntTowerDotExplosion(spittingAntTower, true);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ARMOR_TEAR_STUN);
    ParticleSystem acidExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerAcidExplosion(spittingAntTower, acidExplosion);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    float expectedEnemyHP = enemyHP - (target.MaxAcidStacks * target.AcidDamagePerStackPerSecond);

    InvokeHandleAcidEffects(spittingAntTower, target);

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
    target.data.currHP = enemyHp;
    enemyInRange.data.currHP = enemyHp;
    enemyOutOfRange.data.currHP = enemyHp;

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplashExplosionRangeMultiplier(spittingAntTower, 10.0f);
    SetSpittingAntTowerArmorTearExplosion(spittingAntTower, armorTearExplosion);
    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplashExplosion(spittingAntTower, splashExplosion);
    float expectedStunTime = 0.0f;
    if (acidStun) {
      spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ARMOR_TEAR_STUN);
      expectedStunTime = spittingAntTower.StunTime;
    }

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    float expectedDamage = spittingAntTower.Damage;

    InvokeHandleSplashEffects(spittingAntTower, target, expectedDamage);

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
  [Test]
  public void ProcessDamageAndEffectsContinuousFireNoArmor() {
    Enemy target = CreateEnemy(Vector3.zero, armor: 0.0f, hp: 1.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplash(spittingAntTower, splash);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;
    
    float expectedHp = target.HP;

    InvokeProcessDamageAndEffects(spittingAntTower, target);

    Assert.That(target.HP, Is.LessThan(expectedHp));
    Assert.That(target.Armor, Is.EqualTo(0.0f));
  }

  // Test continuous fire on an armored target.
  [Test]
  public void ProcessDamageAndEffectsContinuousFireWithArmor() {
    Enemy target = CreateEnemy(Vector3.zero, armor: 1.0f, hp: 1.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplash(spittingAntTower, splash);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;

    float expectedHp = target.HP;
    float expectedArmor = target.Armor;

    InvokeProcessDamageAndEffects(spittingAntTower, target);

    Assert.That(target.HP, Is.EqualTo(expectedHp));
    Assert.That(target.Armor, Is.LessThan(expectedArmor));
  }

  // Test with splash fire on targets of varying armor.
  [Test]
  public void ProcessDamageAndEffectsSplashShot([Values(1.0f, 10.0f)] float enemyArmor) {
    Enemy target = CreateEnemy(Vector3.zero, armor: enemyArmor, hp: 10.0f);
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplash(spittingAntTower, splash);
    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplashExplosion(spittingAntTower, splashExplosion);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    spittingAntTower.Damage = 1.0f;
    spittingAntTower.AttackSpeed = 1.0f;
    spittingAntTower.ArmorTear = 1.0f;

    float expectedArmor = target.Armor - spittingAntTower.ArmorTear;
    float expectedHP = target.HP - (Mathf.Max(spittingAntTower.Damage - expectedArmor, 0.0f));

    InvokeProcessDamageAndEffects(spittingAntTower, target);

    Assert.That(target.HP, Is.EqualTo(expectedHP));
    Assert.That(target.Armor, Is.EqualTo(expectedArmor));
  }

  #endregion

  #region TestHelperMethods

  private SpittingAntTower CreateSpittingAntTower(Vector3 position) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    SpittingAntTower spittingAntTower = gameObject.AddComponent<SpittingAntTower>();
    return spittingAntTower;
  }

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

  private ObjectPool CreateObjectPool() {
    return new GameObject().AddComponent<ObjectPool>();
  }

  // All helper methods below this point use reflection to access private or protected properties, fields, or methods.

  private void SetSpittingAntTowerDotSlow(SpittingAntTower spittingAntTower, bool dotSlow) {
    typeof(SpittingAntTower)
        .GetProperty("DotSlow")
        .SetValue(spittingAntTower, dotSlow);
  }

  private void SetSpittingAntTowerDotExplosion(SpittingAntTower spittingAntTower, bool dotExplosion) {
    typeof(SpittingAntTower)
        .GetProperty("DotExplosion")
        .SetValue(spittingAntTower, dotExplosion);
  }

  private void SetSpittingAntTowerArmorTearExplosion(SpittingAntTower spittingAntTower, bool tearExplosion) {
    typeof(SpittingAntTower)
        .GetProperty("ArmorTearExplosion")
        .SetValue(spittingAntTower, tearExplosion);
  }

  private void SetSpittingAntTowerSplash(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("splash", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerSplashExplosion(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("splashExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerSplashExplosionRangeMultiplier(SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("splashExplosionMultiplier", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, range);
  }

  private void SetSpittingAntTowerAcidExplosion(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("acidExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerAcidExplosionRangeMultiplier(SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("acidExplosionMultiplier", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, range);
  }

  private void SetSpittingAntTowerObjectPool(SpittingAntTower spittingAntTower, ObjectPool objectPool) {
    typeof(SpittingAntTower)
        .GetField("objectPool", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, objectPool);
  }

  private void SetObjectPoolActiveEnemies(ObjectPool objectPool, HashSet<Enemy> enemies) {
    typeof(ObjectPool)
        .GetField("activeEnemies", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(objectPool, enemies);
  }

  private void InvokeProcessDamageAndEffects(SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo handleAcidEffects = typeof(SpittingAntTower).GetMethod(
        "ProcessDamageAndEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleAcidEffects.Invoke(spittingAntTower, args);
  }

  private void InvokeHandleAcidEffects(SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo handleAcidEffects = typeof(SpittingAntTower).GetMethod(
        "HandleAcidEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleAcidEffects.Invoke(spittingAntTower, args);
  }

  private void InvokeHandleSplashEffects(SpittingAntTower spittingAntTower, Enemy enemy, float onHitDamage) {
    object[] args = { enemy, onHitDamage };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo handleAcidEffects = typeof(SpittingAntTower).GetMethod(
        "HandleSplashEffects",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    handleAcidEffects.Invoke(spittingAntTower, args);
  }

  private List<Enemy> InvokeGetEnemiesInExplosionRange(SpittingAntTower spittingAntTower, Enemy enemy, float explosionRange) {
    object[] args = { enemy, explosionRange };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo getEnemiesInExplosionRange = typeof(SpittingAntTower).GetMethod(
        "GetEnemiesInExplosionRange",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    return (List<Enemy>) getEnemiesInExplosionRange.Invoke(spittingAntTower, args);
  }

  private bool InvokeApplyArmorTearAndCheckForArmorTearStun(SpittingAntTower spittingAntTower, Enemy enemy, float armorTear) {
    object[] args = { enemy, armorTear };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo applyArmorTearAndCheckForAcidStun = typeof(SpittingAntTower).GetMethod(
        "ApplyArmorTearAndCheckForArmorTearStun",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    return (bool) applyArmorTearAndCheckForAcidStun.Invoke(spittingAntTower, args);
  }

  #endregion
}
