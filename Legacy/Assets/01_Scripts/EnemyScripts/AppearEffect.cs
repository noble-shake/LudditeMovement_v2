using UnityEngine;

public class AppearEffect : MonoBehaviour
{
    [SerializeField] private Enemy owner;
    private bool isAppear;
    private float curFlow;
    [SerializeField] SpriteRenderer disRender;
    [SerializeField] Material mat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        owner = GetComponentInParent<Enemy>();
        // mat = Instantiate(disRender.material);
        mat = disRender.material;
        curFlow = 0f;
        isAppear = true;
    }

    private void Update()
    {
        if (isAppear == false) return;

        if (mat == null) return;
        curFlow += Time.deltaTime;
        if(curFlow > 1f) curFlow = 1f;
        mat.SetFloat("_Progression", curFlow);
        

        if (curFlow == 1f)
        {
            owner.isIdleCheck = false;
            isAppear = false;
            owner.AppearOn();
            gameObject.SetActive(false);
        }
    }
}