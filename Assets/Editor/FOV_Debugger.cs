using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AISenseHandler))]
public class NewMonoBehaviourScript : Editor
{
    private void OnSceneGUI()
    {
        /*
        AISenseHandler handler = (AISenseHandler)target;
        Handles.color = Color.red;
        Vector3 EnemyUp = -handler.EnemyBaseControllerRef.GetGravityDirection();
        Vector3 EnemyForward = handler.EnemyBaseControllerRef.GetEnemyForward();
        Handles.DrawWireArc(handler.transform.position, EnemyUp, EnemyForward, 360, handler.ScanRadius);

        Quaternion CharRotation = Quaternion.LookRotation(, EnemyUp);
        Vector3 ViewAngle01 = DirectionFromAngle(handler.EnemyBaseControllerRef.EnemyBaseCharacterRef.CapsuleCollision.transform.rotation, -handler.FOV_Angle / 2);
        Vector3 ViewAngle02 = DirectionFromAngle(handler.transform.rotation, handler.FOV_Angle / 2);
        */
    }

    private Vector3 DirectionFromAngle(float EulerY, float AngleDegrees)
    {
        AngleDegrees += EulerY;
        float AngleRads = AngleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(AngleRads), 0, Mathf.Cos(AngleRads));
    }
}
