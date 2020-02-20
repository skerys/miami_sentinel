using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int hitsToKill = 3;

    private int currentHealth;

    void OnEnable()
    {
        currentHealth = hitsToKill;
    }

    public void Damage(int hitCount)
    {
        currentHealth -= hitCount;
        
        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        //Temp solution until object pooling is added
        Destroy(gameObject);
    }
}
