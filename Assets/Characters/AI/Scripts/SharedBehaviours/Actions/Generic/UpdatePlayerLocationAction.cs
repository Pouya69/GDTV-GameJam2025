using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdatePlayerLocation", story: "Update Player Location", category: "Enemy Character Actions", id: "f568b8a59d573ef80c71dc2786f5f32d")]
public partial class UpdatePlayerLocationAction : Action
{
    public BlackboardVariable<PlayerCharacter> PlayerRef;
    public BlackboardVariable<Vector3> OutVectorResult;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PlayerRef.Value == null)
        {
            return Status.Success;
        }
        OutVectorResult.Value = PlayerRef.Value.CapsuleCollision.transform.position;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

