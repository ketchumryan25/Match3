using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchOnUIImageDetector : MonoBehaviour, IPointerDownHandler
{
    public bool touchStartedOnImage = false;

    public delegate void TouchStartedHandler();
    public event TouchStartedHandler OnTouchStarted;

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartedOnImage = true;
        OnTouchStarted?.Invoke();
        Debug.Log("Touch started on UI Image");
    }
}