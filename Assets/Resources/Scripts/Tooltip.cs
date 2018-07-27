using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Item item;
    private string data;
    private GameObject tooltip;

    Hotbar hotbar;
    Weapon currentWeapon;
    Weapon secondaryWeapon;

    Color32 greenColor = new Color32(66, 255, 66, 255); // Green
    Color32 redColor = new Color32(255, 40, 0, 255); // Red
    string greenTextColor;
    string accuracyTextColor;
    string secondaryAccuracyTextColor;

    string equippedFireRateText;
    string secondaryFireRateText;

    string equippedFireRateNameAddOn;
    string secondaryFireRateNameAddOn;

    string equippedDamageNameAddOn;
    string secondaryDamageNameAddOn;

    string equippedAverageNameAddOn;
    string secondaryAverageNameAddOn;

    string equippedTaintedNameAddOn;
    string secondaryTaintedNameAddOn;

    string equippedPerfectNameAddOn;
    string secondaryPerfectNameAddOn;

    string equippedNearPerfectNameAddOn;
    string secondaryNearPerfectNameAddOn;

    string equippedUselessNameAddOn;
    string secondaryUselessNameAddOn;

    string equippedBasicallyUselessNameAddOn;
    string secondaryBasicallyUselessNameAddOn;

    string equippedSilencedText;
    string secondarySilencedText;

    string equippedClipSizeText;
    string secondaryClipSizeText;

    string equippedAccuracyText;
    string secondaryAccuracyText;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);

        hotbar = GameObject.Find("Hotbar").GetComponent<Hotbar>();

        greenTextColor = ColorUtility.ToHtmlStringRGBA(greenColor);
    }

    void Update () {
        if (tooltip.activeSelf)
        {
            if (hotbar.weaponSlot1.transform.childCount == 3 && item.ItemName == hotbar.weaponSlot1.transform.GetChild(2).name)
            {
                tooltip.transform.position = hotbar.weaponSlot1.transform.position + new Vector3(-138, 360);
            }
            else if (hotbar.weaponSlot2.transform.childCount == 3 && item.ItemName == hotbar.weaponSlot2.transform.GetChild(2).name)
            {
                tooltip.transform.position = hotbar.weaponSlot2.transform.position + new Vector3(-138, 360);
            }
            else
            {
                tooltip.transform.position = Input.mousePosition + new Vector3(-97, 200);
            }
        }
	}

    public void Activate(Item item)
    {
        //currentWeapon = hotbar.currentlyEquippedWeapon.GetComponent<Weapon>();
        if (hotbar.currentlyEquippedWeapon != null)
        {
            currentWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        }

        if (hotbar.secondaryWeapon != null)
        {
            secondaryWeapon = hotbar.secondaryWeapon.GetComponent<Weapon>();
        }

        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        DetermineFireRateText();
        DetermineNameAddOns();
        DeterminePerkText();

        if (item.Type == Item.ItemType.Gun)
        {
            if ((hotbar.currentlyEquippedWeaponSlot == 1 && hotbar.weaponSlot1.transform.GetChild(2).name == item.ItemName) 
                || (hotbar.currentlyEquippedWeaponSlot == 2 && hotbar.weaponSlot2.transform.GetChild(2).name == item.ItemName))
            {
                data = "<size=22px>"
                    + equippedBasicallyUselessNameAddOn + equippedNearPerfectNameAddOn + equippedPerfectNameAddOn 
                    + equippedTaintedNameAddOn + equippedFireRateNameAddOn + equippedDamageNameAddOn + equippedAverageNameAddOn
                    + currentWeapon.name + "</size>\n\n"
                    + item.ItemDescription + "\n"
                    + "<color=#" + greenTextColor + ">"
                    + equippedSilencedText
                    + equippedClipSizeText
                    + "</color>"
                    + "<color=#" + accuracyTextColor + ">" + equippedAccuracyText + "</color>"
                    + "\n" + currentWeapon.actionType
                    + "\n\nDamage: " + currentWeapon.damage
                    + equippedFireRateText
                    + "\n\nClip Size: " + currentWeapon.clipSize
                    + "\n\nAmmo Type: " + item.AmmoType;
            }
            else
            {
                data = "<size=22px>"
                    + secondaryBasicallyUselessNameAddOn + secondaryNearPerfectNameAddOn + secondaryPerfectNameAddOn 
                    + secondaryTaintedNameAddOn + secondaryFireRateNameAddOn + secondaryDamageNameAddOn + secondaryAverageNameAddOn
                    + hotbar.secondaryWeapon.name + "</size>\n\n"
                    + item.ItemDescription + "\n"
                    + "<color=#" + greenTextColor + ">"
                    + secondarySilencedText
                    + secondaryClipSizeText
                    + "</color>"
                    + "<color=#" + secondaryAccuracyTextColor + ">" + secondaryAccuracyText + "</color>"
                    + "\n" + secondaryWeapon.actionType
                    + "\n\nDamage: " + secondaryWeapon.damage
                    + secondaryFireRateText
                    + "\n\nClip Size: " + secondaryWeapon.clipSize
                    + "\n\nAmmo Type: " + item.AmmoType;
            }
        }
        else
        {
            data = "<size=22px>" + item.ItemName + "</size>\n\n" + item.ItemDescription;
        }
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }

    private void DeterminePerkText()
    {
        if (currentWeapon.isSilenced)
            equippedSilencedText = " + Silenced\n";
        else
            equippedSilencedText = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.isSilenced)
                secondarySilencedText = " + Silenced\n";
            else
                secondarySilencedText = "";
        }

        if (currentWeapon.hasIncreasedClipSize)
        {
            if (currentWeapon.clipSizeMultiplier == 1.2f)
                equippedClipSizeText = " + Barely Increased Clip Size\n";
            else if (currentWeapon.clipSizeMultiplier == 1.4f)
                equippedClipSizeText = " + Slightly Increased Clip Size\n";
            else if (currentWeapon.clipSizeMultiplier == 1.6f)
                equippedClipSizeText = " + Moderately Increased Clip\n"
                                        + "   Size\n";
            else if (currentWeapon.clipSizeMultiplier == 1.8f)
                equippedClipSizeText = " + Greatly Increased Clip Size\n";
            else
                equippedClipSizeText = " + Doubled Clip Size\n";
        }
        else
            equippedClipSizeText = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.hasIncreasedClipSize)
            {
                if (secondaryWeapon.clipSizeMultiplier == 1.2f)
                    secondaryClipSizeText = " + Barely Increased Clip Size\n";
                else if (secondaryWeapon.clipSizeMultiplier == 1.4f)
                    secondaryClipSizeText = " + Slightly Increased Clip Size\n";
                else if (secondaryWeapon.clipSizeMultiplier == 1.6f)
                    secondaryClipSizeText = " + Moderately Increased Clip\n"
                                        + "   Size\n";
                else if (secondaryWeapon.clipSizeMultiplier == 1.8f)
                    secondaryClipSizeText = " + Greatly Increased Clip Size\n";
                else
                    secondaryClipSizeText = " + Doubled Clip Size\n";
            }
            else
                secondaryClipSizeText = "";
        }

        if (currentWeapon.hasAlteredInaccuracyFactor)
        {
            if (currentWeapon.inaccuracyFactor == -0.015f)
                equippedAccuracyText = " + Barely Increased Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == -0.03f)
                equippedAccuracyText = " + Slightly Increased Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == -0.045f)
                equippedAccuracyText = " + Moderately Increased\n   Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == -0.06f)
                equippedAccuracyText = " + Greatly Increased Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == -0.075f)
                equippedAccuracyText = " + Considerably Increased\n   Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == 0.015f)
                equippedAccuracyText = " - Barely Decreased Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == 0.03f)
                equippedAccuracyText = " - Slightly Decreased Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == 0.045f)
                equippedAccuracyText = " - Moderately Decreased\n   Accuracy\n";
            else if (currentWeapon.inaccuracyFactor == 0.06f)
                equippedAccuracyText = " - Greatly Decreased Accuracy\n";
            else
                equippedAccuracyText = " - Considerably Decreased\n   Accuracy\n";

            if (currentWeapon.inaccuracyFactor < 0)
            {
                accuracyTextColor = ColorUtility.ToHtmlStringRGBA(greenColor);
            }
            else
                accuracyTextColor = ColorUtility.ToHtmlStringRGBA(redColor);
        }
        else
            equippedAccuracyText = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.hasAlteredInaccuracyFactor)
            {
                if (secondaryWeapon.inaccuracyFactor == -0.015f)
                    secondaryAccuracyText = " + Barely Increased Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == -0.03f)
                    secondaryAccuracyText = " + Slightly Increased Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == -0.045f)
                    secondaryAccuracyText = " + Moderately Increased\n   Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == -0.06f)
                    secondaryAccuracyText = " + Greatly Increased Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == -0.075f)
                    secondaryAccuracyText = " + Considerably Increased\n   Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == 0.015f)
                    secondaryAccuracyText = " - Barely Decreased Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == 0.03f)
                    secondaryAccuracyText = " - Slightly Decreased Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == 0.045f)
                    secondaryAccuracyText = " - Moderately Decreased\n   Accuracy\n";
                else if (secondaryWeapon.inaccuracyFactor == 0.06f)
                    secondaryAccuracyText = " - Greatly Decreased Accuracy\n";
                else
                    secondaryAccuracyText = " - Considerably Decreased\n   Accuracy\n";

                if (currentWeapon.inaccuracyFactor < 0)
                {
                    secondaryAccuracyTextColor = ColorUtility.ToHtmlStringRGBA(greenColor);
                }
                else
                    secondaryAccuracyTextColor = ColorUtility.ToHtmlStringRGBA(redColor);
            }
            else
                secondaryAccuracyText = "";
        }
    }

    private void DetermineFireRateText()
    {
        if (item.MaxFireRate != 1)
        {
            equippedFireRateText = "\n\nFire Rate: " + currentWeapon.fireRate + " rounds/second";
            if (hotbar.secondaryWeapon != null)
            {
                secondaryFireRateText = "\n\nFire Rate: " + secondaryWeapon.fireRate + " rounds/second";
            }
        }
        else
        {
            equippedFireRateText = "";
            secondaryFireRateText = "";
        }
    }

    private void DetermineNameAddOns()
    {
        if (currentWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
        {
            equippedFireRateNameAddOn = "Rapid ";
        }
        else if (currentWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
        {
            equippedFireRateNameAddOn = "Sluggish ";
        }
        else
            equippedFireRateNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
            {
                secondaryFireRateNameAddOn = "Rapid ";
            }
            else if (secondaryWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
            {
                secondaryFireRateNameAddOn = "Sluggish ";
            }
            else
            {
                secondaryFireRateNameAddOn = "";
            }
        }

        if (currentWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.20f))
        {
            equippedDamageNameAddOn = "Heavy-Hitting ";
        }
        else if (currentWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.20f))
        {
            equippedDamageNameAddOn = "Weak ";
        }
        else
            equippedDamageNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.20f))
            {
                secondaryDamageNameAddOn = "Heavy-Hitting ";
            }
            else if (secondaryWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.20f))
            {
                secondaryDamageNameAddOn = "Weak ";
            }
            else
                secondaryDamageNameAddOn = "";
        }

        if (currentWeapon.fireRate >= ((item.MaxFireRate + item.MinFireRate) / 2) - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.fireRate <= ((item.MaxFireRate + item.MinFireRate) / 2) + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.damage >= ((item.MaxDamage + item.MinDamage) / 2) - ((item.MaxDamage - item.MaxDamage) * 0.05f)
            && currentWeapon.damage <= ((item.MaxDamage + item.MinDamage) / 2) + ((item.MaxDamage - item.MaxDamage) * 0.05f))
        {
            equippedFireRateNameAddOn = "";
            equippedDamageNameAddOn = "";
            equippedAverageNameAddOn = "Painfully Average ";
        }
        else
            equippedAverageNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= ((item.MaxFireRate + item.MinFireRate) / 2) - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && secondaryWeapon.fireRate <= ((item.MaxFireRate + item.MinFireRate) / 2) + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && secondaryWeapon.damage >= ((item.MaxDamage + item.MinDamage) / 2) - ((item.MaxDamage - item.MinDamage) * 0.05f)
                && secondaryWeapon.damage <= ((item.MaxDamage + item.MinDamage) / 2) + ((item.MaxDamage - item.MinDamage) * 0.05f))
            {
                secondaryFireRateNameAddOn = "";
                secondaryDamageNameAddOn = "";
                secondaryAverageNameAddOn = "Painfully Average ";
            }
            else
                secondaryAverageNameAddOn = "";
        }

        if (currentWeapon.fireRate == 6.66f || currentWeapon.damage == 6.66f)
            equippedTaintedNameAddOn = "Tainted ";
        else
            equippedTaintedNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == 6.66f || secondaryWeapon.damage == 6.66f)
                secondaryTaintedNameAddOn = "Tainted ";
            else
                secondaryTaintedNameAddOn = "";
        }

        if (currentWeapon.fireRate == item.MaxFireRate && currentWeapon.damage == item.MaxDamage)
        {
            equippedFireRateNameAddOn = "";
            equippedDamageNameAddOn = "";
            equippedPerfectNameAddOn = "Perfect ";
        }
        else
            equippedPerfectNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == item.MaxFireRate && currentWeapon.damage == item.MaxDamage)
            {
                secondaryFireRateNameAddOn = "";
                secondaryDamageNameAddOn = "";
                secondaryPerfectNameAddOn = "Perfect ";
            }
            else
                secondaryPerfectNameAddOn = "";
        }
        
        if (currentWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && (currentWeapon.fireRate < item.MaxFireRate)
            && currentWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.05f)
            && currentWeapon.damage < item.MaxDamage)
        {
            equippedFireRateNameAddOn = "";
            equippedDamageNameAddOn = "";
            equippedNearPerfectNameAddOn = "Nearly Perfect ";
        }
        else
            equippedNearPerfectNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && (secondaryWeapon.fireRate < item.MaxFireRate)
                && secondaryWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.05f)
                && secondaryWeapon.damage < item.MaxDamage)
            {
                secondaryFireRateNameAddOn = "";
                secondaryDamageNameAddOn = "";
                secondaryNearPerfectNameAddOn = "Nearly Perfect ";
            }
            else
                secondaryNearPerfectNameAddOn = "";
        }

        if (currentWeapon.fireRate == item.MinFireRate && currentWeapon.damage == item.MinDamage)
        {
            equippedFireRateNameAddOn = "";
            equippedDamageNameAddOn = "";
            equippedUselessNameAddOn = "Useless ";
        }
        else
            equippedUselessNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == item.MinFireRate && secondaryWeapon.damage == item.MinDamage)
            {
                secondaryFireRateNameAddOn = "";
                secondaryDamageNameAddOn = "";
                secondaryUselessNameAddOn = "Useless ";
            }
            else
                secondaryUselessNameAddOn = "";
        }

        if (currentWeapon.fireRate > item.MinFireRate
            && currentWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.damage > item.MinDamage
            && currentWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.05f))
        {
            equippedFireRateNameAddOn = "";
            equippedDamageNameAddOn = "";
            equippedBasicallyUselessNameAddOn = "Basically Useless: ";
        }
        else
            equippedBasicallyUselessNameAddOn = "";

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate > item.MinFireRate
            && secondaryWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && secondaryWeapon.damage > item.MinDamage
            && secondaryWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.05f))
            {
                secondaryFireRateNameAddOn = "";
                secondaryDamageNameAddOn = "";
                secondaryBasicallyUselessNameAddOn = "Basically Useless: ";
            }
            else
                secondaryBasicallyUselessNameAddOn = "";
        }
    }
}
