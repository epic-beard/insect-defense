using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumScreen : MonoBehaviour {
  public static TerrariumScreen Instance;

  readonly private string tooltipContainerVEName = "tooltip-container";
  readonly private string tooltipLabelVEName = "tooltip-label";
  readonly private string sidebarVEName = "sidebar";
  readonly private string bottomBarVEName = "bottom-bar-parent";
  readonly private string messageBoxVEName = "message-box-container";
  readonly private string towerSelectorVEName = "tower-selector";
  readonly private string contextPanelVEName = "context-panel-parent";

  public VisualElement TooltipVE { get; private set; }
  public Label TooltipLabel { get; private set; }

  private UIDocument uiDocument;
  private VisualElement sidebarVE;
  private VisualElement bottomBarVE;
  private VisualElement messageBoxVE;
  private VisualElement towerSelectorVE;
  private VisualElement contextPanelVE;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
  }

  void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    sidebarVE = rootElement.Q<VisualElement>(sidebarVEName);
    bottomBarVE = rootElement.Q<VisualElement>(bottomBarVEName);
    messageBoxVE = rootElement.Q<VisualElement>(messageBoxVEName);
    towerSelectorVE = rootElement.Q<VisualElement>(className: towerSelectorVEName);
    contextPanelVE = rootElement.Q<VisualElement>(className: contextPanelVEName);

    TooltipVE = rootElement.Q<VisualElement>(tooltipContainerVEName);
    TooltipLabel = rootElement.Q<Label>(tooltipLabelVEName);
  }

  private void RegisterCallbacks() {
    sidebarVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);
    bottomBarVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);
    messageBoxVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);

    sidebarVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
    bottomBarVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
    messageBoxVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);

    towerSelectorVE.RegisterCallback<TransitionStartEvent>(evt => {
        if (towerSelectorVE.ClassListContains("tower-selector-collapsed")) {
            UiSfx.PlaySfx(UiSfx.tower_select_list_collapse);
        }
    });

    contextPanelVE.RegisterCallback<TransitionStartEvent>(evt => {
        if (evt.target == contextPanelVE && contextPanelVE.ClassListContains("context-panel-parent-collapsed")) {
            UiSfx.PlaySfx(UiSfx.context_panel_collapse);
        }
    });
  }

  private void OnMouseOverUI(MouseEnterEvent evt) {
    GameStateManager.Instance.IsMouseOverUI = true;
  }

  private void OnMouseLeaveUI(MouseLeaveEvent evt) {
    GameStateManager.Instance.IsMouseOverUI = false;
  }

  public void HideUI() {
    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
  }

  public void ShowUI() {
    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
