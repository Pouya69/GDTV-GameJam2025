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
}
