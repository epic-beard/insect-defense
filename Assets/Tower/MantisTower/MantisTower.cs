using UnityEngine;
using static TowerAbility;

public class MantisTower : Tower {
  [SerializeField] Transform bodyMesh;
  [SerializeField] Transform rightScytheMesh;
  [SerializeField] Transform leftScytheMesh;

  public override TowerData.Type TowerType { get; set; } = TowerData.Type.MANTIS_TOWER;

  private Enemy enemy;
  private bool firing = false;

  private void Start() {
    // TODO (eric): remove once the towerdata is out and loading is done.
    DamageOverTime = 0.0f;
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch(ability) {
      case SpecialAbility.M_1_3_DOUBLE_SLASH:
        // Second scythe arm now attacks.
        break;
      case SpecialAbility.M_1_5_FOUR_ARMS:
        // Second set of scythe arms now attacks.
        break;
      case SpecialAbility.M_2_3_JAGGED_CLAWS:
        // Enemies dealt full damage are crippled.
        break;
      case SpecialAbility.M_2_5_SERRATED_CLAWS:
        // Cripple with a cooldown.
        break;
      case SpecialAbility.M_3_3_CAMO_SIGHT:
        CamoSight = true;
        break;
      case SpecialAbility.M_3_5_LONG_CLAWS:
        // Double the length of the mantis' claws.
        break;
      default:
        break;
    }
  }

  private void ProcessDamageAndEffects(Enemy target) {
    // Cause damage, modified by armor pen
    // Cause bleed, if necessary.
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
      bodyMesh.LookAt(enemy.AimPoint);
      firing = true;
    }
  }
}
