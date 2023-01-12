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
        Waypoint waypointUp = GetWaypoint(Vector3.up * tileSpacing);
        Waypoint waypointRight = GetWaypoint(Vector3.right * tileSpacing);
        Waypoint waypointDown = GetWaypoint(Vector3.down * tileSpacing);
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
        Waypoint waypointUp = GetWaypoint(Vector3.up * tileSpacing);
        Waypoint waypointRight = GetWaypoint(Vector3.right * tileSpacing);
        Waypoint waypointCenter = GetWaypoint(Vector3.zero * tileSpacing);

        waypointCenter.exits = new List<Waypoint.Direction>() { Waypoint.Direction.UP };
        waypointCenter.transform.Rotate(-270*Vector3.forward);

        PathManager pathManager = new GameObject().AddComponent<PathManager>();
        object[] args = { new Waypoint[] { waypointUp, waypointRight, waypointCenter } };
        InvokePopulateWaypoints(pathManager, args);

        Assert.That(waypointCenter.nextWaypoints, Is.EquivalentTo(new List<Waypoint>() { waypointRight }));
        Assert.That(waypointUp.prevWaypoints, Is.EquivalentTo(new List<Waypoint>()));
        Assert.That(waypointRight.prevWaypoints, Is.EquivalentTo(new List<Waypoint>() { waypointCenter }));
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
}
