using UnityEngine;

public class LightRaycast : MonoBehaviour
{
    Player player;
    PlayerController playerControllerScript;
    GameObject[] victims;

    public LayerMask whatToHit;

    public int floorLevel;

    float timerMax = 0.25f;

    [Header("For Raycast Positions:")]
    public float feetOffset = 0.135f;
    public float headOffset = 0.17f;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();
        victims = GameObject.FindGameObjectsWithTag("Victim");
	}
	
	void FixedUpdate () {
        RayCastLight();
	}

    private void RayCastLight()
    {
        foreach(GameObject victim in victims)
        {
            Victim victimScript = victim.GetComponent<Victim>();
            VictimMovement victimMovementScript = victim.GetComponent<VictimMovement>();

            if (victimMovementScript.currentFloorLevel == floorLevel)
            {
                float distanceToVictim = Vector2.Distance(transform.position, victim.transform.position);
                Vector2 victimFeet = new Vector2(victim.transform.position.x, victim.transform.position.y - feetOffset);
                Vector2 victimHead = new Vector2(victim.transform.position.x, victim.transform.position.y + headOffset);
                Vector2 lightPosition = new Vector2(transform.position.x, transform.position.y);

                RaycastHit2D castToVictimFeet = Physics2D.Raycast(transform.position, (victimFeet - lightPosition), distanceToVictim, whatToHit);
                RaycastHit2D castToVictimHead = Physics2D.Raycast(transform.position, (victimHead - lightPosition), distanceToVictim, whatToHit);
                Debug.DrawLine(victimFeet, castToVictimFeet.point, Color.red);
                Debug.DrawLine(victimHead, castToVictimHead.point, Color.red);
                
                if ((castToVictimFeet.collider != null && castToVictimFeet.collider.gameObject.tag == "Victim") || (castToVictimHead.collider != null && castToVictimHead.collider.gameObject.tag == "Victim"))
                {
                    victimScript.lightTimer = 0.0f;
                    victimScript.isVisibleByLight = true;
                }
                else
                {
                    if (victimScript.lightTimer < timerMax)
                    {
                        victimScript.lightTimer += Time.deltaTime;
                    }
                    else if (victimScript.lightTimer >= timerMax)
                    {
                        victimScript.isVisibleByLight = false;
                    }
                }
            }
            else
            {
                if (victimScript.lightTimer < timerMax)
                {
                    victimScript.lightTimer += Time.deltaTime;
                }
                else if (victimScript.lightTimer >= timerMax)
                {
                    victimScript.isVisibleByLight = false;
                }
            }
        }

        if (playerControllerScript.currentFloorLevel == floorLevel)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            Vector2 playerFeet = new Vector2(player.transform.position.x, player.transform.position.y - feetOffset);
            Vector2 playerHead = new Vector2(player.transform.position.x, player.transform.position.y + headOffset);
            Vector2 lightPosition = new Vector2(transform.position.x, transform.position.y);

            RaycastHit2D castToPlayerFeet = Physics2D.Raycast(transform.position, (playerFeet - lightPosition), distanceToPlayer, whatToHit);
            RaycastHit2D castToPlayerHead = Physics2D.Raycast(transform.position, (playerHead - lightPosition), distanceToPlayer, whatToHit);
            // Debug.DrawLine(playerFeet, castToPlayerFeet.point, Color.red);
            // Debug.DrawLine(playerHead, castToPlayerFeet.point, Color.red);

            if ((castToPlayerFeet.collider != null && castToPlayerFeet.collider.gameObject.tag == "Player") || (castToPlayerHead.collider != null && castToPlayerHead.collider.gameObject.tag == "Player"))
            {
                player.lightTimer = 0.0f;
                player.isVisibleByLight = true;
            }
            else
            {
                if (player.lightTimer < timerMax)
                {
                    player.lightTimer += Time.deltaTime;
                }
                else if (player.lightTimer >= timerMax)
                {
                    player.isVisibleByLight = false;
                }
            }
        }
        else
        {
            if (player.lightTimer < timerMax)
            {
                player.lightTimer += Time.deltaTime;
            }
            else if (player.lightTimer >= timerMax)
            {
                player.isVisibleByLight = false;
            }
        }
    }
}
