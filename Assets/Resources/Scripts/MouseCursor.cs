using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public static MouseCursor instance;

    public Sprite defaultCursorSprite;
    public Sprite defaultCrosshairSprite;
    
    GameObject upgradeMenu;
    Hotbar hotbar;

    public SpriteRenderer spriteRenderer;

    Weapon weaponScript;
    PlayerController playerControllerScript;
    Gadget gadgetScript;

    public Animator anim;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        anim.enabled = false;

        hotbar = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        upgradeMenu = GameObject.Find("UpgradeMenu");
        gadgetScript = Gadget.instance;

        Cursor.visible = false;

        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (hotbar.currentlyEquippedWeapon != null)
        {
            weaponScript = hotbar.currentlyEquippedWeapon.GetComponent<Weapon>();
        }

        if (weaponScript != null)
        {
            if (weaponScript.isSniper && playerControllerScript.isAiming)
            {
                anim.enabled = false;
                spriteRenderer.sprite = weaponScript.aimCursorSprite;
                transform.localScale = new Vector2(.0004f, .0004f);
            }
            else if (playerControllerScript.isAiming)
            {
                anim.enabled = false;
                spriteRenderer.sprite = defaultCrosshairSprite;
                transform.localScale = new Vector2(.0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f), .0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f));
            }
            else if (gadgetScript.teleporterActive)
            {
                anim.enabled = true;
                transform.localScale = new Vector2(.006f, .006f);
            }
            else
            {
                anim.enabled = false;
                spriteRenderer.sprite = defaultCursorSprite;
                transform.localScale = new Vector2(.11f, .11f);
            }
        }

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
	}
}
