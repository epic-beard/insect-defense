using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class SelectTowerButton : Tooltip {
  readonly private string templatePath = "UI/Components/SelectTowerButton/SelectTowerButton";
  readonly private string imageName = "select-tower-image";
  readonly private string nameLabelName = "select-tower-name";
  readonly private string costLabelName = "select-tower-cost";

  private VisualElement imageVE;
  private Label nameLabelVE;
  private Label costLabelVE;

  private string imagePath = "";
  public string ImagePath {
    get { return imagePath; }
    set {
      imagePath = value;
      imageVE.style.backgroundImage = Resources.Load<Texture2D>(imagePath);
    }
  }

  private new string name = "";
  public string Name {
    get { return name; }
    set {
      name = value;
      nameLabelVE.text = name;
    }
  }

  private int cost = 0;
  public int Cost {
    get { return cost; }
    set {
      cost = value;
      costLabelVE.text = Constants.nu + " " + cost;
    }
  }
  
  public TowerData.Type TowerType { get; set; }

  public SelectTowerButton() {
    var tree = Resources.Load<VisualTreeAsset>(templatePath).CloneTree();
    imageVE = tree.Q<VisualElement>(imageName);
    nameLabelVE = tree.Q<Label>(nameLabelName);
    costLabelVE = tree.Q<Label>(costLabelName);

    Add(tree);
  }

  #region UXML
  [Preserve]
  public new class UxmlFactory : UxmlFactory<SelectTowerButton, UxmlTraits> { }

  [Preserve]
  public new class UxmlTraits : VisualElement.UxmlTraits {
    UxmlStringAttributeDescription ImagePath = new() { name = "image" };
    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
      base.Init(ve, bag, cc);
      var towerButton = ve as SelectTowerButton;
      towerButton.ImagePath = ImagePath.GetValueFromBag(bag, cc);
    }
  }
  #endregion
}
