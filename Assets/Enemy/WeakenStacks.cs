using System.Collections.Generic;

public class WeakenStacks {

  List<WeakenStack> weaks = new();

  public void AddWeakness(float weakness, int num) {
    var stack = weaks.Find((WeakenStack element) => element.weakness == weakness);
    if (stack != null) {
      stack.num += num;
      return;
    }
    weaks.Add(new WeakenStack(weakness, num));
  }

  public class WeakenStack {
    public float weakness;
    public int num;

    public WeakenStack(float weakness, int num) {
      this.weakness = weakness;
      this.num = num;
    }
  }
}

// TODO(emonzon/nnewsom):
//  1. Update new spitting ant tower upgrade path name and upgrade names.
//  2. Remove references to armor tear
//  3. Add WeakenStacks to enemy and add general weakness handling.
//  4. Update tests.