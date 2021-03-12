using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDebugUI : MonoBehaviour
{
    public TMP_Text bulletCountText;

    private PlayerRangedAttack playerAttack;


    void Start()
    {
        playerAttack = GetComponent<PlayerRangedAttack>();
    }

    void Update()
    {
        bulletCountText.text = $"Bullet count: {playerAttack.GetBulletCount()}";
    }
}
