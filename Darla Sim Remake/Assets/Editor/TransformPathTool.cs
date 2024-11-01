using UnityEngine;
using UnityEditor;

public class TransformPathTool : EditorWindow
{
    private Transform selectedTransform;
    private string transformPath;

    [MenuItem("Tools/Copy Transform Path")]
    public static void ShowWindow()
    {
        GetWindow<TransformPathTool>("Transform Path Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Transform Path", EditorStyles.boldLabel);
        selectedTransform = EditorGUILayout.ObjectField("Transform", selectedTransform, typeof(Transform), true) as Transform;

        if (selectedTransform != null)
        {
            transformPath = GetTransformPath(selectedTransform);
            EditorGUILayout.TextField("Path", transformPath);

            if (GUILayout.Button("Copy Path to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = transformPath;
                Debug.Log("Path copied to clipboard: " + transformPath);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a Transform to get its path.", MessageType.Info);
        }
    }

    private string GetTransformPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}