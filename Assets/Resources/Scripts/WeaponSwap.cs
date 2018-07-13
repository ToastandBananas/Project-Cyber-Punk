using UnityEngine;

public class WeaponSwap : MonoBehaviour {

    [Header("Arm sprites determined by equipped weapon:")]
    public Sprite oneHandedWeaponArms;
    public Sprite twoHandedWeaponArms;

    SpriteRenderer armSpriteRenderer;

    Weapon weaponScript;

	void Start () {
        armSpriteRenderer = GetComponent<SpriteRenderer>();

        weaponScript = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
	}
	
	void FixedUpdate ()
    {
        ArmSpriteSwap();
    }

    private void ArmSpriteSwap()
    {
        if (weaponScript.isTwoHanded)
        {
            armSpriteRenderer.sprite = twoHandedWeaponArms;
        }
        else
        {
            armSpriteRenderer.sprite = oneHandedWeaponArms;
        }
    }
}
