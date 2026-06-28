using UnityEngine;

public class VirusLaser : MonoBehaviour
{
    [Header("조준 설정")]
    public int maxBounce = 5;  // 최대 튕길 횟수
    public float maxLaserLength = 30f; // 최대 레이저 길이
    public LayerMask objectLayer;

    private LineRenderer lineRenderer;
    private Vector3[] LaserPathPosition;
    private Vector2 currentDirection = Vector2.up;
    private float currentAngle = 0f;

    private void Awake()
    {
        LaserPathPosition = new Vector3[maxBounce + 1]; // 미리 크기 정하기
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        GetInput();
        CalculateAndDrawBeam();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAngle += 45f;
            UpdateDirection();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAngle -= 45f;
            UpdateDirection();
        }
    }

    private void UpdateDirection()
    {
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        currentDirection = transform.up;
    }

    private void CalculateAndDrawBeam()
    {
        Vector2 currentRayPos = transform.position; // 레이저 발사 위치 설정
        Vector2 currentRayDir = currentDirection; // 레이저 각도 설정

        LaserPathPosition[0] = currentRayPos;
        int vertexCount = 1;

        for (int i = 0; i < maxBounce; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentRayPos, currentRayDir, maxLaserLength, objectLayer);

            if (hit.collider != null)
            {
                LaserPathPosition[vertexCount] = hit.point;
                vertexCount++;

                currentRayDir = Vector2.Reflect(currentRayDir, hit.normal);
            }
            else
            {
                LaserPathPosition[vertexCount] = currentRayPos + (currentRayDir * maxLaserLength); // 레이저 아무것도 안 맞으면 최대거리로 발사 (현재 각도 * 최대 길이 = 끝까지 가야할 점 위치)
                vertexCount++;
                break;
            }
        }

        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < vertexCount; i++)
        {
            lineRenderer.SetPosition(i, LaserPathPosition[i]);
        }
    }
}
