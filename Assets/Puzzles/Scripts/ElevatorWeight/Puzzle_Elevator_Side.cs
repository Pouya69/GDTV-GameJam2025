using System;
using UnityEngine;

public class Puzzle_Elevator_Side : MonoBehaviour
{
    public float MassOfObjects = 0;
    public GameObject movingObject;
    [NonSerialized] public PlayerCharacter PlayerOnThisSide; 


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        MassOfObjects += other.attachedRigidbody.mass;
        if (!other.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef)) return;
        //PlayerRef.transform.SetParent(this.transform, true);
        PlayerOnThisSide = PlayerRef;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (!other.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef)) return;
       // PlayerRef.transform.SetParent(null, true);
        MassOfObjects -= other.attachedRigidbody.mass;
        PlayerOnThisSide = null;
    }


}
