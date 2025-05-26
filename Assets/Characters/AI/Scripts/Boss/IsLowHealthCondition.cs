using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Low Health", story: "Is Health Lower than [HEALTH]", category: "Conditions", id: "7ac572f2da57e8e7bcc3a61f9435343b")]
public partial class IsLowHealthCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> HEALTH;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
