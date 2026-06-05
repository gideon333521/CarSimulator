using System;
using System.Collections;
using System.Drawing.Text;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class CarController : MonoBehaviour
{
    public enum HandlingState
    {
        SteeringWheel,
        HandleButtons
    }
    [Header("Car Settings")]
    [SerializeField] private float motorPower;
    private float currentTorque;
    [SerializeField] private float brakeForce;
    [SerializeField] private float steeringAngle = 30f;

    [SerializeField] private float[] gearRatios;
    [SerializeField] private float MinRpm;
    [SerializeField] private float MaxRpm;
    [SerializeField] private float increaseRpm;
    [SerializeField] private float decreaseRpm;
    [SerializeField] private AnimationCurve hpToCurve;
    [SerializeField] private float differentialRatio;
    private float currentRpm;
    [SerializeField] private int currentGear = 0;

    [SerializeField] private Transform rpmneedle;
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    [SerializeField] private float changeGearTime = 0.5f;

    [SerializeField] WheelCollider[] wheelCollider;
    [SerializeField] Transform[] wheels;
    private Rigidbody carRigidbody;

    [SerializeField] private PedalController gasPedal;
    [SerializeField] private PedalController brakePedal;
    [SerializeField] private SteeringWheelController steeringWheel;
    [SerializeField] private GameObject HandleButtons;
    [SerializeField] private SteeringButton leftBtn;
    [SerializeField] private SteeringButton rightBtn;
    [SerializeField] private HandbrakeController handbrake;
    [SerializeField] private IgnitionKey Key;
    [SerializeField] private AutomatTransmission automat;

    private float speed;
    [SerializeField] private TextMeshProUGUI speedometr;
    [SerializeField] private TextMeshProUGUI tahometr;
    [SerializeField] private TextMeshProUGUI gear;

    private float verticalInput;
    private float horizontalInput;
    private float brakeInput;
    private bool isBraking;
    private bool isEngineRunning;
    public HandlingState handlingState;


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
        Rpm();
        CheckInput();
        CheckHandle();
        CheckAT();
        speed = carRigidbody.linearVelocity.magnitude;
        Speedometr();
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

        if (leftBtn.isPressed)
        {
            horizontalInput -= leftBtn.dampenPress;
        }

        if (rightBtn.isPressed)
        {
            horizontalInput += rightBtn.dampenPress;
        }
    }

    void CheckHandle()
    {
        switch (handlingState)
        {
            case HandlingState.SteeringWheel:
                steeringWheel.reference.SetActive(true);
                HandleButtons.SetActive(false);
                break;

            case HandlingState.HandleButtons:
                steeringWheel.reference.SetActive(false);
                HandleButtons.SetActive(true);
                break;
        }
    }

    private void CheckAT()
    {
        if (automat.IsPark())
        {
            verticalInput = 0;
            brakeInput += automat.gearInput;
            gear.text = "P";

        }

        else if (automat.IsReverse())
        {
            verticalInput -= automat.gearInput;
            verticalInput -= gasPedal.pedalInput;
            gear.text = "R";
        }

        else if (automat.IsDrive())
        {
            verticalInput += automat.gearInput;
            verticalInput += gasPedal.pedalInput;
            gear.text = "D";
        }

        else if (automat.IsNeutral())
        {
            verticalInput = 0;
            gear.text = "N";
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
            currentTorque = CalculateTorqueAT();
            wheel.motorTorque = verticalInput * currentTorque;
        }
    }

    float CalculateTorqueAT()
    {
        float torque = 0;
        currentGear = Mathf.Clamp(currentGear, 0, gearRatios.Length);
        if (currentRpm > increaseRpm)
        {
            StartCoroutine(ChangeGear(1));
        }
        else if (currentRpm < decreaseRpm && currentGear > 1)
        {
            StartCoroutine(ChangeGear(-1));
        }
        float wheelRpm = 0;
        if (isEngineRunning)
        {
            foreach (WheelCollider wheel in wheelCollider)
            {
                wheelRpm += Mathf.Abs(wheel.rpm/2f) * gearRatios[currentGear] * differentialRatio;
            }
            currentRpm = Mathf.Lerp(currentRpm, Mathf.Max(MinRpm, wheelRpm), Time.deltaTime * 3f);
            currentRpm = Mathf.Max(MinRpm, currentRpm);
            torque = (hpToCurve.Evaluate(currentRpm / MaxRpm) * motorPower / currentRpm) * gearRatios[currentGear] * differentialRatio * 5252f; 
        }
        Debug.Log($"Engine RPM: {currentRpm}, Gear: {currentGear}, GearRatio: {gearRatios[currentGear]}, Wheel RPM: {wheelRpm}, Torque: {torque}");
        return torque;
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
       transform.SetPositionAndRotation(position, rotation);
    }

    private void Speedometr()
    {
       speed *= 3.6f;
       speedometr.text = Mathf.RoundToInt(speed).ToString() + "км/ч";
       Debug.Log($"Speed: {speed}");
    }

    private void Rpm()
    {
        //currentRpm = CalculateRPM();
        if (isEngineRunning)
        {
            tahometr.text = Mathf.RoundToInt(currentRpm).ToString() + "об/мин";
            rpmneedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minRotation, maxRotation, currentRpm / (MaxRpm * 1.1f)));
        }
        else
        {
            tahometr.text = $"{0}об/мин";
            rpmneedle.rotation = Quaternion.Euler(0,0, minRotation);
        }
    }

    IEnumerator ChangeGear(int gearChange)
    {
        int newGear = currentGear + gearChange; // Вычисляем новую шестерню
                                                
        if (newGear >= 0 && newGear < gearRatios.Length)
        {
            yield return new WaitForSeconds(changeGearTime);
            currentGear = newGear; // Устанавливаем новую передачу
        }
    }
}
