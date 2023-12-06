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

    mantisTower.EnemiesHit = 4;
    mantisTower.ArmorPierce = 0.0f;
    mantisTower.AttackSpeed = 1;
    mantisTower.Damage = 10;
    mantisTower.Range = 20;
    mantisTower.damageDegredation = 1 / mantisTower.EnemiesHit;
    mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_RIGHT] = 1.0f;

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
    Assert.That(false, Is.EqualTo(mantisTower.CrippleAtFullDamage));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    Assert.True(mantisTower.CrippleAtFullDamage);
  }

  [Test]
  public void SpecialAbilityUpgradeSerratedClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.CanCrippleEnemy));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_5_SERRATED_CLAWS);

    Assert.True(mantisTower.CanCrippleEnemy);
  }

  [Test]
  public void SpecialAbilityUpgradeCamoSight() {
    Assert.That(false, Is.EqualTo(mantisTower.CamoSight));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_3_CAMO_SIGHT);

    Assert.True(mantisTower.CamoSight);
  }

  [Test]
  public void SpecialAbilityUpgradeVorpalClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.VorpalClaw));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_5_VORPAL_CLAWS);

    Assert.True(mantisTower.VorpalClaw);
  }

  #endregion

  [Test]
  public void SingleAttackSingleEnemy() {
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp));

    mantisTower.ProcessDamageAndEffects(enemy, MantisAttackType.UPPER_RIGHT);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
  }

  [Test]
  public void TwoAttacksSingleEnemy() {
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp));

    mantisTower.ProcessDamageAndEffects(enemy, MantisAttackType.UPPER_RIGHT);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));

    mantisTower.ProcessDamageAndEffects(enemy, MantisAttackType.UPPER_RIGHT);

    float hpAfterTwoHits =
        (enemyHp - mantisTower.Damage)
        - (mantisTower.Damage * mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_RIGHT]);
    Assert.That(enemy.HP, Is.EqualTo(hpAfterTwoHits));
  }

  [Test]
  public void SingleAttackTwoEnemies() {
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right, hp: enemyHp);

    mantisTower.ProcessDamageAndEffects(firstEnemy, MantisAttackType.UPPER_RIGHT);
    mantisTower.ProcessDamageAndEffects(secondEnemy, MantisAttackType.UPPER_RIGHT);

    float expectedSecondEnemyHp =
        enemyHp - (mantisTower.Damage * mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_RIGHT]);

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
    Assert.That(secondEnemy.HP, Is.EqualTo(expectedSecondEnemyHp));
  }

  [Test]
  public void TwoAttacksTwoEnemies() {
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right, hp: enemyHp);

    mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_LEFT] = 1.0f;

    mantisTower.ProcessDamageAndEffects(firstEnemy, MantisAttackType.UPPER_RIGHT);
    mantisTower.ProcessDamageAndEffects(secondEnemy, MantisAttackType.UPPER_RIGHT);
    mantisTower.ProcessDamageAndEffects(firstEnemy, MantisAttackType.UPPER_LEFT);
    mantisTower.ProcessDamageAndEffects(secondEnemy, MantisAttackType.UPPER_LEFT);

    float expectedSecondEnemyHp =
        enemyHp -
        (mantisTower.Damage * mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_RIGHT]) -
        (mantisTower.Damage * mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_LEFT]);

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - (mantisTower.Damage * 2)));
    Assert.That(secondEnemy.HP, Is.EqualTo(expectedSecondEnemyHp));
  }

  [Test]
  public void CrippleEnemyWithFullDamage() {
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    mantisTower.ProcessDamageAndEffects(enemy, MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.True(enemy.Crippled);
  }

  [Test]
  public void DoesNotCrippleEnemyWithoutFullDamage() {
    Enemy enemy = CreateEnemy(Vector3.right, armor: 10.0f, hp: enemyHp); ;

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    mantisTower.ProcessDamageAndEffects(enemy, MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.False(enemy.Crippled);
  }

  [Test]
  public void VorpalClaws() {
    mantisTower.EnemiesHit = 1;
    mantisTower.damageDegredation = 1;
    Enemy firstEnemy = CreateEnemy(Vector3.right, hp: enemyHp);
    Enemy secondEnemy = CreateEnemy(Vector3.right, hp: enemyHp);

    mantisTower.ProcessDamageAndEffects(firstEnemy, MantisTower.MantisAttackType.UPPER_RIGHT);
    mantisTower.ProcessDamageAndEffects(secondEnemy, MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
    Assert.That(secondEnemy.HP, Is.EqualTo(enemyHp));

    mantisTower.GetAttacksDictionary()[MantisAttackType.UPPER_RIGHT] = 1.0f;
    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_5_VORPAL_CLAWS);

    mantisTower.ProcessDamageAndEffects(firstEnemy, MantisTower.MantisAttackType.UPPER_RIGHT);
    mantisTower.ProcessDamageAndEffects(secondEnemy, MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.That(firstEnemy.HP, Is.EqualTo(enemyHp - (mantisTower.Damage * 2)));
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

  #endregion
}

#region MantisTowerUtils

public static class MantisTowerUtils {
  public static Dictionary<MantisAttackType, float> GetAttacksDictionary(this MantisTower mantisTower) {
    return (Dictionary<MantisAttackType, float>)typeof(MantisTower)
      .GetField("Attacks", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(mantisTower);

  }
}

#endregion
