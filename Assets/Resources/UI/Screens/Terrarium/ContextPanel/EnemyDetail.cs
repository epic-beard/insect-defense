using UnityEngine;
using UnityEngine.UIElements;

public class EnemyDetail : MonoBehaviour {
  public static EnemyDetail Instance;

  readonly private string enemyNameLabelName = "enemy-name";
  readonly private string enemySizeLabelName = "enemy-size";
  readonly private string enemyHpCurrentHpLabelName = "enemy-current-hp";
  readonly private string enemyHpMaxHpLabelName = "enemy-max-hp";
  readonly private string enemyArmorCurrentArmorLabelName = "enemy-current-armor";
  readonly private string enemyArmorMaxArmorLabelName = "enemy-max-armor";
  readonly private string enemySpeedLabelName = "enemy-speed";
  readonly private string enemyDamageLabelName = "enemy-damage";
  readonly private string enemyNuLabelName = "enemy-nu";

  readonly private string enemyStatSizeName = "enemy-stat-icon-size";
  readonly private string enemyStatHpName = "enemy-stat-icon-hp";
  readonly private string enemyStatArmorName = "enemy-stat-icon-armor";
  readonly private string enemyStatSpeedName = "enemy-stat-icon-speed";
  readonly private string enemyStatDamageName = "enemy-stat-icon-damage";
  readonly private string enemyStatNuName = "enemy-stat-icon-nu";

  private UIDocument uiDocument;

  private Label enemyNameLabelVE;
  private Label enemySizeLabelVE;
  private Label enemyHpCurrentHpLabelVE;
  private Label enemyHpMaxHpLabelVE;
  private Label enemyArmorCurrentArmorLabelVE;
  private Label enemyArmorMaxArmorLabelVE;
  private Label enemySpeedLabelVE;
  private Label enemyDamageLabelVE;
  private Label enemyNuLabelVE;

  private ButtonWithTooltip enemyStatSizeIcon;
  private ButtonWithTooltip enemyStatHpIcon;
  private ButtonWithTooltip enemyStatArmorIcon;
  private ButtonWithTooltip enemyStatSpeedIcon;
  private ButtonWithTooltip enemyStatDamageIcon;
  private ButtonWithTooltip enemyStatNuIcon;

  private void Awake() {
    SetVisualElements();
    Instance = this;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    enemyArmorCurrentArmorLabelVE = rootElement.Q<Label>(enemyArmorCurrentArmorLabelName);
    enemyArmorMaxArmorLabelVE = rootElement.Q<Label>(enemyArmorMaxArmorLabelName);
    enemyDamageLabelVE = rootElement.Q<Label>(enemyDamageLabelName);
    enemyHpCurrentHpLabelVE = rootElement.Q<Label>(enemyHpCurrentHpLabelName);
    enemyHpMaxHpLabelVE = rootElement.Q<Label>(enemyHpMaxHpLabelName);
    enemyNameLabelVE = rootElement.Q<Label>(enemyNameLabelName);
    enemyNuLabelVE = rootElement.Q<Label>(enemyNuLabelName);
    enemySizeLabelVE = rootElement.Q<Label>(enemySizeLabelName);
    enemySpeedLabelVE = rootElement.Q<Label>(enemySpeedLabelName);

    enemyStatSizeIcon = rootElement.Q<ButtonWithTooltip>(enemyStatSizeName);
    enemyStatHpIcon = rootElement.Q<ButtonWithTooltip>(enemyStatHpName);
    enemyStatArmorIcon = rootElement.Q<ButtonWithTooltip>(enemyStatArmorName);
    enemyStatSpeedIcon = rootElement.Q<ButtonWithTooltip>(enemyStatSpeedName);
    enemyStatDamageIcon = rootElement.Q<ButtonWithTooltip>(enemyStatDamageName);
    enemyStatNuIcon = rootElement.Q<ButtonWithTooltip>(enemyStatNuName);

    enemyStatSizeIcon.TooltipText = "Size";
    enemyStatHpIcon.TooltipText = "Health";
    enemyStatArmorIcon.TooltipText = "Armor";
    enemyStatSpeedIcon.TooltipText = "Speed";
    enemyStatDamageIcon.TooltipText = "Damage";
    enemyStatNuIcon.TooltipText = "Nu";
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  // Set all appripriate text, pictures, and miscellaneous information for a specific enemy.
  public void SetContextForEnemy(Enemy enemy) {
    enemyNameLabelVE.text = ToTitleCase(enemy.Type.ToString());
    enemySizeLabelVE.text = ToTitleCase(enemy.Size.ToString());
    enemyHpCurrentHpLabelVE.text = enemy.HP.ToString();
    enemyHpMaxHpLabelVE.text = enemy.MaxHp.ToString();
    enemyArmorCurrentArmorLabelVE.text = enemy.Armor.ToString();
    enemyArmorMaxArmorLabelVE.text = enemy.MaxArmor.ToString();
    enemySpeedLabelVE.text = enemy.Speed.ToString();
    enemyDamageLabelVE.text = enemy.Damage.ToString();
    enemyNuLabelVE.text = enemy.Nu.ToString();
  }

  public void SubscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent += SetContextForEnemy;
  }

  public void DesbuscribeToEnemyStateBroadcast(Enemy enemy) {
    enemy.StatChangedEvent -= SetContextForEnemy;
  }
}
