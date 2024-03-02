using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class Tooltip : VisualElement {
  public string TooltipText { get; set; }

  public Tooltip() {
    this.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    this.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    Tooltip button = evt.target as Tooltip;
    if (button == null) return;

    TerrariumScreen.Instance.TooltipLabel.text = TooltipText;
    VisualElement tooltip = TerrariumScreen.Instance.TooltipVE;
    tooltip.style.left = evt.mousePosition.x;
    tooltip.style.top = evt.mousePosition.y;
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
