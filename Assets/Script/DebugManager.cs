using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public GameObject debugPanel;
    public GameObject debugPanelText;
    public TextMeshProUGUI debugText;
    public GameObject buttonGroup;

    bool debugMode = false;

    void Start()
    {
        debugPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isActive = !debugPanel.activeSelf;
            debugPanel.SetActive(isActive);


            if (isActive)
            {
                debugMode = false;
                buttonGroup.SetActive(true);         
                debugText.gameObject.SetActive(false); 
                debugPanelText.SetActive(true);
            }
        }


        if (debugMode == true)
        {
            UpdateDebugText();
        }
    }

    void UpdateDebugText()
    {
        VirusLaser laser = FindFirstObjectByType<VirusLaser>();

        string resultText = "[노드 연결 상태]\n\n시작";

        List<string> hitObjects = GetLaserHitObjects(laser);

        for (int i = 0; i < hitObjects.Count; i++)
        {
            resultText += " -> " + hitObjects[i];
        }

        if (hitObjects.Count == 0)
        {
            resultText += " -> 충돌 없음";
        }

        debugText.text = resultText;
    }

    List<string> GetLaserHitObjects(VirusLaser laser)
    {
        List<string> objects = new List<string>();

        CheckLine(laser.lineRenderer1, objects, laser.objectLayer);
        CheckLine(laser.lineRenderer2, objects, laser.objectLayer);

        return objects;
    }

    void CheckLine(LineRenderer lr, List<string> nameList, LayerMask layer)
    {
        if (lr == null || lr.positionCount < 2) return;

        Vector3[] points = new Vector3[lr.positionCount];
        lr.GetPositions(points);

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 dir = (points[i + 1] - points[i]).normalized;
            
            Vector3 extendedEndPoint = points[i + 1] + dir * 0.05f;

            RaycastHit2D hit = Physics2D.Linecast(points[i], extendedEndPoint, layer);

            if (hit.collider != null)
            {
                if (!nameList.Contains(hit.collider.name))
                {
                    nameList.Add(hit.collider.name);
                }
            }
        }
    }

    public void DebugButton()
    {
        debugMode = true;
        buttonGroup.SetActive(false);
        debugText.gameObject.SetActive(true);
        debugPanelText.SetActive(false);
    }

    public void SkipLevel()
    {
        GameManager.Instance.NextStage(); 

        debugPanel.SetActive(false);
    }
}