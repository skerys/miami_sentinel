using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    private LineRenderer lineRenderer;
    private float currentLifeTimer;
    public void OnEnable()
    {
        currentLifeTimer = lifetime;
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetPositions(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] positions = { startPos, endPos };
        lineRenderer.SetPositions(positions);
    }

    public void Update()
    {
        lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, currentLifeTimer / lifetime);
        lineRenderer.endColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, currentLifeTimer / lifetime);
        currentLifeTimer -= Time.deltaTime;

        if(currentLifeTimer <= 0.0f)
        {
            //Temporary (until object pooling is done)
            Destroy(gameObject);
        }
    }
}
