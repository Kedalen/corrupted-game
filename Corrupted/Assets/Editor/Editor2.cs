using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor (typeof (Enemy_Behaviour_Improved))]
public class Editor2 : Editor
{
    private void OnSceneGUI()
    {
        Enemy_Behaviour_Improved LineOfSight = (Enemy_Behaviour_Improved)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(LineOfSight.transform.position, Vector3.up, Vector3.forward, 360, LineOfSight.viewRadius);
        Vector3 viewAngleA = LineOfSight.DirFromAngles(-LineOfSight.viewAngle / 2,false);
        Vector3 viewAngleB = LineOfSight.DirFromAngles(+LineOfSight.viewAngle / 2,false);
        Handles.DrawLine(LineOfSight.transform.position, LineOfSight.transform.position + viewAngleA * LineOfSight.viewRadius);
        Handles.DrawLine(LineOfSight.transform.position, LineOfSight.transform.position + viewAngleB * LineOfSight.viewRadius);

    }
}
