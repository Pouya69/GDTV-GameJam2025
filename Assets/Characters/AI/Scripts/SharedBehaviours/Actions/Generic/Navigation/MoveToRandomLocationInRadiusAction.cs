using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToRandomLocationInRadius", story: "Find random location in [Radius]", category: "Action/Navigation", id: "643ebd3ba93ced1637274f9a68fa25a6")]
public partial class MoveToRandomLocationInRadiusAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyRef;
    [SerializeReference] public BlackboardVariable<Vector3> OutLocation;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * Radius;
        Vector3 MyLocation = SelfEnemyRef.Value.MyEnemyController.MyNavAgent.transform.position;
        randomDirection += MyLocation;
        OutLocation.Value = NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, Radius, NavMesh.AllAreas) ? hit.position : MyLocation;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

