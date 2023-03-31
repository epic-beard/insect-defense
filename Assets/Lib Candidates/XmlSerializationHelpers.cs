#nullable enable
using System.IO;
using System.Xml.Serialization;

namespace EpicBeardLib {
  public static class XmlSerializationHelpers {
    // A version of serialization that takes an arbitrary TextWriter.
    // Not intended for general use but may be helpfull for testing.
    public static void Serialize<T>(T data, TextWriter writer) {
      XmlSerializer serializer = new(typeof(T));
      serializer.Serialize(writer, data);
    }

    // Serializes the data to the file given by filename.
    // Creates the file if it does not exist, otherwise overwrites it.
    public static void Serialize<T>(T data, string filename) {
      using TextWriter writer = new StreamWriter(filename);
      Serialize(data, writer);
    }

    // A version of deserialization that takes an arbitrary Stream.
    // Not inteded for general use but may be helpfull for testing.
    public static T? Deserialize<T>(Stream stream) {
      XmlSerializer serializer = new(typeof(T?));
      return (T?)serializer.Deserialize(stream);
    }

    // Deserializes the data in filename.
    public static T? Deserialize<T>(string filename) {
      using Stream reader = new FileStream(filename, FileMode.Open);
      return Deserialize<T?>(reader);
    }
  }
}
