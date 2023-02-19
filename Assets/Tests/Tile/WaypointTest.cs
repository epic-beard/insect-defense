using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WaypointTest {
  // Creates a Waypoint with one next waypoint and asserts that it is returned
  // by GetNextWaypoint.
  [Test]
  public void GetNextWaypointWorks() {
    Waypoint waypoint = CreateWaypoint();
    Waypoint nextWaypoint = CreateWaypoint();
    waypoint.nextWaypoints.Add(nextWaypoint);

    Assert.That(waypoint.GetNextWaypoint(), Is.EqualTo(nextWaypoint));
  }

  // Creates a waypoint with no next waypoint and asserts that GetNextWaypoint
  // returns null.
  [Test]
  public void GetNextWaypointWorksWhenEmpty() {
    Waypoint waypoint = CreateWaypoint();

    Assert.That(waypoint.GetNextWaypoint(), Is.EqualTo(null));
  }

  // Creates a Waypoint with one prev waypoint and asserts that it is returned
  // by GetPrevWaypoint.
  [Test]
  public void GetPrevWaypointWorks() {
    Waypoint waypoint = CreateWaypoint();
    Waypoint prevWaypoint = CreateWaypoint();
    waypoint.prevWaypoints.Add(prevWaypoint);

    Assert.That(waypoint.GetPrevWaypoint(), Is.EqualTo(prevWaypoint));
  }

  // Creates a waypoint with no prev waypoint and asserts that GetNextWaypoint
  // returns null.
  [Test]
  public void GetPrevWaypointWorksWhenEmpty() {
    Waypoint waypoint = CreateWaypoint();

    Assert.That(waypoint.GetPrevWaypoint(), Is.EqualTo(null));
  }

  // Creates a waypoint and sets its possition.  Checks that GetCoordinates returns
  // the right thing.
  [Test]
  public void GetCoordinatesWorks() {
    Waypoint waypoint = CreateWaypoint();
    waypoint.transform.position = new Vector3(10, 0, 20);

    Assert.That(waypoint.GetCoordinates(), Is.EqualTo(new Vector2Int(1, 2)));
  }

  // Creates and returns a Waypoint.
  Waypoint CreateWaypoint() {
    return new GameObject().AddComponent<Waypoint>();
  }
}
