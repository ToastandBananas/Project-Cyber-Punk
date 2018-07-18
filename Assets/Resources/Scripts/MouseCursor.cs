using UnityEngine;

public class MouseCursor : MonoBehaviour {

    public Sprite defaultCursorSprite;
    public Sprite defaultCrosshairSprite;

    Weapon weaponScript;
    PlayerController playerControllerScript;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;

        weaponScript = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (weaponScript.isSniper && playerControllerScript.isAiming)
        {
            transform.GetComponent<SpriteRenderer>().sprite = weaponScript.aimCursorSprite;
            transform.localScale = new Vector2(.0004f, .0004f);
        }
        else if (playerControllerScript.isAiming)
        {
            transform.GetComponent<SpriteRenderer>().sprite = defaultCrosshairSprite;
            transform.localScale = new Vector2(.0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f), .0001f + (Vector2.Distance(transform.position, playerControllerScript.transform.position) * .00002f));
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().sprite = defaultCursorSprite;
            transform.localScale = new Vector2(.11f, .11f);
        }

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
	}
}
