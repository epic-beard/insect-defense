using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using EnemyDictionary = EpicBeardLib.Containers.SerializableDictionary<string, EnemyData>;

public class EnemyDataManagerTest {
  readonly string enemyName = "Beetle";
  EnemyData enemyData;
  EnemyDataManager manager;

  [SetUp]
  public void SetUp() {
    EnemyData child = new() {
      type = EnemyData.Type.BEETLE,
      size = EnemyData.Size.TINY,
    };
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
        num = 6.5f,
        interval = 7.5f,
        child = child,
      },
      carrier = new() {
        num = 8,
        child = child,
      },
    };

    manager = new GameObject().AddComponent<EnemyDataManager>();
  }

  // Round trip an enemy data through serialization then deserialization.
 [Test]
  public void SerializationWorks() {
    EnemyDictionary expectedEnemies = new() {
      { enemyName, enemyData }
    };

    MemoryStream memStream = new();
    TextWriter writer = new StreamWriter(memStream);
    manager.SerializeEnemies(expectedEnemies, writer);

    memStream.Position = 0;

    manager.InvokeDeserializeEnemies(memStream);
    CollectionAssert.AreEquivalent(manager.GetEnemyDataMap(), expectedEnemies);
  }

  [Test]
  public void GetEnemyDataWorks() {
    EnemyDictionary datas = manager.GetEnemyDataMap();
    datas.Add(enemyName, enemyData);

    Assert.That(manager.GetEnemyData(enemyName), Is.EqualTo(enemyData));
  }
}

// Extension methods to hold reflection-based calls to access private fields, properties, or methods of
// EnemyDataManager.
public static class EnemyDataManagerUtils {
  public static void InvokeDeserializeEnemies(this EnemyDataManager manager, Stream stream) {
    object[] args = { stream };
    Type[] argTypes = { typeof(Stream) };
    MethodInfo deserializeEnemies = typeof(EnemyDataManager).GetMethod(
      "DeserializeEnemies",
      BindingFlags.NonPublic | BindingFlags.Instance,
      null, CallingConventions.Standard, argTypes, null);
    deserializeEnemies.Invoke(manager, args);
  }

  public static EnemyDictionary GetEnemyDataMap(this EnemyDataManager manager) {
    return (EnemyDictionary)typeof(EnemyDataManager)
      .GetField("enemies", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(manager);
  }
}