using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public bool allMainMissionsComplete = false;

    [Header("Main Mission Objectives")]
    public LevelObjectives mission1 = LevelObjectives.None;
    public bool mission1Complete = false;
    public LevelObjectives mission2 = LevelObjectives.None;
    public bool mission2Complete = false;
    public LevelObjectives mission3 = LevelObjectives.None;
    public bool mission3Complete = false;

    [Header("Side Mission Objectives")]
    public LevelObjectives sideMission1 = LevelObjectives.None;
    public bool sideMission1Complete = false;
    public LevelObjectives sideMission2 = LevelObjectives.None;
    public bool sideMission2Complete = false;
    public LevelObjectives sideMission3 = LevelObjectives.None;
    public bool sideMission3Complete = false;

    public enum LevelObjectives
    {
        None,
        Assassination,
        AquireItem,
        KillAllEnemies,
        RescueVictims
    }

    [Header("Rescue Victims Mission Variables")]
    public int victimCount = 0;
    public int victimsSaved = 0;
    public int victimsFollowing = 0;
    GameObject[] victims;

    // Use this for initialization
    void Start ()
    {
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
                // Play teleport animation and go to mission complete screen
                print("Level complete!");
            }
        }
    }

    public void RescueVictimsMissionComplete() // This is called in the VictimMovement script when all victims are rescued
    {
        if (mission1 == LevelObjectives.RescueVictims)
            mission1Complete = true;
        else if (mission2 == LevelObjectives.RescueVictims)
            mission2Complete = true;
        else if (mission3 == LevelObjectives.RescueVictims)
            mission3Complete = true;
        else if (sideMission1 == LevelObjectives.RescueVictims)
            sideMission1Complete = true;
        else if (sideMission2 == LevelObjectives.RescueVictims)
            sideMission2Complete = true;
        else if (sideMission3 == LevelObjectives.RescueVictims)
            sideMission3Complete = true;
    }
}
