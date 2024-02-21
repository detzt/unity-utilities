using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMax<>))]
public class MinMaxDrawer : CompositeDrawer {

    protected override List<Couple<string, string>> GetNames() {
        return new List<Couple<string, string>> {
            new(nameof(MinMax<int>.Min), "░ "),
            new(nameof(MinMax<int>.Max), " -")
        };
    }
}
