#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#else
using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming

namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This attribute can be used on public string fields in custom scripts you write. It will show a dropdown of all Custom Events in the Scene.
    /// </summary>
    public class CoreCustomEventAttribute : PropertyAttribute {
    }
}

#endif
