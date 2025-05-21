using UnityEngine;

public class Puzzle_Elevator_Side : MonoBehaviour
{
    public float MassOfObjects = 0;
    public GameObject movingObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        MassOfObjects += other.attachedRigidbody.mass;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        MassOfObjects -= other.attachedRigidbody.mass;
    }


}
