using System;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyBaseCharacter
{
    [Header("Boss")]
    public BossController MyBossController;
    public BossAnimationScript BossAnimationScript;
    [NonSerialized] public PhysicsObjectBasic PhysObjectInHand;
    [NonSerialized] public List<PhysicsObjectBasic> PhysObjectsSuckedIn = new List<PhysicsObjectBasic>();
    public float BossThrowObjectPower = 10f;
    public BossGravityField BossSuckerField;
    public GameObject EnemySpawnPrefab;
    public List<Transform> EnemySpawnPoints;

    public override void Attack()
    {
        // base.Attack();
        // PUNCH HERE.
        BossAnimationScript.StartedMelee();
    }

    public bool StartAttackThrowObjectsAtPlayer()
    {
        if (PhysObjectInHand == null)
        {
            if (PhysObjectsSuckedIn.Count == 0) return false;
            PhysObjectInHand = PhysObjectsSuckedIn[0];
            PhysObjectsSuckedIn.RemoveAt(0);
        }
        else
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

    public void SummonEnemies()
    {
        for (int i = 0; i < EnemySpawnPoints.Count; i++)
        {
            GameObject EnemySpawned = Instantiate(EnemySpawnPrefab, EnemySpawnPoints[i].position, EnemySpawnPoints[i].rotation);
            PlaySummonEffect(EnemySpawnPoints[i].position);
            EnemyBaseCharacter _ = EnemySpawned.GetComponent<EnemyBaseCharacter>();
        }
    }

    public void PlaySummonEffect(Vector3 Location) {
    
    }

    public void SuckStarted()
    {
        BossSuckerField.TurnOnField();
    }

    public void SuckEnded()
    {
        BossSuckerField.TurnOffField();
    }

    public void CaughtPhysicsObjectSuck(PhysicsObjectBasic PhysObj) {
        this.PhysObjectsSuckedIn.Add(PhysObj);
        PhysObj.RigidbodyRef.isKinematic = true;
        PhysObj.RigidbodyRef.detectCollisions = false;
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

    public override void Update()
    {
        // base.Update();
    }

    public override void Die()
    {
        base.Die();
        // TODO: PLAYER WIN.
    }

    public override void Awake()
    {
        //base.Awake();
    }
}
