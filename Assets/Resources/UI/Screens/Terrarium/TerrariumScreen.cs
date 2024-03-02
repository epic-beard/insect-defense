using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumScreen : MonoBehaviour {
  public static TerrariumScreen Instance;

  readonly private string tooltipVisualElementName = "tooltip__visualelement";
  readonly private string tooltipLabelName = "tooltip__label";
  readonly private string rightVEName = "right__ve";
  readonly private string bottomVEName = "bottom__ve";
  readonly private string messageBoxVEName = "i_message_box__ve";

  public VisualElement TooltipVE { get; private set; }
  public Label TooltipLabel { get; private set; }

  private UIDocument terrariumScreen;
  private VisualElement rightVE;
  private VisualElement bottomVE;
  private VisualElement messageBoxVE;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    RegisterCallbacks();
  }

  void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    rightVE = rootElement.Q<VisualElement>(rightVEName);
    bottomVE = rootElement.Q<VisualElement>(bottomVEName);
    messageBoxVE = rootElement.Q<VisualElement>(messageBoxVEName);

    TooltipVE = rootElement.Q<VisualElement>(tooltipVisualElementName);
    TooltipLabel = rootElement.Q<Label>(tooltipLabelName);
  }

  private void RegisterCallbacks() {
    rightVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);
    bottomVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);
    messageBoxVE.RegisterCallback<MouseEnterEvent>(OnMouseOverUI);

    rightVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
    bottomVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
    messageBoxVE.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
  }

  private void OnMouseOverUI(MouseEnterEvent evt) {
    GameStateManager.Instance.IsMouseOverUI = true;
  }

  private void OnMouseLeaveUI(MouseLeaveEvent evt) {
    GameStateManager.Instance.IsMouseOverUI = false;
  }

  public void HideUI() {
    terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  public void ShowUI() {
    terrariumScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
