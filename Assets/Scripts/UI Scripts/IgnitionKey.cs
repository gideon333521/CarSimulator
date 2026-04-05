using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IgnitionKey : MonoBehaviour
{
    [SerializeField] private Button Key;
    [SerializeField] private Text buttonText;
    public bool isPressed;

    void Start()
    {
        SetUpKey();
    }

     void Update()
     {
        if (isPressed)
        {
            buttonText.text = "On";
        }
        else 
        {
            buttonText.text = "Off";
        }
    }

    void SetUpKey()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        var pointerClickDown = new EventTrigger.Entry();
        pointerClickDown.eventID = EventTriggerType.PointerClick;
        pointerClickDown.callback.AddListener((e) => onClick());
        trigger.triggers.Add(pointerClickDown);
    }

    void onClick()
    {
        isPressed = !isPressed;
    }
}
