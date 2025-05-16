using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{
    //[Header("Weapons")]

    [Header("Input Actions")]
    InputAction MoveAction;
    InputAction LookAction;
    InputAction JumpAction;
    InputAction InteractAction;
    InputAction AttackPrimaryAction;
    InputAction AttackSecondaryAction;
    InputAction ChangeWorldGravityAction;
    InputAction ChangeSelfGravityAction;
    InputAction ChangeWorldGravityAction_DIRECTION;
    InputAction ChangeSelfGravityAction_DIRECTION;

    [Header("Movements")]
    public float BoostUpForce = 100f;
    [NonSerialized] public Vector2 MoveDirectionXYKeyboard;  // Just the keyboard WASD in the form of Vector2. For Gravity Change.
    [Header("Components")]
    public CinemachineCamera CameraComp;
    public PlayerController MyPlayerController;
    public BoxCollider InteractionOverlapZone;
    public InventoryComponent InventoryComp;
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

    public override void Awake()
    {
        base.Awake();
    }

    public void SetupPlayerActions() {
        InteractAction = InputSystem.actions.FindAction("Interact");
        InteractAction.Enable();
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
        InteractAction.performed += InteractAction_performed;
        InteractAction.canceled += InteractAction_canceled;
        JumpAction.performed += Jump_performed;
        AttackPrimaryAction.performed += AttackPrimary_performed;
        AttackSecondaryAction.performed += AttackSecondary_performed;
        AttackSecondaryAction.canceled += AttackSecondary_canceled;
        AttackPrimaryAction.canceled += AttackPrimary_canceled;
        JumpAction.canceled += Jump_canceled;
        ChangeSelfGravityAction.performed += ChangeSelfGravityAction_performed;
        ChangeWorldGravityAction.performed += ChangeWorldGravityAction_performed;
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
        IsJumpBoosting = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        IsJumpBoosting = false;
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
        MoveAction.Disable();
        LookAction.Disable();
        JumpAction.Disable();
        AttackPrimaryAction.Disable();
        AttackSecondaryAction.Disable();
        ChangeSelfGravityAction.Disable();
        ChangeWorldGravityAction.Disable();
        ChangeSelfGravityAction_DIRECTION.Disable();
        ChangeWorldGravityAction_DIRECTION.Disable();

        ChangeSelfGravityAction.performed -= ChangeSelfGravityAction_performed;
        ChangeWorldGravityAction.performed -= ChangeWorldGravityAction_performed;
        JumpAction.performed -= Jump_performed;
        AttackPrimaryAction.performed -= AttackPrimary_performed;
        AttackSecondaryAction.performed -= AttackSecondary_performed;
        AttackSecondaryAction.canceled -= AttackSecondary_canceled;
        AttackPrimaryAction.canceled -= AttackPrimary_canceled;
        JumpAction.canceled -= Jump_canceled;
    }

    public void OnDisable()
    {
        DisablePlayerActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        SetupPlayerActions();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        HandleInteract();
    }

    public void FixedUpdate()
    {
        HandleInputs();
    }

    void HandleInputs()
    {
        if (IsJumpBoosting)
            BoostJump();
        MoveDirectionXYKeyboard = MoveAction.ReadValue<Vector2>();
        Move(MoveDirectionXYKeyboard);
        Look(LookAction.ReadValue<Vector2>());
    }

    public void BoostJump() {
        MyPlayerController.AddMovementInput(-MyPlayerController.GetGravityDirection(), BoostUpForce);
    }

    public void Look(Vector2 Direction) {
        Vector3 capsuleUp = -MyPlayerController.GetGravityDirection();
        MyPlayerController.CameraRotation.x += Direction.y * MyPlayerController.LookSensivityY * (capsuleUp.y < 0 ? -1f : 1f) * (MyPlayerController.InvertLookY ? -1f : 1f);
        MyPlayerController.CameraRotation.x = Mathf.Clamp(MyPlayerController.CameraRotation.x, MyPlayerController.MinVerticalRotation, MyPlayerController.MaxVerticalRotation);
        MyPlayerController.CameraRotation.y += Direction.x * MyPlayerController.LookSensivityX * (MyPlayerController.InvertLookX ? -1f : 1f);
        // Vector3 capsuleUp = -MyPlayerController.GetGravityDirection();
        
        if (capsuleUp == Vector3.zero)
            capsuleUp = CapsuleCollision.transform.up;
        Vector3 capsuleRight = MyPlayerController.GetRightBasedOnGravity();
        Quaternion TargetCameraRotation = Quaternion.AngleAxis(MyPlayerController.CameraRotation.y, capsuleUp) * Quaternion.AngleAxis(MyPlayerController.CameraRotation.x, capsuleRight);
        Vector3 FocusPosition = MyPlayerController.CameraFocusTarget.transform.position;// + new Vector3(MyPlayerController.FramingOffset.x, MyPlayerController.FramingOffset.y, 0);
        Vector3 CameraDistance = TargetCameraRotation * new Vector3(0, 0, MyPlayerController.CameraDistance);
        CameraComp.transform.position = FocusPosition - CameraDistance;
        CameraComp.transform.rotation = Quaternion.LookRotation((CapsuleCollision.transform.position - CameraComp.transform.position).normalized, capsuleUp);
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

    public void GetNearestInteractable()
    {
        if (!NearbyInteractables.Any()) return;
        // TODO: MAKE SURE TO USE LAYERS
        InteractableBase closest = null;
        float closestDist = float.MaxValue;
        Vector3 position = transform.position;
        foreach (GameObject obj in NearbyInteractables)
        {
            if (obj == null) continue;
            if (!obj.TryGetComponent<InteractableBase>(out var InteractableComp)) continue;
            float dist = Vector3.Distance(position, obj.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = InteractableComp;
            }
        }
        if (closest == null) return;
        InteractablePickable CloseInteractablePickable = null;
        bool IsPickable = closest.TryGetComponent<InteractablePickable>(out CloseInteractablePickable);
        if (IsPickable && CloseInteractablePickable != null && CloseInteractablePickable.IsInstantPickup && closestDist > this.PickableInteractablePickupDistance)
        {
            bool AddedToInventory = InventoryComp.AddItemToInventory(this, CloseInteractablePickable);
            if (AddedToInventory) return;
        }
        if (ClosestInteractable == null)
            ClosestInteractable = closest;
        else if (!closest.Equals(ClosestInteractable) && closestDist < Vector3.Distance(position, ClosestInteractable.transform.position))
        {
            IsInteracting = false;
            CanInteract = false;
            InteractionAmount = 0;
            ClosestInteractable = closest;
        }
    }

    public void HandleInteract()
    {
        GetNearestInteractable();
        if (ClosestInteractable && this.IsInteracting && this.CanInteract) {
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
        InteractionAmount = 0;
        CanInteract = false;
    }

    public void PickupInteractable(InteractablePickable interactablePickable) {
        InventoryComp.AddItemToInventory(this, interactablePickable);
    }

    public void AimWeapon(bool IsAiming)
    {
        MyPlayerController.TargetCameraDistance = IsAiming ? MyPlayerController.CameraDistanceAiming : MyPlayerController.CameraDistanceInit;
    }

    public override void Attack()
    {
        base.Attack();
        if (CurrentWeaponEquipped == null) return;
        if (CurrentWeaponEquipped.IsWeaponSingleShot())
            CurrentWeaponEquipped.Shoot();
        else
            CurrentWeaponEquipped.StartShooting();
    }

    public void StopShootingWeapon()
    {
        if (CurrentWeaponEquipped == null) return;
        CurrentWeaponEquipped.StopShoot();
    }

}
