using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SwitchController : MonoBehaviour, IPointerDownHandler
{
    public List<GameObject> redObjects;
    public List<GameObject> blueObjects;
    public bool isRed = true;
    public float offAlpha = 0.3f;

    SpriteRenderer switchRenderer;

    void Start()
    {
        switchRenderer = GetComponent<SpriteRenderer>();
        UpdateObjects();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.IsCleared) return;

        isRed = !isRed;
        UpdateObjects();
    }

    void UpdateObjects()
    {
        foreach (GameObject obj in redObjects)
        {
            SetState(obj, isRed);
        }

        foreach (GameObject obj in blueObjects)
        {
            SetState(obj, !isRed);
        }

        switchRenderer.color = isRed ? Color.red : Color.blue;
    }

    void SetState(GameObject obj, bool active)
    {
        obj.GetComponent<Collider2D>().enabled = active;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = active ? 1f : offAlpha;
        sr.color = c;
    }
}