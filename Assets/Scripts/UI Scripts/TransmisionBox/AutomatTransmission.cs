using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutomatTransmission : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Gear
    {
        P,
        R,
        N,
        D
    }

    [SerializeField] private Slider gearSlider;
    [SerializeField] private RectTransform Selector;
    [SerializeField] private Text[] gearText;

    public float gearInput;

    private Color select = Color.green;
    private Color deselect = Color.white;

    public Gear currentGear;
    private bool snapToPositions = true;  // Прилипание к позициям

    public Gear[] gearOrder = { Gear.P, Gear.R, Gear.N, Gear.D };
    private bool isDragging;
    private float[] gearPositions;

    void Awake()
    {
        if (gearSlider == null)
        gearSlider = GetComponent<Slider>();
        gearSlider.minValue = 0;
        gearSlider.maxValue = gearOrder.Length - 1;
        gearSlider.wholeNumbers = false;

        gearPositions = new float[gearOrder.Length];
        for (int i = 0; i < gearOrder.Length; i++)
        {
            gearPositions[i] = i;
            gearText[i].fontSize = 25;
            gearText[i].fontStyle = FontStyle.Normal;
        }
    }

    void Start()
    {
        // Подписываемся на событие слайдера
        if (gearSlider != null)
        {
            gearSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        currentGear = gearOrder[0];
        gearText[0].color = select;
        gearText[0].fontSize = 30;
        gearText[0].fontStyle = FontStyle.Bold;
    }

    void Update()
    {
        if (isDragging)
        {
           gearInput = gearSlider.value;
           OnSliderValueChanged(gearInput);
           UpdateGearFromValue();     
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (!isDragging)
        {
            // Если не перетаскиваем, просто обновляем UI
            UpdateUI();
            return;
        }

        // Находим ближайшую передачу
        Gear nearestGear = GetNearestGear(value);

        // Обновляем подсветку в реальном времени
        UpdateUI(value);

        // Если прилипание включено и перетаскивание закончено
        if (snapToPositions)
        {
            // Визуально показываем подсветку текущей ближайшей передачи
            HighlightNearestGear(nearestGear);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Слайдер уже обновляется автоматически
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (snapToPositions)
        {
            // Находим ближайшую передачу и прилипаем к ней
            Gear nearestGear = GetNearestGear(gearInput);
            SetGear(nearestGear, true);
        }
        else
        {
            // Просто обновляем передачу по текущему значению
            UpdateGearFromValue();
        }
    }

    private Gear GetNearestGear(float value)
    {
        int nearestIndex = Mathf.RoundToInt(Mathf.Clamp(value, 0, gearOrder.Length - 1));
        return gearOrder[nearestIndex];
    }

    private void UpdateGearFromValue()
    {
        Gear newGear = GetNearestGear(gearInput);
        SetGear(newGear, true);
    }

    public void SetGear(Gear newGear, bool playFeedback = true)
    {
        if (currentGear == newGear) return;

        currentGear = newGear;

        // Обновляем позицию слайдера
        if (gearSlider != null)
        {
            gearSlider.value = (int)currentGear;
        }

        UpdateUI();

        Debug.Log($"Передача изменена на: {currentGear}");
    }

    private void UpdateUI()
    {
        UpdateUI(gearInput);
    }

    private void UpdateUI(float sliderValue)
    {
        // Обновляем цвет текстов
        int currentIndex = (int)currentGear;

        for (int i = 0; i < gearText.Length && i < gearOrder.Length; i++)
        {
            if (gearText[i] != null)
            {
                // Активная передача подсвечивается
                bool isActive = (i == currentIndex);
                gearText[i].color = isActive ? select : deselect;

                // Дополнительно увеличиваем размер активного текста
                if (isActive)
                {
                    gearText[i].fontSize = 30;
                    gearText[i].fontStyle = FontStyle.Bold;
                }
                else
                {
                    gearText[i].fontSize = 25;
                    gearText[i].fontStyle = FontStyle.Normal;
                }
            }
        }
    }

         private void HighlightNearestGear(Gear gear)
         {
            int index = (int)gear;
             for (int i = 0; i < gearText.Length; i++)
             {
                if (gearText[i] != null)
                {
                  // Временно подсвечиваем ближайшую передачу
                  bool isNearest = (i == index);
                  gearText[i].color = isNearest ? select : deselect;
                }
            }
         }

    public void NextGear()
    {
        int nextIndex = Mathf.Min((int)currentGear + 1, gearOrder.Length - 1);
        SetGear(gearOrder[nextIndex], true);
    }

    public void PreviousGear()
    {
        int prevIndex = Mathf.Max((int)currentGear - 1, 0);
        SetGear(gearOrder[prevIndex], true);
    }

    public bool IsReverse() => currentGear == Gear.R;
    public bool IsDrive() => currentGear == Gear.D;
    public bool IsPark() => currentGear == Gear.P;
    public bool IsNeutral() => currentGear == Gear.N;
}
