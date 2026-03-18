using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PedalController : MonoBehaviour
{

    [Header("Scrollbar Config")]
    [SerializeField] private Slider pedal;
    public float pedalInput;
    public bool isPressed;

    private void Start()
    {
        pedalInput = 0f;
        pedal.direction = Slider.Direction.BottomToTop;
        SetUpPedal();
    }

    void Update()
    {
        if (isPressed)
        {
            pedalInput = pedal.value;
        }
        else
        {
            pedalInput = 0;
        }
        pedalInput = Mathf.Clamp(pedalInput, pedal.minValue, pedal.maxValue); 
    }


    void SetUpPedal()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => onClickDown());

        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => onClickUp());

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);

    }


    void onClickDown()
    {
        isPressed = true;
        pedalInput = pedal.value;
    }


    void onClickUp()
    {
        isPressed = false;
        pedal.value = 0f;
    }
}
