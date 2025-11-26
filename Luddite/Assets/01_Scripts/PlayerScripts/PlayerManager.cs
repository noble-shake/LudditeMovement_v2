using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [SerializeField] float Speed;
    [SerializeField] PlayerOrb player;
    [SerializeField] int RecordIndex;
    private Vector3[] PositionRecords;
    private int[] ClockCheckArray;
    private bool isClockwise = false;
    private bool isCounterClockwise = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        Cursor.visible = false;

    }

    private void Start()
    {
        PositionRecords = new Vector3[RecordIndex];
        ClockCheckArray = new int[RecordIndex - 1];
    }

    private void Update()
    {
        if (player == null) return;

        MoveUpdate(); // Move, CircularDetect

    }

    private void FixedUpdate()
    {
        if (player == null) return;

        RecordUpdate();

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

    private void MoveUpdate()
    {
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

        for (int i = 0; i < RecordIndex - 1; i++)
        {
            Debug.DrawLine(PositionRecords[i], PositionRecords[i + 1], Color.red);
            Vector3 CenterVec = (PositionRecords[i] + PositionRecords[i + 1]) / 2f;
            // Vector3 CenterYVec = CenterVec + Vector3.up;
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

        float CenterDist = 0f;


        // 평균 중심점
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in PositionRecords)
        {
            center += point;
        }

        center /= PositionRecords.Length;

        (int, int, int) ClockCheck = (0, 0, 0);


        float totalAngleChange = 0f;

        for (int i = 0; i < RecordIndex - 1; i++)
        {
            Vector3 CenterVec = (PositionRecords[i] + PositionRecords[i + 1]) / 2f;

            if (i == RecordIndex - 2 || i == 0)
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

            Vector3 CurFromCenter = PositionRecords[i + 1] - center;
            Vector3 PreFromCenter = PositionRecords[i] - center;

            float angle1 = Mathf.Atan2(CurFromCenter.z, CurFromCenter.x);
            float angle2 = Mathf.Atan2(PreFromCenter.z, PreFromCenter.x);
            float angleDelta = Mathf.DeltaAngle(angle1 * Mathf.Rad2Deg, angle2 * Mathf.Rad2Deg);
            totalAngleChange += angleDelta;

            if (Vector3.Cross(CurFromCenter, PreFromCenter).y > 0)
            {
                ClockCheck.Item3++;
            }
            else if (Vector3.Cross(CurFromCenter, PreFromCenter).y < 0)
            {
                ClockCheck.Item1++;
            }
            else
            {
                ClockCheck.Item2++;
            }

        }



        int TotalClock = ClockCheck.Item1 + ClockCheck.Item2 + ClockCheck.Item3;

        isClockwise = false;
        isCounterClockwise = false;

        if (ClockCheck.Item1 == TotalClock)
        {
            isClockwise = false;
            isCounterClockwise = true;
        }
        else if (ClockCheck.Item3 == TotalClock)
        {
            isClockwise = true;
            isCounterClockwise = true;
        }

        if (isClockwise == false && isCounterClockwise == false) return;
        if (Mathf.Abs(totalAngleChange) < 320f) return;
        if (MinCneterDist < 0.3f || MaxCneterDist > 1f) return;

        //Debug.Log($"{totalAngleChange} Total Angle");
        //Debug.Log($"{CenterDist / PositionRecords.Length} Center Average Distance");
        //Debug.Log($"{MaxCneterDist} MAX {MinCneterDist} Min Center Dist");

        float radius = CenterDist / PositionRecords.Length;

        RaycastHit hitInfo;
        Physics.SphereCast(center + Vector3.up * 2f, radius, Vector3.down, out hitInfo, 3f, LayerMask.GetMask("Player"));
        {
            Debug.Log(hitInfo.collider);
            if (hitInfo.collider != null)
            {
                if (isClockwise)
                {
                    hitInfo.collider.GetComponent<Player>().OrbInteract(true);
                }
                else if (isCounterClockwise)
                {
                    hitInfo.collider.GetComponent<Player>().OrbInteract(false);
                }
            }
        }

        RaycastHit hitInfo2;
        Physics.SphereCast(center + Vector3.up * 2f, radius, Vector3.down, out hitInfo2, 3f, LayerMask.GetMask("Enemy"));
        {
            if (hitInfo2.collider != null)
            {
                if (isClockwise)
                {
                    hitInfo2.collider.GetComponent<Enemy>().OrbInteract(true);
                }
                else if (isCounterClockwise)
                {
                    hitInfo2.collider.GetComponent<Enemy>().OrbInteract(false);
                }
            }
        }


    }

    private void RecordUpdate()
    {
        for (int i = RecordIndex - 1; i > 0; i--)
        {
            PositionRecords[i] = PositionRecords[i - 1];
        }

        PositionRecords[0] = player.transform.position;
    }
}
