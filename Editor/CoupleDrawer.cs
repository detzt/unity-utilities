using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(Couple<,>))]
public class CoupleDrawer : CompositeDrawer {

    protected override List<Couple<string, string>> GetNames() {
        return new List<Couple<string, string>> {
            new(nameof(Couple<int, int>.Item1), "1"),
            new(nameof(Couple<int, int>.Item2), "2")
        };
    }
}
