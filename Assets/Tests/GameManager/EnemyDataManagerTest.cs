using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using static EpicBeardLib.XmlSerializationHelpers;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

public class EnemyDataManagerTest {
  readonly string enemyName = "Beetle";
  EnemyData enemyData;

  [SetUp]
  public void SetUp() {
    enemyData = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.NORMAL,
      maxHP = 1,
      maxArmor = 2,
      speed = 3.5f,
      damage = 4,
      nu = 5,
      properties = EnemyData.Properties.CAMO,
      spawner = new() {
        num = 6,
        interval = 7.5f,
        childKey = "child",
      },
      carrier = new() {
        num = 8,
        childKey = "child",
      },
    };
  }

  // Round trip an enemy data through serialization then deserialization.
 [Test]
  public void SerializationWorks() {
    EnemyDictionary expected = new() {
      { enemyName, enemyData }
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    Serialize(expected, writer);

    memStream.Position = 0;

    EnemyDictionary actual = Deserialize<EnemyDictionary>(memStream);
    CollectionAssert.AreEquivalent(actual, expected);
  }
}

#region EnemyDataManagerUtils

public static class EnemyDataManagerUtils {
  public static void SetFileName(this EnemyDataManager manager, string fileName) {
    typeof(EnemyDataManager)
        .GetField("filename", BindingFlags.Instance | BindingFlags.NonPublic)
        .SetValue(manager, fileName);
  }

  public static void InvokeAwake(this EnemyDataManager manager) {
    Type[] argTypes = {};
    MethodInfo awake = typeof(EnemyDataManager).GetMethod(
        "Awake",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    awake.Invoke(manager, null);
  }
}

#endregion EnemyDataManagerUtils