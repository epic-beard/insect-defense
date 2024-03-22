using UnityEngine;
using UnityEngine.UIElements;

public class ContextPanel : MonoBehaviour {
  public static ContextPanel Instance;

  readonly private string noDetailName = "no-detail";
  readonly private string towerDetailName = "tower-detail";
  readonly private string enemyDetailName = "enemy-detail";

  private UIDocument uiDocument;

  private VisualElement noDetailVE;
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

    noDetailVE = rootElement.Q<VisualElement>(noDetailName);
    towerDetailVE = rootElement.Q<VisualElement>(towerDetailName);
    enemyDetailVE = rootElement.Q<VisualElement>(enemyDetailName);
  }

  public void SetNoContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.None;
    noDetailVE.style.display = DisplayStyle.Flex;
  }

  public void SetTowerContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.None;
    towerDetailVE.style.display = DisplayStyle.Flex;
    noDetailVE.style.display = DisplayStyle.None;
  }

  public void SetEnemyContextPanel() {
    enemyDetailVE.style.display = DisplayStyle.Flex;
    towerDetailVE.style.display = DisplayStyle.None;
    noDetailVE.style.display = DisplayStyle.None;
  }
}
