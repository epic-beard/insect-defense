using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class WDStringList : VisualElement {
  public event Action<List<string>> OnItemsChanged = delegate { };
  public List<string> Items;

  private Button button = new();
  private ListView list = new();

  public WDStringList(List<string> items, string name) {
    this.style.flexDirection = FlexDirection.Column;
    this.Items = items;

    this.Add(new Label(name));

    this.Add(list);
    list.makeItem = () => new WDStringListItem();
    list.bindItem += BindItem;
    list.itemsSource = Items;
    list.reorderable = true;
    list.itemHeight = 40;

    this.Add(button);
    button.text = "+";
    button.RegisterCallback<ClickEvent>(evt => AddItem());
  }

  private void AddItem() {
    Items.Add("");
    list.Rebuild();
  }

  private void BindItem(VisualElement ve, int index) {
    WDStringListItem wdListItem = ve as WDStringListItem;
    wdListItem.Initialize(Items[index]);
    wdListItem.OnValueChanged += ((newValue, oldValue) => {
      int index = Items.FindIndex(str => str == oldValue);
      Items.RemoveAt(index);
      Items.Insert(index, newValue);
      OnItemsChanged.Invoke(Items);
    });
    wdListItem.OnRemove += (() => {
      Items.Remove(wdListItem.Value);
      list.Rebuild();
    });
  } 
}

public class WDStringListItem : VisualElement {
  public event Action<string,string> OnValueChanged = delegate { };
  public event Action OnRemove = delegate { };

  private string value;
  public string Value {
    get { return value; }
    set { OnValueChanged.Invoke(value, this.value); this.value = value; }
  }

  private TextField field = new();
  private Button button = new();

  public WDStringListItem() { }

  public void Initialize(string value) {
    this.style.flexDirection = FlexDirection.Row;

    Value = value;

    this.Add(field);
    field.RegisterValueChangedCallback<string>(evt => {
      field.AddToClassList("wd-list-item-base__field");
      Value = evt.newValue;
    });
    field.value = value;
    field.style.flexWrap = Wrap.Wrap;

    this.Add(button);
    button.text = "-";
    button.RegisterCallback<ClickEvent>(evt => {
      OnRemove.Invoke();
    });
  }
}