using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;

public class EnemyAnimationScript : MonoBehaviour
{
    [Header("Components")]
    public EnemyBaseCharacter EnemyCharacterRef;
    public Animator EnemyAnimator;
    [Header("Parameters")]
    public float CharacterSpeedDamping = 0.2f;
    public float CharacterAimingSpeedDamping = 0.2f;
    public float CharacterAimDamping = 0.2f;
    public bool IsRagdolling = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        UpdateAnimator();
    }

    // Updating the animator's parameters and etc.
    public void UpdateAnimator()
    {
        float deltaTime = Time.deltaTime;
        bool IsEnemyOnGround = EnemyCharacterRef.MyController.IsOnGround;
        EnemyAnimator.SetFloat("TimeDIlation", EnemyCharacterRef.MyEnemyController.CustomTimeDilation);
        EnemyAnimator.SetBool("IsCharacterOnGround", IsEnemyOnGround);
        Vector3 Vel = EnemyCharacterRef.MyEnemyController.GetEnemyForward();  // THIS IS MOVEMENT DIRECTION OF NAVAGENT
        EnemyAnimator.SetFloat("CharacterSpeed", Vel.magnitude, CharacterSpeedDamping, deltaTime);
        EnemyAnimator.SetBool("IsAimingWeapon", EnemyCharacterRef.IsAimingWeapon);
        
        if (EnemyCharacterRef.IsAimingWeapon)
        {
            Vector3 LocalVel = EnemyCharacterRef.CapsuleCollision.transform.InverseTransformDirection(Vel);
            // Debug.LogError("Forward: " + LocalVel.z + ", Right: " + LocalVel.x);
            EnemyAnimator.SetFloat("MovementDirection_LR", LocalVel.x, 0.2f, deltaTime);
            EnemyAnimator.SetFloat("MovementDirection_FB", LocalVel.z, 0.2f, deltaTime);
            // EnemyAnimator.SetFloat("CharacterRotationPitch", , CharacterAimDamping, deltaTime);
        }

        // EnemyAnimator.SetFloat("CharacterRotation", );
    }

    public void StartRagdolling()
    {
        Debug.LogWarning("Trying to ragdoll...");
        this.IsRagdolling = true;
        EnemyAnimator.SetBool("IsRagdolling", true);
        this.EnemyAnimator.enabled = false;
        //EnemyCharacterRef.MyEnemyController.IK_Aim_RigAnimation.enabled = false;
    }

    public void StopRagdolling(bool IsFront)
    {
        Debug.LogWarning("Stopping Ragdoll. Front: " + IsFront);

        EnemyCharacterRef.MyEnemyController.CacheRagdollPose();
        EnemyAnimator.Play(IsFront ? "GetUpFront" : "GetUpBack", 0);
        EnemyAnimator.Play(IsFront ? "GetUpFront" : "GetUpBack", 1);
        EnemyAnimator.Update(0f); // force update to apply pose
        EnemyAnimator.enabled = true;
        StartCoroutine(EnemyCharacterRef.MyEnemyController.BlendToAnimatorPose(0.5f));
        EnemyCharacterRef.MyEnemyController.IsTryingToRecoverFromRagdoll = false;
        this.IsRagdolling = false;
        // this.EnemyAnimator.enabled = true;
        //EnemyCharacterRef.MyEnemyController.IK_Aim_RigAnimation.enabled = true;

        // EnemyAnimator.SetBool("IsGettingUpFromFront", IsFront);
        // EnemyAnimator.SetBool("IsRagdolling", false);
    }

    public void RagdollRecoverCompleted()
    {
        EnemyCharacterRef.RagdollRecoverComplete();
    }
}
