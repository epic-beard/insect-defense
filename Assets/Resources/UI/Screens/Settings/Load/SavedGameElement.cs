using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SavedGameElement : VisualElement {
  public PlayerState PlayerState;

  public static string nameLabelName = "save-game-name-label";
  public static string levelLabelName = "save-game-level-label";

  public SavedGameElement() {
    VisualElement root = new();
    root.style.paddingTop = 3f;
    root.style.paddingRight = 3f;
    root.style.paddingBottom = 3f;
    root.style.paddingLeft = 12f;
    root.style.borderBottomColor = Color.gray;
    root.style.borderBottomWidth = 1f;
    root.style.alignItems = Align.Stretch;
    root.style.flexDirection = FlexDirection.Row;
    VisualElement nameContainer = new();
    nameContainer.style.flexGrow = 3f;
    nameContainer.style.alignItems = Align.FlexStart;
    Label nameLabel = new() { name = nameLabelName };
    nameLabel.style.color = Color.white;
    nameContainer.Add(nameLabel);
    root.Add(nameContainer);
     
    VisualElement levelContainer = new();
    levelContainer.style.borderLeftWidth = 1f;
    levelContainer.style.borderLeftColor = Color.gray;
    levelContainer.style.paddingLeft = 5f;
    levelContainer.style.flexGrow = 1f;
    levelContainer.style.alignItems = Align.FlexStart;
    Label levelLabel = new() { name = levelLabelName };
    levelLabel.style.color = Color.white;
    levelContainer.Add(levelLabel);
    root.Add(levelContainer);
    Add(root);
  }
}
