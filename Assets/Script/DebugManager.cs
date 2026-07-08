using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DebugManager : MonoBehaviour
{
    public GameObject debugPanel;
    public Text debugText;

    private int currentMode = 0;

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
                debugText.text = "버튼을 눌러 정보를 확인하세요.";
                currentMode = 0;
            }
        }

        if (debugPanel == null || !debugPanel.activeSelf) return;

        UpdateDebugText();
    }

    void UpdateDebugText()
    {
        VirusLaser laser = FindFirstObjectByType<VirusLaser>();
        SwitchController[] switches = FindObjectsByType<SwitchController>(FindObjectsSortMode.None);

        string resultText = "===== [디버그 모드] =====\n\n";

        if (currentMode == 1)
        {
            if (laser != null)
            {
                resultText += "[노드 연결 상태]\n시작";

                List<string> hitObjects = GetLaserHitObjects(laser);

                for (int i = 0; i < hitObjects.Count; i++)
                {
                    resultText += " -> " + hitObjects[i];
                }

                if (hitObjects.Count == 0)
                {
                    resultText += " -> 충돌 없음";
                }
            }
            else
            {
                resultText += "맵에서 레이저를 찾을 수 없습니다.";
            }
        }
        else if (currentMode == 2) // 2번 버튼: 현재 스위치 상태
        {
            if (switches != null && switches.Length > 0)
            {
                for (int i = 0; i < switches.Length; i++)
                {
                    SwitchController sw = switches[i];
                    string stateStr = "";

                    if (sw.isRed)
                    {
                        stateStr = "빨강 (RED)";
                    }
                    else
                    {
                        stateStr = "파랑 (BLUE)";
                    }

                    resultText += "[스위치 " + sw.gameObject.name + "] 상태: " + stateStr + "\n";
                }
            }
            else
            {
                resultText += "맵에 배치된 스위치가 없습니다.";
            }
        }
        else if (currentMode == 3) // 3번 버튼: 레이저 기본 수치 및 메모리 상태
        {
            if (laser != null)
            {
                // 소수점 떼고 정수로 각도 저장
                float angle = Mathf.Round(laser.transform.eulerAngles.z);

                resultText += "[레이저 정보]\n";
                resultText += "현재 각도: " + angle.ToString() + "도\n";
                resultText += "남은 반사 횟수: " + laser.maxBounce.ToString() + "회\n\n";
            }
            else
            {
                resultText += "레이저 정보 없음\n\n";
            }

            // GameManager가 있다면 메모리 정보도 추가 (구현된 GameManager 상태에 맞게 수정 가능)
            if (GameManager.Instance != null)
            {
                resultText += "[메모리 상태]\n";
                resultText += "게임 매니저 정상 연결됨\n";
                // 예시: resultText += "현재 메모리: " + GameManager.Instance.memoryCount.ToString();
            }
        }
        else
        {
            resultText += "아래 버튼 중 하나를 선택해주세요.";
        }

        // 최종적으로 조합된 텍스트를 화면에 출력 ($, {} 사용 안 함)
        debugText.text = resultText;
    }

    // 선에 닿은 물체 가져오는 함수
    List<string> GetLaserHitObjects(VirusLaser laser)
    {
        List<string> objects = new List<string>();
        if (laser == null) return objects;

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
            RaycastHit2D hit = Physics2D.Linecast(points[i], points[i + 1], layer);
            if (hit.collider != null)
            {
                if (!nameList.Contains(hit.collider.name))
                {
                    nameList.Add(hit.collider.name);
                }
            }
        }
    }

    // ==========================================
    // UI 버튼에 연결할 함수들입니다.
    // ==========================================

    public void OnClickPathButton()
    {
        currentMode = 1;
    }

    public void OnClickSwitchButton()
    {
        currentMode = 2;
    }

    public void OnClickInfoButton()
    {
        currentMode = 3;
    }

    // 레벨 스킵 버튼 기능
    public void ForceSkipLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}