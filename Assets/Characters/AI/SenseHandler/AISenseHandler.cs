using System;
using UnityEngine;

public class AISenseHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Sight Sense")]
    public MeshCollider SightSense_Collider;
    public EnemyBaseController EnemyBaseControllerRef;
    public float RaycastDistanceCheck = 15f;  // This basically detects how far away the player is detected from.
    public int ScanLayerMask = 0;  // Just in case we add our own layer for player.
    [NonSerialized] private PlayerCharacter PlayerCharacterRef_InCollisionArea;  // This is for checking whether he is blocked or not.
    [NonSerialized] bool HasBeenPreviouslyBlocked = false;  // For optimizied checking and less Blackboard value setting.

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerCharacterRef_InCollisionArea != null) return;
        Transform ColliderRootTransform = other.transform.root;
        if (ColliderRootTransform.Equals(this.transform.root)) return;
        ColliderRootTransform.TryGetComponent<PlayerCharacter>(out PlayerCharacterRef_InCollisionArea);
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerCharacterRef_InCollisionArea == null) return;
        EnemyBaseControllerRef.UpdatePlayerCharacterRef();  // Setting it to null.
        PlayerCharacterRef_InCollisionArea = null;
        HasBeenPreviouslyBlocked = false;
    }

    public bool IsPlayerBlocked()
    {
        Vector3 MySightLocation = transform.position;
        RaycastHit HitResult;
        bool Blocked = Physics.Raycast(MySightLocation, (PlayerCharacterRef_InCollisionArea.CapsuleCollision.transform.position - MySightLocation).normalized, out HitResult, RaycastDistanceCheck, ScanLayerMask);
        if (!Blocked) return false;
        return !HitResult.collider.transform.root.Equals(PlayerCharacterRef_InCollisionArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacterRef_InCollisionArea == null) return;
        bool IsBlockedResult = IsPlayerBlocked();
        if (IsBlockedResult && !HasBeenPreviouslyBlocked)
        {
            HasBeenPreviouslyBlocked = true;
            EnemyBaseControllerRef.UpdatePlayerCharacterRef(null);
            return;
        } else if (!IsBlockedResult && HasBeenPreviouslyBlocked)
        {
            EnemyBaseControllerRef.UpdatePlayerCharacterRef(PlayerCharacterRef_InCollisionArea);
            HasBeenPreviouslyBlocked = false;
        }
        
    }
}
