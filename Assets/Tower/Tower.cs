using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
  protected Dictionary<TowerData.Stat, float> attributes;
  public float Damage { get { return attributes[TowerData.Stat.DAMAGE]; } }
  public float Range { get { return attributes[TowerData.Stat.RANGE]; } }
  public float AreaOfEffect { get { return attributes[TowerData.Stat.AREA_OF_EFFECT]; } }
  public float AttackSpeed { get { return attributes[TowerData.Stat.ATTACK_SPEED]; } }
  public float ArmorPierce { get { return attributes[TowerData.Stat.ATTACK_SPEED]; } }
  public float ArmorTear { get { return attributes[TowerData.Stat.ARMOR_TEAR]; } }
  public float DamageOverTime { get { return attributes[TowerData.Stat.DAMAGE_OVER_TIME]; } }
  protected Dictionary<TowerData.TowerAbility, bool> towerAbilities = new Dictionary<TowerData.TowerAbility, bool> {
    { TowerData.TowerAbility.CAMO_SIGHT, false },
    { TowerData.TowerAbility.ANTI_AIR, false },
    { TowerData.TowerAbility.CRIPPLE, false }
  };
  int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-4.

  public void Upgrade(Ability ability) {
    if (ability.Mode == TowerData.Mode.SPECIAL) {
      SpecialAbilityUpgrade(ability.SpecialAbility);
    } else {
      foreach (Ability.AttributeModifier mod in ability.AttributeModifiers) {
        attributes[mod.attribute] *= mod.mult;
      }
    }

    upgradeLevels[ability.UpgradePath]++;
  }

  public virtual void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {}
}
