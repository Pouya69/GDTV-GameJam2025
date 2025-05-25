using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Spot Around Location", story: "Run EQS", category: "Action", id: "8f1bc6a21e09c66b8ebc9d5847e50280")]
public partial class FindSpotAroundLocationAction : Action
{
    public struct FEQS_Result
    {
        public Vector3 Location;
        public bool IsCollidingWithEnvironment;
        public bool IsBlockedByStartPoint;
        public float Distance;
        public bool IsNavigable;

        public FEQS_Result(Vector3 Location, bool IsCollidingWithEnvironment, bool IsBlockedByStartPoint, float Distance, bool IsNavigable)
        {
            this.Location = Location;
            this.IsCollidingWithEnvironment = IsCollidingWithEnvironment;
            this.IsBlockedByStartPoint = IsBlockedByStartPoint;
            this.Distance = Distance;
            this.IsNavigable = IsNavigable;
        }
    }

    public enum EFindSpotPreference
    {
        PREFER_CLOSER,
        PREFER_FURTHER,
    }

    public enum EFindSpotBlockPreference
    {
        DEFAULT,
        PREFER_BLOCKED,
        PREFER_NOT_BLOCKED,
    }

    public enum EFindSpotPriority
    {
        RANDOM,
        RANDOM_TOP_5,
        RANDOM_TOP_10,
        BEST,
    }

    [SerializeReference] public BlackboardVariable<int> DivideScanIntoSectionsNum = new BlackboardVariable<int>(50);
    [SerializeReference] public BlackboardVariable<float> MinDistanceFromStartPoint;
    [SerializeReference] public BlackboardVariable<bool> ClosenessCheckToSelfRef;  // If true, we sort the distance based on SelfRef instead of StartPoint
    [SerializeReference] public BlackboardVariable<bool> HasToBeNavigable;
    [SerializeReference] public BlackboardVariable<Vector3> CheckAreaBoxSize;// = new Vector3(8, 8, 8);
    [SerializeReference] public BlackboardVariable<float> SphereRadiusCheck;// = 0.5f;  // The sphere itself and how big it is.
    [SerializeReference] public BlackboardVariable<EFindSpotPriority> FindSpotPriority;// = EFindSpotPriority.RANDOM_TOP_5;
    [SerializeReference] public BlackboardVariable<EFindSpotPreference> FindSpotPreference;// = EFindSpotPreference.PREFER_CLOSER;
    [SerializeReference] public BlackboardVariable<EFindSpotBlockPreference> FindSpotBlockPreference;// = EFindSpotBlockPreference.DEFAULT;
    [SerializeReference] public BlackboardVariable<int> ScanLayer;
    [SerializeReference] public BlackboardVariable<GameObject> SelfRef;
    // [SerializeReference] public BlackboardVariable<bool> IsStartPointVector3;
    // [SerializeReference] public BlackboardVariable<Transform> StartPointTransform;  // Usually the Self itself suffices. But sometime you want to hide from player and need to use PlayerRef instead.
    [SerializeReference] public BlackboardVariable<Vector3> StartPointVector3;  // Usually the Self itself suffices. But sometime you want to hide from player and need to use PlayerRef instead.
    [SerializeReference] public BlackboardVariable<Vector3> OutResult;  // This is where the result of the EQS is stored.

    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyBaseCharacter;
    [NonSerialized] private List<FEQS_Result> EQS_Results;
    [NonSerialized] private bool IsRunningEQS = true;
    [NonSerialized] private bool EQS_Result_Success = false;

