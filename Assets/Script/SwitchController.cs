using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SwitchController : MonoBehaviour, IPointerDownHandler
{
    public List<GameObject> redObjects;
    public List<GameObject> blueObjects;
    public bool isRed = true;
    public float offAlpha = 0.3f;

    public Color redStateColor = Color.red;
    public Color blueStateColor = Color.blue;

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

        switchRenderer.color = isRed ? redStateColor : blueStateColor;
    }

    void SetState(GameObject obj, bool active)
    {
        Collider2D[] allColliders = obj.GetComponentsInChildren<Collider2D>();
        
        foreach (Collider2D col in allColliders)
        {
            col.enabled = active;
        }
    }
}