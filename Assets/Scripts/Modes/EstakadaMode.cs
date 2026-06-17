using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EstakadaMode : MonoBehaviour
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
    [SerializeField] private GameObject[] checkPoints;
    private enum Step
    {
        Belt,
        StartEngine,
        DriveMode,
        ReleaseHandbrake,
        LeftTurnSignal,
        Gas,
        DisableLeftTS,
        CheckPointBrake,
        DriveStraight,
        Completed
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
                if(LeftTS.isPressed)
                {
                    currentStep = Step.Gas;
                }
                break;

            case Step.Gas:
                if (gasPedal.isPressed)
                {
                    currentStep = Step.DisableLeftTS;
                }
                break;

            case Step.DisableLeftTS:
                if (!LeftTS.isPressed)
                {
                    currentStep = Step.CheckPointBrake;
                }
                break;

            case Step.CheckPointBrake:
                if (brakePedal.isPressed)
                {
                    currentStep = Step.DriveStraight;
                }
                break;

            case Step.DriveStraight:
                StartCoroutine(CompleteAfterDelay(10f));
                if (gasPedal.isPressed)
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

            case Step.LeftTurnSignal:
                text.text = "Включите левый поворотник";
                break;

            case Step.Gas:
                text.text = "Нажмите на педаль газа";
                break;

            case Step.DisableLeftTS:
                text.text = "Выключите левый поворотник";
                break;

            case Step.CheckPointBrake:
                if (checkPoints.Length > 0 && checkPoints[0] != null)
                {
                    checkPoints[0].SetActive(true);
                }
                text.text = "Напрявляйтесь к указанной точке и выполните остановку";
                break;

            case Step.DriveStraight:
                text.text = "Продолжаем движение в прямом направлении";
                panel.SetActive(true);
                break;

            case Step.Completed:
                text.text = "Направляйтесь к указанной точке и выполните остановку";
                if (checkPoints.Length > 1 && checkPoints[1] != null)
                {
                    checkPoints[1].SetActive(true);
                }
                ResultText.text = "Вы выполнили упражнение Остановка и начало движения на подъеме без отката машины назад";
                break;
        }
    } 

    IEnumerator CompleteAfterDelay(float delay)
    {
        panel.SetActive(false);
        // Показываем текст
        if (text != null && text.gameObject != null && currentStep == Step.DriveStraight)
        {
            text.gameObject.SetActive(true);
        }
        // Ждём указанное количество секунд
        yield return new WaitForSeconds(delay);

    }
}
