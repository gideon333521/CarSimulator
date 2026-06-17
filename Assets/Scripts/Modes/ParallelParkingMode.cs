using UnityEngine;
using UnityEngine.UI;

public class ParallelParkingMode : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text text;
    [SerializeField] private Text ResultText;
    [SerializeField] private IgnitionKey key;
    [SerializeField] private AutomatTransmission automat;
    [SerializeField] private HandbrakeController handbrake;
    [SerializeField] private SeatBelt seatBelt;
    [SerializeField] private PedalController gasPedal;
    [SerializeField] private PedalController brakePedal;
    [SerializeField] private TurnSignal LeftTS;
    [SerializeField] private TurnSignal RightTS;
    [SerializeField] private SteeringWheelController steeringWheel;
    [SerializeField] private SteeringButton LeftBtn;
    [SerializeField] private SteeringButton RightBtn;
    [SerializeField] private GameObject[] checkPoints;
    private enum Step
    {
        Belt,
        StartEngine,
        DriveMode,
        ReleaseHandbrake,
        LeftTurnSignal,
        RightTurnSignal,
        Gas,
        CheckPointBrake,
        SteeringRight,
        ReverseMode,
        CheckPointReverse,
        Complete
    }
    private Step currentStep = Step.Belt;
    void Start()
    {
        panel.SetActive(true);
        foreach (GameObject obj in checkPoints)
        {
            obj.SetActive(false);
        }
        UpdateText();
    }

    void Update()
    {
        switch (currentStep)
        {
            case Step.Belt:
                if (seatBelt.isPressed)
                {
                    currentStep = Step.StartEngine;
                }
                break;

            case Step.StartEngine:
                if (key.isPressed)
                {
                    currentStep = Step.DriveMode;
                }
                break;

            case Step.DriveMode:
                if (automat.IsDrive())
                {
                    currentStep = Step.ReleaseHandbrake;
                }
                break;

            case Step.ReleaseHandbrake:
                if (!handbrake.isPressed)
                {
                    currentStep = Step.LeftTurnSignal;
                }
                break;

            case Step.LeftTurnSignal:
                if (LeftTS.isPressed)
                {
                    currentStep = Step.Gas;
                }
                break;

            case Step.Gas:
                if (gasPedal.isPressed)
                {
                    currentStep = Step.CheckPointBrake;
                }
                break;

            case Step.CheckPointBrake:
                if (brakePedal.isPressed)
                {
                    currentStep = Step.ReverseMode;
                }
                else
                {
                    currentStep = Step.CheckPointBrake;
                }
                break;

            case Step.ReverseMode:
                if (automat.IsReverse())
                {
                    currentStep = Step.SteeringRight;
                }
                break;

            case Step.SteeringRight:
                if (steeringWheel.OutPut == 1f || RightBtn.dampenPress == 1f)
                {
                    currentStep = Step.RightTurnSignal;
                }
                break;

            case Step.RightTurnSignal:
                if (RightTS.isPressed)
                {
                    currentStep = Step.CheckPointReverse;
                }
                break;

            case Step.CheckPointReverse:
                if (automat.IsNeutral() && handbrake.isPressed)
                {
                    currentStep = Step.Complete;
                }
                break;
        }
        UpdateText();
    }

    void UpdateText()
    {
        switch (currentStep)
        {
            case Step.Belt:
                text.text = "Пристегните ремень безопасности";
                break;

            case Step.StartEngine:
                text.text = "Запустите двигатель";
                break;

            case Step.DriveMode:
                text.text = "Включите передачу D";
                break;

            case Step.ReleaseHandbrake:
                text.text = "Опустите ручной тормоз";
                break;

            case Step.LeftTurnSignal:
                text.text = "Включите левый поворотник";
                break;

            case Step.Gas:
                text.text = "Нажмите на педаль газа";
                break;

            case Step.CheckPointBrake:
                if (checkPoints.Length > 0 && checkPoints[0] != null)
                {
                    checkPoints[0].SetActive(true);
                }
                text.text = "Напрявляйтесь к указанной точке по линии разметки и выполните остановку";
                break;

            case Step.ReverseMode:
                text.text = "Включите передачу R";
                break;

            case Step.SteeringRight:
                text.text = "Поверните руль вправо до упора";
                break;

            case Step.RightTurnSignal:
                text.text = "Включите правый поворотник";
                break;

            case Step.CheckPointReverse:
                if (checkPoints.Length > 0 && checkPoints[1] != null)
                {
                    checkPoints[1].SetActive(true);
                }
                text.text = "Двигайтесь к указанной точке и выполните остановку";
                break;

            case Step.Complete:
                if (checkPoints.Length > 0 && checkPoints[2] != null)
                {
                    checkPoints[2].SetActive(true);
                }
                ResultText.text = "Вы выполнили упражнение Параллельная паркока задним ходом параллельно краю проезжей части";
                break;
        }
    }
}
