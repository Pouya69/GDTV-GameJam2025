using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot Player", story: "Shoot at Player", category: "Enemy Character Actions", id: "e17816ea6abce3f1eaa135629e4d5646")]
public partial class ShootPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> SelfRef;
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyBaseCharacter;
    [SerializeReference] public BlackboardVariable<bool> IsShootingRef;  // true => StartShooting(), false => StopShootingWeapon()
    protected override Status OnStart()
    {
        if (IsShootingRef.Value)
            SelfEnemyBaseCharacter.Value.Attack();
        else
            SelfEnemyBaseCharacter.Value.StopShootingWeapon();
        return Status.Success;
    }
}
