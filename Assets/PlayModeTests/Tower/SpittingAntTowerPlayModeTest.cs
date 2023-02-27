using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.GraphicsBuffer;

public class SpittingAntTowerPlayModeTest {

  // Play test for the basic splash attack. This test focuses on those effects only available to the
  // splash effect. It does not delve into the DoT effects, SA_2_3_DOT_SLOW and SA_2_5_DOT_EXPLOSION should
  // not be true for this test.
  [UnityTest]
  public IEnumerator SplashAttackTest() {
    // Ensure enemies won't die in this test.
    float enemyHp = 1000.0f;
    float enemyArmor = 90.0f;
    EnemyData.Size enemySize = EnemyData.Size.TINY;

    Enemy target = CreateEnemy(Vector3.zero, hp: enemyHp, armor: enemyArmor - 5.0f, size: enemySize);
    Enemy enemyInRange = CreateEnemy(Vector3.right, hp: enemyHp, armor: enemyArmor, size: enemySize);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(100, 0, 0), hp: enemyHp, armor: enemyArmor, size: enemySize);

    SpittingAntTower spittingAntTower = CreateSpittingAntTower(
      Vector3.up,
      attackSpeed : 10.0f,
      areaOfEffect : 10.0f,
      armorTear : enemyArmor * 0.8f,
      damage : 100.0f,
      range : 10.0f);

    // These four simple additions are required for the tower to not error out. 
    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    SetSpittingAntTowerUpperMesh(spittingAntTower, upperMesh);

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplash(spittingAntTower, splash);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplashExplosion(spittingAntTower, splashExplosion);

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    SetSpittingAntTowerBeam(spittingAntTower, beam);

