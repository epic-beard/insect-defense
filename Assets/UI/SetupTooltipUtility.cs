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
  }

  private void OnMouseEnterEvent(MouseEnterEvent evt) {
    // Do stuff to make the label visible and format it.
  }
}
