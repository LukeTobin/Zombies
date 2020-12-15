using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] float horizontalSpeed = 10f;
    [SerializeField] float verticalSpeed = 10f;
    [SerializeField] float clampAngle = 80f;

    InputManager inputManager;
    Vector3 startingRotation;

    protected override void Awake()
    {
        inputManager = InputManager.Instance;
        if(startingRotation == null)
        {
            startingRotation = transform.localRotation.eulerAngles;
        }

        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if(stage == CinemachineCore.Stage.Aim)
            {
                if(inputManager != null)
                {
                    Vector2 deltaInput = inputManager.GetMouseDelta();

                    startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                    startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime * -1;
                    startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

                    state.RawOrientation = Quaternion.Euler(startingRotation.y, startingRotation.x, 0f);
                }
                    
            }
        }
    }
}
