using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    private static HitStopManager instance;
    public static HitStopManager Instance{get {return instance;}}


    private float hitStopTimer = 0.0f;
    private float fixedDeltaTimeCopy;
    private bool stopped;

    void Awake()
    {
        fixedDeltaTimeCopy = Time.fixedDeltaTime;

        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if(hitStopTimer > 0.0f)
        {
            if(!stopped)
            {
                stopped = true;
                Time.timeScale = 0.0f;
                Time.fixedDeltaTime = 0.0f;
            }
            hitStopTimer -= Time.unscaledDeltaTime;
            if(hitStopTimer < 0.0f)
            {
                stopped = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = fixedDeltaTimeCopy;
            }
        }
    }

    public void HitStop(float time)
    {
        hitStopTimer = time;
    }
}
