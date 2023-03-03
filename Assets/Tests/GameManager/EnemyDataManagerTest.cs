using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyDataManagerTest {
  readonly string filename = "Assets/Tests/delete_me";
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
    List<EnemyDataManager.KvPair> enemies = new() {
      new() { Key = enemyName, Value = enemyData, }
    };

    Dictionary<string, EnemyData> expectedDatas = new() {
      { enemyName, enemyData }
    };

    manager.SerializeEnemies(enemies, filename);
    manager.InvokeDeserializeEnemies(filename);
    CollectionAssert.AreEquivalent(manager.GetEnemyDataMap(), expectedDatas);
  }

  [Test]
  public void GetEnemyDataWorks() {
    Dictionary<string, EnemyData> datas = manager.GetEnemyDataMap();
    datas.Add(enemyName, enemyData);

    Assert.That(manager.GetEnemyData(enemyName), Is.EqualTo(enemyData));
  }
}

// Extension methods to hold reflection-based calls to access private fields, properties, or methods of
// EnemyDataManager.
public static class EnemyDataManagerUtils {
  public static void InvokeDeserializeEnemies(this EnemyDataManager manager, string filename) {
    object[] args = { filename };
    Type[] argTypes = { typeof(string) };
    MethodInfo deserializeEnemies = typeof(EnemyDataManager).GetMethod(
      "DeserializeEnemies",
      BindingFlags.NonPublic | BindingFlags.Instance,
      null, CallingConventions.Standard, argTypes, null);
    deserializeEnemies.Invoke(manager, args);
  }

  public static Dictionary<string, EnemyData> GetEnemyDataMap(this EnemyDataManager manager) {
    return (Dictionary<string, EnemyData>)typeof(EnemyDataManager)
      .GetField("datas", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(manager);
  }
}