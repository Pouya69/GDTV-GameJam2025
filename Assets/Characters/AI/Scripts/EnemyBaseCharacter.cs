using System;
using UnityEngine;

public class EnemyBaseCharacter : CharacterBase
{
    public EnemyBaseController MyEnemyController;
    public EnemyAnimationScript EnemyAnimationScript;
    public Animator EnemyAnimator;
    [Header("Death")]
    public float DisappearTimerAfterDeath = 10f;
    public EItemDrop ManualItemDrop = EItemDrop.RANDOM;
    public GameObject GravityGrenadeConsumablePrefab;
    public GameObject TimeGrenadeConsumablePrefab;
    public GameObject AmmoConsumablePrefab;
    public GameObject HealthConsumablePrefab;

    public enum EItemDrop
    {
        GRAVITY_GRENADE,
        TIME_GRENADE,
        AMMO,
        HEALTH,
        RANDOM,
    }

    public EItemDrop GetRandomItemDrop() { return (EItemDrop)UnityEngine.Random.Range(0, 3); }

    public GameObject DropLootOnDeath()
    {
        if (ManualItemDrop == EItemDrop.RANDOM)
            ManualItemDrop = GetRandomItemDrop();
        GameObject PrefabToInst;
        switch (ManualItemDrop)
        {
            case EItemDrop.HEALTH:
                PrefabToInst = HealthConsumablePrefab;
                break;
            case EItemDrop.GRAVITY_GRENADE:
                PrefabToInst = GravityGrenadeConsumablePrefab;
                break;
            case EItemDrop.TIME_GRENADE:
                PrefabToInst = TimeGrenadeConsumablePrefab;
                break;
            case EItemDrop.AMMO:
                PrefabToInst = AmmoConsumablePrefab;
                break;
            default:
                PrefabToInst = AmmoConsumablePrefab;
                break;
        }
        Vector3 SpawnLocation = CapsuleCollision.transform.position + (CapsuleCollision.transform.forward * 1.5f);
        GameObject SpawnedLoot = Instantiate(PrefabToInst, SpawnLocation, Quaternion.identity);
        return SpawnedLoot;
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    public override void Awake()
    {
        base.Awake();
        if (this.CurrentWeaponEquipped != null)
        {
            this.CurrentWeaponEquipped.AddedWeaponToCharacter(this);
        }
        Physics.IgnoreCollision(CapsuleCollision, MyEnemyController.PelvisCollider, true);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();
        if (CurrentWeaponEquipped == null) return;
        // Enemy Shoot at player logic.
        CurrentWeaponEquipped.StartShooting();
    }

    public override void Die()
    {
        if (HasDied) return;
        HasDied = true;
        base.Die();
        Destroy(MyEnemyController.MyNavAgent);
        Destroy(MyEnemyController.MyBehaviourTreeAgent);
        Destroy(MyEnemyController.MySenseHandler);
        StartRagdolling();
        GameObject SpawnedLoot = DropLootOnDeath();
        Destroy(this.gameObject, DisappearTimerAfterDeath);
    }

    public override void StopShootingWeapon()
    {
        base.StopShootingWeapon();
    }

    public Vector3 GetRandomShotSpreadDirection(Vector3 TargetShotDirection)
    {
        // TODO
        return TargetShotDirection;
    }

    public void StartRagdolling()
    {
        MyEnemyController.RigidbodyRef.detectCollisions = true;
        MyEnemyController.MyNavAgent.ResetPath();
        MyEnemyController.MyNavAgent.velocity = Vector3.zero;
        MyEnemyController.MyNavAgent.isStopped = true;
        MyEnemyController.LookAtPlayer(false);
        MyEnemyController.RigidbodyRef.isKinematic = true;
        CapsuleCollision.enabled = false;
        MyEnemyController.MyNavAgent.enabled = false;
        MyEnemyController.MyBehaviourTreeAgent.enabled = false;
        this.EnemyAnimationScript.StartRagdolling();
        SkeletalMesh.transform.SetParent(null, true);
        EnemyAnimator.enabled = false;
        StopShootingWeapon();
    }

    public void StopRagdolling(bool IsFront, Vector3 GroundLoc)
    {
        Vector3 FinalLoc = GroundLoc - (MyEnemyController.GetGravityDirection() * GetCapsuleCollisionHeight());
        transform.position = FinalLoc;
        CapsuleCollision.transform.position = FinalLoc;
        SkeletalMesh.transform.SetParent(CapsuleCollision.transform, true);
        SkeletalMesh.transform.position = CapsuleCollision.transform.position + MyEnemyController.SkeletalMeshCapsuleOffset;
        MyEnemyController.MyNavAgent.ResetPath();
        MyEnemyController.MyNavAgent.velocity = Vector3.zero;
        this.EnemyAnimationScript.StopRagdolling(IsFront);
        
    }



    public void RagdollRecoverComplete()
    {
        Debug.LogWarning("Ragdoll Recover complete.");
        CapsuleCollision.enabled = true;
        MyEnemyController.RigidbodyRef.isKinematic = false;
        MyEnemyController.RigidbodyRef.detectCollisions = true;
        
        MyEnemyController.MyNavAgent.enabled = true;
        MyEnemyController.MyNavAgent.ResetPath();
        MyEnemyController.MyNavAgent.velocity = Vector3.zero;
        // MyEnemyController.MyNavAgent.SetDestination(transform.position);
        MyEnemyController.MyBehaviourTreeAgent.enabled = true;
    }

    public bool IsRagdollRecoveryCompleted() { return MyEnemyController.MyNavAgent.enabled; }

    public bool IsRagdolling() { return this.EnemyAnimationScript.IsRagdolling; }

}
