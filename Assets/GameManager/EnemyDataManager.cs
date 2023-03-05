using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

// A class that contains a dictionary of EnemyDatas keyed by strings.
// Allows you to reference enemy stats by name for convenient wave creation.
public class EnemyDataManager : MonoBehaviour {
  [SerializeField] private string filename;
  private EnemyDictionary enemies = new();

  // A simple Key Value pair class since the builtin KeyValuePair does not
  // serialize well.
  public class KvPair {
    public string Key;
    public EnemyData Value;
  }

  void Start() {
    DeserializeEnemies(filename);
  }

  // Returns the EnemyData associated with the key.
  // Errors out if key is not contained in the dictionary.
  public EnemyData GetEnemyData(string key) {
    return enemies[key];
  }

  // A version of serialization that takes an arbitrary TextWriter.
  // Not intended for general use but may be helpfull for testing.
  public void SerializeEnemies(EnemyDictionary enemies, TextWriter writer) {
    XmlSerializer serializer = new(typeof(EnemyDictionary));
    serializer.Serialize(writer, enemies);
  }

  // Serializes the list of enemies to the file given by filename.
  // Creates the file if it does not exist, otherwise overwrites it.
  public void SerializeEnemies(EnemyDictionary enemies, string filename) {
    using TextWriter writer = new StreamWriter(filename);
    SerializeEnemies(enemies, writer);
  }

  // A version of deserialization that takes an arbitrary Stream.
  // Not inteded for general use but may be helpfull for testing.
  void DeserializeEnemies(Stream stream) {
    XmlSerializer serializer = new(typeof(EnemyDictionary));
    enemies = (EnemyDictionary)serializer.Deserialize(stream);
  }

  // Deserializes the list of KvPairs in filename and adds them to the
  // EnemyData dictionary.
  void DeserializeEnemies(string filename) {
    using Stream reader = new FileStream(filename, FileMode.Open);
    DeserializeEnemies(reader);
  }

  public override string ToString() {
    string str = "EnemyDataManager:";
    foreach (var (key, value) in enemies) {
      str += "\nKey: " + key + "Value: " + value;
    }
    return str;
  }
}