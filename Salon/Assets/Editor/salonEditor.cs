#if UNITY_EDITOR

using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (salon))]
public class salonEditor : Editor {

    public override void OnInspectorGUI () {
        DrawDefaultInspector ();

        salon editorScript = (salon) target;
        if (GUILayout.Button ("Export OBJ ")) {
            editorScript.ExportOBJ ();
        }
    }
}
#endif