using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [Header("Components")]
    public PlayerCharacter PlayerCharacterRef;
    public Animator PlayerAnimator;
    [Header("Parameters")]
    public float CharacterSpeedDamping = 0.2f;
    public float CharacterAimingSpeedDamping = 0.2f;
    public float CharacterAimDamping = 0.2f;
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
        // PlayerAnimator.SetBool("IsAimingWeapon", true);
        PlayerAnimator.SetBool("IsAimingWeapon", PlayerCharacterRef.IsAimingWeapon);
        PlayerAnimator.SetFloat("MovementDirection_LR", PlayerCharacterRef.MoveDirectionXYKeyboard.x, 0.2f, deltaTime);
        PlayerAnimator.SetFloat("MovementDirection_FB", PlayerCharacterRef.MoveDirectionXYKeyboard.y, 0.2f, deltaTime);
        if (PlayerCharacterRef.IsAimingWeapon)
        {
            
            // PlayerAnimator.SetFloat("CharacterRotationPitch", , CharacterAimDamping, deltaTime);
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

    public void Magazine_InsideWeapon()
    {
        PlayerCharacterRef.MagazineInHand.transform.SetParent(PlayerCharacterRef.CurrentWeaponEquipped.MagazineAttachmentTo.transform, false);
        PlayerCharacterRef.MagazineInHand.transform.SetLocalPositionAndRotation(PlayerCharacterRef.CurrentWeaponEquipped.MagazineAttachmentOffsetPosition,
            Quaternion.Euler(PlayerCharacterRef.CurrentWeaponEquipped.MagazineAttachmentOffsetRotation));
        PlayerCharacterRef.MagazineInHand = null;
    }

    public void AttachMagazineToHand_Eject()
    {
        PlayerCharacterRef.CurrentWeaponEquipped.WeaponMagazine.transform.SetParent(PlayerCharacterRef.MagazineHoldTransform, false);
        PlayerCharacterRef.MagazineInHand = PlayerCharacterRef.CurrentWeaponEquipped.WeaponMagazine;
        PlayerCharacterRef.MagazineInHand.transform.SetLocalPositionAndRotation(PlayerCharacterRef.MagazineHandAttachmentOffsetPosition,
            Quaternion.Euler(PlayerCharacterRef.MagazineHandAttachmentOffsetRotation));
    }

    public void DetachMagazineFromHand()
    {
        PlayerCharacterRef.MagazineInHand.SetActive(false);
    }

    public void AttachMagazineToHand()
    {
        PlayerCharacterRef.MagazineInHand.SetActive(true);
    }
}
