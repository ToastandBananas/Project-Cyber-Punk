using UnityEngine;

public class MouseCursor : MonoBehaviour {

    public Sprite defaultCursorSprite;
    public Sprite defaultCrosshairSprite;
    
    GameObject upgradeMenu;
    Hotbar hotbar;

    public SpriteRenderer spriteRenderer;

    Weapon weaponScript;
    PlayerController playerControllerScript;

	// Use this for initialization
	void Start () {
        //Cursor.visible = false;
        hotbar = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        upgradeMenu = GameObject.Find("UpgradeMenu");

        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        Cursor.visible = false;

        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (hotbar.currentlyEquippedWeapon != null)
        {
            weaponScript = hotbar.currentlyEquippedWeapon.GetComponent<Weapon>();
        }

        if (weaponScript != null)
        {
            if (weaponScript.isSniper && playerControllerScript.isAiming)
            {
                spriteRenderer.sprite = weaponScript.aimCursorSprite;
                transform.localScale = new Vector2(.0004f, .0004f);
            }
            else if (playerControllerScript.isAiming)
            {
                spriteRenderer.sprite = defaultCrosshairSprite;
                transform.localScale = new Vector2(.0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f), .0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f));
            }
            else
            {
                spriteRenderer.sprite = defaultCursorSprite;
                transform.localScale = new Vector2(.11f, .11f);
            }
        }

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
	}
}
