using UnityEngine;
using UnityEngine.UI;

public class WeaponPickup : MonoBehaviour
{
    GameObject weaponSlot1;
    GameObject weaponSlot2;

    public Material highlightMaterial;
    public Material defaultMaterial;

    Hotbar hotbarScript;
    ItemDatabase itemDatabase;

    Item item;

    GameObject playerArm;
    Weapon playerEquippedWeapon;
    GameObject playerSecondaryWeapon;
    Player player;

    GameObject weaponPrefab;
    public int weaponID;
    
    BoxCollider2D thisBoxCollider;
    CapsuleCollider2D playerCapsuleCollider;
    GameObject[] enemies;

    // These are set when either of the two drop weapon methods are called (there's one in the Enemy Script and one in the Hotbar script)
    public string ammoType;
    public int currentAmmoAmount;
    public int clipSize;
    public float damage;
    public float fireRate;
    public bool isSilenced;
    public bool hasIncreasedClipSize;
    public float clipSizeMultiplier;

    int roomInPlayersEquippedWeaponClip;
    int roomInPlayersSecondaryWeaponClip;

    float timeSinceQPressed;
    float delay = 0.25f;

    void Start()
    {
        player = Player.instance;
        thisBoxCollider = GetComponent<BoxCollider2D>();
        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreCollision(playerCapsuleCollider, thisBoxCollider);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(enemy.GetComponent<CapsuleCollider2D>(), thisBoxCollider);
        }

        weaponSlot1 = GameObject.Find("WeaponSlot1");
        weaponSlot2 = GameObject.Find("WeaponSlot2");
        
        hotbarScript = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();

        playerArm = GameObject.Find("Arm");
        playerEquippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerSecondaryWeapon = hotbarScript.secondaryWeapon;
    }

    void Update()
    {
        playerEquippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerSecondaryWeapon = hotbarScript.secondaryWeapon;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "WeaponDrop")
        {
            print("yep");
            print(gameObject.transform.parent.name + " " + collision.transform.parent.name); // To do: figure out why this isn't moving them
            gameObject.transform.parent.transform.position += new Vector3(1f, 0);
            collision.transform.parent.transform.position += new Vector3(-1f, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = highlightMaterial;

            if (playerEquippedWeapon.currentAmmoAmount < playerEquippedWeapon.clipSize && playerEquippedWeapon.ammoType == ammoType)
            {
                roomInPlayersEquippedWeaponClip = playerEquippedWeapon.clipSize - playerEquippedWeapon.currentAmmoAmount;

                while (this.currentAmmoAmount > 0 && roomInPlayersEquippedWeaponClip > 0)
                {
                    this.currentAmmoAmount--;
                    playerEquippedWeapon.IncreaseAmmo(1);
                    roomInPlayersEquippedWeaponClip--;
                }
            }
            else if (hotbarScript.secondaryWeaponAmmoAmount < hotbarScript.secondaryWeaponClipSize && hotbarScript.secondaryWeaponAmmoType == ammoType)
            {
                roomInPlayersSecondaryWeaponClip = playerSecondaryWeapon.GetComponent<Weapon>().clipSize - playerSecondaryWeapon.GetComponent<Weapon>().currentAmmoAmount;

                while (this.currentAmmoAmount > 0 && roomInPlayersSecondaryWeaponClip > 0)
                {
                    this.currentAmmoAmount--;
                    playerSecondaryWeapon.GetComponent<Weapon>().currentAmmoAmount++;
                    roomInPlayersSecondaryWeaponClip--;
                }
            }

            timeSinceQPressed += Time.deltaTime;
            if (timeSinceQPressed > delay)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    print("Q pressed");
                    if (playerEquippedWeapon.name != this.name && (playerSecondaryWeapon == null || playerSecondaryWeapon.name != this.name)) // The player may not have two of the same weapon
                    {
                        if (hotbarScript.weaponSlot1.GetComponent<Slot>().isEmpty || hotbarScript.weaponSlot2.GetComponent<Slot>().isEmpty)
                        {
                            hotbarScript.secondaryWeaponAmmoAmount = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                            hotbarScript.secondaryWeaponClipSize = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                            hotbarScript.secondaryWeaponAmmoType = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;

                            hotbarScript.secondaryWeapon = hotbarScript.currentlyEquippedWeapon;
                            hotbarScript.secondaryWeapon.GetComponent<Weapon>().isSilenced = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().isSilenced;
                            hotbarScript.secondaryWeapon.GetComponent<Weapon>().hasIncreasedClipSize = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().hasIncreasedClipSize;
                            hotbarScript.secondaryWeapon.GetComponent<Weapon>().clipSizeMultiplier = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSizeMultiplier;
                        }

                        hotbarScript.EquipWeapon(weaponID);
                        hotbarScript.AddItemToInventory(weaponID);

                        foreach (GameObject weaponObject in hotbarScript.weaponObjects)
                        {
                            if (weaponObject.name == this.name)
                            {
                                weaponObject.GetComponent<WeaponPerks>().enabled = false; // We disable this script so that it doesn't run every time you pick up a new weapon
                                hotbarScript.currentlyEquippedWeapon = weaponObject;

                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize = clipSize;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount = currentAmmoAmount;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().damage = damage;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().fireRate = fireRate;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().isSilenced = isSilenced;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().hasIncreasedClipSize = hasIncreasedClipSize;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSizeMultiplier = clipSizeMultiplier;
                            }
                        }

                        if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot1.transform.GetChild(2).name)
                        {
                            hotbarScript.currentlyEquippedWeaponSlot = 1;
                            GameObject.Find("AmmoText1").GetComponent<Text>().text = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                            hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.equippedColor;
                            hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.unequippedColor;
                        }
                        else if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot2.transform.GetChild(2).name)
                        {
                            hotbarScript.currentlyEquippedWeaponSlot = 2;
                            GameObject.Find("AmmoText2").GetComponent<Text>().text = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                            hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.unequippedColor;
                            hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.equippedColor;
                        }

                        timeSinceQPressed = 0f;
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }
}