    // Create the object pool and construct activeEnemies appropriately.
    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);
    target.pool = objectPool;
    enemyInRange.pool = objectPool;
    enemyOutOfRange.pool = objectPool;

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHp));
    Assert.That(target.Armor, Is.LessThan(enemyArmor));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    // Cache target's new hp and enable armor tear explosion for testing that.
    float damagedTargetHP = target.HP;
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_1_5_ARMOR_TEAR_EXPLOSION);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(damagedTargetHP));
    Assert.That(target.Armor, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHp));
    Assert.That(enemyInRange.Armor, Is.LessThan(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  // Play test for the continuous attack and the acid DoT effects.
  [UnityTest]
  public IEnumerator BeamAttackTest() {
    float enemyHp = 100000.0f;
    float enemyArmor = 90.0f;
    EnemyData.Size enemySize = EnemyData.Size.NORMAL;

    Enemy target = CreateEnemy(Vector3.zero, hp: enemyHp, armor: enemyArmor - 5.0f, size: enemySize);
    Enemy enemyInRange = CreateEnemy(Vector3.right, hp: enemyHp, armor: enemyArmor, size: enemySize);
    Enemy enemyOutOfRange = CreateEnemy(new Vector3(100, 0, 0), hp: enemyHp, armor: enemyArmor, size: enemySize);

    float towerDamage = 100.0f;
    SpittingAntTower spittingAntTower = CreateSpittingAntTower(
      Vector3.up,
      attackSpeed: 10.0f,
      areaOfEffect: 10.0f,
      armorTear: enemyArmor * 0.8f,
      damage: towerDamage,
      damageOverTime: 50.0f,
      range: 10.0f,
      slowDuration: 10.0f,
      slowPower: 0.5f); ;

    // These four simple additions are required for the tower to not error out. 
    MeshRenderer upperMesh = new GameObject().AddComponent<MeshRenderer>();
    SetSpittingAntTowerUpperMesh(spittingAntTower, upperMesh);

    ParticleSystem splash = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerSplash(spittingAntTower, splash);

    ParticleSystem splashExplosion = new GameObject().AddComponent<ParticleSystem>();
    SetSpittingAntTowerAcidExplosion(spittingAntTower, splashExplosion);

    LineRenderer beam = new GameObject().AddComponent<LineRenderer>();
    SetSpittingAntTowerBeam(spittingAntTower, beam);

    // Create the object pool and construct activeEnemies appropriately.
    ObjectPool objectPool = CreateObjectPool();
    HashSet<Enemy> activeEnemies = new() { enemyInRange, enemyOutOfRange, target };
    SetObjectPoolActiveEnemies(objectPool, activeEnemies);
    SetSpittingAntTowerObjectPool(spittingAntTower, objectPool);
    target.pool = objectPool;
    enemyInRange.pool = objectPool;
    enemyOutOfRange.pool = objectPool;

    // Turn on continuous fire.
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_3_5_CONSTANT_FIRE);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(enemyHp));
    Assert.That(target.Armor, Is.LessThan(enemyArmor));
    Assert.That(target.SlowDuration, Is.EqualTo(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    // Cache target's new stats and enable the acid dot based slow.
    float damagedTargetHP = target.HP;
    float damagedTargetArmor = target.Armor;
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_3_DOT_SLOW);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(damagedTargetHP));
    Assert.That(target.Armor, Is.LessThan(damagedTargetArmor));
    Assert.That(target.SlowDuration, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    // Cache the target's new stats and enable the acid dot based explosion.
    damagedTargetHP = target.HP;
    damagedTargetArmor= target.Armor;
    spittingAntTower.SpecialAbilityUpgrade(Ability.SpecialAbilityEnum.SA_2_5_DOT_EXPLOSION);

    yield return new WaitForSeconds(0.11f);

    Assert.That(target.HP, Is.LessThan(damagedTargetHP));
    Assert.That(target.Armor, Is.LessThanOrEqualTo(damagedTargetArmor));
    Assert.That(target.SlowDuration, Is.GreaterThan(0.0f));
    Assert.That(enemyInRange.HP, Is.LessThan(enemyHp));
    Assert.That(enemyInRange.Armor, Is.EqualTo(enemyArmor));
    Assert.That(enemyOutOfRange.HP, Is.EqualTo(enemyHp));
    Assert.That(enemyOutOfRange.Armor, Is.EqualTo(enemyArmor));

    yield return null;
  }

  #region TestHelperMethods

  // Create a spitting ant tower with all the values explicitly set for clarity.
  private SpittingAntTower CreateSpittingAntTower(
      Vector3 position,
      Targeting.Priority priority = Targeting.Priority.LEASTARMOR,
      float attackSpeed = 0.0f,
      float areaOfEffect = 0.0f,
      float armorTear = 0.0f,
      float damage = 0.0f,
      float damageOverTime = 0.0f,
      float range = 0.0f,
      float projectileSpeed = 100.0f,  // To cover distance as quickly as possible.
      float slowDuration = 0.0f,
      float slowPower = 0.0f,
      float stunTime = 0.0f) {
    GameObject gameObject = new();
    gameObject.transform.position = position;
    SpittingAntTower spittingAntTower = gameObject.AddComponent<SpittingAntTower>();

    spittingAntTower.priority = priority;
    spittingAntTower.AttackSpeed = attackSpeed;
    spittingAntTower.AreaOfEffect = areaOfEffect;
    spittingAntTower.ArmorTear = armorTear;
    spittingAntTower.Damage = damage;
    spittingAntTower.DamageOverTime = damageOverTime;
    spittingAntTower.Range = range;
    spittingAntTower.ProjectileSpeed = projectileSpeed;
    spittingAntTower.SlowDuration = slowDuration;
    spittingAntTower.SlowPower = slowPower;
    spittingAntTower.StunTime = stunTime;

    return spittingAntTower;
  }

  // Create and return an enemy with optional args, create and set the prev waypoing based on position.
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
    enemy.data = data;
    gameObject.SetActive(true);

    return enemy;
  }

  Waypoint CreateWaypoint(Vector3 position) {
    Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
    waypoint.transform.position = position;
    return waypoint;
  }

  private ObjectPool CreateObjectPool() {
    return new GameObject().AddComponent<ObjectPool>();
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

  private void SetSpittingAntTowerAcidExplosion(SpittingAntTower spittingAntTower, ParticleSystem particleSystem) {
    typeof(SpittingAntTower)
        .GetField("acidExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, particleSystem);
  }

  private void SetSpittingAntTowerBeam(SpittingAntTower spittingAntTower, LineRenderer lineRenderer) {
    typeof(SpittingAntTower)
        .GetField("beam", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, lineRenderer);
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

  private void SetSpittingAntTowerUpperMesh(SpittingAntTower spittingAntTower, MeshRenderer meshRenderer) {
    typeof(SpittingAntTower)
        .GetField("upperMesh", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(spittingAntTower, meshRenderer.transform);
  }

  #endregion
}
