using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;

 public class CarController : MonoBehaviour
{
    [SerializeField] float motorPower = 1500f; 
    [SerializeField] float brakeForce = 1500f;
    [SerializeField] float maxSteerAngle = 30f;

    [SerializeField] WheelCollider frontLeftWheelColider;
    [SerializeField] WheelCollider frontRightWheelColider;
    [SerializeField] WheelCollider rearLeftWheelColider;
    [SerializeField] WheelCollider rearRightWheelColider;

    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform rearLeftTransform;
    [SerializeField] Transform rearRightTransform;

    [SerializeField] private float currentSpeed;

    [SerializeField] PedalController gasPedal;
    [SerializeField] PedalController brakePedal;

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;
    private float currentSteerAngle;

    private float currentGasInput = 0f;
    private float currentBrakeInput = 0f;
    private float steeringInput = 0f;

    Vector3 forward = new Vector3(0,0,1);

    public TypeDrive typeDrive;
    public float allWheelDriveBalance = 0.5f;


    void Start()
     {
        gasPedal = GetComponent<PedalController>();
        brakePedal = GetComponent<PedalController>();
        rb = GetComponent<Rigidbody>();
        gasPedal.pedalInput = currentGasInput;
        brakePedal.pedalInput = currentBrakeInput;
     }

     void Update()
     {
        GetInput();
       
     }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleBrake();
        HandleSteering();
        UpdateCurrentSpeed();
        UpdateWheelPoses();
    }

    [Serializable]
    public enum TypeDrive
    { 
        FWD,
        RWD,
        AWD
    }

    void GetInput()
    {

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");      
        if (gasPedal.isPressed)
        {
            currentGasInput = gasPedal != null ?  gasPedal.PedalScrollbar.value : 0f;
            HandleMotor();
        }

        if (brakePedal.isPressed)
        {
            currentBrakeInput = brakePedal != null ? brakePedal.PedalScrollbar.value : 0f;
            HandleBrake();
        }
    }

            void HandleSteering()
            {
                float steerAngle = horizontalInput * currentSteerAngle * maxSteerAngle;
                frontLeftWheelColider.steerAngle = steerAngle;
                frontRightWheelColider.steerAngle = steerAngle;
            }

    void ApplyFrontWheel(float torque)
    {
        frontLeftWheelColider.motorTorque = torque;
        frontRightWheelColider.motorTorque = torque;

        rearLeftWheelColider.motorTorque = 0;
        rearRightWheelColider.motorTorque = 0;
    }

    void ApplyRearWheel(float torque)
    {
        frontLeftWheelColider.motorTorque = 0;
        frontRightWheelColider.motorTorque = 0;

        rearLeftWheelColider.motorTorque = torque;
        rearRightWheelColider.motorTorque = torque;
    }

    void ApllyAllWheel(float torque)
    {
        float frontTorque = torque * (1f - allWheelDriveBalance);
        float rearTorque = torque * allWheelDriveBalance;

        frontLeftWheelColider.motorTorque = frontTorque;
        frontRightWheelColider.motorTorque = frontTorque;
        rearLeftWheelColider.motorTorque = rearTorque;
        rearRightWheelColider.motorTorque = rearTorque;
    }
    void HandleMotor()
    {
        float torque = currentGasInput * verticalInput * motorPower;
        if(gasPedal.isPressed)
        {
            rb.MovePosition(transform.position += forward * torque);
        }

        switch (typeDrive)
        {
            case TypeDrive.FWD:
                ApplyFrontWheel(torque);
                break;

            case TypeDrive.RWD:
                ApplyRearWheel(torque);
                break;

            case TypeDrive.AWD:
                ApllyAllWheel(torque);
                break;
        }
    }

    void HandleBrake()
    {
        float brakeTorque = isBraking ? currentBrakeInput * brakeForce : 0f;

        if (brakePedal.isPressed)
        { 
        frontLeftWheelColider.brakeTorque = brakeTorque;
        frontRightWheelColider.brakeTorque = brakeTorque;
        rearLeftWheelColider.brakeTorque = brakeTorque;
        rearRightWheelColider.brakeTorque = brakeTorque;
        }

        if (verticalInput == 0 && !isBraking && currentSpeed > 1f)
        {
            float engineBrakeTorque = motorPower * 0.3f;
            switch (typeDrive)
            {
                case TypeDrive.FWD:
                    frontLeftWheelColider.brakeTorque = engineBrakeTorque;
                    frontRightWheelColider.brakeTorque = engineBrakeTorque;
                    break;

                case TypeDrive.RWD:
                    rearLeftWheelColider.brakeTorque = engineBrakeTorque;
                    rearRightWheelColider.brakeTorque = engineBrakeTorque;
                    break;

                    case TypeDrive.AWD:
                    frontLeftWheelColider.brakeTorque = engineBrakeTorque * (1f - allWheelDriveBalance);
                    frontRightWheelColider.brakeTorque = engineBrakeTorque * (1f - allWheelDriveBalance);
                    rearLeftWheelColider.brakeTorque = engineBrakeTorque * allWheelDriveBalance;
                    rearRightWheelColider.brakeTorque = engineBrakeTorque * allWheelDriveBalance;
                    break;
            }
        }
    }

    void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheelColider, frontLeftTransform);
        UpdateWheelPose(frontRightWheelColider, frontRightTransform);
        UpdateWheelPose(rearLeftWheelColider, rearLeftTransform);
        UpdateWheelPose(rearRightWheelColider, rearRightTransform);

    }

    void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        transform.position = position;
        transform.rotation = rotation;
    }

    void UpdateCurrentSpeed()
    {
        currentSpeed = rb.linearVelocity.magnitude * 3.6f;
    }
}
