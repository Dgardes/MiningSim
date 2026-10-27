using UnityEngine;
using UnityEngine.EventSystems;

//
public class JoystickHint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private bool useStartPositionAsRest = true;
    [SerializeField] private Vector2 restPosition;
    [SerializeField, Range(0f, 1f)] private float restAlpha = 0.4f;

    private CanvasGroup group;
    private bool pressed;

    private void Awake()
    {
        if (background == null) background = transform.Find("Background") as RectTransform;
        if (background == null)
        {
            Debug.LogWarning("[JoystickHint] Не знайдено Background у " + name);
            enabled = false;
            return;
        }

        if (useStartPositionAsRest) restPosition = background.anchoredPosition;

        group = background.GetComponent<CanvasGroup>();
        if (group == null) group = background.gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        if (group != null) group.alpha = 1f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }

    private void LateUpdate()
    {
        if (pressed) return;

        if (GameSettings.ShowJoystick)
        {
            if (!background.gameObject.activeSelf) background.gameObject.SetActive(true);
            background.anchoredPosition = restPosition;
            group.alpha = restAlpha;
        }
        else if (background.gameObject.activeSelf)
        {
            background.gameObject.SetActive(false);
            group.alpha = 1f;
        }
    }
}
