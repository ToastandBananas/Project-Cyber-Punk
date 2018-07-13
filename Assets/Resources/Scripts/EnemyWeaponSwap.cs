using UnityEngine;

public class EnemyWeaponSwap : MonoBehaviour {

    [Header("Arm sprites determined by equipped weapon:")]
    public Sprite oneHandedWeaponArms;
    public Sprite twoHandedWeaponArms;

    SpriteRenderer armSpriteRenderer;

    EnemyWeapon enemyWeaponScript;

    void Start()
    {
        armSpriteRenderer = GetComponent<SpriteRenderer>();

        enemyWeaponScript = transform.GetComponentInChildren<EnemyWeapon>();
    }

    void FixedUpdate()
    {
        ArmSpriteSwap();
    }

    private void ArmSpriteSwap()
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
}
