using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGen = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridGen.GenerateGrid();
        }
    }
}