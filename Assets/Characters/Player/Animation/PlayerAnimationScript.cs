using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [Header("Components")]
    public PlayerCharacter PlayerCharacterRef;
    public Animator PlayerAnimator;
    [Header("Parameters")]
    public float CharacterSpeedDamping = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();
    }

    // Updating the animator's parameters and etc.
    public void UpdateAnimator()
    {
        float deltaTime = Time.deltaTime;
        bool IsPlayerIOnGround = PlayerCharacterRef.MyController.IsOnGround;
        PlayerAnimator.SetBool("IsCharacterOnGround", IsPlayerIOnGround);
        PlayerAnimator.SetFloat("CharacterSpeed", PlayerCharacterRef.MyPlayerController.RigidbodyRef.linearVelocity.magnitude, CharacterSpeedDamping, deltaTime);
        PlayerAnimator.SetBool("HasGrenadeInHand", PlayerCharacterRef.HasGrenadeInHand());
       //  Debug.Log(PlayerCharacterRef.GetCurrentWeaponId());
        PlayerAnimator.SetInteger("CurrentWeaponID", PlayerCharacterRef.GetCurrentWeaponId());
        PlayerAnimator.SetBool("IsAimingWeapon", PlayerCharacterRef.IsAimingWeapon);
        if (PlayerCharacterRef.HasWeaponEquipped())
        {
            //if (!PlayerCharacterRef.CurrentWeaponEquipped.IsReloading)
                
            
        }

        // PlayerAnimator.SetFloat("CharacterRotation", );
    }

    public void PlayMontage(string ClipName, bool UseRootMotion, float NormalizedBlendTime = 0.1f, int MontageLayerIndex=0, float NormalizedTimeOffset=0f)
    {
        PlayerAnimator.applyRootMotion = UseRootMotion;
        PlayerAnimator.CrossFade(ClipName, NormalizedBlendTime, MontageLayerIndex, NormalizedTimeOffset);
    }

    public void GrenadeThrownComplete()
    {
        PlayerCharacterRef.ThrowGrenade();
    }

    public void TriggerGrenadeThrow()
    {
        // Debug.LogWarning("Trying to throw grenade STARTANIM.");
        PlayerAnimator.SetTrigger("ThrowGrenadeTrigger");
    }

    public void ReloadComplete()
    {
        PlayerCharacterRef.ReloadComplete();
    }

    public void TriggerStartReload()
    {
        PlayerAnimator.SetTrigger("ReloadTrigger");
    }
}
