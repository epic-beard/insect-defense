using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumUI : MonoBehaviour {
  public static TerrariumUI Instance;

  readonly private string tooltipVisualElementName = "tooltip__visualelement";
  readonly private string tooltipLabelName = "tooltip__label";

  public VisualElement TooltipVE { get; private set; }
  public Label TooltipLabel { get; private set; }

  UIDocument terrariumUI;

  private void Awake() {
    Instance = this;
    SetVisualElements();
  }

  void SetVisualElements() {
    terrariumUI = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumUI.rootVisualElement;

    TooltipVE = rootElement.Q<VisualElement>(tooltipVisualElementName);
    TooltipLabel = rootElement.Q<Label>(tooltipLabelName);
  }

  public void HideUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.None;
  }

  public void ShowUI() {
    terrariumUI.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
