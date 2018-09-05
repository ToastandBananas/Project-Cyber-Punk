using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    public bool allMainMissionsComplete = false;

    [Header("Main Mission Objectives")]
    public LevelObjectives mission1 = LevelObjectives.None;
    public bool mission1Complete = false;
    public bool mission1Failed = false;
    public LevelObjectives mission2 = LevelObjectives.None;
    public bool mission2Complete = false;
    public bool mission2Failed = false;
    public LevelObjectives mission3 = LevelObjectives.None;
    public bool mission3Complete = false;
    public bool mission3Failed = false;

    [Header("Side Mission Objectives")]
    public LevelObjectives sideMission1 = LevelObjectives.None;
    public bool sideMission1Complete = false;
    public bool sideMission1Failed = false;
    public LevelObjectives sideMission2 = LevelObjectives.None;
    public bool sideMission2Complete = false;
    public bool sideMission2Failed = false;
    public LevelObjectives sideMission3 = LevelObjectives.None;
    public bool sideMission3Complete = false;
    public bool sideMission3Failed = false;

    public enum LevelObjectives
    {
        None,
        Assassination,
        AquireItem,
        KillAllEnemies,
        RescueVictims
    }

    [Header("Assassination Mission Variables")]
    public int assassinationTargetCount = 0;
    public int targetsAssassinated = 0;

    [Header("Kill All Enemies Mission Variables")]
    public int enemyCount = 0;
    public int enemiesKilled = 0;
    GameObject[] enemies;

    [Header("Rescue Victims Mission Variables")]
    public int victimCount = 0;
    public int victimsSaved = 0;
    public int victimsFollowing = 0;
    GameObject[] victims;

    GameObject missionFailedText;
    AudioManager audioManager;
    Player player;
    PlayerController playerControllerScript;

    // Use this for initialization
    void Start ()
    {
        missionFailedText = GameObject.Find("MissionFailedText");
        missionFailedText.SetActive(false);

        audioManager = AudioManager.instance;
        if (audioManager == null)
            Debug.LogError("No AudioManager found in the scene");

        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        foreach (GameObject enemy in enemies)
            if (enemy.GetComponent<Enemy>().isAssassinationTarget)
                assassinationTargetCount++;

        victims = GameObject.FindGameObjectsWithTag("Victim");
        victimCount = victims.Length;

        // mission1 will never start off as LevelObjectives.None
        if (mission1 == LevelObjectives.None)
            Debug.LogError("Mission 1 needs to be set in the inspector.");

        if (mission2 == LevelObjectives.None)
            mission2Complete = true;

        if (mission3 == LevelObjectives.None)
            mission3Complete = true;

        if (sideMission1 == LevelObjectives.None)
            sideMission1Complete = true;

        if (sideMission2 == LevelObjectives.None)
            sideMission2Complete = true;

        if (sideMission3 == LevelObjectives.None)
            sideMission3Complete = true;

        if (sideMission1 == LevelObjectives.RescueVictims || sideMission2 == LevelObjectives.RescueVictims || sideMission3 == LevelObjectives.RescueVictims)
            Debug.LogError("The Rescue Victims mission should always be a main mission, but it is currently assigned as a side mission.");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!allMainMissionsComplete && mission1Complete && mission2Complete && mission3Complete)
            allMainMissionsComplete = true;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && victimsFollowing > 0)
        {
            // Then teleport victim to safe house
            foreach (GameObject victim in victims)
            {
                if (victim != null)
                {
                    if (victim.GetComponent<VictimMovement>().currentState == VictimMovement.State.Follow)
                    {
                        victim.GetComponent<VictimMovement>().currentState = VictimMovement.State.ExitLevel;
                        victimsFollowing--;
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && allMainMissionsComplete)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Level complete!");
                playerControllerScript.isTeleporting = true;
                playerControllerScript.playerAnim.SetBool("isTeleporting", true);
                Invoke("DeactivatePlayer", 0.5f);
                Invoke("LoadNextScene", 2f);
            }
        }
    }

    void DeactivatePlayer()
    {
        player.GetComponent<SpriteRenderer>().enabled = false;
    }

    void LoadNextScene()
    {
        // Currently just reloads the level, but eventually will load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AquireItemMissionComplete() // This is called in the ___
    {
        if (mission1 == LevelObjectives.AquireItem)
            mission1Complete = true;
        else if (mission2 == LevelObjectives.AquireItem)
            mission2Complete = true;
        else if (mission3 == LevelObjectives.AquireItem)
            mission3Complete = true;
        else if (sideMission1 == LevelObjectives.AquireItem)
            sideMission1Complete = true;
        else if (sideMission2 == LevelObjectives.AquireItem)
            sideMission2Complete = true;
        else if (sideMission3 == LevelObjectives.AquireItem)
            sideMission3Complete = true;
    }

    public void AssassinationMissionComplete() // This is called in the Enemy script when the target(s) is killed
    {
        if (mission1 == LevelObjectives.Assassination)
            mission1Complete = true;
        else if (mission2 == LevelObjectives.Assassination)
            mission2Complete = true;
        else if (mission3 == LevelObjectives.Assassination)
            mission3Complete = true;
        else if (sideMission1 == LevelObjectives.Assassination)
            sideMission1Complete = true;
        else if (sideMission2 == LevelObjectives.Assassination)
            sideMission2Complete = true;
        else if (sideMission3 == LevelObjectives.Assassination)
            sideMission3Complete = true;
    }

    public void KillAllEnemiesMissionComplete() // This is called in the Enemy script when all enemies are killed
    {
        if (mission1 == LevelObjectives.KillAllEnemies)
            mission1Complete = true;
        else if (mission2 == LevelObjectives.KillAllEnemies)
            mission2Complete = true;
        else if (mission3 == LevelObjectives.KillAllEnemies)
            mission3Complete = true;
        else if (sideMission1 == LevelObjectives.KillAllEnemies)
            sideMission1Complete = true;
        else if (sideMission2 == LevelObjectives.KillAllEnemies)
            sideMission2Complete = true;
        else if (sideMission3 == LevelObjectives.KillAllEnemies)
            sideMission3Complete = true;
    }

    public void RescueVictimsMissionComplete() // This is called in the VictimMovement script when all victims are rescued
    {
        if (mission1 == LevelObjectives.RescueVictims) // Rescue Victims will never be a side mission
            mission1Complete = true;
        else if (mission2 == LevelObjectives.RescueVictims)
            mission2Complete = true;
        else if (mission3 == LevelObjectives.RescueVictims)
            mission3Complete = true;
    }

    public IEnumerator RescueVictimsMissionFailed() // This is called in the Victim script when any victim is killed
    {
        if (mission1 == LevelObjectives.RescueVictims) // Rescue Victims will never be a side mission
            mission1Failed = true;
        else if (mission2 == LevelObjectives.RescueVictims)
            mission2Failed = true;
        else if (mission3 == LevelObjectives.RescueVictims)
            mission3Failed = true;

        missionFailedText.SetActive(true);

        audioManager.PlaySound("MissionFailure");

        while(missionFailedText.GetComponent<Text>().fontSize < 180)
        {
            yield return new WaitForSeconds(0.01f);
            missionFailedText.GetComponent<Text>().fontSize += 2;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
