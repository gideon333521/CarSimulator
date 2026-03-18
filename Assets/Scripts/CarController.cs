using TMPro;
using Unity.VisualScripting;
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


    private float speed;
    private float currentSpeed;
    [SerializeField] private TextMeshProUGUI speedometr;
    [SerializeField] private TextMeshProUGUI tahometr;

    private float verticalInput;
    private float horizontalInput;
    private float brakeInput;
    private bool isBraking;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        isBraking = true;
    }


    void Update()
    {
        CheckInput();
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

        brakeInput = Mathf.Abs(brakeInput);
        if (brakePedal.isPressed )
        {
            brakeInput = verticalInput;
            verticalInput -= brakePedal.pedalInput;
        }
        else
        {
            brakeInput = 0;        
        }

        isBraking = handbrake.isPressed;
        if (handbrake.isPressed)
        {
            brakeInput += handbrake.impact;
            isBraking = true;
        }
        else
        {
            brakeInput = 0;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        if (steeringWheel.Wheelbeingheld)
        {
            horizontalInput += steeringWheel.OutPut;
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
        currentSpeed = Mathf.Round(speed * 36f);
        speedometr.text = currentSpeed.ToString() +  $"км/ч" ;
    }

    private void Rpm()
    {
        foreach (WheelCollider wheelCollider in wheelCollider)
        {
            currentRpm = wheelCollider.rpm * 1000f;
            tahometr.text = currentRpm.ToString() + $"об/мин";
        }
    }
}
