using UnityEngine;

public class ArmSwap : MonoBehaviour {

    [Header("Arm sprites determined by equipped weapon:")]
    public Sprite oneHandedWeaponArms;
    public Sprite twoHandedWeaponArms;

    SpriteRenderer armSpriteRenderer;

    Weapon weaponScript;

	void Start () {
        armSpriteRenderer = GetComponent<SpriteRenderer>();

        if (GameObject.Find("Hotbar").GetComponent<Hotbar>().currentlyEquippedWeapon != null)
        {
            weaponScript = GameObject.Find("Hotbar").GetComponent<Hotbar>().currentlyEquippedWeapon.GetComponent<Weapon>();
        }
    }
	
	void FixedUpdate ()
    {
        ArmSpriteSwap();
    }

    private void ArmSpriteSwap()
    {
        if (weaponScript != null)
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
}
