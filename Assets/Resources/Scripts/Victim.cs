using UnityEngine;

public class Victim : MonoBehaviour
{
    [Header("Stats:")]
    public float maxHealth = 1;
    public float currentHealth;
    public bool isDead = false;

    [Header("Sounds")]
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    [Header("Other")]
    public float lightTimer = 0.0f; // For the LightRaycast script
    public bool isVisibleByLight = false;

    CapsuleCollider2D thisVictimCollider;
    GameObject[] otherVictims;

    Animator anim;
    AudioManager audioManager;

    Player player;
    VictimMovement victimMovementScript;
    LevelExit levelExitScript;

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        anim.SetFloat("health", currentHealth);

        player = Player.instance;
        victimMovementScript = GetComponent<VictimMovement>();
        levelExitScript = GameObject.Find("LevelExitTrigger").GetComponent<LevelExit>();

        thisVictimCollider = GetComponent<CapsuleCollider2D>();
        otherVictims = GameObject.FindGameObjectsWithTag("Victim");

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    public void DamageVictim(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            anim.SetFloat("health", currentHealth);
        }

        if (currentHealth <= 0 && isDead == false) // If enemy health falls to 0 or less they will die
        {
            // Death sound
            audioManager.PlaySound(deathSoundName);

            isDead = true;
            anim.SetBool("isDead", isDead);
            victimMovementScript.currentState = VictimMovement.State.Dead;

            StartCoroutine(levelExitScript.RescueVictimsMissionFailed());

            GetComponent<SpriteRenderer>().material = victimMovementScript.defaultMaterial;

            foreach(GameObject victim in otherVictims)
            {
                Physics2D.IgnoreCollision(thisVictimCollider, victim.GetComponent<CapsuleCollider2D>());
            }

            thisVictimCollider.direction = CapsuleDirection2D.Horizontal;
            thisVictimCollider.offset = new Vector2(3.43f, -19.05f);
            thisVictimCollider.size = new Vector2(46.55f, 10.41f);
        }
        else if (currentHealth > 0)
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }
    }
}
