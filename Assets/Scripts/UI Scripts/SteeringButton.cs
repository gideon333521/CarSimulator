using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SteeringButton : MonoBehaviour
{
    [SerializeField] private Button HandleBtn;
    public float dampenPress;
    public bool isPressed;
    void Start()
    {
        SetUp();
        dampenPress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            dampenPress = 1;
        }
        else 
        {
            dampenPress = 0;
        }
        dampenPress = Mathf.Clamp01(dampenPress);
    }

    void SetUp()
    {

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => OnClickDown());

        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => OnClickUp());

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }

    void OnClickDown()
    {
        isPressed = true;
    }

    void OnClickUp()
    {
        isPressed = false;
    }
}

