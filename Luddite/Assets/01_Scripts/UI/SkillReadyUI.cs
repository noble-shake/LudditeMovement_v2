using UnityEngine;
using UnityEngine.UI;

public class SkillReadyUI : MonoBehaviour
{
    [SerializeField] Image canvasImage;
    [SerializeField] Material material;
    [SerializeField] float Progression;

    private void Start()
    {
        material = canvasImage.material;
        Progression = 0f;
    }

    public void SetProgress(float value)
    {
        Progression = value;
        material.SetFloat("_Progression", Progression);
        canvasImage.material = material;
    }

    private void Update()
    {
        //material.SetFloat("_Progression", Progression);
        //canvasImage.material = material;
    }
}
