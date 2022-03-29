using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable))]
public class InteractionObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // If we call base the default inspector will get drawn too.
        // Remove this line if you don't want that to happen.
        base.OnInspectorGUI();
    }
}