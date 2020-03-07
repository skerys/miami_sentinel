using System;
using System.Collections.Generic;
using UnityEngine;

public class KillAfterParticles : MonoBehaviour
{
    ParticleSystem[] childParticles;
    ParticleSystem thisParticles;

    void Start()
    {
        childParticles = GetComponentsInChildren<ParticleSystem>();
        thisParticles = GetComponent<ParticleSystem>();

        float maxTime = 0.0f;
        foreach(var particleSystem in childParticles)
        {
            maxTime = Mathf.Max(maxTime, particleSystem.main.duration);
        }
        
        if(thisParticles)
        {
            maxTime = Mathf.Max(maxTime, thisParticles.main.duration);
        }

        Destroy(gameObject, maxTime);
    }

   
}
