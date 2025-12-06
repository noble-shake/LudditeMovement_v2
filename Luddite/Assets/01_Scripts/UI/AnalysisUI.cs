using TMPro;
using UnityEngine;
using UnityEngine.UI;

// View
public class AnalysisUI : MonoBehaviour
{
    public static AnalysisUI Instance;

    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Description;
    [SerializeField] private Image Portrait;
    [SerializeField] private TMP_Text AttackDescription;
    [SerializeField] private TMP_Text MoveDescription;
    [SerializeField] private TMP_Text Comment1;
    [SerializeField] private TMP_Text Comment2;
    [SerializeField] private TMP_Text Comment3;
    [SerializeField] private AudioSource audioSource;

    public string NameValue { get { return Name.text; } set { Name.text = value; } }
    public string DescriptionValue { get { return Description.text; } set { Description.text = value; } }
    public string AttackDescriptionValue { get { return AttackDescription.text; } set { AttackDescription.text = value; } }
    public string MoveDescriptionValue { get { return MoveDescription.text; } set { MoveDescription.text = value; } }
    public string Comment1Value { get { return Comment1.text; } set { Comment1.text = value; } }
    public string Comment2Value { get { return Comment2.text; } set { Comment2.text = value; } }
    public string Comment3Value { get { return Comment3.text; } set { Comment3.text = value; } }

    public Sprite PortraitValue { get { return Portrait.sprite; } set { Portrait.sprite = value; } }
    public AudioClip AudioValue { get { return audioSource.clip; } set { audioSource.clip = value; } }

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

}