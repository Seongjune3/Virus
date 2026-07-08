using UnityEngine;
using System.Collections.Generic;

public class VirusLaser : MonoBehaviour
{
    public int maxBounce = 5;
    public float maxLaserLength = 30f;
    public LayerMask objectLayer;

    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;

    List<Vector3> path1 = new List<Vector3>();
    List<Vector3> path2 = new List<Vector3>();

    Vector2 currentDirection = Vector2.up;
    float currentAngle = 0f;

    void Update()
    {
        GetInput();
        DrawBeam();
    }

    void GetInput()
    {
        if (GameManager.Instance.IsCleared) return;

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

    void UpdateDirection()
    {
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        currentDirection = transform.up;
    }

    void DrawBeam()
    {
        path1.Clear();
        path2.Clear();

        Vector2 rayPos = transform.position;
        Vector2 rayDir = currentDirection;

        path1.Add(rayPos);

        bool wentPortal = false;
        float length = maxLaserLength;

        for (int i = 0; i < maxBounce; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rayPos, rayDir, length, objectLayer);

            if (hit.collider == null)
            {
                Vector2 endPoint = rayPos + rayDir * length;
                if (!wentPortal) path1.Add(endPoint);
                else path2.Add(endPoint);
                break;
            }

            if (!wentPortal) path1.Add(hit.point);
            else path2.Add(hit.point);

            length -= hit.distance;

            if (length <= 0f) break;

            if (hit.collider.CompareTag("Mirror"))
            {
                rayDir = Vector2.Reflect(rayDir, hit.normal);
                rayPos = hit.point + hit.normal * 0.01f;
            }
            else if (hit.collider.CompareTag("Portal") && !wentPortal)
            {
                Portal portal = hit.collider.GetComponent<Portal>();
                rayPos = (Vector2)portal.targetPortal.transform.position;
                wentPortal = true;
                path2.Add(rayPos);
            }
            else if (hit.collider.CompareTag("CDrive"))
            {
                GameManager.Instance.StageClear();
                break;
            }
            else if (hit.collider.CompareTag("Memory"))
            {
                Star memory = hit.collider.GetComponent<Star>();

                GameManager.Instance.AddMemory();

                memory.Boom();

                break;
                
            }
            else
            {
                break;
            }
        }

        DrawLine(lineRenderer1, path1);
        DrawLine(lineRenderer2, path2);
    }

    void DrawLine(LineRenderer lr, List<Vector3> points)
    {
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i]);
        }
    }
}