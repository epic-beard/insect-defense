using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Save : VisualElement {
  public PlayerState PlayerState;

  public Save(PlayerState playerState) {
    PlayerState = playerState;

    VisualElement root = new();
    root.style.paddingTop = 3f;
    root.style.paddingRight = 3f;
    root.style.paddingBottom = 3f;
    root.style.paddingLeft = 12f;
    root.style.borderBottomColor = Color.gray;
    root.style.borderBottomWidth = 1f;
  }

  private void OnEnable() {
  }
}
