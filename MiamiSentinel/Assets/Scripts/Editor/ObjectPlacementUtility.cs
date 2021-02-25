using UnityEngine;
using UnityEditor;

public class ObjectPlacementUtility
{
    [MenuItem("Tools/Randomize Rotation")]
    private static void RandomizeRotation()
    {
        foreach (var go in Selection.gameObjects)
        {
            go.transform.rotation = Random.rotation;
        }
    }
}
