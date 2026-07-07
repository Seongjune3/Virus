using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RotateMirror : MonoBehaviour, IPointerDownHandler
{
    public List<float> angles;
    int currentIndex = 0;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, angles[currentIndex]);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.IsCleared) return;

        currentIndex = (currentIndex + 1) % angles.Count;
        transform.rotation = Quaternion.Euler(0, 0, angles[currentIndex]);
    }
}