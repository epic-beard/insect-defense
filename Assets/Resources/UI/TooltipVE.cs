using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class TooltipVE : VisualElement {
  public string TooltipText { get; set; }

  public TooltipVE() {
    this.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    this.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    TooltipVE button = evt.target as TooltipVE;
    if (button == null) return;

    TerrariumUI.Instance.TooltipLabel.text = TooltipText;
    VisualElement tooltipVE = TerrariumUI.Instance.TooltipVE;
    tooltipVE.style.left = evt.mousePosition.x;
    tooltipVE.style.top = evt.mousePosition.y;
    tooltipVE.style.display = DisplayStyle.Flex;
  }

  private void OnMouseLeaveEvent(MouseLeaveEvent evt) {
    if (evt.target is not TooltipVE buttonWithTooltip) return;

    VisualElement tooltipVE = TerrariumUI.Instance.TooltipVE;
    tooltipVE.style.display = DisplayStyle.None;
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<TooltipVE, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits {
    UxmlStringAttributeDescription Tooltip = new() { name = "custom-tooltip" };

    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
      base.Init(ve, bag, cc);
      var tooltipVE = ve as TooltipVE;
      tooltipVE.TooltipText = Tooltip.GetValueFromBag(bag, cc);
    }
  }
  #endregion
}
