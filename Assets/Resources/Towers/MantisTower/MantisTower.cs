using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerAbility;

public class MantisTower : Tower {
  [SerializeField] public Transform bodyMesh;

  public override TowerData.Type Type { get; set; } = TowerData.Type.MANTIS_TOWER;
  public override int EnemiesHit {
    get { return base.EnemiesHit; }
    set { base.EnemiesHit = value; }
  }

  public bool ApexAttack { get; private set; } = false;
  public bool CrippleAtFullDamage { get; private set; } = false;
  public float CrippleCooldownSpeed { get { return AttackSpeed; } }
  public bool CanCrippleEnemy { get; private set; } = false;
  public bool SecondAttack { get; private set; } = false;
  public bool VorpalClaw { get; private set; } = false;

  public float damageDegredation;
  public enum MantisAttackType {
    UPPER_RIGHT,
    UPPER_LEFT,
    LOWER_RIGHT,
    LOWER_LEFT,
  }
  // Tracker for the Mantis blade's damage degredation.
  private Dictionary<MantisAttackType, float> Attacks = new();

  private Animator animator;
  private Enemy enemy;
  private bool firing = false;
  private Dictionary<MantisAttackType, Transform> attackOriginMap = new();

  private readonly string upperRightName = "UR Shoulder";
  private readonly string upperLeftName = "UL Shoulder";
  private readonly string lowerRightName = "LR Shoulder";
  private readonly string lowerLeftName = "LL Shoulder";

  private void Start() {
    animator = GetComponent<Animator>();
    damageDegredation = 1 / EnemiesHit;

    attackOriginMap = new() {
      { MantisAttackType.UPPER_RIGHT, this.transform.Find(upperRightName).transform },
      { MantisAttackType.UPPER_LEFT, this.transform.Find(upperLeftName).transform },
      { MantisAttackType.LOWER_RIGHT, this.transform.Find(lowerRightName).transform },
      { MantisAttackType.LOWER_LEFT, this.transform.Find(lowerLeftName).transform }
    };

    StartCoroutine(Attack());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.M_1_3_DOUBLE_SLASH:
        SecondAttack = true;
        break;
      case SpecialAbility.M_1_5_FOUR_ARMS:
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
      target.ApplyCripple();
    }
    // Handle the "Serrated Claws" upgrade.
    if (CanCrippleEnemy && !target.Crippled) {
      target.ApplyCripple();
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

  // TODO(emonzon): To complete the Mantis Tower refactor:
  //                - Remove scaling down damage, anything in AoE takes secondary damage.
  //                - Rename and reorganize tower parts.
  //                  - This includes makins sure the tower has origin points for the shoulders.
  //                - Redo animations for the mantis tower.
  //                - Add animation events to the animations, with the correct arguments.
  //                - Update tests and add new tests if appropriate.
  //                - Clean up class and remove unused code.

  public void ProcessDamageAndEffects(MantisAttackType attackType) {
    if (enemy.enabled) {
      // Hurt the enemy. Maybe mock their fashion sense.
    }

    Vector3 A = attackOriginMap[attackType].position;
    Vector3 B = enemy.transform.position;

    A.y = 0;
    B.y = 0;

    List<Enemy> potentialVictims = targeting.GetAllValidEnemiesInRange(
        enemies: ObjectPool.Instance.GetActiveEnemies(),
        towerPosition: transform.position,
        towerRange: Range,
        camoSight: CamoSight,
        antiAir: AntiAir);

    foreach (Enemy pv in potentialVictims) {
      Vector3 C = pv.transform.position;
      C.y = 0;

      Vector3 vHat = (B - A).normalized;
      float x = Vector3.Dot(C - A, vHat);
      if (x <= 0) continue;
      Vector3 w = x * vHat;
      float dist = (C - w - A).magnitude;

      if (dist < this.AreaOfEffect) {
        // Do secondary
      }
    }
  }

  private float GetSecondaryDamagePercentage(Vector2 centerOfDamage, Vector2 targetLocation) {
    return 0.0f;
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

  // Launch the animations that swing the mantis arms.
  private void Stab() {
    Attacks[MantisAttackType.UPPER_RIGHT] = 1.0f;
    animator.Play("First Attack Layer.MantisAttack");
    if (SecondAttack) {
      Attacks[MantisAttackType.UPPER_LEFT] = 1.0f;
      animator.Play("Second Attack Layer.SecondMantisAttack");
    }
    if (ApexAttack) {
      Attacks[MantisAttackType.LOWER_RIGHT] = 1.0f;
      animator.Play("Third Attack Layer.ThirdMantisAttack");

      Attacks[MantisAttackType.LOWER_LEFT] = 1.0f;
      animator.Play("Fourth Attack Layer.FourthMantisAttack");
    }
  }

  private IEnumerator Attack() {
    while (true) {
      while (firing && DazzleTime <= 0.0f) {
        Stab();

        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
      yield return new WaitUntil(() => firing);
    }
  }


  // Monitor and refresh the cripple ability as needed.
  private IEnumerator CrippleCooldown() {
    while (true) {
      while (!CanCrippleEnemy) {
        CanCrippleEnemy = true;

        yield return new WaitForSeconds(1 / CrippleCooldownSpeed);
      }
      yield return new WaitUntil(() => !CanCrippleEnemy);
    }
  }
}
