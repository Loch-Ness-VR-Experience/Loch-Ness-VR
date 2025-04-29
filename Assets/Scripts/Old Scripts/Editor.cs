using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class WaypointCreator : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private Color waypointColor = Color.blue;
    [SerializeField] private float waypointSize = 0.5f;
    [SerializeField] private bool showWaypointLabels = true;
    
    [Header("Path Settings")]
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private bool showPath = true;
    
    // List of waypoints
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    
    private void OnDrawGizmos()
    {
        if (waypoints.Count == 0)
            return;
            
        // Draw waypoints
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
                continue;
                
            // Draw waypoint sphere
            Gizmos.color = waypointColor;
            Gizmos.DrawSphere(waypoints[i].position, waypointSize);
            
            // Draw waypoint number
            if (showWaypointLabels)
            {
                Handles.Label(waypoints[i].position + Vector3.up * waypointSize * 1.5f, "Waypoint " + i);
            }
            
            // Draw path
            if (showPath && i < waypoints.Count - 1 && waypoints[i + 1] != null)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
    
    [ContextMenu("Add Waypoint")]
    public void AddWaypoint()
    {
        GameObject waypointObj;
        
        if (waypointPrefab != null)
        {
            waypointObj = PrefabUtility.InstantiatePrefab(waypointPrefab) as GameObject;
        }
        else
        {
            waypointObj = new GameObject("Waypoint " + waypoints.Count);
        }
        
        // Position the new waypoint
        if (waypoints.Count > 0 && waypoints[waypoints.Count - 1] != null)
        {
            waypointObj.transform.position = waypoints[waypoints.Count - 1].position + Vector3.forward * 2f;
        }
        else
        {
            waypointObj.transform.position = transform.position + Vector3.forward * 2f;
        }
        
        // Parent to this object
        waypointObj.transform.parent = transform;
        
        // Add to list
        waypoints.Add(waypointObj.transform);
    }
    
    [ContextMenu("Remove Last Waypoint")]
    public void RemoveLastWaypoint()
    {
        if (waypoints.Count == 0)
            return;
            
        int lastIndex = waypoints.Count - 1;
        if (waypoints[lastIndex] != null)
        {
            DestroyImmediate(waypoints[lastIndex].gameObject);
        }
        
        waypoints.RemoveAt(lastIndex);
    }
    
    [ContextMenu("Clear All Waypoints")]
    public void ClearWaypoints()
    {
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            if (waypoints[i] != null)
            {
                DestroyImmediate(waypoints[i].gameObject);
            }
        }
        
        waypoints.Clear();
    }
    
    public Transform[] GetWaypointsArray()
    {
        // Clean the list from null entries
        waypoints.RemoveAll(item => item == null);
        
        return waypoints.ToArray();
    }
}

// Custom editor for the WaypointCreator
[CustomEditor(typeof(WaypointCreator))]
public class WaypointCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaypointCreator waypointCreator = (WaypointCreator)target;
        
        // Draw the default inspector
        DrawDefaultInspector();
        
        // Add waypoint button
        if (GUILayout.Button("Add Waypoint"))
        {
            waypointCreator.AddWaypoint();
        }
        
        // Remove last waypoint button
        if (GUILayout.Button("Remove Last Waypoint"))
        {
            waypointCreator.RemoveLastWaypoint();
        }
        
        // Clear all waypoints button
        if (GUILayout.Button("Clear All Waypoints"))
        {
            if (EditorUtility.DisplayDialog("Clear Waypoints", 
                "Are you sure you want to clear all waypoints?", "Yes", "No"))
            {
                waypointCreator.ClearWaypoints();
            }
        }
        
        // Set to VR Controller button
        if (GUILayout.Button("Set Waypoints to VR Controller"))
        {
            VRMovementController vrController = FindObjectOfType<VRMovementController>();
            if (vrController != null)
            {
                SerializedObject vrControllerObj = new SerializedObject(vrController);
                SerializedProperty waypointsProperty = vrControllerObj.FindProperty("waypoints");
                
                waypointsProperty.arraySize = 0;
                
                Transform[] waypoints = waypointCreator.GetWaypointsArray();
                waypointsProperty.arraySize = waypoints.Length;
                
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = waypoints[i];
                }
                
                vrControllerObj.ApplyModifiedProperties();
                
                Debug.Log("Set " + waypoints.Length + " waypoints to VR Movement Controller");
            }
            else
            {
                Debug.LogError("No VR Movement Controller found in the scene!");
            }
        }
    }
}
#endif