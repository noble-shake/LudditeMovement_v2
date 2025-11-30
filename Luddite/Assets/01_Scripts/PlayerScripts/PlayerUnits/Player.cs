using UnityEngine;


// ∞¯≈Î¡° : ¿Ãµø πÊΩƒ, »∞º∫»≠, »πµÊ, Ω∫≈»

public enum PlayerType
{ 
    Knight,
    Healer,
    Wizard,
    Ranger,
    Thief,
}

public abstract class Player : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected bool isOrbTriggered = false;
    [SerializeField] protected const float OriginMaterialProperyValue = 2.0f;
    [SerializeField] protected SpriteRenderer MainSprite;
    [SerializeField] protected SpriteRenderer OutlineSprite;
    [SerializeField] protected PlayerType playerType;

    [SerializeField] bool isDetected;
    [SerializeField] float OrbDetectRemained = 0.5f;
    [SerializeField] float OrbDetectCurFlow;

    [SerializeField] SkillReadyUI test;

    #region MatLitResponse Control (isOrbTriggered / MainSprite / OutlineSprite)
    // Orb Triggered
    public void MatLitResponseOff()
    {
        isOrbTriggered = true;
        MainSprite.material.SetFloat("_LightResponse", 0f);
        OutlineSprite.material.SetColor("_BaseColor", new Color(60f / 256f, 1f, 0f));
        animator.SetBool("Activated", true);

    }

    // Orb Exit
    public void MatLitResponseOn()
    {
        isOrbTriggered = false;
        MainSprite.material.SetFloat("_LightResponse", OriginMaterialProperyValue);
        OutlineSprite.material.SetColor("_BaseColor", new Color(27f / 256f, 115f / 256f, 0f));
        animator.SetBool("Activated", false);
        animator.Play("Idle");
    }
    #endregion

    protected virtual void Update()
    {
        if (isDetected)
        {
            Debug.Log($"Remained = {OrbDetectRemained}");
            OrbDetectRemained -= Time.deltaTime;
        }

        if (OrbDetectRemained < 0f)
        {
            OrbDetectRemained = 0.5f;
            isDetected = false;
        }

    }

    int cnt = 0;
    public void OrbInteract(bool isClockwise)
    {
        isDetected = true;
        OrbDetectRemained = 0.5f;

        test.SetProgress(0.5f);
        // isClockwise == true => Clockwise
        // isClockwise == false => CounterClockwise

    }
}