    protected override Status OnStart()
    {
        EQS_Results = new List<FEQS_Result>();
        IsRunningEQS = true;

        var host = SelfRef.Value.GetComponent<MonoBehaviour>();
        host.StartCoroutine(RunCheckCoroutine());

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsRunningEQS)
            return Status.Running;
        else
            return Status.Success;
    }

    protected override void OnEnd()
    {
        if (EQS_Results != null)
        {
            EQS_Results.Clear();
            EQS_Results = null;
        }
    }

    private IEnumerator RunCheckCoroutine()
    {
        Vector3 areaSize = CheckAreaBoxSize.Value;
        float sphereRad = SphereRadiusCheck.Value;
        int stepsX = Mathf.FloorToInt(areaSize.x / sphereRad);
        int stepsY = Mathf.FloorToInt(areaSize.y / sphereRad);
        int stepsZ = Mathf.FloorToInt(areaSize.z / sphereRad);
        Vector3 halfSize = areaSize * 0.5f;
        float spacing = sphereRad;
        float minDistAcceptable = MinDistanceFromStartPoint.Value;

        Vector3 startPos = StartPointVector3.Value;
        Quaternion rotation = SelfEnemyBaseCharacter.Value.CapsuleCollision.transform.rotation;

        EQS_Results.Clear();

        for (int x = 0; x <= stepsX; x++)
        {
            for (int y = 0; y <= stepsY; y++)
            {
                for (int z = 0; z <= stepsZ; z++)
                {
                    Vector3 offset = new Vector3(
                        -halfSize.x + x * spacing,
                        -halfSize.y + y * spacing,
                        -halfSize.z + z * spacing);

                    float distance = offset.magnitude;
                    if (minDistAcceptable > 0.5f && distance <= minDistAcceptable)
                        continue;

                    Vector3 checkPos = startPos + (rotation * offset);

                    bool isNavigable = !HasToBeNavigable.Value || NavMesh.SamplePosition(checkPos, out _, distance, 0);

                    bool isHit = Physics.CheckSphere(checkPos, sphereRad, ScanLayer.Value, QueryTriggerInteraction.Ignore);
                    RaycastHit hit;
                    bool isBlocked = Physics.Raycast(checkPos, (startPos - checkPos).normalized, out hit, distance, ScanLayer.Value);

                    if (isBlocked)
                    {
                        bool isEnemy = hit.collider.transform.root.TryGetComponent(out EnemyBaseCharacter checkOther);
                        isBlocked = !isEnemy || !checkOther.Equals(SelfEnemyBaseCharacter.Value);
                    }

                    EQS_Results.Add(new FEQS_Result(checkPos, isHit, isBlocked, distance, isNavigable));

                    // Yield every few iterations to spread work
                    if (EQS_Results.Count % DivideScanIntoSectionsNum == 0)
                        yield return null; // resume next frame
                }
            }
        }

        ProcessResults();
    }




    private void ProcessResults()
    {
        int numResults = EQS_Results.Count;
        if (numResults == 0)
        {
            Debug.LogWarning("No EQS results.");
            IsRunningEQS = false;
            EQS_Result_Success = false;
            return;
        }

        switch (FindSpotPriority.Value)
        {
            case EFindSpotPriority.RANDOM:
                SetFinalOutResultEQS(EQS_Results[Random.Range(0, numResults)]);
                break;
            case EFindSpotPriority.BEST:
                GetBestOutOfResult(numResults);
                break;
            case EFindSpotPriority.RANDOM_TOP_5:
                GetBestOutOfResult(numResults, 5);
                break;
        }

        IsRunningEQS = false;
        EQS_Result_Success = true;
    }








    public bool RunCheck()
    {
        Vector3 AreaBoxValue = CheckAreaBoxSize.Value;
        bool MustBeNav = HasToBeNavigable.Value;
        float SphereRad = SphereRadiusCheck.Value;
        int stepsX = Mathf.FloorToInt(AreaBoxValue.x / SphereRad);
        int stepsY = Mathf.FloorToInt(AreaBoxValue.y / SphereRad);
        int stepsZ = Mathf.FloorToInt(AreaBoxValue.z / SphereRad);
        float spacing = SphereRad;
        float minDistAcceptable = MinDistanceFromStartPoint.Value;
        Vector3 StartLocation = StartPointVector3.Value;
        Vector3 halfSize = AreaBoxValue * 0.5f;

        for (int x = 0; x <= stepsX; x++)
        {
            for (int y = 0; y <= stepsY; y++)
            {
                for (int z = 0; z <= stepsZ; z++)
                {
                    Vector3 Offset = new Vector3(-halfSize.x + x * spacing,
                        -halfSize.y + y * spacing,
                        -halfSize.z + z * spacing);
                    float Distance = Offset.magnitude;
                    if (minDistAcceptable > 0.5 && Distance <= minDistAcceptable) continue;
                    Vector3 TargetLocationCheck = StartLocation + (SelfEnemyBaseCharacter.Value.CapsuleCollision.transform.rotation * Offset);
                    bool IsNavigable = MustBeNav ? NavMesh.SamplePosition(TargetLocationCheck, out _, Distance, 0) || NavMesh.SamplePosition(TargetLocationCheck, out _, Offset.magnitude, 2) : true;
                    bool IsHit = Physics.CheckSphere(TargetLocationCheck, SphereRad, ScanLayer.Value, QueryTriggerInteraction.Ignore);
                    RaycastHit raycastHit;
                    bool IsBlockedByStartPoint = Physics.Raycast(TargetLocationCheck, (StartLocation - TargetLocationCheck).normalized, out raycastHit, Distance, ScanLayer.Value);
                    if (IsBlockedByStartPoint)
                    {
                        bool IsAnotherEnemy = raycastHit.collider.transform.root.TryGetComponent<EnemyBaseCharacter>(out EnemyBaseCharacter CheckOther);
                        IsBlockedByStartPoint = !IsAnotherEnemy || !CheckOther.Equals(SelfEnemyBaseCharacter.Value);
                    }
                    EQS_Results.Add(new FEQS_Result(
                        TargetLocationCheck,
                        IsHit,
                        IsBlockedByStartPoint,
                        Distance,
                        IsNavigable
                    ));
                    //Debug.DrawRay(point, Vector3.u-halfSize.z + z * spacingp * 0.5f, Color.green, 1f);
                }
            }
        }

        int NumResults = EQS_Results.Count;

        if (NumResults == 0)
        {
            Debug.LogError("Could not find a point for EQS.");
            IsRunningEQS = false;
            EQS_Result_Success = false;
            return false;
        }

        switch (FindSpotPriority.Value)
        {
            case EFindSpotPriority.RANDOM:
                SetFinalOutResultEQS(NumResults == 1 ? EQS_Results[0] : EQS_Results[Random.Range(0, NumResults - 1)]);
                break;
            case EFindSpotPriority.BEST:
                GetBestOutOfResult(NumResults);
                break;
            case EFindSpotPriority.RANDOM_TOP_5:
                GetBestOutOfResult(NumResults, 5);
                break;
            default:
                break;
        }
        IsRunningEQS = false;
        EQS_Result_Success = true;
        return true;
    }






    public void GetBestOutOfResult(int NumResults, int TopAmount = 1)
    {
        bool IsCheckingDistanceWithSelfRef = ClosenessCheckToSelfRef.Value;
        Vector3 SelfLoc = SelfEnemyBaseCharacter.Value.CapsuleCollision.transform.position;
        EQS_Results.Sort((x, y) =>
        {
            int result = FindSpotBlockPreference.Value == EFindSpotBlockPreference.PREFER_NOT_BLOCKED ? x.IsBlockedByStartPoint.CompareTo(y.IsBlockedByStartPoint) : y.IsBlockedByStartPoint.CompareTo(x.IsBlockedByStartPoint);
            if (result == 0)
            {
                if (IsCheckingDistanceWithSelfRef)
                    result = FindSpotPreference.Value == EFindSpotPreference.PREFER_CLOSER ? Vector3.Distance(x.Location, SelfLoc).CompareTo(Vector3.Distance(y.Location, SelfLoc)) : Vector3.Distance(y.Location, SelfLoc).CompareTo(Vector3.Distance(x.Location, SelfLoc));

                else
                    result = FindSpotPreference.Value == EFindSpotPreference.PREFER_CLOSER ? x.Distance.CompareTo(y.Distance) : y.Distance.CompareTo(x.Distance);
            }
            return result;
        });
        if (TopAmount > 1 && TopAmount > NumResults)
            TopAmount -= 1;
        SetFinalOutResultEQS(TopAmount == 1 ? EQS_Results[0] : EQS_Results[Random.Range(0, TopAmount - 1)]);
    }

    public void SetFinalOutResultEQS(FEQS_Result EQS_ResultSelected)
    {
        this.OutResult.Value = EQS_ResultSelected.Location;
    }

}
