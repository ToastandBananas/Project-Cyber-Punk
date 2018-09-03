using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gadget : MonoBehaviour
{
    public static Gadget instance;

    [Header("Gadget Slots")]
    public GameObject gadgetSlot1;
    public GameObject gadgetSlot2;
    Slot gadgetSlot1Script;
    Slot gadgetSlot2Script;
    GameObject gadget1;
    GameObject gadget2;

    [Header("Gadget Charges")]
    public int gadget1MaxCharge = 0;
    public int gadget2MaxCharge = 0;
    public int gadget1CurrentCharge = 0;
    public int gadget2CurrentCharge = 0;

    [Header("Gadget Bools")]
    public bool teleporterActive = false;
    public bool hackerActive = false;
    public bool scannerActive = false;
    public bool enemySelected = false;

    [Header("Gadget Other")]
    public Transform enemyBeingScanned;

    GameObject[] enemies;
    Transform targetEnemy;
    [HideInInspector] public GameObject scannerInfoTooltip;

    ItemDatabase itemDatabase;
    GameMaster gameMaster;
    MouseCursor cursor;
    Player player;
    PlayerController playerControllerScript;
    AudioManager audioManager;

    Item teleportationDevice;
    Item AIHackingDevice;
    Item AIScanningDevice;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start ()
    {
        itemDatabase = ItemDatabase.instance;
        gameMaster = GameMaster.gm;
        cursor = MouseCursor.instance;
        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();
        audioManager = AudioManager.instance;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        scannerInfoTooltip = GameObject.Find("ScannerInfo");
        scannerInfoTooltip.transform.position -= new Vector3(0, (Screen.height / 10) * 8);
        scannerInfoTooltip.SetActive(false);

        gadgetSlot1Script = gadgetSlot1.GetComponent<Slot>();
        gadgetSlot2Script = gadgetSlot2.GetComponent<Slot>();

        teleportationDevice = itemDatabase.FetchItemByID(34);
        AIHackingDevice = itemDatabase.FetchItemByID(35);
        AIScanningDevice = itemDatabase.FetchItemByID(36);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if ((gadget1 == null && gadgetSlot1Script.isEmpty == false) || (gadget2 == null && gadgetSlot2Script.isEmpty == false))
            SetGadgetCharges();

        ActivateGadgetItem();
        StartCoroutine(UseTeleporter());
        UseHacker();
        UseScanner();
        DisplayCharges();
    }

    private void SetGadgetCharges()
    {
        if (gadget1 == null)
            if (gadgetSlot1Script.isEmpty == false)
                gadget1 = gadgetSlot1.transform.GetChild(2).gameObject;

        if (gadget2 == null)
            if (gadgetSlot2Script.isEmpty == false)
                gadget2 = gadgetSlot2.transform.GetChild(2).gameObject;

        if (gadget1 != null)
        {
            if (gadget1.name == teleportationDevice.ItemName)
            {
                gadget1MaxCharge = gameMaster.teleportationDeviceMaxCharge;
                gadget1CurrentCharge = gadget1MaxCharge;
            }
            else if (gadget1.name == AIHackingDevice.ItemName)
            {
                gadget1MaxCharge = gameMaster.AIHackingDeviceMaxCharge;
                gadget1CurrentCharge = gadget1MaxCharge;
            }
            else if (gadget1.name == AIScanningDevice.ItemName)
            {
                gadget1MaxCharge = gameMaster.AIScanningDeviceMaxCharge;
                gadget1CurrentCharge = gadget1MaxCharge;
            }
        }

        if (gadget2 != null)
        {
            if (gadget2.name == teleportationDevice.ItemName)
            {
                gadget2MaxCharge = gameMaster.teleportationDeviceMaxCharge;
                gadget2CurrentCharge = gadget2MaxCharge;
            }
            else if (gadget2.name == AIHackingDevice.ItemName)
            {
                gadget2MaxCharge = gameMaster.AIHackingDeviceMaxCharge;
                gadget2CurrentCharge = gadget2MaxCharge;
            }
            else if (gadget2.name == AIScanningDevice.ItemName)
            {
                gadget2MaxCharge = gameMaster.AIScanningDeviceMaxCharge;
                gadget2CurrentCharge = gadget2MaxCharge;
            }
        }
    }

    private void UseScanner()
    {
        if (scannerActive)
        {
            if (gadget1.name == AIScanningDevice.ItemName || gadget2.name == AIScanningDevice.ItemName)
            {
                enemySelected = false;
                if (targetEnemy != null)
                    targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().defaultMaterial;
                targetEnemy = null;

                foreach (GameObject enemy in enemies)
                {
                    if (Vector2.Distance(enemy.transform.position, cursor.transform.position) < 0.25f
                        && Vector2.Distance(player.transform.position, cursor.transform.position) <= gameMaster.AIScanningDeviceRange
                        && targetEnemy == null)
                    {
                        enemySelected = true;
                        targetEnemy = enemy.transform;
                        targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().highlightMaterial;
                    }
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    if (enemySelected && targetEnemy.GetComponent<EnemyMovement>().isScanned == false)
                    {
                        print(targetEnemy.name + " scanned");
                        targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().defaultMaterial;
                        audioManager.PlaySound("AI Hack"); // TO DO: Get AI Scan sound

                        targetEnemy.GetComponent<EnemyMovement>().isScanned = true;

                        if (gadget1.name == AIScanningDevice.ItemName)
                            gadget1CurrentCharge--;
                        else if (gadget2.name == AIScanningDevice.ItemName)
                            gadget2CurrentCharge--;

                        scannerActive = false;
                        cursor.anim.SetBool("scannerActive", scannerActive);
                    }
                    else
                        audioManager.PlaySound("Gadget Empty");
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    scannerActive = false;
                    cursor.anim.SetBool("scannerActive", scannerActive);
                    audioManager.PlaySound("Gadget Empty");
                }
            }
        }
    }

    private void UseHacker()
    {
        if (hackerActive)
        {
            if (gadget1.name == AIHackingDevice.ItemName || gadget2.name == AIHackingDevice.ItemName)
            {
                enemySelected = false;
                if (targetEnemy != null)
                    targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().defaultMaterial;
                targetEnemy = null;

                foreach (GameObject enemy in enemies)
                {
                    if (Vector2.Distance(enemy.transform.position, cursor.transform.position) < 0.25f 
                        && Vector2.Distance(player.transform.position, cursor.transform.position) <= gameMaster.AIHackingDeviceRange
                        && targetEnemy == null && enemy.GetComponent<Enemy>().isHackable)
                    {
                        enemySelected = true;
                        targetEnemy = enemy.transform;
                        targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().highlightMaterial;
                    }
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    if (enemySelected)
                    {
                        // print(targetEnemy.name + " hacked");
                        targetEnemy.GetComponent<SpriteRenderer>().material = targetEnemy.GetComponent<Enemy>().defaultMaterial;
                        audioManager.PlaySound("AI Hack");

                        targetEnemy.GetComponent<EnemyMovement>().isHacked = true;
                        targetEnemy.GetComponent<EnemyMovement>().currentState = targetEnemy.GetComponent<EnemyMovement>().defaultState;

                        if (gadget1.name == AIHackingDevice.ItemName)
                            gadget1CurrentCharge--;
                        else if (gadget2.name == AIHackingDevice.ItemName)
                            gadget2CurrentCharge--;

                        hackerActive = false;
                        cursor.anim.SetBool("hackerActive", hackerActive);
                    }
                    else
                        audioManager.PlaySound("Gadget Empty");
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    hackerActive = false;
                    cursor.anim.SetBool("hackerActive", hackerActive);
                    audioManager.PlaySound("Gadget Empty");
                }
            }
        }
    }

    private IEnumerator UseTeleporter()
    {
        if (teleporterActive && !playerControllerScript.isTeleporting && !playerControllerScript.isReappearing)
        {
            if (gadget1.name == teleportationDevice.ItemName || gadget2.name == teleportationDevice.ItemName)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (Vector2.Distance(player.transform.position, cursor.transform.position) <= gameMaster.teleportationDeviceRange)
                    {
                        Vector3 targetPosition = cursor.transform.position;

                        playerControllerScript.isTeleporting = true;
                        playerControllerScript.playerAnim.SetBool("isTeleporting", true);

                        if (gadget1.name == teleportationDevice.ItemName)
                            gadget1CurrentCharge--;
                        else if (gadget2.name == teleportationDevice.ItemName)
                            gadget2CurrentCharge--;

                        audioManager.PlaySound("Teleport");

                        yield return new WaitForSeconds(0.5f);
                        playerControllerScript.isTeleporting = false;
                        playerControllerScript.playerAnim.SetBool("isTeleporting", false);

                        playerControllerScript.isReappearing = true;
                        playerControllerScript.playerAnim.SetBool("isReappearing", true);
                        player.transform.position = targetPosition;

                        yield return new WaitForSeconds(0.5f);
                        playerControllerScript.isReappearing = false;
                        playerControllerScript.playerAnim.SetBool("isReappearing", false);

                        teleporterActive = false;
                        cursor.anim.SetBool("teleporterActive", teleporterActive);
                    }
                    else
                        audioManager.PlaySound("Gadget Empty");
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    teleporterActive = false;
                    cursor.anim.SetBool("teleporterActive", teleporterActive);
                    audioManager.PlaySound("Gadget Empty");
                }
            }
        }
    }

    private void ActivateGadgetItem()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && gadgetSlot1Script.isEmpty == false)
        {
            teleporterActive = false;
            cursor.anim.SetBool("teleporterActive", teleporterActive);
            hackerActive = false;
            cursor.anim.SetBool("hackerActive", hackerActive);
            scannerActive = false;
            cursor.anim.SetBool("scannerActive", scannerActive);

            if (gadget1CurrentCharge <= 0)
            {
                audioManager.PlaySound("Gadget Empty");
            }
            else if (gadget1.name == teleportationDevice.ItemName && !playerControllerScript.isTeleporting && !playerControllerScript.isReappearing)
            {
                // Then use gadget in gadget slot 1
                if (gadget1CurrentCharge > 0)
                {
                    teleporterActive = true;
                    cursor.anim.SetBool("teleporterActive", teleporterActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else if (gadget1.name == AIHackingDevice.ItemName)
            {
                if (gadget1CurrentCharge > 0)
                {
                    hackerActive = true;
                    cursor.anim.SetBool("hackerActive", hackerActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else if (gadget1.name == AIScanningDevice.ItemName)
            {
                if (gadget1CurrentCharge > 0)
                {
                    scannerActive = true;
                    cursor.anim.SetBool("scannerActive", scannerActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else
                audioManager.PlaySound("Gadget Empty");
        }

        if ((Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) && gadgetSlot2Script.isEmpty == false)
        {
            teleporterActive = false;
            cursor.anim.SetBool("teleporterActive", teleporterActive);
            hackerActive = false;
            cursor.anim.SetBool("hackerActive", hackerActive);
            scannerActive = false;
            cursor.anim.SetBool("scannerActive", scannerActive);

            if (gadget2CurrentCharge <= 0)
            {
                audioManager.PlaySound("Gadget Empty");
            }
            else if (gadget2.name == teleportationDevice.ItemName && !playerControllerScript.isTeleporting && !playerControllerScript.isReappearing)
            {
                // Then use gadget in gadget slot 2
                if (gadget2CurrentCharge > 0)
                {
                    teleporterActive = true;
                    cursor.anim.SetBool("teleporterActive", teleporterActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else if (gadget2.name == AIHackingDevice.ItemName)
            {
                if (gadget2CurrentCharge > 0)
                {
                    hackerActive = true;
                    cursor.anim.SetBool("hackerActive", hackerActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else if (gadget2.name == AIScanningDevice.ItemName)
            {
                if (gadget2CurrentCharge > 0)
                {
                    scannerActive = true;
                    cursor.anim.SetBool("scannerActive", scannerActive);
                    audioManager.PlaySound("Gadget Active");
                }
            }
            else
                audioManager.PlaySound("Gadget Empty");
        }
    }

    private void DisplayCharges()
    {
        if (gadget1 != null)
            gadgetSlot1.transform.GetChild(1).GetComponent<Text>().text = gadget1CurrentCharge + "/" + gadget1MaxCharge;

        if (gadget2 != null)
            gadgetSlot2.transform.GetChild(1).GetComponent<Text>().text = gadget2CurrentCharge + "/" + gadget2MaxCharge;
    }
}
