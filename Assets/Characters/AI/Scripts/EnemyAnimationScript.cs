using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;
using GLTFast.Schema;

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
        Vector3 Vel = EnemyCharacterRef.MyEnemyController.RigidbodyRef.linearVelocity;
        Vector3 EnemyRight = EnemyCharacterRef.CapsuleCollision.transform.right;
        Vector3 EnemyForward = EnemyCharacterRef.CapsuleCollision.transform.forward;
        EnemyAnimator.SetFloat("CharacterSpeed", Vel.magnitude, CharacterSpeedDamping, deltaTime);
        //EnemyAnimator.SetBool("HasGrenadeInHand", EnemyCharacterRef.HasGrenadeInHand());
        //  Debug.Log(EnemyCharacterRef.GetCurrentWeaponId());
        // EnemyAnimator.SetInteger("CurrentWeaponID", EnemyCharacterRef.GetCurrentWeaponId());
        // EnemyAnimator.SetBool("IsAimingWeapon", true);
        EnemyAnimator.SetBool("IsAimingWeapon", EnemyCharacterRef.IsAimingWeapon);
        Vel.Normalize();
        //output = output_start + ((output_end - output_start) / (input_end - input_start)) * (input - input_start);
        //Vector3.Angle(EnemyRight, Vel, EnemyRight)
        //EnemyCharacterRef.CapsuleCollision.transform.right
        //    EnemyCharacterRef.CapsuleCollision.transform.forward
        //EnemyAnimator.SetFloat("MovementDirection_LR", EnemyCharacterRef.MoveDirectionXYKeyboard.x, 0.2f, deltaTime);
        //EnemyAnimator.SetFloat("MovementDirection_FB", EnemyCharacterRef.MoveDirectionXYKeyboard.y, 0.2f, deltaTime);
        if (EnemyCharacterRef.IsAimingWeapon)
        {

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
