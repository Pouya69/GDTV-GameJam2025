using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{

    public enum EPlayerState
    {
        GAMEPLAY_DEFAULT,
        IN_CINEMATIC,
        IN_COMBAT,
    }

    //[Header("Weapons")]

    [Header("Input Actions")]
    InputAction MoveAction;
    InputAction LookAction;
    InputAction JumpAction;
    InputAction SprintAction;
    InputAction InteractAction;
    InputAction AttackPrimaryAction;
    InputAction AttackSecondaryAction;
    InputAction ChangeWorldGravityAction;
    InputAction ChangeSelfGravityAction;
    InputAction ChangeWorldGravityAction_DIRECTION;
    InputAction ChangeSelfGravityAction_DIRECTION;
    InputAction ReloadAction;
    InputAction GrenadeAction;
    InputAction ChangeSelectedGrenade;

    [Header("Movements")]
    public float BoostUpForce = 100f;
    [NonSerialized] public Vector2 MoveDirectionXYKeyboard;  // Just the keyboard WASD in the form of Vector2. For Gravity Change.
    [Header("Components")]
    public CinemachineCamera CameraComp;
    public PlayerController MyPlayerController;
    public BoxCollider InteractionOverlapZone;
    public InventoryComponent InventoryComp;
    public PlayerAnimationScript PlayerAnimation;
    [Header("Interaction")]
    public float InteractionDistance = 1f;  // How far away an object should be for interaction.
    public float PickableInteractablePickupDistance = 0.2f;
    [NonSerialized] public float InteractionAmount = 0f;
    [NonSerialized] List<GameObject> NearbyInteractables = new List<GameObject>();
    [NonSerialized] public InteractableBase ClosestInteractable = null;
    [NonSerialized] public bool IsInteracting = false;
    [NonSerialized] public bool CanInteract = true;
    [Header("Time Dilation On Objects")]
    [NonSerialized] public TimeDilationField CurrentTimeDilationFieldActive = null;
    [NonSerialized] public bool IsJumpBoosting = false;
    [Header("Grenades")]
    public Transform GrenadeAttachPointToHand;
    public float GrenadeThrowPower = 5f;
    [NonSerialized] public bool CurrentGrenadeSelected = true;  // For deciding whether to throw the time or gravity grenade. true => TimeDilationField   false => GravityField
    [NonSerialized] public FieldBaseGrenade CurrentGrenadeInHand = null;
    [Header("Weapon Reloads")]
    public Vector3 MagazineHandAttachmentOffsetRotation;
    public Vector3 MagazineHandAttachmentOffsetPosition;
    public Transform MagazineHoldTransform;
    [NonSerialized] public GameObject MagazineInHand = null;

    [Header("Player State")]
    public EPlayerState CurrentPlayerState = EPlayerState.GAMEPLAY_DEFAULT;  // Can be used to check for combat, and gameplay states.

    public override void Awake()
    {
        base.Awake();
    }

    public void SetupPlayerActions() {
        SprintAction = InputSystem.actions.FindAction("Sprint");
        SprintAction.Enable();
        ChangeSelectedGrenade = InputSystem.actions.FindAction("Change Selected Grenade");
        ChangeSelectedGrenade.Enable();
        GrenadeAction = InputSystem.actions.FindAction("Grenade");
        GrenadeAction.Enable();
        InteractAction = InputSystem.actions.FindAction("Interact");
        InteractAction.Enable();
        ReloadAction = InputSystem.actions.FindAction("Reload");
        ReloadAction.Enable();
        ChangeWorldGravityAction = InputSystem.actions.FindAction("Change Gravity World");
        ChangeWorldGravityAction.Enable();
        ChangeWorldGravityAction_DIRECTION = InputSystem.actions.FindAction("Change Gravity World Direction");
        ChangeWorldGravityAction_DIRECTION.Enable();
        ChangeSelfGravityAction = InputSystem.actions.FindAction("Change Gravity Self");
        ChangeSelfGravityAction.Enable();
        ChangeSelfGravityAction_DIRECTION = InputSystem.actions.FindAction("Change Gravity Self Direction");
        ChangeSelfGravityAction_DIRECTION.Enable();
        MoveAction = InputSystem.actions.FindAction("Move");
        MoveAction.Enable();
        LookAction = InputSystem.actions.FindAction("Look");
        LookAction.Enable();
        JumpAction = InputSystem.actions.FindAction("Jump");
        JumpAction.Enable();
        AttackPrimaryAction = InputSystem.actions.FindAction("Attack");
        AttackPrimaryAction.Enable();
        AttackSecondaryAction = InputSystem.actions.FindAction("AttackSecondary");
        AttackSecondaryAction.Enable();
        SprintAction.performed += SprintAction_performed;
        SprintAction.canceled += SprintAction_canceled;
        ChangeSelectedGrenade.performed += ChangeSelectedGrenade_performed;
        GrenadeAction.performed += GrenadeAction_performed;
        GrenadeAction.canceled += GrenadeAction_canceled;
        InteractAction.performed += InteractAction_performed;
        InteractAction.canceled += InteractAction_canceled;
        ReloadAction.performed += ReloadAction_performed;
        JumpAction.performed += Jump_performed;
        AttackPrimaryAction.performed += AttackPrimary_performed;
        AttackSecondaryAction.performed += AttackSecondary_performed;
        AttackSecondaryAction.canceled += AttackSecondary_canceled;
        AttackPrimaryAction.canceled += AttackPrimary_canceled;
        JumpAction.canceled += Jump_canceled;
        ChangeSelfGravityAction.performed += ChangeSelfGravityAction_performed;
        ChangeWorldGravityAction.performed += ChangeWorldGravityAction_performed;
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        if (IsAimingWeapon) return;
        this.StopSprint();
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        if (IsAimingWeapon) return;
        this.StartSprint();
    }

    private void ChangeSelectedGrenade_performed(InputAction.CallbackContext obj)
    {
        this.CurrentGrenadeSelected = !this.CurrentGrenadeSelected;
        Debug.LogWarning("Grenade Selected: " + (this.CurrentGrenadeSelected ? "Time" : "Gravity"));
    }

    private void GrenadeAction_canceled(InputAction.CallbackContext obj)
    {
        StartThrowingGrenade();
    }

    private void GrenadeAction_performed(InputAction.CallbackContext obj)
    {
        if (CurrentGrenadeSelected)
            GiveTimeGrenadeToPlayerFromInventory();
        else
            GiveGravityGrenadeToPlayerFromInventory();
    }

    private void ReloadAction_performed(InputAction.CallbackContext obj)
    {
        Reload();
    }

    private void InteractAction_canceled(InputAction.CallbackContext obj)
    {
        IsInteracting = false;
        InteractionAmount = 0f;
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        IsInteracting = true;
        CanInteract = true;
    }

    private void ChangeWorldGravityAction_performed(InputAction.CallbackContext obj)
    {
        Vector3 Dir = ChangeWorldGravityAction_DIRECTION.ReadValue<Vector3>().normalized;
        // Debug.Log(Dir * GravityForce);
        MyPlayerController.ChangeWorldGravity(Dir * GravityForce);
        // throw new System.NotImplementedException();
        //MyController.SetGravityForceAndDirection(GetRandomGravityDirection());
    }

    private void ChangeSelfGravityAction_performed(InputAction.CallbackContext obj)
    {
        Vector3 Dir = ChangeSelfGravityAction_DIRECTION.ReadValue<Vector3>().normalized;
        // Debug.Log(Dir * GravityForce);
        MyPlayerController.SetGravityForceAndDirection(Dir * GravityForce);
        //Physics.gravity = GetRandomGravityDirection();
    }

    private void AttackSecondary_performed(InputAction.CallbackContext obj)
    {
        AimWeapon(true);
    }

    private void AttackSecondary_canceled(InputAction.CallbackContext obj)
    {
        AimWeapon(false);
    }
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        BoostJump();
        //IsJumpBoosting = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        //IsJumpBoosting = false;
    }

    private void AttackPrimary_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }

    private void AttackPrimary_canceled(InputAction.CallbackContext obj)
    {
        StopShootingWeapon();
    }

    public void DisablePlayerActions() {
        ChangeSelfGravityAction.performed -= ChangeSelfGravityAction_performed;
        ChangeWorldGravityAction.performed -= ChangeWorldGravityAction_performed;
        JumpAction.performed -= Jump_performed;
        AttackPrimaryAction.performed -= AttackPrimary_performed;
        AttackSecondaryAction.performed -= AttackSecondary_performed;
        AttackSecondaryAction.canceled -= AttackSecondary_canceled;
        AttackPrimaryAction.canceled -= AttackPrimary_canceled;
        JumpAction.canceled -= Jump_canceled;
        InteractAction.performed -= InteractAction_performed;
        InteractAction.canceled -= InteractAction_canceled;
        ReloadAction.performed -= ReloadAction_performed;
        GrenadeAction.performed -= GrenadeAction_performed;
        GrenadeAction.canceled -= GrenadeAction_canceled;
        ChangeSelectedGrenade.performed -= ChangeSelectedGrenade_performed;
        SprintAction.performed -= SprintAction_performed;
        SprintAction.canceled -= SprintAction_canceled;
        MoveAction.Disable();
        LookAction.Disable();
        InteractAction.Disable();
        GrenadeAction.Disable();
        JumpAction.Disable();
        ChangeSelectedGrenade.Disable();
        AttackPrimaryAction.Disable();
        AttackSecondaryAction.Disable();
        ChangeSelfGravityAction.Disable();
        ChangeWorldGravityAction.Disable();
        ChangeSelfGravityAction_DIRECTION.Disable();
        ChangeWorldGravityAction_DIRECTION.Disable();
        SprintAction.Disable();


    }

    public void OnDisable()
    {
        DisablePlayerActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        AimWeapon(false);
        SetupPlayerActions();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        HandleInteract();
        HandleGrenadeInHand();
    }

    public void FixedUpdate()
    {
        HandleInputs();
    }

    void HandleInputs()
    {
        //if (IsJumpBoosting)
            //BoostJump();
        MoveDirectionXYKeyboard = MoveAction.ReadValue<Vector2>();
        Move(MoveDirectionXYKeyboard);
        Look(LookAction.ReadValue<Vector2>());
    }

    public void BoostJump() {
        if (this.MyPlayerController.IsOnGround)
            this.MyPlayerController.RigidbodyRef.AddForce(-MyPlayerController.GetGravityDirection() * BoostUpForce);
       //  MyPlayerController.AddMovementInput(-MyPlayerController.GetGravityDirection(), BoostUpForce);
    }

    public void Look(Vector2 Direction) {
        Vector3 capsuleUp = -MyPlayerController.GetGravityDirection();
        float XAmount = Direction.y * MyPlayerController.LookSensivityY * (capsuleUp.y < 0 ? -1f : 1f) * (MyPlayerController.InvertLookY ? -1f : 1f) * Time.deltaTime;
        MyPlayerController.CameraRotation.x += XAmount;
        MyPlayerController.CameraRotation.x = Mathf.Clamp(MyPlayerController.CameraRotation.x, MyPlayerController.MinVerticalRotation, MyPlayerController.MaxVerticalRotation);
        MyPlayerController.CameraRotation.y += Direction.x * MyPlayerController.LookSensivityX * (MyPlayerController.InvertLookX ? -1f : 1f) * Time.deltaTime;
        // Vector3 capsuleUp = -MyPlayerController.GetGravityDirection();   

        if (capsuleUp == Vector3.zero)
            capsuleUp = CapsuleCollision.transform.up;
        Vector3 capsuleRight = MyPlayerController.GetRightBasedOnGravity();
        Quaternion TargetCameraRotation = Quaternion.AngleAxis(MyPlayerController.CameraRotation.y, capsuleUp) *
            Quaternion.AngleAxis(MyPlayerController.CameraRotation.x + (capsuleUp.Equals(Vector3.forward) ? 90 : (capsuleUp.Equals(Vector3.back) ? -90 : 0)), capsuleRight);
        Vector3 FocusPosition = IsAimingWeapon ? MyPlayerController.AimFocusPoint.transform.position : MyPlayerController.CameraFocusTarget.transform.position + new Vector3(MyPlayerController.FramingOffset.x, MyPlayerController.FramingOffset.y, MyPlayerController.FramingOffset.z);
        Vector3 CameraDistance = TargetCameraRotation * new Vector3(0, 0, MyPlayerController.CameraDistance);
        bool IsCameraBlocked = Physics.Raycast(FocusPosition, (FocusPosition - CameraDistance).normalized, out RaycastHit HitResult, MyPlayerController.CameraDistance, MyPlayerController.CameraClippingLayerMask);
        if (IsCameraBlocked)
        {
            CameraDistance = TargetCameraRotation * new Vector3(0, 0, HitResult.distance - 1f);
            // Debug.LogWarning(HitResult.collider.gameObject.name);
            
        }
        Debug.DrawLine(FocusPosition, FocusPosition - CameraDistance, Color.green);

        CameraComp.transform.position = Vector3.Lerp(CameraComp.transform.position, FocusPosition - CameraDistance, Time.deltaTime * 10);
        CameraComp.transform.rotation = Quaternion.LookRotation((FocusPosition - CameraComp.transform.position).normalized, capsuleUp);
        // CameraComp.transform.rotation = TargetCameraRotation;

    }

    public override void Move(Vector2 Direction)
    {
        if (Mathf.Abs(Direction.x) + Mathf.Abs(Direction.y) > 0)
        {
            Vector3 Dir = new Vector3(Direction.x, 0f, Direction.y).normalized;
            Vector3 LocalUp = -MyPlayerController.GetGravityDirection();
            Vector3 moveDirection = Quaternion.LookRotation(MyPlayerController.GetForwardBasedOnGravity() * (LocalUp.y < 0 ? -1f : 1f), LocalUp) * MyPlayerController.CameraPlanarRotation * Dir;
            MyPlayerController.AddMovementInput(moveDirection.normalized, CurrentMovementSpeed);
        }
    }
    public Vector3 GetCameraForwardVector() { return CameraComp.transform.forward; }

    private void OnTriggerEnter(Collider other)
    {
        NearbyInteractables.Add(other.gameObject);
        // Debug.Log(other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        NearbyInteractables.Remove(other.gameObject);
    }

    public bool GetNearestInteractable()
    {
        if (!NearbyInteractables.Any()) return false;
        // TODO: MAKE SURE TO USE LAYERS
        InteractableBase closest = null;
        float closestDist = float.MaxValue;
        Vector3 position = transform.position;
        foreach (GameObject obj in NearbyInteractables)
        {
            if (obj == null) continue;
            if (!obj.TryGetComponent<InteractableBase>(out var InteractableComp) || !InteractableComp.IsInteractable) continue;
            float dist = Vector3.Distance(position, obj.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = InteractableComp;
            }
        }
        if (closest == null) return false;
        InteractablePickable CloseInteractablePickable;
        bool IsPickable = closest.transform.root.TryGetComponent<InteractablePickable>(out CloseInteractablePickable);
        if (IsPickable)
        {
            if (closestDist <= this.PickableInteractablePickupDistance && CloseInteractablePickable.IsInstantPickup)
            {
                InteractionComplete(CloseInteractablePickable);
                //bool AddedToInventory = InventoryComp.AddItemToInventory(this, CloseInteractablePickable);
                CanInteract = false;
                InteractionAmount = 0;
                IsInteracting = false;
                closest = null;
                //if (AddedToInventory) return;
                // After pickup code here.
            }
            return false;
        }
        if (closest == null) return false;
        if (ClosestInteractable == null && closestDist <= InteractionDistance)
        {
            ClosestInteractable = closest;
        }
        if (ClosestInteractable == null) return false;
            if (!closest.Equals(ClosestInteractable) && closestDist < Vector3.Distance(position, ClosestInteractable.transform.position) && closestDist <= InteractionDistance)
        {
            IsInteracting = false;
            CanInteract = false;
            InteractionAmount = 0;
            ClosestInteractable = closest;
        }
        return true;
    }

    public void HandleInteract()
    {
        bool ShouldProceed = GetNearestInteractable();
        if (ClosestInteractable != null && ShouldProceed && this.IsInteracting && this.CanInteract) {
            InteractionAmount = Mathf.MoveTowards(InteractionAmount, 100f, ClosestInteractable.InteractionSpeed * Time.deltaTime);
            if (InteractionAmount >= 100)
            {
                InteractionComplete(ClosestInteractable);
                return;
            }
            return;
        }
        // Not Interacting.
        InteractionAmount = 0;
        if (ClosestInteractable == null) return;
    }

    public void InteractionComplete(InteractableBase Interactable)
    {
        Interactable.Interact(this);
        ClosestInteractable = null;
        InteractionAmount = 0;
        CanInteract = false;
    }

    public void PickupInteractable(InteractablePickable interactablePickable) {
        interactablePickable.IsInteractable = false;
        InventoryComp.AddItemToInventory(this, interactablePickable);
    }

    public override void AimWeapon(bool IsAiming)
    {
        IsAimingWeapon = IsAiming;
        MyPlayerController.IK_Aim.weight = IsAiming ? 1f : 0f;
        MyPlayerController.IK_Aim_Rig.weight = IsAiming ? 1f : 0f;
        MyPlayerController.IK_Aim_RigBuilder.layers[0].active = IsAiming;
        MyPlayerController.IK_Aim_RigAnimation.enabled = IsAiming;
        if (IsSprinting && IsAiming)
            this.StopSprint();
        base.AimWeapon(IsAiming);
        this.CurrentMovementSpeed = IsAiming ? AimingMovementSpeed : MovementSpeed;
        MyPlayerController.TargetCameraDistance = IsAiming ? MyPlayerController.CameraDistanceAiming : MyPlayerController.CameraDistanceInit;
    }

    public override void Attack()
    {
        base.Attack();
        if (CurrentWeaponEquipped == null) return;
        CurrentWeaponEquipped.StartShooting();
    }

    public override void Reload()
    {
        if (CurrentWeaponEquipped == null) return;
        CurrentWeaponEquipped.Reload();
    }

    public override void ReloadComplete()
    {
        if (CurrentWeaponEquipped == null) return;
        CurrentWeaponEquipped.ReloadCompleted();
    }

    // ONLY IS CALLED BY WEAPONBASE. Don't call it here. It gets called in Reload()
    public override void PlayReloadAnimation()
    {
        PlayerAnimation.TriggerStartReload();
    }

    /*
    public void GiveItemToPlayerFromInventory(int ItemIndex=0)
    {
        GameObject PrefabToSpawn = InventoryComp.RemoveItem(this, ItemIndex);
        if (PrefabToSpawn == null) return;

        Instantiate(PrefabToSpawn,,, this.transform);
    }
    */

    public void GiveTimeGrenadeToPlayerFromInventory()
    {
        if (CurrentGrenadeInHand) return;
        GameObject PrefabToSpawn = InventoryComp.RemoveItem(this, "Time Grenade Consumable");
        if (PrefabToSpawn == null) return;
        GameObject GrenadeSpawned = Instantiate(PrefabToSpawn, GrenadeAttachPointToHand.position, GrenadeAttachPointToHand.rotation, GrenadeAttachPointToHand);
        CurrentGrenadeInHand = GrenadeSpawned.GetComponent<FieldBaseGrenade>();
        Collider GrenadeCollider = GrenadeSpawned.GetComponent<Collider>();
        Physics.IgnoreCollision(GrenadeCollider, CapsuleCollision);
        //Physics.IgnoreCollision(GrenadeCollider, SkeletalMesh.GetComponent<Collider>());
    }

    public void GiveGravityGrenadeToPlayerFromInventory()
    {
        if (CurrentGrenadeInHand) return;
        GameObject PrefabToSpawn = InventoryComp.RemoveItem(this, "Gravity Grenade Consumable");
        if (PrefabToSpawn == null) return;
        GameObject GrenadeSpawned = Instantiate(PrefabToSpawn, GrenadeAttachPointToHand.position, GrenadeAttachPointToHand.rotation, GrenadeAttachPointToHand);
        CurrentGrenadeInHand = GrenadeSpawned.GetComponent<FieldBaseGrenade>();
        Collider GrenadeCollider = GrenadeSpawned.GetComponent<Collider>();
        Physics.IgnoreCollision(GrenadeCollider, CapsuleCollision);
        //Physics.IgnoreCollision(GrenadeCollider, SkeletalMesh.GetComponent<Collider>());
    }

    public void HandleGrenadeInHand()
    {
        // The more it is in-hand, we charge it more.
        if (this.CurrentGrenadeInHand == null) return;
        bool IsFullyCharged = this.CurrentGrenadeInHand.ChargeGrenade();
        // TODO: If fully charged logic here.
    }

    public bool HasGrenadeInHand() { return this.CurrentGrenadeInHand; }

    public void StartThrowingGrenade()
    {
        if (CurrentGrenadeInHand == null) return;
        PlayerAnimation.TriggerGrenadeThrow();
    }

    // Is called as an event for the animation.
    public void ThrowGrenade()
    {
        if (CurrentGrenadeInHand == null) return;
        Vector3 BaseVel = MyPlayerController.PlayerCameraRef.transform.forward * GrenadeThrowPower;
        CurrentGrenadeInHand.GrenadeThrown(BaseVel);
        CurrentGrenadeInHand = null;
    }
}
