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
  }
  void Start() {
    //----------------------------------------
    //EnemyData testData1 = new() {
    //  maxArmor = 1,
    //  maxHP = 20,
    //  size = EnemyData.Size.NORMAL,
    //  nu = 20,
    //  type = EnemyData.Type.ANT,
    //  speed = 1.0f
    //  damage = 1
    //};
    //EnemyData testData2 = new() {
    //  maxArmor = 5,
    //  maxHP = 5,
    //  size = EnemyData.Size.TINY,
    //  nu = 10,
    //  type = EnemyData.Type.BEETLE,
    //  speed = 0.5f
    //  damage = 2
    //};
    //EnemyDictionary testEnemies = new() {
    //  { "ant", testData1 },
    //  { "beetle", testData2 },
    //};

    //Serialize<EnemyDictionary>(testEnemies, filename);
    //----------------------------------------
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
