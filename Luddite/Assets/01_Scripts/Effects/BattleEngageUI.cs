using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleEngageUI : MonoBehaviour
{
    public static BattleEngageUI Instance;

    [SerializeField] public CanvasGroup FadeCanvas;

    // 3 => -3
    [SerializeField] public Image TopSideEffect;
    [SerializeField] public Material TopSideEffectMat;
    [SerializeField] public Image BottomSideEffect;
    [SerializeField] public Material BottomSideEffectMat;

    // Rect Height : 0 => 1200f
    [SerializeField] public Image CenterEffect;
    [SerializeField] public Material CenterEffectMat;
    private Vector2 CenterDeltaVector;
    private Vector2 OriginCenterDeltaVector;
    private IEnumerator EngageEffect;
    private float CurFlow;
    private WaitForSeconds CenterDelay = new WaitForSeconds(1f);
    private Vector2 TopVector;
    private Vector2 TopVectorDest;
    private Vector2 BottomVector;
    private Vector2 BottomVectorDest;

    public bool test;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    private void Start()
    {
        TopVector = new Vector2(TopSideEffect.rectTransform.anchoredPosition.x, 25f);
        TopVectorDest = new Vector2(TopSideEffect.rectTransform.anchoredPosition.x, 480f);
        BottomVector = new Vector2(BottomSideEffect.rectTransform.anchoredPosition.x, -25f);
        BottomVectorDest = new Vector2(BottomSideEffect.rectTransform.anchoredPosition.x, -480f);
        OriginCenterDeltaVector = CenterEffect.rectTransform.sizeDelta;
        CenterDeltaVector = new Vector2(CenterEffect.rectTransform.sizeDelta.x, 1200f);

        TopSideEffectMat = TopSideEffect.material;
        BottomSideEffectMat = BottomSideEffect.material;
        CenterEffectMat = CenterEffect.material;
        TopSideEffectMat.SetFloat("_Appear", 3f);
        BottomSideEffectMat.SetFloat("_Appear", 3f);
        CenterEffectMat.SetFloat("_Appear", -3f);
        TopSideEffect.material = TopSideEffectMat;
        BottomSideEffect.material = BottomSideEffectMat;
        CenterEffect.material = CenterEffectMat;

        CurFlow = 0f;
        FadeCanvas.alpha = 0f;
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            BattleEngageStart();
        }
    }

    public void BattleEngageStart()
    {
        if (EngageEffect != null) StopCoroutine(EngageEffect);
        EngageEffect = BattleEngageEffect();
        StartCoroutine(EngageEffect);
    }

    IEnumerator BattleEngageEffect()
    {
        TopSideEffect.rectTransform.anchoredPosition = TopVector;
        BottomSideEffect.rectTransform.anchoredPosition = BottomVector;
        TopSideEffectMat.SetFloat("_Appear", 3f);
        BottomSideEffectMat.SetFloat("_Appear", 3f);
        CenterEffectMat.SetFloat("_Appear", -3f);
        TopSideEffect.material = TopSideEffectMat;
        BottomSideEffect.material = BottomSideEffectMat;
        CenterEffect.rectTransform.sizeDelta = OriginCenterDeltaVector;
        CenterEffect.material = CenterEffectMat;

        CurFlow = 0f;
        FadeCanvas.alpha = 0f;

        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow > 1f) CurFlow = 1f;

            FadeCanvas.alpha = Mathf.Lerp(0f, 1f, CurFlow);
            yield return null;
        }

        BattleReadyUI.Instance.gameObject.SetActive(false);
        
        CurFlow = 0f;
        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow > 1f) CurFlow = 1f;

            TopSideEffectMat.SetFloat("_Appear", Mathf.Lerp(3f, -3f, CurFlow));
            BottomSideEffectMat.SetFloat("_Appear", Mathf.Lerp(3f, -3f, CurFlow));
            TopSideEffect.material = TopSideEffectMat;
            BottomSideEffect.material = BottomSideEffectMat;
            yield return null;
        }

        yield return CenterDelay;

        CurFlow = 0f;
        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow > 1f) CurFlow = 1f;
            TopSideEffect.rectTransform.anchoredPosition = Vector2.Lerp(TopVector, TopVectorDest, CurFlow);
            BottomSideEffect.rectTransform.anchoredPosition = Vector2.Lerp(BottomVector, BottomVectorDest, CurFlow);
            FadeCanvas.alpha = Mathf.Lerp(1f, 0f, CurFlow);
            CenterEffect.rectTransform.sizeDelta = Vector2.Lerp(OriginCenterDeltaVector, CenterDeltaVector, CurFlow);
            yield return null;
            
        }

        PlayerManager.Instance.GetPlayerOrb().gameObject.SetActive(true);
        PlayerManager.Instance.HPValue = PlayerManager.Instance.MaxHPValue;
        PlayerManager.Instance.SoulValue = 0;
        foreach (SpawnEnv env in ResourceManager.Instance.SpawnPoints)
        {
            env.SpawnCharacter();
        }

        yield return CenterDelay;
        CurFlow = 0f;
        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow > 1f) CurFlow = 1f;
            TopSideEffectMat.SetFloat("_Appear", Mathf.Lerp(-3f, 3f, CurFlow));
            BottomSideEffectMat.SetFloat("_Appear", Mathf.Lerp(-3f, 3f, CurFlow));

            TopSideEffect.material = TopSideEffectMat;
            BottomSideEffect.material = BottomSideEffectMat;
            yield return null;
        }

        CurFlow = 0f;
        while (CurFlow < 1f)
        {
            CurFlow += Time.deltaTime;
            if (CurFlow > 1f) CurFlow = 1f;
            CenterEffectMat.SetFloat("_Appear", Mathf.Lerp(-3f, 3f, CurFlow));
            CenterEffect.material = CenterEffectMat;
            yield return null;
        }

        GameManager.Instance.GameStart();

    }
}