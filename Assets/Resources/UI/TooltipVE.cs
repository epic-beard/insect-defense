using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipVE : VisualElement {
  public string TooltipText { get; protected set; }

  public TooltipVE() {
    this.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    this.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  public void SetTooltipText(string tooltip) {
    TerrariumUI.Instance.TooltipLabel.text = tooltip;
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    TooltipVE button = evt.target as TooltipVE;
    if (button == null) return;

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
}
