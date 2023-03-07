using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class WebShootingSpiderTowerTest {

  WebShootingSpiderTower wssTower;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    gameObject.transform.position = Vector3.zero;
    wssTower = gameObject.AddComponent<WebShootingSpiderTower>();

    ParticleSystem web = new GameObject().AddComponent<ParticleSystem>();

    ProjectileHandler projectileHandler = new(web, wssTower.ProjectileSpeed, Tower.hitRange);
    wssTower.SetProjectileHandler(projectileHandler);
  }

  #region SpecialAbilityUpgradeTests

  // Test setting SlowStun.
  [Test]
  public void SpecialAbilityUpgradeSlowStun() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_3_SLOW_STUN);

    Assert.That(true, Is.EqualTo(wssTower.SlowStun));
    Assert.That(false, Is.EqualTo(wssTower.PermanentSlow));
  }

  // Test setting PermanentSlow.
  [Test]
  public void SpecialAbilityUpgradePermanentSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_1_5_PERMANENT_SLOW);

    Assert.That(true, Is.EqualTo(wssTower.PermanentSlow));
    Assert.That(false, Is.EqualTo(wssTower.LingeringSlow));
  }

  // Test setting LingeringSlow.
  [Test]
  public void SpecialAbilityUpgradeLingeringSlow() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_2_5_LINGERING_SLOW);

    Assert.That(true, Is.EqualTo(wssTower.LingeringSlow));
    Assert.That(false, Is.EqualTo(wssTower.AntiAir));
  }

  // Test setting AntiAir.
  [Test]
  public void SpecialAbilityUpgradeAntiAir() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_3_ANTI_AIR);

    Assert.That(true, Is.EqualTo(wssTower.AntiAir));
    Assert.That(false, Is.EqualTo(wssTower.AAAssist));
  }

  // Test setting AAAssist.
  [Test]
  public void SpecialAbilityUpgradeAAAssist() {
    wssTower.SpecialAbilityUpgrade(TowerAbility.SpecialAbility.WSS_3_5_AA_ASSIST);

    Assert.That(true, Is.EqualTo(wssTower.AAAssist));
  }

  #endregion

  #region AoESlowTests

  [Test]
  public void EnemieshitBySlow() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();

    Assert.That(0, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(1, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(2, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(3, Is.EqualTo(wssTower.EnemiesHitBySlow));

    wssTower.Upgrade(ability);

    Assert.That(3, Is.EqualTo(wssTower.EnemiesHitBySlow));
  }

  [Test]
  public void SlowAppliedToSecondaryTargets() {
    TowerAbility ability = CreateTowerAbilityForAoESlowTests();

    Assert.That(0.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.5f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.5f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(0.75f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(1.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));

    wssTower.Upgrade(ability);

    Assert.That(1.0f, Is.EqualTo(wssTower.SlowAppliedToSecondaryTargets));
  }

  #endregion

  #region TestHelperMethods

  private TowerAbility CreateTowerAbilityForAoESlowTests() {
    TowerAbility ability = new();
    ability.mode = TowerAbility.Mode.MULTIPLICATIVE;
    ability.attributeModifiers = new TowerAbility.AttributeModifier[0];
    ability.upgradePath = 1;

    return ability;
  }

  #endregion
}

#region WebShootingSpiderTowerUtils

public static class WebShootingSpiderTowerUtils {
  public static void SetProjectileHandler(this WebShootingSpiderTower wssTower, ProjectileHandler projectileHandler) {
    typeof(WebShootingSpiderTower)
        .GetField("projectileHandler", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(wssTower, projectileHandler);
  }
}

#endregion
