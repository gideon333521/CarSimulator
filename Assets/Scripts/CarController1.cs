using TMPro;
using Unity.VisualScripting;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float motorPower;
    [SerializeField] private float brakeForce;
    [SerializeField] private float steeringAngle = 30f;

    [SerializeField] private float[] gearRatios;
    [SerializeField] private float MinRpm;
    [SerializeField] private float MaxRpm;
    private float gearRatio;
    private float currentRpm;
    private int currentGear;

    [SerializeField] WheelCollider[] wheelCollider;
    [SerializeField] Transform[] wheels;
    private Rigidbody carRigidbody;

    [SerializeField] private PedalController gasPedal;
    [SerializeField] private PedalController brakePedal;
    [SerializeField] private SteeringWheelController steeringWheel;
    [SerializeField] private HandbrakeController handbrake;
    [SerializeField] private IgnitionKey Key;
    [SerializeField] private AutomatTransmission automat;

    [SerializeField] private GameObject LeftMirror;
    [SerializeField] private GameObject RightMirror;

    private float speed;
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

        if (brakePedal.isPressed || handbrake.isPressed)
        {
            brakePedal.pedalInput = isBraking ? brakePedal.pedalInput : 0f;
            brakeInput = Mathf.Min(brakeInput + brakePedal.pedalInput * Time.deltaTime * 3f, brakeForce);
            verticalInput = 0;
            isBraking = true;
        }
        else
        {
            brakeInput = 0;
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
            LeftMirror.SetActive(true);
            RightMirror.SetActive(true);
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
        speedometr.text = $"{(int)speed}км/ч";
    }

    private void Rpm()
    {
        currentRpm = CalculateRPM();
        if (isEngineRunning)
        {
            tahometr.text = Mathf.RoundToInt(currentRpm).ToString() + "об/мин";
        }
        else
        {
            tahometr.text = $"{0}об/мин";
        }
    }

    float CalculateRPM()
    {
        float totalRPM = 0f;
        if (speed < 0.5f && gasPedal.pedalInput < 0.1f)
        {
            return MinRpm; 
        }

        foreach (var wheelCollider in wheelCollider)
        {
            totalRPM += Mathf.Abs(wheelCollider.rpm);
        }
        float avgRPM = totalRPM / wheelCollider.Length;
        gearRatio = gearRatios[currentGear];
        float engineRPM = avgRPM * gearRatio;
        return Mathf.Clamp(engineRPM, MinRpm, MaxRpm);
    }
}
