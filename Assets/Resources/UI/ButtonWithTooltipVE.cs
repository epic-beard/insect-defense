using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class ButtonWithTooltipVE : VisualElement {

  readonly private string templatePath = "UI/ButtonWithTooltipVE";
  readonly private string buttonName = "button-with-tooltip--button";

  public ButtonWithTooltipVE() {
    VisualTreeAsset buttonTree = Resources.Load<VisualTreeAsset>(templatePath);
    var buttonElement = buttonTree.CloneTree();

    Add(buttonElement);
  }

  // TODO to finish tooltip:
  // 1. Complete the Custom visual element work. Each element type (button, label, etc.) will
  //    have its own custom visual element in much the style of this one. The purpose of this is
  //    to have a data field unity does not interact with for our own tooltip.
  // 2. Complete a utility you can simply pass these custom visual elements to. This utility
  //    will wire up the OnMouseEnter and OnMouseLeave Events and set up the construction/destruction
  //    of the tooltip as appropriate.
  // Note: These are not necessarily dependent on each other.

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<ButtonWithTooltipVE, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits { }
  #endregion
}
