using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

// Load this custom composite into Unity's action map editor UI.
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DisplayStringFormat("{modifier1}+{modifier2}+{x}")]

public class CustomMouseFloatComposite : InputBindingComposite<float> {
  ButtonWithTwoModifiers b = new();
  [InputControl(layout = "Button")]
  public int modifier1;

  [InputControl(layout = "Button")]
  public int modifier2;

  [InputControl(layout = "Axis")]
  public int x;

  public bool overrideModifiersNeedToBePressedFirst;

  // This method computes the resulting input value of the composite based
  // on the input from its part bindings.
  public override float ReadValue(ref InputBindingCompositeContext context) {
    if (ModifiersArePressed(ref context)) {
      return context.ReadValue<float>(x);
    }
    return 0.0f;
  }

  private bool ModifiersArePressed(ref InputBindingCompositeContext context) {
    return context.ReadValueAsButton(modifier1) && context.ReadValueAsButton(modifier2);
  }

  // This method computes the current actuation of the binding as a whole.
  public override float EvaluateMagnitude(ref InputBindingCompositeContext context) {
    return ReadValue(ref context);
  }

  protected override void FinishSetup(ref InputBindingCompositeContext context) {
    if (!overrideModifiersNeedToBePressedFirst) {
      overrideModifiersNeedToBePressedFirst = !InputSystem.settings.shortcutKeysConsumeInput;
    }
  }

  static CustomMouseFloatComposite() {
    InputSystem.RegisterBindingComposite<CustomMouseFloatComposite>();
  }

  [RuntimeInitializeOnLoadMethod]
  static void Init() { } // Trigger static constructor.
}
