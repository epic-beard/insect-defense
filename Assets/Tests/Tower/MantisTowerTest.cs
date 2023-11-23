using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class MantisTowerTest {
  MantisTower mantisTower;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    mantisTower = gameObject.AddComponent<MantisTower>();

    mantisTower.EnemiesHit = 4;
    mantisTower.ArmorPierce = 0.1f;
    mantisTower.AttackSpeed = 1;
    mantisTower.Damage = 10;
    mantisTower.Range = 20;
    mantisTower.damageDegredation = 1 / mantisTower.EnemiesHit;
    mantisTower.SetUpperRightAttack();

    Time.captureDeltaTime = 1;
  }

  #region SpecialAbilityUpgradeTests

  [Test]
  public void SpecialAbilityUpgradeDoubleSlash() {
    Assert.That(false, Is.EqualTo(mantisTower.SecondAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_1_3_DOUBLE_SLASH);

    Assert.That(true, Is.EqualTo(mantisTower.SecondAttack));
  }

  [Test]
  public void SpecialAbilityUpgradeFourArms() {
    Assert.That(false, Is.EqualTo(mantisTower.ApexAttack));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_1_5_FOUR_ARMS);

    Assert.That(true, Is.EqualTo(mantisTower.ApexAttack));
  }

  [Test]
  public void SpecialAbilityUpgradeJaggedClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.CrippleAtFullDamage));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_3_JAGGED_CLAWS);

    Assert.That(true, Is.EqualTo(mantisTower.CrippleAtFullDamage));
  }

  [Test]
  public void SpecialAbilityUpgradeSerratedClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.CanCrippleEnemy));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_2_5_SERRATED_CLAWS);

    Assert.That(true, Is.EqualTo(mantisTower.CanCrippleEnemy));
  }

  [Test]
  public void SpecialAbilityUpgradeCamoSight() {
    Assert.That(false, Is.EqualTo(mantisTower.CamoSight));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_3_CAMO_SIGHT);

    Assert.That(true, Is.EqualTo(mantisTower.CamoSight));
  }

  [Test]
  public void SpecialAbilityUpgradeVorpalClaws() {
    Assert.That(false, Is.EqualTo(mantisTower.VorpalClaw));

    mantisTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.M_3_5_VORPAL_CLAWS);

    Assert.That(true, Is.EqualTo(mantisTower.VorpalClaw));
  }

  #endregion

  [Test]
  public void SingleAttackSingleEnemy() {
    float enemyHp = 100.0f;
    Enemy enemy = CreateEnemy(Vector3.right, hp: enemyHp);

    Assert.That(enemy.HP, Is.EqualTo(100.0f));

    mantisTower.ProcessDamageAndEffects(enemy, MantisTower.MantisAttackType.UPPER_RIGHT);

    Assert.That(enemy.HP, Is.EqualTo(enemyHp - mantisTower.Damage));
  }

  // To test:
  // Hitting 1 enemy with 1 attack.
  // Hit 1 enemy with multiple attacks.
  // Hit multiple enemnies with 1 attack.
  // Hit multiple enemies with multiple attacks.
  // Cripple 1 enemy at full health.
  // Does not cripple second enemy hit.
  // Camo sight.
  // Confirm vorpal strike functions as desired.

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
    return new GameObject().AddComponent<ObjectPool>();
  }

  #endregion
}

#region MantisTowerUtils

public static class MantisTowerUtils {
  public static void SetUpperRightAttack(this MantisTower mantisTower) {
    typeof(MantisTower)
      .GetField("Attacks[MantisAttackType.UPPER_RIGHT]", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(mantisTower, 1.0f);
      
  }
}

#endregion
