using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Location Navigable", story: "Is [Location] Navigable", category: "Conditions", id: "7986bd10321d78ba46827b10503afaaa")]
public partial class IsLocationNavigableCondition : Condition
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
