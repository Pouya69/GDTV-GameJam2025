
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{
    [Header("Weapons")]
    [DoNotSerialize] private WeaponBase CurrentWeaponEquipped;
    // [DoNotSerialize] WeaponBase CurrentWeaponEquipped;

    [Header("Input Actions")]
    InputAction MoveAction;
    InputAction LookAction;
    InputAction JumpAction;
    InputAction AttackPrimaryAction;
    InputAction AttackSecondaryAction;
    InputAction ChangeWorldGravityAction;
    InputAction ChangeSelfGravityAction;
    InputAction ChangeWorldGravityAction_DIRECTION;
    InputAction ChangeSelfGravityAction_DIRECTION;

    [Header("Movements")]
    public float BoostUpForce = 100f;
    [DoNotSerialize] public Vector2 MoveDirectionXYKeyboard;  // Just the keyboard WASD in the form of Vector2. For Gravity Change.
    [Header("Components")]
    public CinemachineCamera CameraComp;
    public PlayerController MyPlayerController;

    public override void Awake()
    {
        base.Awake();
    }

    public void SetupPlayerActions() {
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
        JumpAction.performed += Jump_performed;
        AttackPrimaryAction.performed += AttackPrimary_performed;
        AttackSecondaryAction.performed += AttackSecondary_performed;
        AttackSecondaryAction.canceled += AttackSecondary_canceled;
        AttackPrimaryAction.canceled += AttackPrimary_canceled;
        JumpAction.canceled += Jump_canceled;
        ChangeSelfGravityAction.performed += ChangeSelfGravityAction_performed;
        ChangeWorldGravityAction.performed += ChangeWorldGravityAction_performed;
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
        //throw new System.NotImplementedException();
    }

    private void AttackSecondary_canceled(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
    }
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
    }

    private void AttackPrimary_performed(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
    }

    private void AttackPrimary_canceled(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
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
        HandleInputs();
    }

    void HandleInputs()
    {
        MoveDirectionXYKeyboard = MoveAction.ReadValue<Vector2>();
        Move(MoveDirectionXYKeyboard);
        Look(LookAction.ReadValue<Vector2>());
    }

    public void BoostJump() {
        MyPlayerController.AddMovementInput(transform.up, BoostUpForce);
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
        // Vector3 capsuleRight = -transform.right;
        //Debug.DrawLine(transform.position, transform.position + (capsuleRight * 4));
        //Debug.DrawLine(transform.position, transform.position + (capsuleUp * 4));
        // Vector3 capsuleRight = CapsuleCollision.transform.right;
        Quaternion TargetCameraRotation = Quaternion.AngleAxis(MyPlayerController.CameraRotation.y, capsuleUp) * Quaternion.AngleAxis(MyPlayerController.CameraRotation.x, capsuleRight);
        Vector3 FocusPosition = MyPlayerController.CameraFocusTarget.transform.position;// + new Vector3(MyPlayerController.FramingOffset.x, MyPlayerController.FramingOffset.y, 0);
        Vector3 CameraDistance = TargetCameraRotation * new Vector3(0, 0, MyPlayerController.CameraDistance);
        CameraComp.transform.position = FocusPosition - CameraDistance;
        CameraComp.transform.rotation = Quaternion.LookRotation(CapsuleCollision.transform.position - CameraComp.transform.position, capsuleUp);
    }

    public override void Move(Vector2 Direction)
    {
        if (Mathf.Abs(Direction.x) + Mathf.Abs(Direction.y) > 0)
        {
            Vector3 Dir = new Vector3(Direction.x, 0f, Direction.y).normalized;
            Vector3 moveDirection = Quaternion.LookRotation(Vector3.forward , -MyPlayerController.GetGravityDirection()) * MyPlayerController.CameraPlanarRotation * Dir;
            MyPlayerController.AddMovementInput(moveDirection, CurrentMovementSpeed);
        }
    }
    public Vector3 GetCameraForwardVector() { return CameraComp.transform.forward; }

    public void HandleInteract()
    {

    }

    public void InteractionComplete()
    {

    }

    public void AimWeapon()
    {

    }

    public void ShootWeapon()
    {

    }

}
