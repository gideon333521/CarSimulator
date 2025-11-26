using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IgnitionKey : MonoBehaviour
{
    [SerializeField] Button Key;
    public Text buttonText;

    public bool isEngineRunning;
    
    void Start()
    {
        Key.onClick.AddListener(ToggleEngine);
    }
   

    public void ToggleEngine()
    {
        isEngineRunning = !isEngineRunning;
        if (isEngineRunning || Input.GetKeyDown(KeyCode.F))
        {
            isEngineRunning = true;
            buttonText.text = "Start";
            Debug.Log("Машина запущена");
        }
        else 
        {
            isEngineRunning = false;
            buttonText.text = "Stop";
            Debug.Log("Машина заглушена");
        }
    }

    public bool EngineOn() => isEngineRunning = true;
    public bool EngineOf() => isEngineRunning = false;
}
