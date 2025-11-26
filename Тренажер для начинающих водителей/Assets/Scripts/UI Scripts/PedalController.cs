using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PedalController : MonoBehaviour
{
    public enum PedalType { Gas, Brake }
    private PedalType pedalType;

    [Header("Scrollbar Config")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float sensitivity = 1.5f;

    private void Start()
    {
        if (scrollbar == null)
            scrollbar = GetComponent<Scrollbar>();

        // Подписываемся на событие изменения значения
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);

        // Настраиваем направление (снизу вверх для педали)
        scrollbar.direction = Scrollbar.Direction.BottomToTop;

        // Инициализация
        scrollbar.value = 0f;
        scrollbar.size = 0.2f;
    }

    private void OnScrollbarValueChanged(float value)
    {
        // Применяем чувствительность
        float adjustedValue = Mathf.Pow(value, sensitivity);
        UpdateVisuals(adjustedValue);
    }


    private void UpdateVisuals(float value)
    {
        // Визуальная обратная связь через цвет
        Image handleImage = scrollbar.handleRect.GetComponent<Image>();
        if (handleImage != null)
        {
            Color targetColor = pedalType == PedalType.Gas ?
                Color.Lerp(Color.white, Color.green, value):
                Color.Lerp(Color.white, Color.red, value);

            handleImage.color = targetColor;
        }
    }
}
