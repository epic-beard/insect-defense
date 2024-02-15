using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class TowerButton : TooltipVE {
  readonly private string templatePath = "UI/TowerButton";
  readonly private string imageVeName = "image__ve";
  readonly private string nameLabelName = "name__label";
  readonly private string costLabelName = "cost__label";

  private VisualElement imageVe;
  private Label nameLabel;
  private Label costLabel;

  private string image = "";
  public string Image {
    get { return image; }
    set {
      image = value;
      imageVe.style.backgroundImage = Resources.Load<Texture2D>(image);
    }
  }

  private string name = "";
  public string Name {
    get { return name; }
    set {
      name = value;
      nameLabel.text = name;
    }
  }

  private int cost = 0;
  public int Cost {
    get { return cost; }
    set {
      cost = value;
      costLabel.text = Constants.nu + cost;
    }
  }
  
  public TowerData.Type TowerType { get; set; }

  public TowerButton() {
    var tree = Resources.Load<VisualTreeAsset>(templatePath).CloneTree();
    imageVe = tree.Q<VisualElement>(imageVeName);
    nameLabel = tree.Q<Label>(nameLabelName);
    costLabel = tree.Q<Label>(costLabelName);

    Add(tree);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<TowerButton, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits {
    UxmlStringAttributeDescription Image = new() { name = "image" };
    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
      base.Init(ve, bag, cc);
      var towerButton = ve as TowerButton;
      towerButton.Image = Image.GetValueFromBag(bag, cc);
    }
  }
  #endregion
}
