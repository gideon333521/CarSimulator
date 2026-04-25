using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] private Button PauseBtn;
    [SerializeField] private Button ResumeBtn;
    void Start()
    {
        PauseBtn.onClick.AddListener(Pause);
        ResumeBtn.onClick.AddListener(Resume);  
    }

     void Pause()
    {
        Time.timeScale = 0;
        PausePanel.SetActive(true);    
    }

    void Resume()
    {
        Time.timeScale = 1;
        PausePanel?.SetActive(false);
    }
}
