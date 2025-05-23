using System;
using System.Collections;
using UnityEngine;

public class AISenseHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Sight Sense")]
    public Transform RaycastStartPoint;
    public EnemyBaseController EnemyBaseControllerRef;
    public float RaycastDistanceCheck = 20f;  // This basically detects how far away the player is detected from.
    public LayerMask PlayerScanLayerMask = new LayerMask();  // Just in case we add our own layer for player.
    public LayerMask ObstructionLayerMask = new LayerMask();  // Just in case we add our own layer for player.
    public float ScanRadius = 30f;
    public int TimesToCheckPerSecond = 5;
    [SerializeReference] public PlayerCharacter PlayerCharacterRef_CHECK_ONLY;  // This is for checking whether he is blocked or not. Has to be set for every enemy.
    // ONLY pre-defined enemies.
    [NonSerialized] bool HasBeenPreviouslyBlocked = false;  // For optimizied checking and less Blackboard value setting.
    [Range(0, 360)]
    public float FOV_Angle = 120f;
    [NonSerialized] public bool CanSeePlayer = false;

    // For spawners.
    public void InitializeEnemy(PlayerCharacter PlayerCharacterRef_FromLevel)
    {
        this.PlayerCharacterRef_CHECK_ONLY = PlayerCharacterRef_FromLevel;
    }

    private void Awake()
    {
        if (this.TimesToCheckPerSecond < 1)
            this.TimesToCheckPerSecond = 1;
        if (ScanRadius < 1)
            ScanRadius = 1;
    }

    public void CheckForPlayer_SIGHT()
    {
        Vector3 MyLocation = this.RaycastStartPoint.position;
        Collider[] RangeChecks = Physics.OverlapSphere(MyLocation, ScanRadius, PlayerScanLayerMask);
        if (RangeChecks.Length == 0) {
            if (CanSeePlayer)
                EnemyBaseControllerRef.UpdatePlayerCharacterRef(null, PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform.position);
            CanSeePlayer = false;
            return;
        }
        Transform Target = PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform;
        Vector3 DirectionToTarget = (Target.position - MyLocation).normalized;
        float Angle = Vector3.Angle(this.RaycastStartPoint.forward, DirectionToTarget);
        //Debug.Log("Angle to player from forward: " + Angle);
        //Debug.DrawLine(MyLocation, MyLocation + this.RaycastStartPoint.forward * 5, Color.red);
        if (Angle >= FOV_Angle / 2)
        {
            if (CanSeePlayer)
                EnemyBaseControllerRef.UpdatePlayerCharacterRef(null, Target.position);
            CanSeePlayer = false;
            return;
        }
        float DistanceToTarget = Vector3.Distance(Target.position, MyLocation);
        RaycastHit HitResult;

        bool CanSeePlayerNow = !Physics.Raycast(MyLocation, DirectionToTarget, out HitResult, DistanceToTarget, ObstructionLayerMask); //|| HitResult.collider.transform.root.TryGetComponent<PlayerCharacter>(out _);
        if (CanSeePlayerNow)
        {
            if (!CanSeePlayer)
                EnemyBaseControllerRef.UpdatePlayerCharacterRef(PlayerCharacterRef_CHECK_ONLY);
            CanSeePlayer = true;
        }
    }

    private void Start()
    {
        StartCoroutine(FOV_Routine());
    }

    private IEnumerator FOV_Routine()
    {
        WaitForSeconds WaitTime = new WaitForSeconds((float) 1 / TimesToCheckPerSecond);

        while (true)
        {
            CheckForPlayer_SIGHT();

            yield return WaitTime;
        }
    }
}
