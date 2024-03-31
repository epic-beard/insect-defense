using Codice.Client.BaseCommands;
using System.IO;
using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

// A class that contains a dictionary of EnemyDatas keyed by strings.
// Allows you to reference enemy stats by name for convenient wave creation.
public class EnemyDataManager : MonoBehaviour {
  static public EnemyDataManager Instance;

  [SerializeField] private string filename;
  private EnemyDictionary enemies = new();

  private void Awake() {
    Instance = this;
    enemies = Deserialize<EnemyDictionary>(filename);
  }

  // Returns the EnemyData associated with the key.
  // Errors out if key is not contained in the dictionary.
  public EnemyData GetEnemyData(string key) {
    return enemies[key];
  }

  public override string ToString() {
    string str = "EnemyDataManager:";
    foreach (var (key, value) in enemies) {
      str += "\nKey: " + key + "Value: " + value;
    }
    return str;
  }
}
