using UnityEngine;

public class MouseCursor : MonoBehaviour {

    public Sprite defaultCursorSprite;
    public Sprite defaultCrosshairSprite;

    GameObject inventory;
    GameObject upgradeMenu;

    SpriteRenderer spriteRenderer;

    Weapon weaponScript;
    PlayerController playerControllerScript;

	// Use this for initialization
	void Start () {
        //Cursor.visible = false;
        inventory = GameObject.Find("Inventory");
        upgradeMenu = GameObject.Find("UpgradeMenu");

        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        if (GameObject.Find("Hotbar").GetComponent<Hotbar>().currentlyEquippedWeapon != null)
        {
            weaponScript = GameObject.Find("Hotbar").GetComponent<Hotbar>().currentlyEquippedWeapon.GetComponent<Weapon>();
        }
        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (inventory.activeSelf || upgradeMenu.activeSelf)
        {
            Cursor.visible = true;
            spriteRenderer.enabled = false;
        }
        else
        {
            Cursor.visible = false;
            spriteRenderer.enabled = true;
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
