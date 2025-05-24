using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : CustomCharacterController
{
    [Header("Components")]
    public PlayerCharacter PlayerCharacterRef;
    public CinemachineCamera PlayerCameraRef;

    [Header("Movements")]
    public LayerMask CameraClippingLayerMask = new LayerMask();
    public bool InvertLookX = false;
    public bool InvertLookY = false;
    public float LookSensivityX = 1f;
    public float LookSensivityY = 1f;
    public float MinVerticalRotation = -45f;
    public float MaxVerticalRotation = 45f;
    // public bool ShouldCharacterUseZRotation = false;  // For aiming and etc.
    public Vector3 FramingOffset;
    public Vector3 CameraRotation;
    public float CameraDistance = 3f;
    [NonSerialized] public float TargetCameraDistance;
    public float CameraDistanceInterpSpeed = 0.5f;
    public float CameraDistanceAiming = 1f;
    public float AimingYawRotationDifference = 20;
    public float CapsuleAimingTransitionSpeed = 50;
    [NonSerialized] public float CameraDistanceInit;
    public GameObject CameraFocusTarget;
    public LayerMask AimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform TransformAimPoint;
    [SerializeField] private float AimTransitionSpeed = 50f;
    public GameObject AimFocusPoint;
    public GameObject CrosshairObject;  // The UI object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CameraDistanceInit = CameraDistance;
        TargetCameraDistance = CameraDistance;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        CheckRaycastFromViewPoint();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (CameraDistance != TargetCameraDistance)
        {
            CameraDistance = Mathf.Lerp(CameraDistance, TargetCameraDistance, 1 - Mathf.Exp(-CameraDistanceInterpSpeed * Time.deltaTime));
            if (Math.Abs(CameraDistance - TargetCameraDistance) <= 0.001) CameraDistance = TargetCameraDistance;
            // Debug.Log(CameraDistance);
        }
    }

    public override void AddMovementInput(Vector3 Direction, float Scale)
    {
        Debug.Log(Scale);
        base.AddMovementInput(Direction, Scale);
    }

    public override void InteroplateCharacterRotation()
    {
        if (PlayerCharacterRef.IsAimingWeapon)
        {
            Vector3 LocalUp = -GetGravityDirection();
            if (LocalUp.Equals(Vector3.zero))
                LocalUp = Vector3.up;
            TargetRotation = Quaternion.AngleAxis(CameraRotation.y - (LocalUp.y < 0 ? AimingYawRotationDifference : 180 + AimingYawRotationDifference), LocalUp) * Quaternion.LookRotation(GetForwardBasedOnGravity(), LocalUp);
            CameraFocusTarget.transform.rotation = Quaternion.RotateTowards(CameraFocusTarget.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
            return;
        }
        else
            base.InteroplateCharacterRotation();
        CameraFocusTarget.transform.rotation = Quaternion.RotateTowards(CameraFocusTarget.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
        //if (InputVelocity.magnitude > 0)
        //{
        //   CameraFocusTarget.transform.rotation = Quaternion.RotateTowards(CameraFocusTarget.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
        //}
    }

    public override void CheckRaycastFromViewPoint()
    {
        Vector3 Start = PlayerCameraRef.transform.position;
        // Vector3 Direction = GetForwardShootingVector();
        Vector3 Direction = PlayerCameraRef.transform.forward;

        if (Physics.Raycast(Start, Direction, out RaycastHit HitResult, 999f, AimColliderLayerMask))
        {
            TransformAimPoint.position = Vector3.Lerp(TransformAimPoint.position, HitResult.point, Time.deltaTime * AimTransitionSpeed);
        }
        else
        {
            TransformAimPoint.position = Vector3.Lerp(TransformAimPoint.position, Start + Direction * 5, Time.deltaTime * AimTransitionSpeed);
        }
    }

    public override Vector3 GetForwardShootingVector() {
        return (TransformAimPoint.position - PlayerCharacterRef.CurrentWeaponEquipped.ShootLocation_TEST_ONLY.transform.position).normalized;
        // return PlayerCameraRef.transform.forward;
    }

    public override void UpdateCharacterMovement(float Multiplier = 1f)
    {
        base.UpdateCharacterMovement();
    }
    public Quaternion CameraPlanarRotation => Quaternion.Euler(0, CameraRotation.y, 0);
}
