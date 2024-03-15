using Assets;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextPanel : MonoBehaviour {
  public static ContextPanel Instance;

  readonly private string noContextVisualElementName = "no_context__visualelement";
  readonly private string towerDetailName = "TowerDetail";
  readonly private string enemyDetailName = "EnemyDetail";

  private UIDocument uiDocument;

  private VisualElement noContextVisualElement;
  private VisualElement towerDetailVE;
  private VisualElement enemyDetailVE;

  private void Awake() {
    SetVisualElements();
    Instance = this;
  }

  private void Start() {
    SetNoContextPanel();
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    noContextVisualElement = rootElement.Q<VisualElement>(noContextVisualElementName);
    towerDetailVE = rootElement.Q<VisualElement>(towerDetailName);
    enemyDetailVE = rootElement.Q<VisualElement>(enemyDetailName);
  }

  // Capitalize the first letter in a word and make all other letters lowercase.
  private string ToTitleCase(string titleCase) {
    return string.Concat(titleCase[..1].ToUpper(), titleCase[1..].ToLower());
  }

  public void SetNoContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.Flex;
  }

  public void SetTowerContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.Flex;
    noContextVisualElement.style.display = DisplayStyle.None;
  }

  public void SetEnemyContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.Flex;
    towerDetailVE.style.display = DisplayStyle.None;
    noContextVisualElement.style.display = DisplayStyle.None;
  }
}
