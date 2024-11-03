using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ListAllChildrenComponents : EditorWindow
{
    private GameObject parentObject;

    [MenuItem("Tools/List Children and Components")]
    public static void ShowWindow()
    {
        GetWindow<ListAllChildrenComponents>("List Children and Components");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Parent GameObject", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField(parentObject, typeof(GameObject), true);

        if (GUILayout.Button("List Children Components"))
        {
            ListChildrenAndComponents();
        }
    }

    private void ListChildrenAndComponents()
    {
        if (parentObject == null)
        {
            Debug.LogError("Please select a parent GameObject.");
            return;
        }

        // Create the output file path
        string parentName = parentObject.name.Replace("/", "_").Replace("\\", "_"); // Ensure the name is valid for file naming
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{parentName}'s children and their components.txt");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Children of {parentName} and their components:");

        // Start the recursive search from the parent object
        ListComponentsRecursively(parentObject.transform, sb);

        // Write the output to the file
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"Children and components listed in: {filePath}");
    }

    private void ListComponentsRecursively(Transform parentTransform, StringBuilder sb)
    {
        foreach (Transform child in parentTransform)
        {
            sb.AppendLine($"Child: {child.name}");

            // Get all components on the child object
            Component[] components = child.GetComponents<Component>();
            foreach (var component in components)
            {
                sb.AppendLine($"    Component: {component.GetType().Name}");
            }

            // Recursive call to list components of all children
            ListComponentsRecursively(child, sb);
        }
    }
}
