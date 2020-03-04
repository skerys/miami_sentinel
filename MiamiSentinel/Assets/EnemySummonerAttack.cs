using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySummonerAttack : MonoBehaviour
{
    [SerializeField] private EnemyType toSummon = default;
    [SerializeField] private EnemyFactory enemyFactory = default;

    [SerializeField] private float summonMinRadius = 1.0f;
    [SerializeField] private float summonMaxRadius = 3.0f;
    [SerializeField] private float summonCooldown = 5.0f;
    [SerializeField] private float summonCount = 3;

    private float summonTimer = 0.0f;
    private IEnemyAI enemyAI = default;

    void Awake(){
        enemyAI = GetComponent<IEnemyAI>();
    }

    void OnEnable()
    {
        summonTimer = 0.0f;
        enemyAI.OnAttack += SummonMinions;
    }

    void OnDisable(){
        enemyAI.OnAttack -= SummonMinions;
    }

    void SummonMinions()
    {
        if(summonTimer >= summonCooldown){
            ExecuteSummon();
            summonTimer = 0.0f;
        }
        else{
            summonTimer += Time.deltaTime;
        }
    }

    void ExecuteSummon()
    {
        for(int i = 0; i < summonCount; ++i){
            Vector2 summonPos = Random.insideUnitCircle * summonMaxRadius;
            summonPos += summonPos.normalized * summonMinRadius; //mapping a circle to a donut :)
            summonPos += new Vector2(transform.position.x, transform.position.z);
            //TEMPORARY
            //Instantiate(toSummon, new Vector3(summonPos.x, toSummon.transform.position.y, summonPos.y), Quaternion.identity);
            var summoned = enemyFactory.Get(toSummon);
            summoned.transform.position = new Vector3(summonPos.x, summoned.transform.position.y, summonPos.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmoExtensions.DrawCircle(transform.position, summonMaxRadius, 20);
        GizmoExtensions.DrawCircle(transform.position, summonMinRadius, 20);
    }
}
