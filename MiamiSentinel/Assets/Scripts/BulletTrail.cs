using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private float testLifetime;

    private LineRenderer lineRenderer;
    private float currentLifeTimer;
    public void OnEnable()
    {
        currentLifeTimer = lifetime;
        lineRenderer = GetComponent<LineRenderer>();
        Destroy(gameObject, testLifetime);
    }

    public void SetPositions(Vector3[] positions)
    {
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    public void SetTransform(Transform copyTransform)
    {
        transform.position = copyTransform.position;
        transform.rotation = copyTransform.rotation;
    }

    public void Update()
    {
        lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, currentLifeTimer / lifetime);
        lineRenderer.endColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, currentLifeTimer / lifetime);
        currentLifeTimer -= Time.deltaTime;

        if(currentLifeTimer <= 0.0f)
        {
            //Temporary (until object pooling is done)
            lineRenderer.enabled = false;
        }
    }
}
