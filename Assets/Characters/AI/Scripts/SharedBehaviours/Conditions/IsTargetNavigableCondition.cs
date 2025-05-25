using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Target Navigable", story: "Is [Target] Navigable", category: "Conditions", id: "650a6ccb5c34057bcac4b2217ecf9c4b")]
public partial class IsTargetNavigableCondition : Condition
{
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> EnemySelfRef;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    public override bool IsTrue()
    {
        Vector3 MyLoc = EnemySelfRef.Value.CapsuleCollision.transform.position;
        return NavMesh.SamplePosition(MyLoc, out _, Vector3.Distance(MyLoc, Target.Value.position), 0);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
