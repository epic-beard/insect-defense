using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class ButtonWithTooltipVE : TooltipVE {
  readonly private string buttonWithTooltipName = "button_with_tooltip__button";
  readonly private string templatePath = "UI/ButtonWithTooltipVE";

  public Button Button { get; private set; }

  public void SetButtonText(string text) {
    Button.text = text;
  }

  public ButtonWithTooltipVE() {
    var tree = Resources.Load<VisualTreeAsset>(templatePath).CloneTree();
    Button = tree.Q<Button>(buttonWithTooltipName);

    Add(tree);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<ButtonWithTooltipVE, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits { }
  #endregion
}
