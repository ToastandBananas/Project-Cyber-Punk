using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victim : MonoBehaviour
{
    [Header("Stats:")]
    public float maxHealth = 1;
    public float currentHealth;

    [Header("Sounds")]
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    [Header("Other")]
    public bool isDead = false;

    CapsuleCollider2D thisVictimCollider;

    Animator anim;
    AudioManager audioManager;

    Player player;
    VictimMovement victimMovementScript;

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        anim.SetFloat("health", currentHealth);

        player = Player.instance;
        victimMovementScript = GetComponent<VictimMovement>();

        thisVictimCollider = GetComponent<CapsuleCollider2D>();

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    public void DamageEnemy(float damage)
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
