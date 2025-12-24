using System.Collections.Generic;
using TMPro;
using Unity.UI.Shaders.Sample;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    bool isSoulGaugeChanged;
    [Header("HP")]
    [SerializeField] float HP;
    [SerializeField] float MaxHP = 100;
    float HPRegen = 3f;
    bool isDead;

    [Header("Soul")]
    [SerializeField] float Souls = 0;
    [SerializeField] float MaxSouls = 1000;
    private Color SoulColor;
    float SoulDecay = 3f;

    [Header("Orb")]
    [SerializeField] float Speed;
    [SerializeField] PlayerOrb player;
    [SerializeField] int RecordIndex;
    private Vector3[] PositionRecords;
    private int[] ClockCheckArray;
    private bool isClockwise = false;
    private bool isCounterClockwise = false;

    public event Action<float> OnHPEvent;
    public event Action<float> OnMaxHPEvent;
    public event Action<float> OnSPEvent;
    public event Action<float> OnMaxSPEvent;

    public float SoulValue
    {
        get { return Souls; }
        set {
            Souls = value;
            if (Souls <= 0f) Souls = 0f;
            if (Souls > 1000) Souls = 1000;

            OnSPEvent?.Invoke(Souls);
        }
    }

    public float HPValue
    {
        get { return HP; }
        set
        {
            HP = value;
            if (HP <= 0f)
            {
                // Death
                isDead = true;
                HP = 0f;
            } 
            if (HP > MaxHP) HP= MaxHP;
            OnHPEvent?.Invoke(HP);

        }
    }

    public float MaxHPValue
    {
        get { return MaxHP; }
        set
        {
            MaxHP = value;
            OnMaxHPEvent?.Invoke(value);
        }
    }

    public float MaxSoulValue
    {
        get { return MaxSouls; }
        set
        {
            MaxSouls = value;
            OnMaxSPEvent?.Invoke(value);
        }
    }


    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }

    }

    private void Start()
    {
        PositionRecords = new Vector3[RecordIndex];
        ClockCheckArray = new int[RecordIndex - 1];
    }

    public void BattleInit()
    {
        HPValue = MaxHP;
        SoulValue = 0;
    }



    private void Update()
    {
        if (GameManager.Instance.currentCondition == GameCondition.Menu) return;

        //HPGaugeCheck();
        //SoulGaugeCheck();
        HPRegen -= Time.deltaTime;
        SoulDecay -= Time.deltaTime;
        if (HPRegen <= 0f && isDead)
        {
            HPRegen = 3f;
            HPValue += 1f;
        }

        if (SoulDecay <= 0f)
        {
            SoulDecay = 3f;
            SoulValue -= 1f;
        }


        if (player == null) return;

        MoveUpdate(); // Move, CircularDetect
        PointUpdate();
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        RecordUpdate();

    }


    // CameraInput은 Screen Position 값 ( = Resolution 값)
    // 실제 전장에 맞게 값이 변경 되어야 한다. x : -8 에서 8, y : -4.5에서 4.5 사이로 normalize
    private Vector3 mousePos;
    private Vector3 worldPoint; // 0 ~ 1 사이로 normalized
    private Vector3 MoveVector = Vector3.zero;
    private Vector3 GetMoveVector()
    {
        mousePos = InputManager.Instance.CameraInput;
        worldPoint = Camera.main.ScreenToViewportPoint(mousePos); // 0 ~ 1 사이로 normalized
        float x = Mathf.Clamp(remap(worldPoint.x, 0f, 1f, -8f, 8f), -8f, 8f);
        float y = Mathf.Clamp(remap(worldPoint.y, 0f, 1f, -4.5f, 4.5f), -4.5f, 4.5f);
        // Debug.Log($"norm {x}, {y}");
        MoveVector.x = x;
        MoveVector.z = y;
        return MoveVector;
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

    public PlayerOrb GetPlayerOrb()
    {
        if (player == null) return null;
        return player;
    }


    private bool AttackInput; // Mouse Left Click
    private bool isClicked;
    private Player SelectedUnit;
    private void PointUpdate()
    {
        AttackInput = InputManager.Instance.AttackInput;
        float col = player.transform.position.x;
        float row = player.transform.position.z;


        if (AttackInput)
        {
            if (isClicked) return;
            RaycastHit[] hit = Physics.RaycastAll(player.transform.position + Vector3.up, Vector3.down, 2f);
            foreach (RaycastHit target in hit)
            {
                if (target.collider.GetComponent<OrbTriggerZone>() == null) continue;
                Player tempSelect = target.collider.GetComponentInParent<Player>();
                if (tempSelect.ActivatedCheck == false) continue;

                SelectedUnit = tempSelect;
                isClicked = true;

                SelectedUnit.OrbPointInteract(true);

                break; // Only One Character Select
            }


        }
        else
        {

            if (isClicked && SelectedUnit != null)
            {
                Debug.Log($"Mouse Up Point : {player.transform.position}");
                SelectedUnit.MovePoint(player.transform.position);
                SelectedUnit.OrbPointInteract(false);
                SelectedUnit = null;
            } 
            isClicked = false;
        }


    }

    private void MoveUpdate()
    {
        Vector3 CameraInput = GetMoveVector(); // Camera Move Pos.
        if (Vector3.Distance(CameraInput, player.transform.position) > 0.01f)
        {
            Vector3 DifferPosition = CameraInput;
            DifferPosition = Vector3.Lerp(player.transform.position, DifferPosition, Time.deltaTime * Speed);
            player.transform.position = new Vector3(Mathf.Clamp(DifferPosition.x, -8f, 8f), DifferPosition.y, Mathf.Clamp(DifferPosition.z, -4.5f, 4.5f));
        }

        

        float MaxDist = -1f;
        float MinDist = Mathf.Infinity;
        float MaxCneterDist = -1f;
        float MinCneterDist = Mathf.Infinity;

        for (int i = 0; i < RecordIndex - 1; i++)
        {
            Vector3 CenterVec = (PositionRecords[i] + PositionRecords[i + 1]) / 2f;

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

        if (ClockCheck.Item1 > ClockCheck.Item2 + ClockCheck.Item3)
        {
            isClockwise = false;
            isCounterClockwise = true;
        }
        else if (ClockCheck.Item3 > ClockCheck.Item1 + ClockCheck.Item2)
        {
            isClockwise = true;
            isCounterClockwise = true;
        }

        float radius = CenterDist / PositionRecords.Length;



        //Debug.Log($"{ClockCheck.Item1},{ClockCheck.Item2},{ClockCheck.Item3}");
        if (isClockwise == false && isCounterClockwise == false) return;
        //Debug.Log($"{totalAngleChange} Total Angle");
        //Debug.Log($"{CenterDist / PositionRecords.Length} Center Average Distance");
        //Debug.Log($"{MaxCneterDist} MAX {MinCneterDist} Min Center Dist");
        if (Mathf.Abs(totalAngleChange) < 320f) return;
        if (MinCneterDist < 0.3f || MaxCneterDist > 1f) return;
        if (radius > 0.8f || radius < 0.3f) return;


        OrbInteractIngame(center, radius);
        
    }

    private void OrbInteractIngame(Vector3 center, float radius)
    {
        // 우선, 플레이어를 먼저 체크하고. 없으면 다른 상호작용을 진행한다.
        RaycastHit[] CasterHits = Physics.SphereCastAll(center + Vector3.up * 2f, radius, Vector3.down, 3f);
        foreach (RaycastHit hitInfo in CasterHits)
        {
            if (hitInfo.collider == null) continue;
            if (LayerMask.LayerToName(hitInfo.collider.gameObject.layer).Equals("Default")) continue;

            string LayerName = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);
            if (LayerName.Equals("Player"))
            {
                if (isClockwise)
                {
                    hitInfo.collider.GetComponent<Player>().OrbInteract(true);
                }
                else if (isCounterClockwise)
                {
                    hitInfo.collider.GetComponent<Player>().OrbInteract(false);
                }

                return;
            }


        }

        foreach (RaycastHit hitInfo in CasterHits)
        {
            if (hitInfo.collider == null) continue;
            if (LayerMask.LayerToName(hitInfo.collider.gameObject.layer).Equals("Default")) continue;

            Debug.Log(hitInfo.collider);

            string LayerName = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);

            // 플레이어는 발생 할 일이 없겠지만 혹시나...
            if (LayerName.Equals("Player"))
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
            else if (LayerName.Equals("Enemy"))
            {
                hitInfo.collider.GetComponent<Enemy>().OrbInteracted();
            }
            else if (LayerName.Equals("Props"))
            {
                hitInfo.collider.GetComponent<Props>().OrbInteracted();
            }
            else if (LayerName.Equals("Environment"))
            {
                hitInfo.collider.GetComponent<Environments>().OrbInteracted();
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
