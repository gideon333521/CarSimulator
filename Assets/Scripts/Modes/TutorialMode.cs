using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class TutorialMode : MonoBehaviour
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
    [SerializeField] private GameObject checkPoint;
    private enum Step
    {
        Belt,
        StartEngine,
        DriveMode,
        ReleaseHandbrake,
        Gas,
        Brake,
        Completed
    }
    private Step currentStep = Step.Belt;
    void Start()
    {
        panel.SetActive(true);
        checkPoint.SetActive(false);
        UpdateText();
    }

    void Update()
    {
        // Каждый кадр проверяем только текущий шаг
        switch (currentStep)
        {
            case Step.Belt:
                if (seatBelt.isPressed)
                    currentStep = Step.StartEngine;
                break;

            case Step.StartEngine:
                if (key.isPressed)
                    currentStep = Step.DriveMode;
                break;

            case Step.DriveMode:
                if (automat.IsDrive())
                    currentStep = Step.ReleaseHandbrake;
                break;

            case Step.ReleaseHandbrake:
                if (!handbrake.isPressed)
                    currentStep = Step.Gas;
                break;

            case Step.Gas:
                if (gasPedal.isPressed)
                    currentStep = Step.Brake;
                break;

            case Step.Brake:
                if (brakePedal.isPressed)
                {
                    currentStep = Step.Completed;
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
            case Step.Gas:
                text.text = "Нажмите на педаль газа";
                break;
            case Step.Brake:
                text.text = "Нажмите на педаль тормоза";
                break;
            case Step.Completed:
                text.text = "Поупражняйтесь в управлении и поставьте автомобиль в указанной точке";
                checkPoint.SetActive(true);
                ResultText.text = "Поздравляю! Вы прошли ознакомление с упавлением автомобиль";
                break;
        }
    }
}
