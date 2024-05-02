#nullable enable
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine.UIElements;
using UnityEngine;

public class WDNullableField<T, TField> : VisualElement where TField : TextInputBaseField<T>, new() where T : struct {
  public event Action<T?> OnValueChanged = delegate { };
  private T? value;
  public T? Value {
    get { return value; }
    set { this.value = value; OnValueChanged.Invoke(Value); }
  }

  private RadioButton button = new();
  private TField field = new();

  public WDNullableField() : this("") { }

  public WDNullableField(string name) {
    this.style.flexDirection = FlexDirection.Row;
    this.Add(button);
    button.AddToClassList("nullable-field-base__button");
    button.RegisterCallback<ClickEvent>(ToggleButton);
    button.label = name;

    this.Add(field);
    field.AddToClassList("nullable-field-base__field");
    field.style.display = DisplayStyle.None;
    field.RegisterValueChangedCallback<T>(evt => Value = evt.newValue);
  }

  private void ToggleButton(ClickEvent _) {
    if (field.style.display == DisplayStyle.None) {
      field.style.display = DisplayStyle.Flex;
      Value = field.value;
    } else {
      field.style.display = DisplayStyle.None;
      Value = null;
    }
  }
}
