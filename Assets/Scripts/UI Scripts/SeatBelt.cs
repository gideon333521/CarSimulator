using UnityEngine;
using UnityEngine.UI;

public class SeatBelt : MonoBehaviour
{
    [SerializeField] private Button belt;
    [SerializeField] private Image backgroound;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    public bool isPressed;
    void Start()
    {
        backgroound.color = disableColor;
        belt.onClick.AddListener(PointerClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            backgroound.color = enableColor;
            isPressed = true;
        }
        else 
        {
            backgroound.color = disableColor;
            isPressed = false;
        }
    }

    void PointerClick()
    {
        isPressed = !isPressed;
    }
}
