using Assets;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyDetail : MonoBehaviour {
  public static EnemyDetail Instance;

  readonly private string enemyArmorCurrentArmorLabelName = "enemy_current_armor__label";
  readonly private string enemyArmorMaxArmorLabelName = "enemy_max_armor__label";
  readonly private string enemyDamageLabelName = "enemy_damage__label";
  readonly private string enemyHpCurrentHpLabelName = "enemy_current_hp__label";
  readonly private string enemyHpMaxHpLabelName = "enemy_max_hp__label";
  readonly private string enemyNameLabelName = "enemy_name__label";
  readonly private string enemyNuLabelName = "enemy_nu__label";
  readonly private string enemySizeLabelName = "enemy_size__label";
  readonly private string enemySpeedLabelName = "enemy_speed__label";

  private UIDocument uiDocument;

  private Label enemyArmorCurrentArmorLabel;
  private Label enemyArmorMaxArmorLabel;
  private Label enemyDamageLabel;
  private Label enemyHpCurrentHpLabel;
  private Label enemyHpMaxHpLabel;
  private Label enemyNameLabel;
  private Label enemyNuLabel;
  private Label enemySizeLabel;
  private Label enemySpeedLabel;

  private void Awake() {
    SetVisualElements();
    Instance = this;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    enemyArmorCurrentArmorLabel = rootElement.Q<Label>(enemyArmorCurrentArmorLabelName);
    enemyArmorMaxArmorLabel = rootElement.Q<Label>(enemyArmorMaxArmorLabelName);
    enemyDamageLabel = rootElement.Q<Label>(enemyDamageLabelName);
    enemyHpCurrentHpLabel = rootElement.Q<Label>(enemyHpCurrentHpLabelName);
    enemyHpMaxHpLabel = rootElement.Q<Label>(enemyHpMaxHpLabelName);
    enemyNameLabel = rootElement.Q<Label>(enemyNameLabelName);
    enemyNuLabel = rootElement.Q<Label>(enemyNuLabelName);
    enemySizeLabel = rootElement.Q<Label>(enemySizeLabelName);
    enemySpeedLabel = rootElement.Q<Label>(enemySpeedLabelName);
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  // Set all appripriate text, pictures, and miscellaneous information for a specific enemy.
  public void SetContextForEnemy(Enemy enemy) {
    enemyNameLabel.text = ToTitleCase(enemy.Type.ToString());
    enemySizeLabel.text = ToTitleCase(enemy.Size.ToString());
    enemyHpCurrentHpLabel.text = enemy.HP.ToString();
    enemyHpMaxHpLabel.text = enemy.MaxHp.ToString();
    enemyArmorCurrentArmorLabel.text = enemy.Armor.ToString();
    enemyArmorMaxArmorLabel.text = enemy.MaxArmor.ToString();
    enemySpeedLabel.text = enemy.Speed.ToString();
    enemyDamageLabel.text = enemy.Damage.ToString();
    enemyNuLabel.text = enemy.Nu.ToString();
  }

  public void SubscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent += SetContextForEnemy;
  }

  public void DesbuscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent -= SetContextForEnemy;
  }
}
