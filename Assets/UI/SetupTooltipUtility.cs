using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetupTooltipUtility : MonoBehaviour {
  public static SetupTooltipUtility Instance;

  private void Awake() {
    Instance = this;
  }

  public void SetupClickHandling(VisualElement ve, Label label) {
    ve.RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
    ve.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {

    Debug.Log("In OnMouseEnterEvent - Before validity check.");
    TooltipVE button = evt.target as TooltipVE;
    if (button == null) return;
    Debug.Log("In OnMouseEnterEvent - After validity check.");

    Label tooltip = button.TooltipLabel;

    tooltip.style.display = DisplayStyle.Flex;
    tooltip.style.left = evt.localMousePosition.x;
    tooltip.style.top = evt.localMousePosition.y;

    Debug.Log("tooltip text: " + tooltip.text);
  }

  private void OnMouseLeaveEvent(MouseLeaveEvent evt) {
    Debug.Log("In OnMouseLeaveEvent - Before validity check.");
    if (evt.target is not TooltipVE buttonWithTooltip) return;
    Debug.Log("In OnMouseLeaveEvent - After validity check.");

    Label tooltip = buttonWithTooltip.TooltipLabel;

    tooltip.style.display= DisplayStyle.None;
  }
}
