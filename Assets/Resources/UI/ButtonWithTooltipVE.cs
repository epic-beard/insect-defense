using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class ButtonWithTooltipVE : TooltipVE {
  readonly private string buttonWithTooltipName = "button_with_tooltip__button";
  readonly private string templatePath = "UI/ButtonWithTooltipVE";

  public Button ButtonWithTooltip { get; private set; }

  public ButtonWithTooltipVE() {
    VisualTreeAsset buttonTree = Resources.Load<VisualTreeAsset>(templatePath);

    var buttonElement = buttonTree.CloneTree();
    ButtonWithTooltip = buttonElement.Q<Button>(buttonWithTooltipName);

    Add(buttonElement);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<ButtonWithTooltipVE, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits { }
  #endregion
}
