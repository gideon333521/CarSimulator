using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IgnitionKey : MonoBehaviour
{
    [SerializeField] private Button Key;
    [SerializeField] private Image image;
    [SerializeField] private Color activateColor;
    [SerializeField] private Color deactivateColor;
    public bool isPressed;

    void Start()
    {
        SetUpKey();
    }

     void Update()
     {
        if (isPressed)
        {
            image.color = activateColor;
        }
        else 
        {
            image.color = deactivateColor;
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
