using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class ButtonWithTooltip : Tooltip {
  readonly private string buttonWithTooltipName = "button-with-tooltip";
  readonly private string templatePath = "UI/Components/ButtonWithTooltip/ButtonWithTooltip";

  public Button Button { get; private set; }

  public void SetButtonText(string text) {
    Button.text = text;
  }

  public ButtonWithTooltip() {
    var tree = Resources.Load<VisualTreeAsset>(templatePath).CloneTree();
    Button = tree.Q<Button>(buttonWithTooltipName);

    Add(tree);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<ButtonWithTooltip, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits { }
  #endregion
}
