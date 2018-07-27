﻿using UnityEngine;

public class WeaponPerks : MonoBehaviour {

    Weapon weaponScript;
    EnemyWeapon enemyWeaponScript;
    ItemDatabase itemDatabase;
    Player player;

    string rootTag;

    int randomNumber1;
    int randomNumber2;
    int randomNumber3;
    int randomNumber4;
    int randomNumber5;

	// Use this for initialization
	void Start () {
        itemDatabase = ItemDatabase.instance;
        player = Player.instance;

        rootTag = transform.root.tag;

        if (rootTag == "Player")
        {
            weaponScript = GetComponent<Weapon>();
        }
        else if (rootTag == "Enemy")
        {
            enemyWeaponScript = GetComponent<EnemyWeapon>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RandomizePerks(int weaponID)
    {
        Item thisWeapon = itemDatabase.FetchItemByID(weaponID);

        randomNumber1 = Random.Range(0, 100); // Used in DetermineSilencedWeaponChance()
        randomNumber2 = Random.Range(0, 100); // Used in DetermineIncreasedClipSizeChance()
        randomNumber3 = Random.Range(0, 100); // Used in DetermineIncreasedClipSizeChance()
        randomNumber4 = Random.Range(0, 100); // Used in DetermineWeaponAccuracy()
        randomNumber5 = Random.Range(0, 100); // Used in DetermineWeaponAccuracy()

        // print(randomNumber1 + " " + randomNumber2 + " " + randomNumber3 + " " + randomNumber4 + " " + randomNumber5);

        DetermineSilencedWeaponChance();
        DetermineIncreasedClipSizeChance(thisWeapon);
        DetermineWeaponAccuracyChangeChance();
    }

    void DetermineWeaponAccuracyChangeChance()
    {
        // Subtracting from inaccuracyFactor increases accuracy
        // And vice versa for adding to inaccuracyFactor
        if (randomNumber4 < 20) // 20% chance to increase accuracy
        {
            if (rootTag == "Player")
            {
                weaponScript.hasAlteredInaccuracyFactor = true;
                if (randomNumber5 < 50) // 50% chance
                {
                    weaponScript.inaccuracyFactor -= 0.015f; // Increase accuracy barely
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    weaponScript.inaccuracyFactor -= 0.03f; // Increase accuracy slightly
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    weaponScript.inaccuracyFactor -= 0.045f; // Increase accuracy moderately
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    weaponScript.inaccuracyFactor -= 0.06f; // Increase accuracy greatly
                }
                else // 3% chance
                {
                    weaponScript.inaccuracyFactor -= 0.075f; // Increase accuracy considerably
                }
            }
            else // If enemy
            {
                enemyWeaponScript.hasAlteredInaccuracyFactor = true;
                if (randomNumber5 < 50) // 50% chance
                {
                    enemyWeaponScript.inaccuracyFactor -= 0.015f; // Increase accuracy barely
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    enemyWeaponScript.inaccuracyFactor -= 0.03f; // Increase accuracy slightly
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    enemyWeaponScript.inaccuracyFactor -= 0.045f; // Increase accuracy moderately
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    enemyWeaponScript.inaccuracyFactor -= 0.06f; // Increase accuracy greatly
                }
                else // 3% chance
                {
                    enemyWeaponScript.inaccuracyFactor -= 0.075f; // Increase accuracy considerably
                }
            }
        }
        else if (randomNumber4 < 100 && randomNumber4 > 80) // 20% chance to decrease accuracy
        {
            if (rootTag == "Player")
            {
                weaponScript.hasAlteredInaccuracyFactor = true;
                if (randomNumber5 < 50) // 50% chance
                {
                    weaponScript.inaccuracyFactor += 0.015f; // Decrease accuracy barely
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    weaponScript.inaccuracyFactor += 0.03f; // Decrease accuracy slightly
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    weaponScript.inaccuracyFactor += 0.045f; // Decrease accuracy moderately
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    weaponScript.inaccuracyFactor += 0.06f; // Decrease accuracy greatly
                }
                else // 3% chance
                {
                    weaponScript.inaccuracyFactor += 0.075f; // Decrease accuracy considerably
                }
            }
            else // If enemy
            {
                enemyWeaponScript.hasAlteredInaccuracyFactor = true;
                if (randomNumber5 < 50) // 50% chance
                {
                    enemyWeaponScript.inaccuracyFactor += 0.015f; // Decrease accuracy barely
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    enemyWeaponScript.inaccuracyFactor += 0.03f; // Decrease accuracy slightly
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    enemyWeaponScript.inaccuracyFactor += 0.045f; // Decrease accuracy moderately
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    enemyWeaponScript.inaccuracyFactor += 0.06f; // Decrease accuracy greatly
                }
                else // 3% chance
                {
                    enemyWeaponScript.inaccuracyFactor += 0.075f; // Decrease accuracy considerably
                }
            }
        }
    }

    void DetermineIncreasedClipSizeChance(Item thisWeapon)
    {
        if (randomNumber2 < 20) // 20% chance - Increased Clip Size
        {
            if (rootTag == "Player")
            {
                weaponScript.hasIncreasedClipSize = true;
                if (randomNumber3 < 50) // 50% chance
                {
                    weaponScript.clipSizeMultiplier = 1.2f;
                    weaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * weaponScript.clipSizeMultiplier); // Increase clip size by 20%
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    weaponScript.clipSizeMultiplier = 1.4f;
                    weaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * weaponScript.clipSizeMultiplier); // Increase clip size by 40%
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    weaponScript.clipSizeMultiplier = 1.6f;
                    weaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * weaponScript.clipSizeMultiplier); // Increase clip size by 60%
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    weaponScript.clipSizeMultiplier = 1.8f;
                    weaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * weaponScript.clipSizeMultiplier); // Increase clip size by 80%
                }
                else // 3% chance
                {
                    weaponScript.clipSizeMultiplier = 2.0f;
                    weaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * weaponScript.clipSizeMultiplier); // Increase clip size by 100%
                }
                weaponScript.currentAmmoAmount = weaponScript.clipSize;
            }
            else // If enemy
            {
                enemyWeaponScript.hasIncreasedClipSize = true;
                if (randomNumber3 < 50) // 50% chance
                {
                    enemyWeaponScript.clipSizeMultiplier = 1.2f;
                    enemyWeaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * enemyWeaponScript.clipSizeMultiplier); // Increase clip size by 20%
                }
                else if (randomNumber3 >= 50 && randomNumber3 < 75) // 25% chance
                {
                    enemyWeaponScript.clipSizeMultiplier = 1.4f;
                    enemyWeaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * enemyWeaponScript.clipSizeMultiplier); // Increase clip size by 40%
                }
                else if (randomNumber3 >= 75 && randomNumber3 < 90) // 15% chance
                {
                    enemyWeaponScript.clipSizeMultiplier = 1.6f;
                    enemyWeaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * enemyWeaponScript.clipSizeMultiplier); // Increase clip size by 60%
                }
                else if (randomNumber3 >= 90 && randomNumber3 < 97) // 7% chance
                {
                    enemyWeaponScript.clipSizeMultiplier = 1.8f;
                    enemyWeaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * enemyWeaponScript.clipSizeMultiplier); // Increase clip size by 80%
                }
                else // 3% chance
                {
                    enemyWeaponScript.clipSizeMultiplier = 2.0f;
                    enemyWeaponScript.clipSize = (int)Mathf.Round(thisWeapon.ClipSize * enemyWeaponScript.clipSizeMultiplier); // Increase clip size by 100%
                }
                enemyWeaponScript.currentAmmoAmount = enemyWeaponScript.clipSize;
            }
        }
        else
        {
            if (rootTag == "Player")
            {
                weaponScript.clipSize = thisWeapon.ClipSize;
            }
            else // If enemy
            {
                enemyWeaponScript.clipSize = thisWeapon.ClipSize;
            }
        }
    }

    void DetermineSilencedWeaponChance()
    {
        if (randomNumber1 < 15) // 15% chance - Silenced weapon
        {
            if (rootTag == "Player")
            {
                weaponScript.isSilenced = true;
                weaponScript.transform.GetComponent<ProduceSoundTrigger>().enabled = false;
                weaponScript.DetermineSoundName();
            }
            else // If enemy
            {
                enemyWeaponScript.isSilenced = true;
                enemyWeaponScript.transform.GetComponent<ProduceSoundTrigger>().enabled = false;
                enemyWeaponScript.DetermineSoundName();
            }
        }
    }

    // Perk Ideas:
    //      Better grip (increased accuracy)
    //      
}
