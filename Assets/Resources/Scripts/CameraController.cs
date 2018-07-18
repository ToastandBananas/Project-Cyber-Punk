using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;
    public float yPosRestrictionMin = 0.3f;
    public int yPosRestrictionMax = 10;
    public int xPosRestriction = 10;

    float offsetZ;
    Vector3 lastTargetPosition;
    Vector3 currentVelocity;
    Vector3 lookAheadPos;

    float nextTimeToSearch = 0;

    Player player;

    Weapon weaponScript;
    PlayerController playerControllerScript;
    MouseCursor mouseCursorScript;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        
        weaponScript = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerControllerScript = player.GetComponent<PlayerController>();
        mouseCursorScript = GameObject.Find("Crosshair").GetComponent<MouseCursor>();

        lastTargetPosition = target.position;
        offsetZ = (transform.position = target.position).z - 1;
        transform.parent = null;
	}

    // Update is called once per frame
    void Update () {
        if (target == null)
        {
            FindPlayer();
            return;
        }
        else if (weaponScript.isSniper && playerControllerScript.isAiming)
        {
            target = mouseCursorScript.transform;
            damping = 1.2f;
        }
        else
        {
            target = player.transform;
            damping = 0.3f;
        }

        float xMoveDelta = (target.position - lastTargetPosition).x;

        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        }
        else
        {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);
        
        newPos = new Vector3(Mathf.Clamp(newPos.x, player.transform.position.x - xPosRestriction, player.transform.position.x + xPosRestriction), Mathf.Clamp(newPos.y, yPosRestrictionMin, player.transform.position.y + yPosRestrictionMax), newPos.z);
       

        transform.position = newPos;

        lastTargetPosition = target.position;
	}

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject searchResult = player.gameObject; // Finds player
            if (searchResult != null)
            {
                target = searchResult.transform;
            }
            nextTimeToSearch = Time.time + 0.5f;
        }
    }
}
