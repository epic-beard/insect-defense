using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

// Load this custom composite into Unity's action map editor UI.
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DisplayStringFormat("{modifier}+{x}/{y}")]
public class Custom2DVectorWithModifierComposite : InputBindingComposite<Vector2> {
  [InputControl(layout = "Button")]
  public int modifier;

  [InputControl(layout = "Axis")]
  public int x;

  [InputControl(layout = "Axis")]
  public int y;

  public bool overrideModifiersNeedToBePressedFirst;

  // This method computes the resulting input value of the composite based
  // on the input from its part bindings.
  public override Vector2 ReadValue(ref InputBindingCompositeContext context) {
    if (ModifierIsPressed(ref context)) {
      float xValue = context.ReadValue<float>(x);
      float yValue = context.ReadValue<float>(y);
      return new Vector2(xValue, yValue);
    }
    return Vector2.zero;
  }

  private bool ModifierIsPressed(ref InputBindingCompositeContext context) {
    return context.ReadValueAsButton(modifier);
  }

  // This method computes the current actuation of the binding as a whole.
  public override float EvaluateMagnitude(ref InputBindingCompositeContext context) {
    return ReadValue(ref context).magnitude;
  }

  protected override void FinishSetup(ref InputBindingCompositeContext context) {
    if (!overrideModifiersNeedToBePressedFirst) {
      overrideModifiersNeedToBePressedFirst = !InputSystem.settings.shortcutKeysConsumeInput;
    }
  }

  static Custom2DVectorWithModifierComposite() {
    InputSystem.RegisterBindingComposite<Custom2DVectorWithModifierComposite>();
  }

  [RuntimeInitializeOnLoadMethod]
  static void Init() { } // Trigger static constructor.
}
