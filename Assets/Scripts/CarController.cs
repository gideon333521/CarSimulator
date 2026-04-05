using TMPro;
using Unity.VisualScripting;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float motorPower;
    [SerializeField] private float brakeForce;
    [SerializeField] private float steeringAngle = 30f;

    [SerializeField] private float MinRpm;
    [SerializeField] private float MaxRpm;
    private float currentRpm;

    [SerializeField] WheelCollider[] wheelCollider;
    [SerializeField] Transform[] wheels;
    private Rigidbody carRigidbody;

    [SerializeField] private PedalController gasPedal;
    [SerializeField] private PedalController brakePedal;
    [SerializeField] private SteeringWheelController steeringWheel;
    [SerializeField] private HandbrakeController handbrake;
    [SerializeField] private IgnitionKey Key;
    [SerializeField] private AutomatTransmission automat;

    private float speed;
    private float currentSpeed;
    [SerializeField] private TextMeshProUGUI speedometr;
    [SerializeField] private TextMeshProUGUI tahometr;

    private float verticalInput;
    private float horizontalInput;
    private float brakeInput;
    private bool isBraking;
    private bool isEngineRunning;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        isBraking = true;
        isEngineRunning = false;
        handbrake.isPressed = isBraking;
        Key.isPressed = isEngineRunning;
        automat.currentGear = automat.gearOrder[0];
    }


    void Update()
    {
        CheckInput();
        CheckAT();
        speed = carRigidbody.linearVelocity.magnitude;
        Speedometr();
        Rpm();
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        HandleBrake();
        UpdateWheelVisuals();
    }

    void CheckInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        if (gasPedal.isPressed)
        {
            verticalInput += gasPedal.pedalInput;
        }
        else
        {
            verticalInput = 0;
        }

        brakeInput = isBraking ? brakeForce : 0f;
        if (brakePedal.isPressed)
        {
            verticalInput = brakeInput;
            brakeInput -= brakePedal.pedalInput;
            for (float i = speed; brakePedal.pedalInput > 0; i--)
            {
                isBraking = true;
            }
        }
        else
        {
            brakeInput = 0;
        }

        if (handbrake.isPressed)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        if (Key.isPressed)
        {
            isEngineRunning = true;
        }
        else 
        {
            isEngineRunning = false;
            verticalInput = 0;
            horizontalInput = 0;
            currentRpm = 0;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        if (steeringWheel.Wheelbeingheld)
        {
            horizontalInput += steeringWheel.OutPut;
        }
    }

    private void CheckAT()
    {
        if (automat.IsPark())
        {
            verticalInput = 0;
            brakeInput += automat.gearInput;
        }

        else if (automat.IsReverse())
        {
            verticalInput -= automat.gearInput;
            verticalInput -= gasPedal.pedalInput;
        }

        else if (automat.IsDrive())
        {
            verticalInput += automat.gearInput;
            verticalInput += gasPedal.pedalInput;
        }

        else if (automat.IsNeutral())
        {
            verticalInput = 0;
        }
    }
    private void HandleSteering()
    {
        wheelCollider[0].steerAngle = horizontalInput * steeringAngle;
        wheelCollider[1].steerAngle = horizontalInput * steeringAngle;
    }

    private void HandleMotor()
    {
        foreach (WheelCollider wheel in wheelCollider)
        {
           wheel.motorTorque = verticalInput * motorPower * Time.deltaTime;
        }
    }

    private void HandleBrake()
    {
       brakeInput = isBraking ? brakeForce : 0f;
       foreach (WheelCollider wheel in wheelCollider)
       { 
          wheel.brakeTorque = brakeInput;  
       }
    }

    private void UpdateWheelVisuals()
    {
        for (int i = 0; i < wheelCollider.Length; i++)
        {
            UpdateWheelTransform(wheelCollider[i], wheels[i]);
        }
    }

     private void UpdateWheelTransform(WheelCollider collider, Transform transform)
    {
       Vector3 position;
       Quaternion rotation;
       collider.GetWorldPose(out position, out rotation);
       transform.position = position;
       transform.rotation = rotation;
    }

    private void Speedometr()
    {
        speed *= 3.6f;
        speedometr.text = $"{(int)speed} км/ч";
    }

    private void Rpm()
    {
        foreach (WheelCollider wheelCollider in wheelCollider)
        {
            currentRpm = wheelCollider.rpm * 1000f;
            //currentRpm = Mathf.Clamp(currentRpm, MinRpm, MaxRpm);
            tahometr.text = $"{(int)currentRpm} об/мин";
        }
    }
}
