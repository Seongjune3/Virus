using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RotateMirror : MonoBehaviour, IPointerDownHandler
{
    public List<float> angles;
    int currentIndex = 0;

    float currentX;
    float currentY;

    void Start()
    {
        currentX = transform.eulerAngles.x;
        currentY = transform.eulerAngles.y;

        transform.rotation = Quaternion.Euler(currentX , currentY, angles[currentIndex]);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.IsCleared) return;

        currentIndex = (currentIndex + 1) % angles.Count;
        transform.rotation = Quaternion.Euler(currentX , currentY, angles[currentIndex]);
    }
}