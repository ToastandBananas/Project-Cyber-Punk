﻿using UnityEngine;

public class EnemyWeaponSwap : MonoBehaviour {

    [Header("Arm sprites determined by equipped weapon:")]
    public Sprite oneHandedWeaponArms;
    public Sprite twoHandedWeaponArms;

    SpriteRenderer armSpriteRenderer;

    EnemyWeapon enemyWeaponScript;

    void Start()
    {
        armSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        ArmSpriteSwap();
    }

    private void ArmSpriteSwap()
    {
        if (gameObject.transform.childCount == 1 && enemyWeaponScript != null)
        {
            if (enemyWeaponScript.isTwoHanded)
            {
                armSpriteRenderer.sprite = twoHandedWeaponArms;
            }
            else
            {
                armSpriteRenderer.sprite = oneHandedWeaponArms;
            }
        }
        else if (enemyWeaponScript == null)
        {
            enemyWeaponScript = transform.GetComponentInChildren<EnemyWeapon>();
        }
    }
}
