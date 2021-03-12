using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private float destroyLifetime;

    [SerializeField]
    private LineRenderer lineRendererInitial;
    [SerializeField]
    private LineRenderer lineRendererAfterBounce;

    private float currentLifeTimer;
    public void OnEnable()
    {
        currentLifeTimer = lifetime;
        Destroy(gameObject, destroyLifetime);
    }

    public void SetPositionsInitial(Vector3[] positions)
    {
        lineRendererInitial.positionCount = positions.Length;
        lineRendererInitial.SetPositions(positions);
    }

    public void SetPositionsAfterBounce(Vector3[] positions)
    {
        lineRendererAfterBounce.positionCount = positions.Length;
        lineRendererAfterBounce.SetPositions(positions);
    }

    public void SetTransform(Transform copyTransform)
    {
        transform.position = copyTransform.position;
        transform.rotation = copyTransform.rotation;
    }

    public void Update()
    {
        lineRendererInitial.startColor = new Color(lineRendererInitial.startColor.r, lineRendererInitial.startColor.g, lineRendererInitial.startColor.b, currentLifeTimer / lifetime);
        lineRendererInitial.endColor = new Color(lineRendererInitial.startColor.r, lineRendererInitial.startColor.g, lineRendererInitial.startColor.b, currentLifeTimer / lifetime);

        lineRendererAfterBounce.startColor = new Color(lineRendererAfterBounce.startColor.r, lineRendererAfterBounce.startColor.g, lineRendererAfterBounce.startColor.b, currentLifeTimer / lifetime);
        lineRendererAfterBounce.endColor = new Color(lineRendererAfterBounce.startColor.r, lineRendererAfterBounce.startColor.g, lineRendererAfterBounce.startColor.b, currentLifeTimer / lifetime);

        currentLifeTimer -= Time.deltaTime;

        if(currentLifeTimer <= 0.0f)
        {
            //Temporary (until object pooling is done)
            lineRendererInitial.enabled = false;
            lineRendererAfterBounce.enabled = false;
        }
    }
}
