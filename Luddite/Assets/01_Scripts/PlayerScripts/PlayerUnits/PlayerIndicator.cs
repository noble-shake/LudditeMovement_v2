using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private void OnEnable()
    {
        if(lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.enabled = false;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, PlayerManager.Instance.GetPlayerTrs().position);
            
        }
    }
}