using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static MantisTower;

public class MantisTowerTest {

  private MantisTower mantisTower;
  private float enemyHp = 100.0f;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    mantisTower = gameObject.AddComponent<MantisTower>();

    mantisTower.ArmorPierce = 0.0f;
    mantisTower.AttackSpeed = 1;
    mantisTower.Damage = 10;
    mantisTower.Range = 20;
    mantisTower.AreaOfEffect = 10.0f;
    mantisTower.CamoSight = false;
    mantisTower.AntiAir = false;

    mantisTower.GetAttackOriginMap()[MantisAttackType.UPPER_RIGHT] = gameObject.transform;
    mantisTower.GetAttackOriginMap()[MantisAttackType.UPPER_LEFT] = gameObject.transform;
    mantisTower.GetAttackOriginMap()[MantisAttackType.LOWER_RIGHT] = gameObject.transform;
    mantisTower.GetAttackOriginMap()[MantisAttackType.LOWER_LEFT] = gameObject.transform;

    Time.captureDeltaTime = 1;
  }

  #region SpecialAbilityUpgradeTests

  [Test]
  public void SpecialAbilityUpgradeDoubleSlash() {
    Assert.That(false, Is.EqualTo(mantisTower.SecondAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_1_3_DOUBLE_SLASH);

    Assert.True(mantisTower.SecondAttack);
  }

  [Test]
  public void SpecialAbilityUpgradeFourArms() {
    Assert.That(false, Is.EqualTo(mantisTower.ApexAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_1_5_FOUR_ARMS);

    Assert.True(mantisTower.ApexAttack);
  }

  [Test]
  public void SpecialAbilityUpgradeJaggedClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.CrippleAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    Assert.True(mantisTower.CrippleAttack);
  }

  [Test]
  public void SpecialAbilityUpgradeSerratedClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.AoECrippleAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_5_BLOODY_EXECUTION);

    Assert.True(mantisTower.AoECrippleAttack);
  }

  [Test]
  public void SpecialAbilityUpgradeCamoSight() {
    Assert.That(false, Is.EqualTo(mantisTower.CamoSight));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_2_CAMO_SIGHT);

    Assert.True(mantisTower.CamoSight);
  }

  [Test]
  public void SpecialAbilityUpgradeVorpalClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.Shrike));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_5_SHRIKE);

    Assert.True(mantisTower.Shrike);
  }

  #endregion

  [Test]
  public void SingleAttackSingleEnemy() {
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemy };
    objectPool.SetActiveEnemies(activeEnemies);
    mantisTower.SetEnemy(enemy);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp));

    mantisTower.ProcessDamageAndEffects(MantisAttackType.UPPER_RIGHT);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
  }

  [Test]
  public void SingleAttackTwoEnemies() {
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right, hp: enemyHp);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { firstEnemy, secondEnemy };
    objectPool.SetActiveEnemies(activeEnemies);
    mantisTower.SetEnemy(firstEnemy);

    mantisTower.ProcessDamageAndEffects(MantisAttackType.UPPER_RIGHT);

    float expectedSecondEnemyHp = enemyHp - mantisTower.SecondaryDamage;

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
    Assert.That(secondEnemy.HP, Is.EqualTo(expectedSecondEnemyHp));
  }

  [Test]
  public void CrippleEnemyWhenOffCooldown() {
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemy };
    objectPool.SetActiveEnemies(activeEnemies);
    mantisTower.SetEnemy(enemy);

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    mantisTower.ProcessDamageAndEffects(MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.True(enemy.Crippled);
  }

  [Test]
  public void DoesNotCrippleEnemyIfNotOnCooldown() {
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right * 100, hp: enemyHp);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { firstEnemy, secondEnemy };
    objectPool.SetActiveEnemies(activeEnemies);
    mantisTower.SetEnemy(firstEnemy);

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    mantisTower.ProcessDamageAndEffects(MantisAttackType.UPPER_RIGHT);
    
    Assert.True(firstEnemy.Crippled);

    firstEnemy.transform.position = Vector3.right * 100;
    secondEnemy.transform.position = Vector3.right;

    mantisTower.SetEnemy(secondEnemy);
    mantisTower.ProcessDamageAndEffects(MantisAttackType.UPPER_RIGHT);

    Assert.False(secondEnemy.Crippled);
  }

  [Test]
  public void VorpalClaws() {
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right, hp: enemyHp);

    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { firstEnemy, secondEnemy };
    objectPool.SetActiveEnemies(activeEnemies);
    mantisTower.SetEnemy(firstEnemy);

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_5_SHRIKE);

    mantisTower.ProcessDamageAndEffects(MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
    Assert.That(secondEnemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
  }

  #region TestHelperMethods

  private Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 100.0f) {
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
    ObjectPool pool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = pool;
    return pool;
  }

  #endregion
}

#region MantisTowerUtils

public static class MantisTowerUtils {

  public static void SetEnemy(this MantisTower mantisTower, Enemy enemy) {
    typeof(MantisTower)
        .GetField("enemy", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(mantisTower, enemy);
  }

  public static Dictionary<MantisAttackType, Transform> GetAttackOriginMap(this MantisTower mantisTower) {
    return (Dictionary<MantisAttackType, Transform>)typeof(MantisTower)
        .GetField("attackOriginMap", BindingFlags.Instance | BindingFlags.NonPublic)
        .GetValue(mantisTower);
  }
}

#endregion
