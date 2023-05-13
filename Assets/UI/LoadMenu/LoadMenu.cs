using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using static EpicBeardLib.XmlSerializationHelpers;

public class LoadMenu : MonoBehaviour {
  [SerializeField] private string saveDirectory;

  readonly private string saveListName = "save_list__name";

  UIDocument loadMenu;

  private ListView saveList;

  private void OnEnable() {
    SetVisualElements();

    string saveLocation = Path.Combine(Application.persistentDataPath + saveDirectory);
    if (File.Exists(saveLocation)) {
      var saves = Deserialize<List<PlayerState>>(saveLocation);
      foreach (var save in saves) {
        saveList.Add(new Save(save));
      }
    }
  }

  private void SetVisualElements() {
    loadMenu = GetComponent<UIDocument>();
    VisualElement rootElement = loadMenu.rootVisualElement;

    saveList = rootElement.Q<ListView>(saveListName);
  }
}
