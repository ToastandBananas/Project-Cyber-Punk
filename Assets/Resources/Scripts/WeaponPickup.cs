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
    Weapon equippedWeaponScript;
    Weapon secondaryWeaponScript;
    GameObject secondaryWeapon;
    Player player;

    AudioManager audioManager;

    GameObject weaponPrefab;
    public int weaponID;

    // These are set when either of the two drop weapon methods are called (there's one in the Enemy Script and one in the Hotbar script (for when the player drops a weapon))
    public string ammoType;
    public int currentAmmoAmount;
    public int clipSize;
    public float damage;
    public float fireRate;
    public float durability;
    public bool isSilenced;
    public bool hasIncreasedClipSize;
    public float clipSizeMultiplier;
    public float inaccuracyFactor;
    public bool hasAlteredDurability;
    public float durabilityMultiplier;

    int roomInPlayersEquippedWeaponClip;
    int roomInPlayersSecondaryWeaponClip;

    float delay = 0.35f;


    void Start()
    {
        player = Player.instance;

        weaponSlot1 = GameObject.Find("WeaponSlot1");
        weaponSlot2 = GameObject.Find("WeaponSlot2");
        
        hotbarScript = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();

        playerArm = GameObject.Find("Arm");
        equippedWeaponScript = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    void FixedUpdate()
    {
        equippedWeaponScript = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        secondaryWeapon = hotbarScript.secondaryWeapon;
        if (secondaryWeapon != null)
        {
            secondaryWeaponScript = secondaryWeapon.GetComponent<Weapon>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.itemToPickup == null)
            {
                player.itemToPickup = gameObject;
            }

            GetComponent<SpriteRenderer>().material = highlightMaterial;

            PickUpAmmo();

            player.timeSinceQPressed += Time.deltaTime;
            if (player.timeSinceQPressed > delay)
            {
                if (Input.GetKeyDown(KeyCode.Q) && player.itemToPickup == gameObject)
                {
                    if (equippedWeaponScript.name != this.name && (secondaryWeapon == null || secondaryWeapon.name != this.name)) // The player may not have two of the same weapon
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
                            hotbarScript.secondaryWeapon.GetComponent<Weapon>().durabilityMultiplier = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().durabilityMultiplier;
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
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().inaccuracyFactor = inaccuracyFactor;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().finalAccuracyFactor = player.playerStats.inaccuracyFactor + inaccuracyFactor;
                                if (hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().finalAccuracyFactor < 0)
                                    hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().finalAccuracyFactor = 0;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().hasAlteredDurability = hasAlteredDurability;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().durabilityMultiplier = durabilityMultiplier;
                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().durability = durability;

                                hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().DetermineSoundName();
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

                        player.timeSinceQPressed = 0f;
                        player.itemToPickup = null;
                        audioManager.PlaySound("Weapon Pickup");
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                    else if (equippedWeaponScript.name == this.name)
                    {
                        GameObject weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + equippedWeaponScript.name + " Item Drop") as GameObject;
                        hotbarScript.DropWeapon(weaponToDrop, equippedWeaponScript.currentAmmoAmount, equippedWeaponScript.clipSize, equippedWeaponScript.ammoType, equippedWeaponScript.damage,
                                                equippedWeaponScript.fireRate, equippedWeaponScript.isSilenced, equippedWeaponScript.hasIncreasedClipSize, equippedWeaponScript.clipSizeMultiplier,
                                                equippedWeaponScript.inaccuracyFactor, equippedWeaponScript.durability, equippedWeaponScript.hasAlteredDurability, equippedWeaponScript.durabilityMultiplier);

                        equippedWeaponScript.clipSize = clipSize;
                        equippedWeaponScript.currentAmmoAmount = currentAmmoAmount;
                        equippedWeaponScript.damage = damage;
                        equippedWeaponScript.fireRate = fireRate;
                        equippedWeaponScript.isSilenced = isSilenced;
                        equippedWeaponScript.hasIncreasedClipSize = hasIncreasedClipSize;
                        equippedWeaponScript.clipSizeMultiplier = clipSizeMultiplier;
                        equippedWeaponScript.inaccuracyFactor = inaccuracyFactor;
                        equippedWeaponScript.finalAccuracyFactor = player.playerStats.inaccuracyFactor + inaccuracyFactor;
                        if (equippedWeaponScript.finalAccuracyFactor < 0)
                            equippedWeaponScript.finalAccuracyFactor = 0;
                        equippedWeaponScript.hasAlteredDurability = hasAlteredDurability;
                        equippedWeaponScript.durabilityMultiplier = durabilityMultiplier;
                        equippedWeaponScript.durability = durability;

                        player.timeSinceQPressed = 0f;
                        player.itemToPickup = null;
                        audioManager.PlaySound("Weapon Pickup");
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                    else if (secondaryWeapon != null && secondaryWeapon.name == this.name)
                    {
                        GameObject weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + secondaryWeapon.name + " Item Drop") as GameObject;
                        hotbarScript.DropWeapon(weaponToDrop, secondaryWeaponScript.currentAmmoAmount, secondaryWeaponScript.clipSize, secondaryWeaponScript.ammoType, secondaryWeaponScript.damage,
                                                secondaryWeaponScript.fireRate, secondaryWeaponScript.isSilenced, secondaryWeaponScript.hasIncreasedClipSize, secondaryWeaponScript.clipSizeMultiplier,
                                                secondaryWeaponScript.inaccuracyFactor, secondaryWeaponScript.durability, secondaryWeaponScript.hasAlteredDurability, secondaryWeaponScript.durabilityMultiplier);

                        secondaryWeaponScript.clipSize = clipSize;
                        secondaryWeaponScript.currentAmmoAmount = currentAmmoAmount;
                        secondaryWeaponScript.damage = damage;
                        secondaryWeaponScript.fireRate = fireRate;
                        secondaryWeaponScript.isSilenced = isSilenced;
                        secondaryWeaponScript.hasIncreasedClipSize = hasIncreasedClipSize;
                        secondaryWeaponScript.clipSizeMultiplier = clipSizeMultiplier;
                        secondaryWeaponScript.inaccuracyFactor = inaccuracyFactor;
                        secondaryWeaponScript.finalAccuracyFactor = player.playerStats.inaccuracyFactor + inaccuracyFactor;
                        if (secondaryWeaponScript.finalAccuracyFactor < 0)
                            secondaryWeaponScript.finalAccuracyFactor = 0;
                        secondaryWeaponScript.hasAlteredDurability = hasAlteredDurability;
                        secondaryWeaponScript.durabilityMultiplier = durabilityMultiplier;
                        secondaryWeaponScript.durability = durability;

                        player.timeSinceQPressed = 0f;
                        player.itemToPickup = null;
                        audioManager.PlaySound("Weapon Pickup");
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                }
            }
        }
    }

    private void PickUpAmmo()
    {
        if (equippedWeaponScript.currentAmmoAmount < equippedWeaponScript.clipSize && equippedWeaponScript.ammoType == ammoType)
        {
            roomInPlayersEquippedWeaponClip = equippedWeaponScript.clipSize - equippedWeaponScript.currentAmmoAmount;

            while (this.currentAmmoAmount > 0 && roomInPlayersEquippedWeaponClip > 0)
            {
                this.currentAmmoAmount--;
                equippedWeaponScript.IncreaseAmmo(1);
                roomInPlayersEquippedWeaponClip--;
            }
        }
        else if (hotbarScript.secondaryWeaponAmmoAmount < hotbarScript.secondaryWeaponClipSize && hotbarScript.secondaryWeaponAmmoType == ammoType)
        {
            roomInPlayersSecondaryWeaponClip = secondaryWeapon.GetComponent<Weapon>().clipSize - secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount;

            while (this.currentAmmoAmount > 0 && roomInPlayersSecondaryWeaponClip > 0)
            {
                this.currentAmmoAmount--;
                secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount++;
                roomInPlayersSecondaryWeaponClip--;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.itemToPickup == gameObject)
            {
                player.itemToPickup = null;
            }
            GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }
}
