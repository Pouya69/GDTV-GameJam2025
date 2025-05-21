using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CUSTOM Patrol", story: "CUSTOM [Agent] patrols [PatrolPoints]", category: "Enemy Character Actions", id: "c89ba34b3c184a9173aa1ce699ce0202")]
public partial class CustomPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyBaseRef;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");
    [Tooltip("Should patrol restart from the latest point?")]
    [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);

    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;
    private float m_PreviousStoppingDistance;

    [CreateProperty]
    private Vector3 m_CurrentTarget;
    [CreateProperty]
    private int m_CurrentPatrolPoint = 0;
    [CreateProperty]
    private bool m_Waiting;
    [CreateProperty]
    private float m_WaypointWaitTimer;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent assigned.");
            return Status.Failure;
        }

        if (PatrolPoints.Value == null || PatrolPoints.Value.Count == 0)
        {
            LogFailure("No waypoints to patrol assigned.");
            return Status.Failure;
        }

        Initialize();

        m_Waiting = false;
        m_WaypointWaitTimer = 0.0f;

        MoveToNextWaypoint();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || PatrolPoints.Value == null)
        {
            return Status.Failure;
        }

        if (m_Waiting)
        {
            if (m_WaypointWaitTimer > 0.0f)
            {
                m_WaypointWaitTimer -= Time.deltaTime;
            }
            else
            {
                m_WaypointWaitTimer = 0f;
                m_Waiting = false;
                MoveToNextWaypoint();
            }
        }
        else
        {
            float distance = GetDistanceToWaypoint();
            Vector3 agentPosition = Agent.Value.transform.position;

            // If we are using navmesh, get the animator speed out of the velocity.
            if (m_Animator != null && m_NavMeshAgent != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, m_NavMeshAgent.velocity.magnitude);
            }

            if (distance <= DistanceThreshold)
            {
                if (m_Animator != null)
                {
                    m_Animator.SetFloat(AnimatorSpeedParam, 0);
                }

                m_WaypointWaitTimer = WaypointWaitTime.Value;
                m_Waiting = true;
            }
            else if (m_NavMeshAgent == null)
            {
                //float speed = Mathf.Min(Speed, distance);


                // toDestination.y = 0.0f;
                // toDestination.Normalize();
                Vector3 MovementDirection = m_CurrentTarget - agentPosition;
                float speed = Speed.Value;//SelfEnemyBaseRef.Value.CurrentMovementSpeed * Speed.Value;
                /*
                if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
                {
                    float ratio = distance / SlowDownDistance;
                    speed = Mathf.Max(0.05f, Speed * ratio);
                }
                */
                // Vector3 MovementDirection = locationPosition - agentPosition;
                // toDestination.y = 0.0f;  // TODO: This needs attention. Default y would be 0 but now we need to see in what direction the localY is 0. MAYBE...?
                MovementDirection.Normalize();

                // agentPosition += toDestination * (speed * Time.deltaTime);
                //Agent.Value.transform.position = agentPosition;
                //Agent.Value.transform.forward = toDestination;



                //agentPosition += MovementDirection * (speed * Time.deltaTime);
                //Agent.Value.transform.position = agentPosition;

                // Look at the target.
                // Agent.Value.transform.forward = MovementDirection;  We are looking at the target through 
                Debug.LogError("No navmeshagent. Patrolling default normalized direction.");
                SelfEnemyBaseRef.Value.MyEnemyController.AddMovementInput(MovementDirection, SelfEnemyBaseRef.Value.CurrentMovementSpeed);
            }
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
    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_Animator = Agent.Value.GetComponentInChildren<Animator>();
        if (m_Animator != null)
        {
            m_Animator.SetFloat(AnimatorSpeedParam, 0);
        }

        m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_NavMeshAgent.speed = Speed.Value;
            m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;
            m_NavMeshAgent.stoppingDistance = DistanceThreshold;
        }

        m_CurrentPatrolPoint = PreserveLatestPatrolPoint.Value ? m_CurrentPatrolPoint - 1 : -1;
    }

    private float GetDistanceToWaypoint()
    {
        if (m_NavMeshAgent != null)
        {
            return m_NavMeshAgent.remainingDistance;
        }

        Vector3 targetPosition = m_CurrentTarget;
        Vector3 agentPosition = SelfEnemyBaseRef.Value.CapsuleCollision.transform.position;

        // agentPosition.y = targetPosition.y; // Ignore y for distance check.

        return Vector3.Distance(
            agentPosition,
            targetPosition
        );
    }

    private void MoveToNextWaypoint()
    {
        m_CurrentPatrolPoint = (m_CurrentPatrolPoint + 1) % PatrolPoints.Value.Count;

        m_CurrentTarget = PatrolPoints.Value[m_CurrentPatrolPoint].transform.position;
        if (m_NavMeshAgent != null)
        {
            m_NavMeshAgent.SetDestination(m_CurrentTarget);
        }
        else if (m_Animator != null)
        {
            // We set the animator speed once if we are using transform.
            m_Animator.SetFloat(AnimatorSpeedParam, Speed.Value);
        }
    }
}

