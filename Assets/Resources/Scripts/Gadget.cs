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

    public bool teleporterActive = false;

    ItemDatabase itemDatabase;
    GameMaster gameMaster;
    MouseCursor cursor;
    Player player;
    PlayerController playerControllerScript;
    AudioManager audioManager;

    Item teleportationDevice;

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

        gadgetSlot1Script = gadgetSlot1.GetComponent<Slot>();
        gadgetSlot2Script = gadgetSlot2.GetComponent<Slot>();

        teleportationDevice = itemDatabase.FetchItemByID(34);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if ((gadget1 == null && gadgetSlot1Script.isEmpty == false) || (gadget2 == null && gadgetSlot2Script.isEmpty == false))
            SetTeleporterCharges();

        UseGadgetItem();
        StartCoroutine(UseTeleporter());
        DisplayCharges();
    }

    private void SetTeleporterCharges()
    {
        if (gadget1 == null)
            if (gadgetSlot1Script.isEmpty == false)
                gadget1 = gadgetSlot1.transform.GetChild(2).gameObject;

        if (gadget2 == null)
            if (gadgetSlot2Script.isEmpty == false)
                gadget2 = gadgetSlot2.transform.GetChild(2).gameObject;

        if (gadget1 != null && gadget1.name == teleportationDevice.ItemName)
        {
            gadget1MaxCharge = gameMaster.teleportationDeviceMaxCharge;
            gadget1CurrentCharge = gadget1MaxCharge;
            // print(gadget1CurrentCharge + " / " + gadget1MaxCharge);
        }

        if (gadget2 != null && gadget2.name == teleportationDevice.ItemName)
        {
            gadget2MaxCharge = gameMaster.teleportationDeviceMaxCharge;
            gadget2CurrentCharge = gadget2MaxCharge;
            // print(gadget2CurrentCharge + " / " + gadget2MaxCharge);
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

    private void UseGadgetItem()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && gadgetSlot1Script.isEmpty == false)
        {
            if (!playerControllerScript.isTeleporting && !playerControllerScript.isReappearing)
            {
                // Then use gadget in gadget slot 1
                if (gadget1.name == teleportationDevice.ItemName && gadget1CurrentCharge > 0)
                {
                    teleporterActive = true;
                    cursor.anim.SetBool("teleporterActive", teleporterActive);
                    audioManager.PlaySound("Gadget Active");
                }
                else if (gadget1.name == teleportationDevice.ItemName && gadget1CurrentCharge <= 0)
                    audioManager.PlaySound("Gadget Empty");
            }
            else
                audioManager.PlaySound("Gadget Empty");
        }

        if ((Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) && gadgetSlot2Script.isEmpty == false)
        {
            if (!playerControllerScript.isTeleporting && !playerControllerScript.isReappearing)
            {
                // Then use gadget in gadget slot 2
                if (gadget2.name == teleportationDevice.ItemName && gadget2CurrentCharge > 0)
                {
                    teleporterActive = true;
                    cursor.anim.SetBool("teleporterActive", teleporterActive);
                    audioManager.PlaySound("Gadget Active");
                }
                else if (gadget2.name == teleportationDevice.ItemName && gadget2CurrentCharge <= 0)
                    audioManager.PlaySound("Gadget Empty");
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
