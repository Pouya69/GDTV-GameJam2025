using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : CustomCharacterController
{
    [Header("Components")]
    public PlayerCharacter PlayerCharacterRef;
    public CinemachineCamera PlayerCameraRef;

    [Header("Movements")]
    public bool InvertLookX = false;
    public bool InvertLookY = false;
    public float LookSensivityX = 1f;
    public float LookSensivityY = 1f;
    public float MinVerticalRotation = -45f;
    public float MaxVerticalRotation = 45f;
    public bool ShouldCharacterUseZRotation = false;  // For aiming and etc.
    public Vector2 FramingOffset;
    public Vector3 CameraRotation;
    public float CameraDistance = 3f;
    [NonSerialized] public float TargetCameraDistance;
    public float CameraDistanceInterpSpeed = 0.5f;
    public float CameraDistanceAiming = 1f;
    [NonSerialized] public float CameraDistanceInit;
    public GameObject CameraFocusTarget;


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
        base.AddMovementInput(Direction, Scale);
    }

    public override void InteroplateCharacterRotation()
    {
        base.InteroplateCharacterRotation();
        CameraFocusTarget.transform.rotation = Quaternion.RotateTowards(CameraFocusTarget.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
        //if (InputVelocity.magnitude > 0)
        //{
         //   CameraFocusTarget.transform.rotation = Quaternion.RotateTowards(CameraFocusTarget.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
        //}
    }

    public override void UpdateCharacterMovement(float Multiplier = 1f)
    {
        base.UpdateCharacterMovement();
    }
    public Quaternion CameraPlanarRotation => Quaternion.Euler(0, CameraRotation.y, 0);
}
