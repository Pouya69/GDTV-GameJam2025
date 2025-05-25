using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Custom Move To Location", story: "Custom Navigate [Agent] to [Location]", category: "Action/Navigation", id: "e541b98d294dd641f024286df964853a")]
public partial class CustomMoveToAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyBaseRef;
    [SerializeReference] public BlackboardVariable<Vector3> Location;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);  // This is CustomTimeDilation.
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");

    // This will only be used in movement without a navigation agent.
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

    private float m_PreviousStoppingDistance;
    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Location.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Location.Value == null)
        {
            return Status.Failure;
        }

        if (m_NavMeshAgent == null)
        {
            Vector3 agentPosition, locationPosition;
            float distance = GetDistanceToLocation(out agentPosition, out locationPosition);
            if (distance <= DistanceThreshold)
            {
                return Status.Success;
            }

            float speed = Speed.Value;//SelfEnemyBaseRef.Value.CurrentMovementSpeed * Speed.Value;

            if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
            {
                float ratio = distance / SlowDownDistance;
                speed = Mathf.Max(0.05f, Speed * ratio);
            }
            
            Vector3 MovementDirection = locationPosition - agentPosition;
            // toDestination.y = 0.0f;  // TODO: This needs attention. Default y would be 0 but now we need to see in what direction the localY is 0. MAYBE...?
            MovementDirection.Normalize();
            
            //agentPosition += MovementDirection * (speed * Time.deltaTime);MySenseHandler
            //Agent.Value.transform.position = agentPosition;

            // Look at the target.
            // Agent.Value.transform.forward = MovementDirection;  We are looking at the target through 
            SelfEnemyBaseRef.Value.MyEnemyController.AddMovementInput(MovementDirection, SelfEnemyBaseRef.Value.CurrentMovementSpeed);
        }
        else if (IsNavigationComplete())
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_Animator != null)
        {
            m_Animator.SetFloat(AnimatorSpeedParam, 0);
        }

        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        }

        m_NavMeshAgent = null;
        m_Animator = null;
    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private Status Initialize()
    {
        if (GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition) <= DistanceThreshold)
        {
            return Status.Success;
        }
        // If using animator, set speed parameter.
        m_Animator = Agent.Value.GetComponentInChildren<Animator>();
        if (m_Animator != null)
        {
            m_Animator.SetFloat(AnimatorSpeedParam, Speed);
        }

        // If using a navigation mesh, set target position for navigation mesh agent.
        m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_NavMeshAgent.speed = Speed;
            m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;
            m_NavMeshAgent.stoppingDistance = DistanceThreshold;
            m_NavMeshAgent.SetDestination(locationPosition);
        }

        return Status.Running;
    }

    private float GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition)
    {
        agentPosition = SelfEnemyBaseRef.Value.CapsuleCollision.transform.position;
        locationPosition = Location.Value;
        return Vector3.Distance(new Vector3(agentPosition.x, agentPosition.y, agentPosition.z), locationPosition);
    }


    public bool IsNavigationComplete()
    {
        return !m_NavMeshAgent.pathPending &&
               m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance &&
               !m_NavMeshAgent.hasPath;
    }
}

