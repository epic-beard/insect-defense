using UnityEngine;
using UnityEngine.UIElements;

public class ContextPanel : MonoBehaviour {
  public static ContextPanel Instance;

  readonly private string noDetailName = "no-detail";
  readonly private string towerDetailName = "tower-detail";
  readonly private string enemyDetailName = "enemy-detail";
  readonly private string contextPanelParentClassName = "context-panel-parent";
  readonly private string towerSelectorClassName = "tower-selector";

    private UIDocument uiDocument;

  private VisualElement noDetailVE;
  private VisualElement towerDetailVE;
  private VisualElement enemyDetailVE;

  private TemplateContainer contextPanelParentVE;
    private TemplateContainer towerSelectorVE;

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

    noDetailVE = rootElement.Q<VisualElement>(noDetailName);
    towerDetailVE = rootElement.Q<VisualElement>(towerDetailName);
    enemyDetailVE = rootElement.Q<VisualElement>(enemyDetailName);

    contextPanelParentVE = rootElement.Q<TemplateContainer>(className: contextPanelParentClassName);
    towerSelectorVE = rootElement.Q<TemplateContainer>(className: towerSelectorClassName);
    }

  public void SetNoContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.None;
    noDetailVE.style.display = DisplayStyle.Flex;
    contextPanelParentVE.style.flexBasis = new StyleLength(Length.Percent(10));
    towerSelectorVE.style.flexBasis = new StyleLength(Length.Percent(70));
  }

  public void SetTowerContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.Flex;
    noDetailVE.style.display = DisplayStyle.None;
    contextPanelParentVE.style.flexBasis = new StyleLength(Length.Percent(70));
    towerSelectorVE.style.flexBasis = new StyleLength(Length.Percent(15));
  }

  public void SetEnemyContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.Flex;
    towerDetailVE.style.display = DisplayStyle.None;
    noDetailVE.style.display = DisplayStyle.None;
    contextPanelParentVE.style.flexBasis = new StyleLength(Length.Percent(70));
    towerSelectorVE.style.flexBasis = new StyleLength(Length.Percent(15));
    }
}
