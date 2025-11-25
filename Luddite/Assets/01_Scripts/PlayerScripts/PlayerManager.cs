using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [SerializeField] float Speed;
    [SerializeField] PlayerOrb player;
    [SerializeField] int RecordIndex;
    private Vector3[] PositionRecords;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        Cursor.visible = false;

    }

    private void Start()
    {
        PositionRecords = new Vector3[RecordIndex];
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 CameraInput = GetMoveVector();
        if (Vector3.Distance(CameraInput, player.transform.position) < 0.1f)
        {
            player.transform.position = CameraInput;
        }
        else
        {
            Vector3 DifferPosition = player.transform.position + (CameraInput - player.transform.position).normalized * Time.deltaTime * Speed;
            player.transform.position = new Vector3(Mathf.Clamp(DifferPosition.x, -8f, 8f), DifferPosition.y, Mathf.Clamp(DifferPosition.z, -4.5f, 4.5f));
        }

        float MaxDist = -1f;
        float MinDist = Mathf.Infinity;
        float MaxCneterDist = -1f;
        float MinCneterDist = Mathf.Infinity;

        for (int i = 0; i < RecordIndex -1 ; i++)
        {
            Debug.DrawLine(PositionRecords[i], PositionRecords[i+1], Color.red);
            Vector3 CenterVec = (PositionRecords[i] + PositionRecords[i+1]) / 2f;
            Vector3 CenterYVec = CenterVec + Vector3.up;
            if (i == 0 || i == RecordIndex - 2)
            {
                Debug.DrawLine(CenterVec, CenterVec + Vector3.Cross(PositionRecords[i + 1] - CenterVec, Vector3.up), Color.red);
            }
            else
            {
                Debug.DrawLine(CenterVec, CenterVec + Vector3.Cross(PositionRecords[i + 1] - CenterVec, Vector3.up), Color.blue);
            }

            float ldist = Vector3.Distance(PositionRecords[i], PositionRecords[i + 1]);
            if (MaxDist < ldist) MaxDist = ldist;
            if (MinDist > ldist) MinDist = ldist;
        }

        Debug.DrawLine(PositionRecords[0], PositionRecords[RecordIndex -1], Color.red);
        Debug.Log($"{Vector3.Distance(PositionRecords[0], PositionRecords[RecordIndex - 1])} Position[0], Position[Last] Distance");


        float CenterDist =0f;

        // 평균 중심점
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in PositionRecords)
        {
            center += point;
        }
        center /= PositionRecords.Length;
        for (int i = 0; i < RecordIndex -1 ; i++)
        {
            Vector3 CenterVec = (PositionRecords[i] + PositionRecords[i + 1]) / 2f;

            if (i == RecordIndex -2 || i == 0)
            {
                Debug.DrawLine(CenterVec, center, Color.magenta);
            }
            else
            {
                Debug.DrawLine(CenterVec, center, Color.yellow);
            }

            float dist = Vector3.Distance(CenterVec, center);
            CenterDist += dist;
            if (MaxCneterDist < dist) MaxCneterDist = dist;
            if (MinCneterDist > dist) MinCneterDist = dist;
        }

        Debug.Log($"{CenterDist / PositionRecords.Length} Center Average Distance");
        Debug.Log($"{MaxCneterDist} MAX {MinCneterDist} Min Center Dist");
        Debug.Log($"{MaxDist} MAX {MinDist} Min Line Dist");

    }

    private void FixedUpdate()
    {
        if (player == null) return;

        for (int i = RecordIndex-1; i > 0; i--)
        {
            PositionRecords[i] = PositionRecords[i - 1];
        }

        PositionRecords[0] = player.transform.position;


    }


    // CameraInput은 Screen Position 값 ( = Resolution 값)
    // 실제 전장에 맞게 값이 변경 되어야 한다. x : -8 에서 8, y : -4.5에서 4.5 사이로 normalize
    private Vector3 GetMoveVector()
    {
        Vector3 mousePos = InputManager.Instance.CameraInput;
        Vector3 worldPoint = Camera.main.ScreenToViewportPoint(mousePos); // 0 ~ 1 사이로 normalized
        float x = Mathf.Clamp(remap(worldPoint.x, 0f, 1f, -8f, 8f), -8f, 8f);
        float y = Mathf.Clamp(remap(worldPoint.y, 0f, 1f, -4.5f, 4.5f), -4.5f, 4.5f);
        // Debug.Log($"norm {x}, {y}");
        return new Vector3(x, 0f, y);
    }

    private float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    public Transform GetPlayerTrs()
    {
        if (player == null) return null;
        return player.transform;
    }
}
