using System;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyBaseCharacter
{
    [Header("Boss")]
    public BossController MyBossController;
    public BossAnimationScript BossAnimationScript;
    [NonSerialized] public PhysicsObjectBasic PhysObjectInHand;
    [NonSerialized] public List<PhysicsObjectBasic> ObjectsSuckedIn = new List<PhysicsObjectBasic>();
    public float BossThrowObjectPower = 10f;

    public override void Attack()
    {
        // base.Attack();
        // PUNCH HERE.
    }

    public bool StartAttackThrowObjectsAtPlayer()
    {
        if (ObjectsSuckedIn.Count == 0) return false;
        BossAnimationScript.StartThrowObject();
        return true;
    }

    public void StartSuckObjectsAround()
    {
        BossAnimationScript.StartSucking();
    }

    public void StartJumpStomp()
    {
        BossAnimationScript.StartJumpStop();
    }

    public void StartSummonEnemies()
    {
        BossAnimationScript.StartSummonEnemies();
    }

    public void SuckStarted()
    {

    }

    public void SuckEnded()
    {

    }

    public void ThrowObject()
    {
        Vector3 Vel = PhysObjectInHand.transform.forward * BossThrowObjectPower;

        PhysObjectInHand.transform.SetParent(null, true);
        PhysObjectInHand.InitializePhysicsObject(this.MyBossController.BaseGravity, Vel);
        PhysObjectInHand.RigidbodyRef.isKinematic = false;
        PhysObjectInHand.RigidbodyRef.detectCollisions = true;
        PhysObjectInHand.BaseVelocity = Vel;
        PhysObjectInHand.RigidbodyRef.linearVelocity = Vel;
        PhysObjectInHand.GravityBeforeCustomGravity = this.MyBossController.BaseGravity;
        PhysObjectInHand.CheckTimeDilationOnSpawn();
        PhysObjectInHand.UpdatePhysicsObjectBasedOnTimeDilation();
    }
}
