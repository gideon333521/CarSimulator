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
    [SerializeField] private float rpm;
    private float brakePower;
    private float motorTorque;

    [SerializeField] WheelCollider[] wheelCollider;
    [SerializeField] Transform[] wheels;
    private Rigidbody carRigidbody;

    [SerializeField] private PedalController gasPedal;
    [SerializeField] private PedalController brakePedal;
    [SerializeField] private SteeringWheelController steeringWheel;


    private float speed;
    [SerializeField] private TextMeshProUGUI speedometr;

    private float verticalInput;
    private float horizontalInput;
    private float brakeInput;
    private bool isBraking;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        CheckInput();
        speed = carRigidbody.linearVelocity.magnitude;
        isBraking = Input.GetKeyDown(KeyCode.Space);
        Speedometr();
    }


    private void FixedUpdate()
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
        if (brakePedal.isPressed)
        {
            verticalInput -= brakePedal.pedalInput;
            //brakeInput += brakePedal.pedalInput;
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
        if (isBraking == false && Mathf.Abs(verticalInput) > 0.1f)
        {
            foreach (WheelCollider wheel in wheelCollider)
            {
                wheel.motorTorque = verticalInput * motorPower * Time.deltaTime;
                motorTorque = wheel.motorTorque * 9550 / rpm;
            }
        }
    }

    private void HandleBrake()
    {
        brakePower = isBraking ? brakeForce : 0f;
        brakeInput = brakePower;
        foreach (WheelCollider wheel in wheelCollider)
        {
            wheel.brakeTorque = brakePower;

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
        float currentSpeed = Mathf.Round(speed * 36f);
        speedometr.text = currentSpeed.ToString() + $"κμ/χ" ;
    }
}
