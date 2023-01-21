using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;


public class PathManagerTest {
  readonly int tileSpacing = 10;

  // Creates a plus orientation of five waypoints where the central waypoint has exits in all directions.
  // Tests that all directions are handled correctly.
  [Test]
  public void PopulateWaypointsWorks() {
    Waypoint waypointUp = GetWaypoint(Vector3.forward * tileSpacing);
    Waypoint waypointRight = GetWaypoint(Vector3.right * tileSpacing);
    Waypoint waypointDown = GetWaypoint(Vector3.back * tileSpacing);
    Waypoint waypointLeft = GetWaypoint(Vector3.left * tileSpacing);
    Waypoint waypointCenter = GetWaypoint(Vector3.zero * tileSpacing);

    waypointCenter.exits =
        Enum.GetValues(typeof(Waypoint.Direction)).OfType<Waypoint.Direction>().ToList();

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    object[] args = { new Waypoint[] { waypointUp, waypointRight, waypointDown, waypointLeft, waypointCenter } };
    InvokePopulateWaypoints(pathManager, args);

    Assert.That(waypointUp.prevWaypoints,
        Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
    Assert.That(waypointRight.prevWaypoints,
        Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
    Assert.That(waypointDown.prevWaypoints,
        Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
    Assert.That(waypointLeft.prevWaypoints,
        Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
    Assert.That(waypointCenter.nextWaypoints,
        Is.EquivalentTo(new List<Waypoint>() { waypointUp, waypointRight, waypointDown, waypointLeft }));
  }

  // Creates a Waypoint with exits in all directions but no neighbors.
  // Tests the bounds checking of the PathManager.
  [Test]
  public void PopulateWaypointsPointsOutOfBounds() {
    Waypoint waypointCenter = GetWaypoint(Vector3.zero * tileSpacing);

    waypointCenter.exits =
        Enum.GetValues(typeof(Waypoint.Direction)).OfType<Waypoint.Direction>().ToList();

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    object[] args = { new Waypoint[] { waypointCenter } };
    InvokePopulateWaypoints(pathManager, args);

    Assert.That(waypointCenter.nextWaypoints, Is.EquivalentTo(new List<Waypoint>()));
  }

  // Creates a central Waypoint with neighbors above and to the right.  The central Waypoint is rotated -270 degrees.
  // Tests that rotation of Waypoints is handled correctly.
  [Test]
  public void PopulateWaypointsRotation() {
    Waypoint waypointUp = GetWaypoint(Vector3.forward * tileSpacing);
    Waypoint waypointRight = GetWaypoint(Vector3.right * tileSpacing);
    Waypoint waypointCenter = GetWaypoint(Vector3.zero * tileSpacing);

    waypointCenter.exits = new List<Waypoint.Direction>() { Waypoint.Direction.UP };
    waypointCenter.transform.Rotate(-270 * Vector3.up);

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    object[] args = { new Waypoint[] { waypointUp, waypointRight, waypointCenter } };
    InvokePopulateWaypoints(pathManager, args);

    Assert.That(waypointCenter.nextWaypoints, Is.EquivalentTo(new List<Waypoint>() { waypointRight }));
    Assert.That(waypointUp.prevWaypoints, Is.EquivalentTo(new List<Waypoint>()));
    Assert.That(waypointRight.prevWaypoints, Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
  }

  // Tests the case where there are two starts and two ends.
  [Test]
  public void GetDistanceToEndWorks() {
    Waypoint waypoint0 = GetWaypoint(Vector3.zero);
    Waypoint waypoint1 = GetWaypoint(waypoint0.transform.position + Vector3.down);
    Waypoint waypoint2 = GetWaypoint(waypoint1.transform.position + Vector3.left);
    Waypoint waypoint3 = GetWaypoint(waypoint2.transform.position + Vector3.left);
    Waypoint waypoint4 = GetWaypoint(waypoint1.transform.position + Vector3.right);
    Waypoint waypoint5 = GetWaypoint(waypoint1.transform.position + Vector3.down);
    waypoint0.nextWaypoints.Add(waypoint1);
    waypoint1.nextWaypoints.Add(waypoint2);
    waypoint2.nextWaypoints.Add(waypoint3);
    waypoint1.nextWaypoints.Add(waypoint4);
    waypoint5.nextWaypoints.Add(waypoint1);

    PathManager pathManager = new GameObject().AddComponent<PathManager>();
    InvokeGetDistanceToEnd(pathManager, new Waypoint[] { 
      waypoint0, waypoint1, waypoint2, waypoint3, waypoint4, waypoint5 });

    Assert.That(waypoint0.DistanceToEnd, Is.EqualTo(2));
    Assert.That(waypoint1.DistanceToEnd, Is.EqualTo(1));
    Assert.That(waypoint2.DistanceToEnd, Is.EqualTo(1));
    Assert.That(waypoint3.DistanceToEnd, Is.EqualTo(0));
    Assert.That(waypoint4.DistanceToEnd, Is.EqualTo(0));
    Assert.That(waypoint5.DistanceToEnd, Is.EqualTo(2));
  }

  // Creates and returns a Waypoint at location v.
  Waypoint GetWaypoint(Vector3 v) {
    GameObject gameObject = new();
    gameObject.transform.position = v;
    return gameObject.AddComponent<Waypoint>();
  }

  // Uses reflection to call the private method PopulateWaypoints.
  void InvokePopulateWaypoints(PathManager pathManager, object[] args) {
    Type[] argTypes = { typeof(Waypoint[]) };
    MethodInfo populateWaypoints = typeof(PathManager).GetMethod(
        "PopulateWaypoints",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    populateWaypoints.Invoke(pathManager, args);
  }

  void InvokeGetDistanceToEnd(PathManager pathManager, Waypoint[] waypoints) {
    object[] args = { waypoints };
    Type[] argTypes = { typeof(Waypoint[]) };
    MethodInfo getDistanceToEnd = typeof(PathManager).GetMethod(
      "GetDistanceToEnd",
       BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    getDistanceToEnd.Invoke(pathManager, args);
  }
}
