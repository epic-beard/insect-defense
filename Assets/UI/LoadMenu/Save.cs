using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Save : VisualElement {
  public PlayerState PlayerState;

  public static string nameLabelName = "name__label";
  public static string levelLabelName = "level__label";
  public Save(PlayerState playerState) {
    PlayerState = playerState;

    VisualElement root = new();
    root.style.paddingTop = 3f;
    root.style.paddingRight = 3f;
    root.style.paddingBottom = 3f;
    root.style.paddingLeft = 12f;
    root.style.borderBottomColor = Color.gray;
    root.style.borderBottomWidth = 1f;
    VisualElement nameLabel = new() { name = nameLabelName };
    nameLabel.style.fontSize = 14f;
    root.Add(nameLabel);
    VisualElement levelLabel = new() { name = levelLabelName };
    levelLabel.style.fontSize = 14f;
    root.Add(levelLabel);
  }

  private void OnEnable() {
  }
}
