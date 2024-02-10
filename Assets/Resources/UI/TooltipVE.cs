using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipVE : VisualElement {
  public string TooltipText { get; protected set; }
  public Label TooltipLabel { get; protected set; }

  public TooltipVE() {
    this.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    this.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  public void SetTooltipText(string tooltip) {
    TooltipLabel.text = tooltip;
  }


  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    TooltipVE button = evt.target as TooltipVE;
    if (button == null) return;

    Label tooltip = button.TooltipLabel;

    tooltip.style.display = DisplayStyle.Flex;
    tooltip.style.left = evt.localMousePosition.x;
    tooltip.style.top = evt.localMousePosition.y;
  }

  private void OnMouseLeaveEvent(MouseLeaveEvent evt) {
    if (evt.target is not TooltipVE buttonWithTooltip) return;

    Label tooltip = buttonWithTooltip.TooltipLabel;

    tooltip.style.display = DisplayStyle.None;
  }
}
