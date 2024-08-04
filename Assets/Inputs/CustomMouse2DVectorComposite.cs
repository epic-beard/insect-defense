using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

// Load this custom composite into Unity's action map editor UI.
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DisplayStringFormat("{modifier1}+{modifier2}+{up}/{left}/{down}/{right}")]
public class CustomMouse2DVectorComposite : InputBindingComposite<Vector2> {
  [InputControl(layout = "Button")]
  public int modifier1;

  [InputControl(layout = "Button")]
  public int modifier2;

  [InputControl(layout = "Axis")]
  public int up;

  [InputControl(layout = "Axis")]
  public int down;

  [InputControl(layout = "Axis")]
  public int left;

  [InputControl(layout = "Axis")]
  public int right;

  public bool overrideModifiersNeedToBePressedFirst;

  // This method computes the resulting input value of the composite based
  // on the input from its part bindings.
  public override Vector2 ReadValue(ref InputBindingCompositeContext context) {
    if (ModifiersArePressed(ref context)) {
      var north = context.ReadValueAsButton(up);
      var south = context.ReadValueAsButton(down);
      var east = context.ReadValueAsButton(left);
      var west = context.ReadValueAsButton(right);

      return DpadControl.MakeDpadVector(north, south, east, west, true);
    }
    return Vector2.zero;
  }

  private bool ModifiersArePressed(ref InputBindingCompositeContext context) {
    return context.ReadValueAsButton(modifier1) && context.ReadValueAsButton(modifier2);
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

  static CustomMouse2DVectorComposite() {
    InputSystem.RegisterBindingComposite<CustomMouse2DVectorComposite>();
  }

  [RuntimeInitializeOnLoadMethod]
  static void Init() { } // Trigger static constructor.
}
