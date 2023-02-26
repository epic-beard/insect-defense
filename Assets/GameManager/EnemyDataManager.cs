using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

//using KvPair = System.Collections.Generic.KeyValuePair<string, EnemyData>;

public class EnemyDataManager : MonoBehaviour {
  [SerializeField] private string filename;
  readonly private Dictionary<string, EnemyData> datas = new();

  public class KvPair {
    public string Key;
    public EnemyData Value;
  }

  void Start() {
    DeserializeEnemies(filename);
  }

  public EnemyData GetEnemyData(string key) {
    return datas[key];
  }

  public void SerializeEnemies(List<KvPair> enemies, TextWriter writer) {
    XmlSerializer serializer = new(typeof(List<KvPair>));
    serializer.Serialize(writer, enemies);
  }

  public void SerializeEnemies(List<KvPair> enemies, string filename) {
    using TextWriter writer = new StreamWriter(filename);
    SerializeEnemies(enemies, writer);
  }

  void DeserializeEnemies(Stream stream) {
    XmlSerializer serializer = new(typeof(List<KvPair>));
    List<KvPair> tmp = (List<KvPair>)serializer.Deserialize(stream);

    foreach (KvPair pair in tmp) {
      Debug.Log("Key: " + pair.Key + " Value: " + pair.Value);
      datas.Add(pair.Key, pair.Value);
    }
  }

  void DeserializeEnemies(string filename) {
    using Stream reader = new FileStream(filename, FileMode.Open);
    DeserializeEnemies(reader);
  }

  public override string ToString() {
    string str = "EnemyDataManager:";
    foreach (var kvPair in datas) {
      str += "\nKey: " + kvPair.Key + "Value: " + kvPair.Value;
    }
    return str;
  }
}
