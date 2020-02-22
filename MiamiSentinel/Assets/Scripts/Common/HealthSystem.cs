using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int hitsToKill = 3;
    [SerializeField]
    private bool canBeStunned = true;

    private int currentHealth;

    private IEnemyAI enemyAI;
    private float stunTimer;

    void Awake()
    {
        enemyAI = GetComponent<IEnemyAI>();
    }

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

    public void Stun(float duration)
    {
        if(canBeStunned)
        {
            enemyAI.DisableAI();
            stunTimer = duration;
        }
    }

    void Update()
    {
        if(stunTimer > 0.0f)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer <= 0.0f)
            {
                enemyAI.EnableAI();
                stunTimer = 0.0f;
            }
        }
    }
}
