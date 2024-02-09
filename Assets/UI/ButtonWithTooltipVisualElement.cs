using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class ButtonWithTooltipVisualElement : VisualElement {

  readonly private string templatePath = "UI/ButtonWithTooltipVisualElement";

  private VisualTreeAsset buttonTree;

  public ButtonWithTooltipVisualElement() {
    buttonTree = Resources.Load<VisualTreeAsset>(templatePath);
    var buttonElement = buttonTree.CloneTree();

    Add(buttonElement);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<ButtonWithTooltipVisualElement, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits { }
  #endregion
}
