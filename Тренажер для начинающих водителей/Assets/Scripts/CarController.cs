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
    [SerializeField] private float steeringAngle = 60f;
    private float brakePower;

    [SerializeField] private WheelCollider[] wheelCollider;
    [SerializeField] private Transform[] wheels;
    private Rigidbody carRigidbody;
    

    //[SerializeField] private PedalController gasPedal;
    //[SerializeField] private PedalController brakePedal;

    private float speed;
    [SerializeField] private Text speedometr;

    private float gasInput;
    private float steeringInput;
    private bool isBraking;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        speed = carRigidbody.linearVelocity.magnitude;
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        isBraking = Input.GetKeyDown(KeyCode.Space);
        if (Input.GetKey(KeyCode.W)) Debug.Log("W pressed - Acceleration");
        if (Input.GetKey(KeyCode.S)) Debug.Log("S pressed - Brake/Reverse");
        if (Input.GetKey(KeyCode.A)) Debug.Log("A pressed - Left");
        if (Input.GetKey(KeyCode.D)) Debug.Log("D pressed - Right");
        Speedometr();
    }


    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        HandleBrake();
        UpdateWheelVisuals();
    }

    private void HandleSteering()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.linearVelocity);
        float speedfactor = Mathf.Clamp01(speed);
        bool isReversing = speed != 0 && localVelocity.z < -0.1f;
        float steer = steeringInput * steeringAngle;
        wheelCollider[0].steerAngle = steer;
        wheelCollider[1].steerAngle = steer;
        if (speed != 0)
        {
           transform.Rotate(Vector3.up * steer * speedfactor * Time.deltaTime);
        if (isReversing)
        {
           transform.Rotate(Vector3.down * steer * speedfactor * Time.deltaTime);
        }

        }
    }

    private void HandleMotor()
    {
       
        if (isBraking == false && Mathf.Abs(gasInput) > 0.1f)
        {
            foreach (WheelCollider wheel in wheelCollider)
            {
                wheel.motorTorque = gasInput * motorPower * Time.deltaTime;
            }
        }
    }

    private void HandleBrake()
    {
        brakePower = isBraking ? brakeForce : 0f;
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
       Quaternion rotation = Quaternion.identity;
       collider.GetWorldPose(out position, out rotation);
       transform.position = position;
       transform.rotation = rotation;
    }

    private void Speedometr()
    {
        float currentSpeed = Mathf.Round(speed * 3.6f);
        speedometr.text = currentSpeed.ToString();
    }
}
