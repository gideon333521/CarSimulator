using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IgnitionKey : MonoBehaviour
{
    [SerializeField] Button Key;
    public Text buttonText;
    private bool isEngineRunning = false;
    
    void Start()
    {
        Key.onClick.AddListener(ToggleEngine);
    }
   

    public void ToggleEngine()
    {
        isEngineRunning = !isEngineRunning;
        if (isEngineRunning)
        {
            isEngineRunning = false;
            buttonText.text = "Stop";
        }
        else 
        {
            isEngineRunning = true;
            buttonText.text = "Start";
        }
    }

    public bool EngineOn() => isEngineRunning = true;
    public bool EngineOf() => isEngineRunning = false;
}
