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
    enemyDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    enemyDetailVE.AddToClassList("context-sub-panel-collapsed");

    towerDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    towerDetailVE.AddToClassList("context-sub-panel-collapsed");

    noDetailVE.RemoveFromClassList("context-sub-panel-collapsed");
    noDetailVE.AddToClassList("context-sub-panel-expanded");

    contextPanelParentVE.AddToClassList("context-panel-parent-collapsed");
    towerSelectorVE.RemoveFromClassList("tower-selector-collapsed");
  }

  public void SetTowerContextPanel() {
    enemyDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    enemyDetailVE.AddToClassList("context-sub-panel-collapsed");

    towerDetailVE.RemoveFromClassList("context-sub-panel-collapsed");
    towerDetailVE.AddToClassList("context-sub-panel-expanded");

    noDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    noDetailVE.AddToClassList("context-sub-panel-collapsed");

    contextPanelParentVE.RemoveFromClassList("context-panel-parent-collapsed");
    towerSelectorVE.AddToClassList("tower-selector-collapsed");
  }

  public void SetEnemyContextPanel() {
    enemyDetailVE.RemoveFromClassList("context-sub-panel-collapsed");
    enemyDetailVE.AddToClassList("context-sub-panel-expanded");

    towerDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    towerDetailVE.AddToClassList("context-sub-panel-collapsed");

    noDetailVE.RemoveFromClassList("context-sub-panel-expanded");
    noDetailVE.AddToClassList("context-sub-panel-collapsed");

    contextPanelParentVE.RemoveFromClassList("context-panel-parent-collapsed");
    towerSelectorVE.AddToClassList("tower-selector-collapsed");
  }
}
