using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpittingAntTowerPlayModeTest {

  SpittingAntTower spittingAntTower;
  Enemy target;
  Enemy enemyInRange;
  Enemy enemyOutOfRange;

  readonly float enemyHP = 100000.0f;
  readonly float enemyArmor = 90.0f;
  readonly float targetArmor = 85.0f;

  [SetUp]
  public void Setup() {
    // Ensure enemies won't die in this test.
    EnemyData.Size enemySize = EnemyData.Size.NORMAL;

    target = CreateEnemy(Vector3.zero, hp: enemyHP, armor: targetArmor, size: enemySize);
    enemyInRange = CreateEnemy(Vector3.right, hp: enemyHP, armor: enemyArmor, size: enemySize);
    enemyOutOfRange = CreateEnemy(Vector3.right * 100, hp: enemyHP, armor: enemyArmor, size: enemySize);

    spittingAntTower = CreateSpittingAntTower(Vector3.up);

    // Mandatory setup.
    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    spittingAntTower.SetUpperMesh(upperMesh);

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplash(splash);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetSplashExplosion(splashExplosion);

    ParticleSystem acidExplosion = new GameObject().AddComponent<ParticleSystem>();
    spittingAntTower.SetAcidExplosion(acidExplosion);

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    spittingAntTower.SetBeam(beam);

    // Create the object pool and construct activeEnemies appropriately.
    ObjectPool objectPool = new GameObject().AddComponent<ObjectPool>();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    objectPool.SetActiveEnemies(activeEnemies);
    spittingAntTower.SetObjectPool(objectPool);
  }

  // Play test for the basic splash attack. This test focuses on those effects only available to the
  // splash effect. It does not delve into the DoT effects, SA_2_3_DOT_SLOW and SA_2_5_DOT_EXPLOSION should
  // not be true for this test.
  [UnityTest]
  public IEnumerator SplashAttackTestWithNoArmorTearExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        armorTear: target.Armor * 0.8f,
        damage: 100.0f,
        range: 10.0f);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHP));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHP));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  // Like the above, this test focuses on splash, but includs the ArmorTearExplosion to make sure it damages
  // the armor of nearby enemies.
  [UnityTest]
  public IEnumerator SplashAttackTestWithArmorTearExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        armorTear: target.Armor * 0.8f,
        damage: 100.0f,
        range: 10.0f);

    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_1_5_ARMOR_TEAR_EXPLOSION);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHP));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHP));
    Assert.That(enemyInRange.Armor, Is.LessThan(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  // Play test for the continuous attack and the acid DoT effects.
  [UnityTest]
  public IEnumerator BeamAttackTestNoSlowOrExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 50.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHP));
    Assert.That(target.Armor, Is.LessThan(enemyArmor));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  // Make sure that the target is slowed when the dot reaches max.
  [UnityTest]
  public IEnumerator BeamAttackTestWithSlowNoExplosion() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 100.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_3_DOT_SLOW);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHP));
    Assert.That(target.Armor, Is.LessThan(targetArmor));
    Assert.That(target.SlowDuration, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  // Make sure nearby enemies are damaged by the acid explosion when dot reaches max.
  [UnityTest]
  public IEnumerator BeamAttackTestWithExplosionNoSlow() {
    SetSpittingAntTowerProperties(
        spittingAntTower,
        attackSpeed: 10.0f,
        areaOfEffect: 10.0f,
        armorTear: enemyArmor,
        damage: 100.0f,
        damageOverTime: 100.0f,
        range: 10.0f,
        slowDuration: 10.0f,
        slowPower: 0.5f);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_3_5_CONSTANT_FIRE);
    spittingAntTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.SA_2_5_DOT_EXPLOSION);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHP));
    Assert.That(target.Armor, Is.LessThanOrEqualTo(targetArmor));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHP));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHP));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  #region TestHelperMethods

  // Create a spitting ant tower with all the values explicitly set for clarity.
  private SpittingAntTower CreateSpittingAntTower(Vector3 position) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    SpittingAntTower spittingAntTower = gameObject.AddComponent<SpittingAntTower>();

    spittingAntTower.Priority = Targeting.Priority.LEASTARMOR;
    spittingAntTower.ProjectileSpeed = 100.0f;  // To cover distance as quickly as possible.

    return spittingAntTower;
  }

  private void SetSpittingAntTowerProperties(
      SpittingAntTower spittingAntTower,
      float attackSpeed = 0.0f,
      float areaOfEffect = 0.0f,
      float armorTear = 0.0f,
      float damage = 0.0f,
      float damageOverTime = 0.0f,
      float range = 0.0f,
      float slowDuration = 0.0f,
      float slowPower = 0.0f,
      float stunTime = 0.0f) {
    spittingAntTower.AttackSpeed = attackSpeed;
    spittingAntTower.AreaOfEffect = areaOfEffect;
    spittingAntTower.ArmorTear = armorTear;
    spittingAntTower.Damage = damage;
    spittingAntTower.DamageOverTime = damageOverTime;
    spittingAntTower.Range = range;
    spittingAntTower.SlowDuration = slowDuration;
    spittingAntTower.SlowPower = slowPower;
    spittingAntTower.StunTime = stunTime;
  }

  // Create and return an enemy with optional args, create and set the prev waypoint based on position.
  private Enemy CreateEnemy(
      Vector3 position,
      float armor = 0.0f,
      float hp = 1.0f,
      EnemyData.Size size = EnemyData.Size.NORMAL) {
    GameObject gameObject = new();
    gameObject.SetActive(false);

    gameObject.transform.position = position;
    Enemy enemy = gameObject.AddComponent<Enemy>();
    Waypoint waypoint = CreateWaypoint(position);
    enemy.PrevWaypoint = waypoint;

    EnemyData data = new() {
      maxArmor = armor,
      maxHP = hp,
      size = size,
    };
    enemy.Data = data;
    gameObject.SetActive(true);

    return enemy;
  }

  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  #endregion
}
