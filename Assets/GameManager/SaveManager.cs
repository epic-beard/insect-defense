using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static EpicBeardLib.XmlSerializationHelpers;

public class SaveManager : MonoBehaviour {
  public static SaveManager Instance;

  [SerializeField] private string saveDirectory = "\\saves";
  public string SaveLocation {
    get { return Path.Combine(Application.persistentDataPath + saveDirectory); }
  }

  private void Awake() {
    //----------------------------------------
    //Save(new PlayerState("10") { SaveName = "Nathan", HighestLevelBeat = 0 });
    //Save(new PlayerState("1") { SaveName = "Eric", HighestLevelBeat = 9 });
    //-----------------------------------------

    Instance = this;
  }

  public List<PlayerState> GetSaves() {
    List<PlayerState> list = new();
    if (Directory.Exists(SaveLocation)) {
      foreach(var path in Directory.GetFiles(SaveLocation)) {
        var save = Deserialize<PlayerState>(path);
        list.Add(save);
      }
    }

    return list;
  }

  public void Save(PlayerState state) {
    state.lastSavedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    string fileName = state.SaveName + state.Id + ".sav";
    string saveFile = Path.Combine(SaveLocation, fileName);
    var dir = Path.GetDirectoryName(saveFile);
    Directory.CreateDirectory(dir);
    Serialize<PlayerState>(state, saveFile);
  }
}
