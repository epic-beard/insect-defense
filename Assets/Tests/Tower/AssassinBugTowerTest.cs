using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class AssassinBugTowerTest {
  AssassinBugTower tower;
  ObjectPool objectPool;

  [SetUp]
  public void SetUp() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    tower = gameObject.AddComponent<AssassinBugTower>();
    objectPool = new GameObject().AddComponent<ObjectPool>();
    ObjectPool.Instance = objectPool;

    GameStateManager gsm = new GameObject().AddComponent<GameStateManager>();
    GameStateManager.Instance = gsm;

    Time.captureDeltaTime = 1;
    TowerManager.Instance = gameObject.AddComponent<TowerManager>();
  }

  #region SpecialAbilityUpgradeTests

  [Test]
  public void ArmoredEnemyBonusTest() {
    tower.Damage = 1.0f;
    Enemy target = CreateEnemy(Vector3.zero);
    objectPool.SetActiveEnemies(new HashSet<Enemy> { target });

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage));

    Assert.False(tower.ArmoredEnemyBonus);
    tower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.AB_1_3_ARMORED_ENEMY_BONUS);
    Assert.True(tower.ArmoredEnemyBonus);

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * 2));

    target.Armor = 0.0f;

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage));
  }

  [Test]
  public void ArmorDepletionBonusTest() {
    tower.Damage = 1.0f;
    Enemy target = CreateEnemy(Vector3.zero);
    target.Armor = 0.0f;
    objectPool.SetActiveEnemies(new HashSet<Enemy> { target });

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage));

    Assert.False(tower.ArmorDepletionBonus);
    tower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.AB_1_5_OVER_PENETRATION_BONUS);
    Assert.True(tower.ArmorDepletionBonus);

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * 2));

    target.Armor = 1.0f;

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage));
  }

  [Test]
  public void MultiHitBonusTest() {
    tower.Damage = 1.0f;
    Enemy target = CreateEnemy(Vector3.zero);
    objectPool.SetActiveEnemies(new HashSet<Enemy> { target });

    for (int i = 0; i <= 5; i++) {
      Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage));
    }

    Assert.False(tower.MultiHitBonus);
    tower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.AB_3_3_CONSECUTIVE_HITS);
    Assert.True(tower.MultiHitBonus);

    for (int i = 0; i <= 5; i++) {
      float hitBonus = 1.0f + 0.2f * i;
      Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * hitBonus));
    }

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * 2.0f));
  }

  [Test]
  public void CriticalMultiHitTest() {
    tower.Damage = 1.0f;
    Enemy target = CreateEnemy(Vector3.zero);
    Enemy alt = CreateEnemy(Vector3.right);
    objectPool.SetActiveEnemies(new HashSet<Enemy> { target, alt });

    tower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.AB_3_3_CONSECUTIVE_HITS);

    for (int i = 0; i <= 5; i++) {
      float hitMult = 1.0f + 0.2f * i;
      Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * hitMult));
    }

    Assert.That(tower.InvokeProcessDamageAndEffects(alt), Is.EqualTo(tower.Damage));

    Assert.False(tower.CriticalMultiHit);
    tower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.AB_3_5_COMBO_FINISHER);
    Assert.True(tower.CriticalMultiHit);

    for (int i = 0; i < 5; i++) {
      float hitMult = 1.0f + 0.2f * i;
      Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * hitMult));
    }

    Assert.That(tower.InvokeProcessDamageAndEffects(target), Is.EqualTo(tower.Damage * 4));
  }

  #endregion

  [Test]
  public void MoveToTest() {
    Transform mesh = tower.transform;
    tower.SetAssassinMesh(mesh);
    float speed = 1.0f;
    Vector3 startPosition = tower.GetAssassinMesh().position;
    Vector3 endPosition = 100 * Vector3.right;
    float distance = (endPosition - startPosition).magnitude;
    Assert.False(tower.InvokeMoveTo(endPosition, speed));
    float newDistance = (endPosition - tower.GetAssassinMesh().position).magnitude;
    float travel = distance - newDistance;
    float expectedTravel = Time.deltaTime * speed;
    Assert.True(Mathf.Abs(travel - expectedTravel) < 0.001f);
  }

  [Test]
  public void MoveToMeleeRangeTest() {
    Transform mesh = tower.transform;
    tower.SetAssassinMesh(mesh);
    float speed = 1.0f;
    Vector3 startPosition = tower.GetAssassinMesh().position;
    Vector3 endPosition = 2 * Vector3.right;
    float distance = (endPosition - startPosition).magnitude;
    Assert.True(tower.InvokeMoveTo(endPosition, speed));
    float newDistance = (endPosition - tower.GetAssassinMesh().position).magnitude;
    float travel = distance - newDistance;
    Assert.That(travel, Is.EqualTo(0.0f));
  }

  // Create and return an enemy with optional args.
  private Enemy CreateEnemy(
      Vector3 position) {
    float maxArmor = 1.0f;
    float hp = 10.0f;
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      maxArmor = maxArmor,
      maxHP = hp,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.SetAimPoint(enemy.transform);
    enemy.Data = data;

    Waypoint start = new GameObject().AddComponent<Waypoint>();
    start.transform.position = position;

    enemy.Initialize(start);

    return enemy;
  }
}

#region AssassinBugTowerUtils
public static class AssassinBugTowerUtils {
  public static float InvokeProcessDamageAndEffects(this AssassinBugTower tower, Enemy enemy) {
    object[] args = { enemy };
    Type[] argTypes = { typeof(Enemy) };
    MethodInfo processDamageAndEffects = typeof(AssassinBugTower).GetMethod(
      "ProcessDamageAndEffects",
      BindingFlags.NonPublic | BindingFlags.Instance,
      null, CallingConventions.Standard, argTypes, null);
    return (float)processDamageAndEffects.Invoke(tower, args);
  }

  public static bool InvokeMoveTo(this AssassinBugTower tower, Vector3 endPosition, float speed) {
    object[] args = { endPosition, speed };
    Type[] argTypes = { typeof(Vector3), typeof(float) };
    MethodInfo moveTo = typeof(AssassinBugTower).GetMethod(
      "MoveTo",
      BindingFlags.NonPublic | BindingFlags.Instance,
      null, CallingConventions.Standard, argTypes, null);
    return (bool)moveTo.Invoke(tower, args);
  }

  public static void SetAssassinMesh(this AssassinBugTower tower, Transform mesh) {
    typeof(AssassinBugTower)
      .GetField("assassinMesh", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(tower, mesh);
  }

  public static Transform GetAssassinMesh(this AssassinBugTower tower) {
    return (Transform)typeof(AssassinBugTower)
      .GetField("assassinMesh", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(tower);
  }
}
#endregion
