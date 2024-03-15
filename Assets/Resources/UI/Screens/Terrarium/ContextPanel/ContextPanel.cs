using Assets;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextPanel : MonoBehaviour {
  public static ContextPanel Instance;

  readonly private string enemyArmorCurrentArmorLabelName = "enemy_current_armor__label";
  readonly private string enemyArmorMaxArmorLabelName = "enemy_max_armor__label";
  readonly private string enemyContextVisualElementName = "enemy_context__visualelement";
  readonly private string enemyDamageLabelName = "enemy_damage__label";
  readonly private string enemyHpCurrentHpLabelName = "enemy_current_hp__label";
  readonly private string enemyHpMaxHpLabelName = "enemy_max_hp__label";
  readonly private string enemyNameLabelName = "enemy_name__label";
  readonly private string enemyNuLabelName = "enemy_nu__label";
  readonly private string enemySizeLabelName = "enemy_size__label";
  readonly private string enemySpeedLabelName = "enemy_speed__label";
  readonly private string noContextVisualElementName = "no_context__visualelement";
  readonly private string towerDetailName = "TowerDetail";

  private UIDocument uiDocument;

  private Label enemyArmorCurrentArmorLabel;
  private Label enemyArmorMaxArmorLabel;
  private VisualElement enemyContextVisualElement;
  private Label enemyDamageLabel;
  private Label enemyHpCurrentHpLabel;
  private Label enemyHpMaxHpLabel;
  private Label enemyNameLabel;
  private Label enemyNuLabel;
  private Label enemySizeLabel;
  private Label enemySpeedLabel;
  
  private VisualElement noContextVisualElement;
  private VisualElement towerDetailVE;

  private void Awake() {
    SetVisualElements();
    Instance = this;
  }

  private void Start() {
    SetNoContextPanel();
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    enemyArmorCurrentArmorLabel = rootElement.Q<Label>(enemyArmorCurrentArmorLabelName);
    enemyArmorMaxArmorLabel = rootElement.Q<Label>(enemyArmorMaxArmorLabelName);
    enemyContextVisualElement = rootElement.Q<VisualElement>(enemyContextVisualElementName);
    enemyDamageLabel = rootElement.Q<Label>(enemyDamageLabelName);
    enemyHpCurrentHpLabel = rootElement.Q<Label>(enemyHpCurrentHpLabelName);
    enemyHpMaxHpLabel = rootElement.Q<Label>(enemyHpMaxHpLabelName);
    enemyNameLabel = rootElement.Q<Label>(enemyNameLabelName);
    enemyNuLabel = rootElement.Q<Label>(enemyNuLabelName);
    enemySizeLabel = rootElement.Q<Label>(enemySizeLabelName);
    enemySpeedLabel = rootElement.Q<Label>(enemySpeedLabelName);

    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);

    towerDetailVE = rootElement.Q<VisualElement>(towerDetailName);
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  public void SetNoContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.Flex;
  }

  public void SetTowerContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.Flex;
    noContextVisualElement.style.display = DisplayStyle.None;
  }

  public void SetEnemyContextPanel() {
    enemyContextVisualElement.style.display = DisplayStyle.Flex;
    towerDetailVE.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.None;
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
