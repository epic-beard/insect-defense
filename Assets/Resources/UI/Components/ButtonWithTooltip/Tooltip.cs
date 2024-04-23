using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class Tooltip : VisualElement {
  public string TooltipText { get; set; }

  private readonly float TOOLTIP_WIDTH = 320;
  private readonly float TOOLTIP_HEIGHT = 120;
  private readonly float SCREEN_WIDTH_POS = 1300;
  private readonly float SCREEN_HEIGHT_POS = 800;

  public Tooltip() {
    this.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    this.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    Tooltip button = evt.target as Tooltip;
    if (button == null) return;

    TerrariumScreen.Instance.TooltipLabel.text = TooltipText;
    VisualElement tooltip = TerrariumScreen.Instance.TooltipVE;

    if (evt.mousePosition.x + TOOLTIP_WIDTH > SCREEN_WIDTH_POS) {
        tooltip.style.left = SCREEN_WIDTH_POS - TOOLTIP_WIDTH;
    } else {
        tooltip.style.left = evt.mousePosition.x;
    }
    
    if (evt.mousePosition.y + TOOLTIP_HEIGHT > SCREEN_HEIGHT_POS) {
        tooltip.style.top = SCREEN_HEIGHT_POS - TOOLTIP_HEIGHT;
    } else {
        tooltip.style.top = evt.mousePosition.y;
    }

    tooltip.style.display = DisplayStyle.Flex;
  }

  private void OnMouseLeaveEvent(MouseLeaveEvent evt) {
    if (evt.target is not Tooltip buttonWithTooltip) return;

    VisualElement tooltip = TerrariumScreen.Instance.TooltipVE;
    tooltip.style.display = DisplayStyle.None;
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<Tooltip, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits {
    UxmlStringAttributeDescription Tooltip = new() { name = "custom-tooltip" };

    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
      base.Init(ve, bag, cc);
      var tooltip = ve as Tooltip;
      tooltip.TooltipText = Tooltip.GetValueFromBag(bag, cc);
    }
  }
  #endregion
}
