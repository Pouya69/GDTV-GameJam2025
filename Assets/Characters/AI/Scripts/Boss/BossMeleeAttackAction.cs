using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Melee Attack", story: "Boss Melee Attack", category: "Enemy Character Actions", id: "6a3d0310494019a4824da396a7fc0de1")]
public partial class BossMeleeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfBossCharacter.Value.Attack();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
