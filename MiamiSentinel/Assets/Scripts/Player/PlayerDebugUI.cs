using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDebugUI : MonoBehaviour
{
    public TMP_Text bulletCountText;
    public Slider reloadSlider;

    private PlayerAttack playerAttack;
    private float reloadTime;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        reloadTime = playerAttack.GetReloadTime();
    }

    void Update()
    {
        bulletCountText.text = $"Bullet count: {playerAttack.GetBulletCount()}";
        reloadSlider.value = playerAttack.GetReloadProgress() / reloadTime;
    }
}
