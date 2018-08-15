using UnityEngine;

public class LightRaycast : MonoBehaviour
{
    Player player;
    PlayerController playerControllerScript;
    GameObject[] victims;

    public LayerMask whatToHit;

    public int floorLevel;

    float timerMax = 0.25f;

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
                Vector2 victimPosition = new Vector2(victim.transform.position.x, victim.transform.position.y);
                Vector2 lightPosition = new Vector2(transform.position.x, transform.position.y);

                RaycastHit2D castToVictim = Physics2D.Raycast(transform.position, (victimPosition - lightPosition), distanceToVictim, whatToHit);
                Debug.DrawLine(victimPosition, castToVictim.point, Color.red);

                if (castToVictim.collider != null)
                {
                    // print(castToVictim.collider.gameObject.name);
                    if (castToVictim.collider.gameObject.tag == "Victim")
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
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 lightPosition = new Vector2(transform.position.x, transform.position.y);

            RaycastHit2D castToPlayer = Physics2D.Raycast(transform.position, (playerPosition - lightPosition), distanceToPlayer, whatToHit);
            Debug.DrawLine(playerPosition, castToPlayer.point, Color.red);

            if (castToPlayer.collider != null)
            {
                // print(castToPlayer.collider.gameObject.name);
                if (castToPlayer.collider.gameObject.tag == "Player")
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
