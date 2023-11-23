using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerAbility;

public class MantisTower : Tower {
  [SerializeField] public Transform bodyMesh;

  Animator animator;

  public enum MantisAttackType {
    UPPER_RIGHT,
    UPPER_LEFT,
    LOWER_RIGHT,
    LOWER_LEFT,
  }

  public override TowerData.Type TowerType { get; set; } = TowerData.Type.MANTIS_TOWER;
  public override int EnemiesHit {
    get { return base.EnemiesHit; }
    set { base.EnemiesHit = value; }
  }

  public bool ApexAttack { get; private set; } = false;
  public bool CrippleAtFullDamage { get; private set; } = false;
  public float CrippleCooldownTime {
    get { return AttackSpeed; }
  }
  public bool CanCrippleEnemy { get; private set; } = false;
  public bool SecondAttack { get; private set; } = false;
  public bool VorpalClaw { get; private set; } = false;

  public float damageDegredation;
  // Tracker for the Mantis blade's damage degredation.
  private Dictionary<MantisAttackType, float> Attacks = new();
  private Enemy enemy;
  private bool firing = false;

  private void Start() {
    animator = GetComponent<Animator>();
    damageDegredation = 1 / EnemiesHit;

    Attacks[MantisAttackType.UPPER_RIGHT] = 1.0f;

    StartCoroutine(Attack());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.M_1_3_DOUBLE_SLASH:
        Attacks[MantisAttackType.UPPER_LEFT] = 1.0f;
        SecondAttack = true;
        break;
      case SpecialAbility.M_1_5_FOUR_ARMS:
        Attacks[MantisAttackType.LOWER_RIGHT] = 1.0f;
        Attacks[MantisAttackType.LOWER_LEFT] = 1.0f;
        ApexAttack = true;
        break;
      case SpecialAbility.M_2_3_JAGGED_CLAWS:
        CrippleAtFullDamage = true;
        break;
      case SpecialAbility.M_2_5_SERRATED_CLAWS:
        StartCoroutine(CrippleCooldown());
        CanCrippleEnemy = true;
        break;
      case SpecialAbility.M_3_3_CAMO_SIGHT:
        CamoSight = true;
        break;
      case SpecialAbility.M_3_5_VORPAL_CLAWS:
        VorpalClaw = true;
        break;
      default:
        break;
    }
  }

  public void ProcessDamageAndEffects(Enemy target, MantisAttackType attackType) {
    // Handle the "Jagged Claws" upgrade.
    if (CrippleAtFullDamage && target.Armor <= ArmorPierce && !target.Crippled) {
      target.Crippled = true;
    }
    // Handle the "Serrated Claws" upgrade.
    if (CanCrippleEnemy && !target.Crippled) {
      target.Crippled = true;
      CanCrippleEnemy = false;
    }
    // Calculate damage, keeping the "Vorpal Claws" upgrade in mind.

    if (VorpalClaw) {
      target.DamageEnemy(Damage, ArmorPierce, false);
    } else if (0.0f < Attacks[attackType]) {
      target.DamageEnemy((Damage * Attacks[attackType]), ArmorPierce, false);
      Attacks[attackType] -= damageDegredation;
    }
  }

  protected override void TowerUpdate() {
    enemy = targeting.FindTarget(
      oldTarget: enemy,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);

    if (enemy == null) {
      firing = false;
    } else {
      bodyMesh.LookAt(enemy.AimPoint - (Vector3.up * 4));
      firing = true;
    }
  }

  private void Swing() {
    animator.Play("First Attack Layer.MantisAttack");
    if (SecondAttack) {
      animator.Play("Second Attack Layer.SecondMantisAttack");
    }
    if (ApexAttack) {
      animator.Play("Third Attack Layer.ThirdMantisAttack");
      animator.Play("Fourth Attack Layer.FourthMantisAttack");
    }
  }

  private IEnumerator Attack() {
    while (true) {
      while (firing && DazzleTime <= 0.0f) {
        Attacks[MantisAttackType.UPPER_RIGHT] = 1.0f;
        Swing();

        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }


  private IEnumerator CrippleCooldown() {
    while (true) {
      while (!CanCrippleEnemy) {
        CanCrippleEnemy = true;

        yield return new WaitForSeconds(1 / CrippleCooldownTime);
      }
      yield return new WaitUntil(() => !CanCrippleEnemy);
    }
  }
}
