using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(Triple<,,>))]
public class TripleDrawer : CompositeDrawer {

    protected override List<Couple<string, string>> GetNames() {
        return new List<Couple<string, string>> {
            new(nameof(Triple<int, int, int>.Item1), "1"),
            new(nameof(Triple<int, int, int>.Item2), "2"),
            new(nameof(Triple<int, int, int>.Item3), "3")
        };
    }
}
