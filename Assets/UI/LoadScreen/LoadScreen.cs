using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadScreen : MonoBehaviour {
  public static LoadScreen Instance;
  readonly private string saveListViewName = "save__list_view";

  UIDocument loadMenu;
  private ListView saveList;
  private List<PlayerState> playerStates = new();
  //private InputActionMap loadScreenActions;

  private void Awake() {
    Instance = this;
  }

  private void OnEnable() {
    SetVisualElements();
    loadMenu.rootVisualElement.style.display = DisplayStyle.None;
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
    levelLabel.text = (playerStates[i].CurrentLevel + 1).ToString();

    item.RegisterCallback<ClickEvent>(OnLoadSave);
  }

  public void OnLoadSave(ClickEvent evt) {
    // Handle confirmation popup
    PlayerState.Instance = (evt.currentTarget as Save).PlayerState;
    SceneManager.LoadScene("Lab");
  }

  public void OpenMenu() {
    playerStates = SaveManager.Instance.GetSaves();
    saveList.itemsSource = playerStates;
    saveList.Rebuild();
    loadMenu.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  public void CloseMenu() {
    loadMenu.rootVisualElement.style.display = DisplayStyle.None;
  }
}
