using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.EventSystems.EventTrigger;

public class SpittingAntTowerTest {
  // To Test for the spitting ant tower:
  //  - ProcessDamageAndEffects (and its effects)

  #region SpecialAbilityUpgradeTests

  [Test]
  public void SpecialAbilityUpgradeAcidStun() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ACID_STUN);
    Assert.That(true, Is.EqualTo(spittingAntTower.AcidStun));
    Assert.That(false, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
  }

  [Test]
  public void SpecialAbilityUpgradeArmorTearExplosion() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_5_ARMOR_TEAR_EXPLOSION);
    Assert.That(true, Is.EqualTo(spittingAntTower.ArmorTearExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotSlow));
  }

  [Test]
  public void SpecialAbilityUpgradeDotSlow() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_3_DOT_SLOW);
    Assert.That(true, Is.EqualTo(spittingAntTower.DotSlow));
    Assert.That(false, Is.EqualTo(spittingAntTower.DotExplosion));
  }

  [Test]
  public void SpecialAbilityUpgradeDotExplosion() {
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_5_DOT_EXPLOSION);
    Assert.That(true, Is.EqualTo(spittingAntTower.DotExplosion));
    Assert.That(false, Is.EqualTo(spittingAntTower.CamoSight));
  }

  [Test]
  public void SpecialAbilityUpgradeConstantFire() {
    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplashExplosion(spittingAntTower, splash);

    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE);
    Assert.That(true, Is.EqualTo(spittingAntTower.ContinuousAttack));
  }

  #endregion

  #region ApplyArmorTearAndCheckForAcidStunTests

  [Test]
  public void ApplyArmorTearAndCheckForAcidStunDoesNotStunWithoutAcidStun() {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: 1.0f);
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    bool result = InvokeApplyArmorTearAndCheckForAcidStun(spittingAntTower, enemy, 2.0f);
    Assert.That(result, Is.EqualTo(false));
  }

  [Test, Sequential]
  public void ApplyArmorTearAndCheckForAcidStun(
    [Values(0.0f, 1.0f, 2.0f)] float armor,
    [Values(1.0f, 1.0f, 1.0f)] float armorTear,
    [Values(false, true, false)] bool expectedResult) {
    Enemy enemy = CreateEnemy(Vector3.zero, armor: armor);
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_3_ACID_STUN);
    bool result = InvokeApplyArmorTearAndCheckForAcidStun(spittingAntTower, enemy, armorTear);
    Assert.That(result, Is.EqualTo(expectedResult));
  }

  #endregion

  #region GetEnemiesInExplosionRangeTest

  [Test]
  public void GetEnemiesInExplosionRange() {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    ObjectPool objectPool = CreateObjectPool();
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplashExplosionRange(spittingAntTower, 10.0f);
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    List<Enemy> enemiesInRange = InvokeGetEnemiesInExplosionRange(spittingAntTower, target, 10.0f);
    List<Enemy> expectedEnemiesInRange = new() { enemyInRange };

    Assert.That(enemiesInRange, Is.EqualTo(expectedEnemiesInRange));
  }

  #endregion

  #region ProcessDamageAndEffectsTests

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

    Assert.That(target.SlowDuration, Is.EqualTo(slowDuration));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));

    InvokeHandleAcidEffects(spittingAntTower, target);

    Assert.That(target.SlowDuration, Is.EqualTo(slowDuration));
    Assert.That(target.SlowPower, Is.EqualTo(slowPower));
  }

  [Test]
  public void HandleAcidEffectsAcidExplosions() {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    float enemyHp = 10000.0f;
    target.data.currHP = enemyHp;
    enemyInRange.data.currHP = enemyHp;
    enemyOutOfRange.data.currHP = enemyHp;

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerAcidExplosionRange(spittingAntTower, 10.0f);
    spittingAntTower.DamageOverTime = 1000.0f;
    SetSpittingAntTowerDotSlow(spittingAntTower, false);
    SetSpittingAntTowerDotExplosion(spittingAntTower, true);
    ParticleSystem acidExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerAcidExplosion(spittingAntTower, acidExplosion);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);

    float expectedDamage = target.MaxAcidStacks * target.AcidDamagePerStack;

    InvokeHandleAcidEffects(spittingAntTower, target);

    Assert.That(target.HP, Is.EqualTo(enemyHp - expectedDamage));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHp - expectedDamage));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
  }

  // Tests for splash effects:
  //  1. nearby enemies take damage.
  //  2. without armor tear explosion no armor tear is applied to nearby enemies.
  //  3. with armor tear explosion, armor tear is applied to nearby enemies.
  //    a. 2 and 3 can probably be the same test with parameterized values. Can combine 1 & 2.
  [Test, Sequential]
  public void HandleSplashEffects(
      [Values(false, true)] bool armorTearExplosion) {
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy enemyInRange = CreateEnemy(Vector3.zero);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(0, 100, 0));
    float enemyHp = 10000.0f;
    target.data.currHP = enemyHp;
    enemyInRange.data.currHP = enemyHp;
    enemyOutOfRange.data.currHP = enemyHp;

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(Vector3.zero);
    SetSpittingAntTowerSplashExplosionRange(spittingAntTower, 10.0f);
    SetSpittingAntTowerArmorTearExplosion(spittingAntTower, armorTearExplosion);
    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplashExplosion(spittingAntTower, splashExplosion);
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

  private void SetSpittingAntTowerSplashExplosion(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("splash", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerSplashExplosionRange(SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("splashExplosionRange", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, range);
  }

  private void SetSpittingAntTowerAcidExplosion(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("acidExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerAcidExplosionRange(SpittingAntTower spittingAntTower, float range) {
    typeof(SpittingAntTower)
        .GetField("acidExplosionRange", BindingFlags.Instance | BindingFlags.NonPublic)
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

  private void InvokeHandleAcidEffects(SpittingAntTower spittingAntTower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo handleAcidEffects = typeof(SpittingAntTower).GetMethod(
        "HandleAcidEffects",
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

  private bool InvokeApplyArmorTearAndCheckForAcidStun(SpittingAntTower spittingAntTower, Enemy enemy, float armorTear) {
    object[] args = { enemy, armorTear };
    Type[] argTypes = { typeof(Enemy), typeof(float) };
    MethodInfo applyArmorTearAndCheckForAcidStun = typeof(SpittingAntTower).GetMethod(
        "ApplyArmorTearAndCheckForAcidStun",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    return (bool) applyArmorTearAndCheckForAcidStun.Invoke(spittingAntTower, args);
  }

  #endregion
}
