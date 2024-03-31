using Assets;
using NUnit.Framework;
using UnityEngine;

public class UtilitiesTest {

  [Test]
  public void Vector3DropYReturnsProperVector2() {
    Vector3 input = new Vector3(1, 2, 3);
    Vector2 expectedOutput = new Vector2(1, 3);

    Assert.That(Utilities.Vector3DropY(input), Is.EqualTo(expectedOutput));
  }

  [Test]
  public void Vector3DropYDoesNotAlterBaseVector3() {
    Vector3 input = new Vector3(1, 2, 3);
    Vector2 expectedOutput = new Vector2(1, 3);

    Assert.That(Utilities.Vector3DropY(input), Is.EqualTo(expectedOutput));
    Assert.That(input, Is.EqualTo(new Vector3(1, 2, 3)));
  }
}
