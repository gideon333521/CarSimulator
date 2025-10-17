using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PedalController : MonoBehaviour
{
    
    public bool isPressed;
    public Scrollbar PedalScrollbar;
    public float pedalInput = 0f;
    public float sensitivity = 2f;

    public Image pedalBackground;
    private Color defaultColor = Color.gray;
    public Color pressedColor;
    
    void Start()
    {
        PedalScrollbar.value = 0f;
        PedalScrollbar.direction = Scrollbar.Direction.BottomToTop;

        if (pedalBackground != null)
        {
            pedalBackground.color = defaultColor;
        }
    }

    void Update()
    {
        pedalInput = PedalScrollbar.value;
        if (isPressed)
        {
            pedalInput = PedalScrollbar.value * sensitivity * Time.deltaTime;
        }
            UpdatePedals();
    }


    private void UpdatePedals()
    {
        if (pedalBackground != null)
        {
            pedalBackground.color = Color.Lerp(defaultColor, pressedColor, pedalInput);
        }
    }
}
