using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadMenu : MonoBehaviour {

  readonly private string saveListViewName = "save__list_view";

  UIDocument loadMenu;
  private ListView saveList;

  private List<PlayerState> playerStates = new();

  private void OnEnable() {
    SetVisualElements();
    loadMenu.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      ToggleScreen();
    }
  }

  private void ToggleScreen() {
    if (loadMenu.rootVisualElement.style.display == DisplayStyle.None) {
      OpenMenu();
    } else {
      CloseMenu();
    }
  }

  private void SetVisualElements() {
    loadMenu = GetComponent<UIDocument>();
    VisualElement rootElement = loadMenu.rootVisualElement;

    saveList = rootElement.Q<ListView>(saveListViewName);

    saveList.makeItem = () => new Save();
    saveList.bindItem = (e, i) => BindItem((e as Save), i);
    saveList.itemsSource = playerStates;
  }

  private void BindItem(Save item, int i) {
    item.PlayerState = playerStates[i];

    var nameLabel = item.Q<Label>(Save.NameLabelName);
    var levelLabel = item.Q<Label>(Save.LevelLabelName);

    nameLabel.text = playerStates[i].SaveName;
    levelLabel.text = (playerStates[i].HighestLevelBeat + 1).ToString();

    item.RegisterCallback<ClickEvent>(OnLoadSave);
  }

  public void OnLoadSave(ClickEvent evt) {
    // Handle confirmation popup
    PlayerState.Instance = (evt.currentTarget as Save).PlayerState;
    SceneManager.LoadScene("Lab");
  }

  private void OpenMenu() {
    PauseManager.Instance.PauseAndLock();
    playerStates = SaveManager.Instance.GetSaves();
    saveList.itemsSource = playerStates;
    saveList.Rebuild();
    loadMenu.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  private void CloseMenu() {
    PauseManager.Instance.UnpauseAndUnlock();
    loadMenu.rootVisualElement.style.display = DisplayStyle.None;
  }
}
