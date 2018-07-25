using System.Collections;
using System.Collections.Generic;
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

    string equippedWeaponFireRateText;
    string secondaryWeaponFireRateText;

    string equippedWeaponFireRateNameAddOn = "";
    string secondaryWeaponFireRateNameAddOn = "";

    string equippedWeaponDamageNameAddOn = "";
    string secondaryWeaponDamageNameAddOn = "";

    string equippedWeaponAverageNameAddOn = "";
    string secondaryWeaponAverageNameAddOn = "";

    string equippedWeaponTaintedNameAddOn = "";
    string secondaryWeaponTaintedNameAddOn = "";

    string equippedWeaponPerfectNameAddOn = "";
    string secondaryWeaponPerfectNameAddOn = "";

    string equippedWeaponNearPerfectNameAddOn = "";
    string secondaryWeaponNearPerfectNameAddOn = "";

    string equippedWeaponUselessNameAddOn = "";
    string secondaryWeaponUselessNameAddOn = "";

    string equippedWeaponBasicallyUselessNameAddOn = "";
    string secondaryWeaponBasicallyUselessNameAddOn = "";

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);

        hotbar = GameObject.Find("Hotbar").GetComponent<Hotbar>();
    }

    void Update () {
        if (tooltip.activeSelf)
        {
            if (hotbar.weaponSlot1.transform.childCount == 3 && item.ItemName == hotbar.weaponSlot1.transform.GetChild(2).name)
            {
                tooltip.transform.position = hotbar.weaponSlot1.transform.position + new Vector3(-123, 330);
            }
            else if (hotbar.weaponSlot2.transform.childCount == 3 && item.ItemName == hotbar.weaponSlot2.transform.GetChild(2).name)
            {
                tooltip.transform.position = hotbar.weaponSlot2.transform.position + new Vector3(-123, 330);
            }
            else
            {
                tooltip.transform.position = Input.mousePosition + new Vector3(-97, 200);
            }
        }
	}

    public void Activate(Item item)
    {
        currentWeapon = hotbar.currentlyEquippedWeapon.GetComponent<Weapon>();
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

        if (item.Type == Item.ItemType.Gun)
        {
            if ((hotbar.currentlyEquippedWeaponSlot == 1 && hotbar.weaponSlot1.transform.GetChild(2).name == item.ItemName) || (hotbar.currentlyEquippedWeaponSlot == 2 && hotbar.weaponSlot2.transform.GetChild(2).name == item.ItemName))
            {
                data = "<size=22px>"
                    + equippedWeaponBasicallyUselessNameAddOn + equippedWeaponNearPerfectNameAddOn + equippedWeaponPerfectNameAddOn 
                    + equippedWeaponTaintedNameAddOn + equippedWeaponFireRateNameAddOn + equippedWeaponDamageNameAddOn + equippedWeaponAverageNameAddOn
                    + currentWeapon.name + "</size>\n\n"
                    + item.ItemDescription
                    + "\n\n" + currentWeapon.actionType
                    + "\n\nDamage: " + currentWeapon.damage
                    + equippedWeaponFireRateText
                    + "\n\nClip Size: " + item.ClipSize
                    + "\n\nAmmo Type: " + item.AmmoType;
            }
            else
            {
                data = "<size=22px>"
                    + secondaryWeaponBasicallyUselessNameAddOn + secondaryWeaponNearPerfectNameAddOn + secondaryWeaponPerfectNameAddOn 
                    + secondaryWeaponTaintedNameAddOn + secondaryWeaponFireRateNameAddOn + secondaryWeaponDamageNameAddOn + secondaryWeaponAverageNameAddOn
                    + hotbar.secondaryWeapon.name + "</size>\n\n"
                    + item.ItemDescription
                    + "\n\n" + secondaryWeapon.actionType
                    + "\n\nDamage: " + secondaryWeapon.damage
                    + secondaryWeaponFireRateText
                    + "\n\nClip Size: " + item.ClipSize
                    + "\n\nAmmo Type: " + item.AmmoType;
            }
        }
        else
        {
            data = "<size=22px>" + item.ItemName + "</size>\n\n" + item.ItemDescription;
        }
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }

    private void DetermineFireRateText()
    {
        if (item.MaxFireRate != 1)
        {
            equippedWeaponFireRateText = "\n\nFire Rate: " + currentWeapon.fireRate + " rounds per second";
            if (hotbar.secondaryWeapon != null)
            {
                secondaryWeaponFireRateText = "\n\nFire Rate: " + secondaryWeapon.fireRate + " rounds per second";
            }
        }
        else
        {
            equippedWeaponFireRateText = "";
            secondaryWeaponFireRateText = "";
        }
    }

    private void DetermineNameAddOns()
    {
        if (currentWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
        {
            equippedWeaponFireRateNameAddOn = "Rapid ";
        }
        else if (currentWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
        {
            equippedWeaponFireRateNameAddOn = "Sluggish ";
        }
        else
        {
            equippedWeaponFireRateNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
            {
                secondaryWeaponFireRateNameAddOn = "Rapid ";
            }
            else if (secondaryWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.20f) && item.MaxFireRate != 1)
            {
                secondaryWeaponFireRateNameAddOn = "Sluggish ";
            }
            else
            {
                secondaryWeaponFireRateNameAddOn = "";
            }
        }

        if (currentWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.20f))
        {
            equippedWeaponDamageNameAddOn = "Heavy-Hitting ";
        }
        else if (currentWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.20f))
        {
            equippedWeaponDamageNameAddOn = "Weak ";
        }
        else
        {
            equippedWeaponDamageNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.20f))
            {
                secondaryWeaponDamageNameAddOn = "Heavy-Hitting ";
            }
            else if (secondaryWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.20f))
            {
                secondaryWeaponDamageNameAddOn = "Weak ";
            }
            else
            {
                secondaryWeaponDamageNameAddOn = "";
            }
        }

        if (currentWeapon.fireRate >= ((item.MaxFireRate + item.MinFireRate) / 2) - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.fireRate <= ((item.MaxFireRate + item.MinFireRate) / 2) + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.damage >= ((item.MaxDamage + item.MinDamage) / 2) - ((item.MaxDamage - item.MaxDamage) * 0.05f)
            && currentWeapon.damage <= ((item.MaxDamage + item.MinDamage) / 2) + ((item.MaxDamage - item.MaxDamage) * 0.05f))
        {
            equippedWeaponFireRateNameAddOn = "";
            equippedWeaponDamageNameAddOn = "";
            equippedWeaponAverageNameAddOn = "Painfully Average ";
        }
        else
        {
            equippedWeaponAverageNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= ((item.MaxFireRate + item.MinFireRate) / 2) - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && secondaryWeapon.fireRate <= ((item.MaxFireRate + item.MinFireRate) / 2) + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && secondaryWeapon.damage >= ((item.MaxDamage + item.MinDamage) / 2) - ((item.MaxDamage - item.MinDamage) * 0.05f)
                && secondaryWeapon.damage <= ((item.MaxDamage + item.MinDamage) / 2) + ((item.MaxDamage - item.MinDamage) * 0.05f))
            {
                secondaryWeaponFireRateNameAddOn = "";
                secondaryWeaponDamageNameAddOn = "";
                secondaryWeaponAverageNameAddOn = "Painfully Average ";
            }
            else
            {
                secondaryWeaponAverageNameAddOn = "";
            }
        }

        if (currentWeapon.fireRate == 6.66f || currentWeapon.damage == 6.66f)
        {
            equippedWeaponTaintedNameAddOn = "Tainted ";
        }
        else
        {
            equippedWeaponTaintedNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == 6.66f || secondaryWeapon.damage == 6.66f)
            {
                secondaryWeaponTaintedNameAddOn = "Tainted ";
            }
            else
            {
                secondaryWeaponTaintedNameAddOn = "";
            }
        }

        if (currentWeapon.fireRate == item.MaxFireRate && currentWeapon.damage == item.MaxDamage)
        {
            equippedWeaponFireRateNameAddOn = "";
            equippedWeaponDamageNameAddOn = "";
            equippedWeaponPerfectNameAddOn = "Perfect ";
        }
        else
        {
            equippedWeaponPerfectNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == item.MaxFireRate && currentWeapon.damage == item.MaxDamage)
            {
                secondaryWeaponFireRateNameAddOn = "";
                secondaryWeaponDamageNameAddOn = "";
                secondaryWeaponPerfectNameAddOn = "Perfect ";
            }
            else
            {
                secondaryWeaponPerfectNameAddOn = "";
            }
        }
        
        if (currentWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && (currentWeapon.fireRate < item.MaxFireRate)
            && currentWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.05f)
            && currentWeapon.damage < item.MaxDamage)
        {
            equippedWeaponFireRateNameAddOn = "";
            equippedWeaponDamageNameAddOn = "";
            equippedWeaponNearPerfectNameAddOn = "Nearly Perfect ";
        }
        else
        {
            equippedWeaponNearPerfectNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate >= item.MaxFireRate - ((item.MaxFireRate - item.MinFireRate) * 0.05f)
                && (secondaryWeapon.fireRate < item.MaxFireRate)
                && secondaryWeapon.damage >= item.MaxDamage - ((item.MaxDamage - item.MinDamage) * 0.05f)
                && secondaryWeapon.damage < item.MaxDamage)
            {
                secondaryWeaponFireRateNameAddOn = "";
                secondaryWeaponDamageNameAddOn = "";
                secondaryWeaponNearPerfectNameAddOn = "Nearly Perfect ";
            }
            else
            {
                secondaryWeaponNearPerfectNameAddOn = "";
            }
        }

        if (currentWeapon.fireRate == item.MinFireRate && currentWeapon.damage == item.MinDamage)
        {
            equippedWeaponFireRateNameAddOn = "";
            equippedWeaponDamageNameAddOn = "";
            equippedWeaponUselessNameAddOn = "Useless ";
        }
        else
        {
            equippedWeaponUselessNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate == item.MinFireRate && secondaryWeapon.damage == item.MinDamage)
            {
                secondaryWeaponFireRateNameAddOn = "";
                secondaryWeaponDamageNameAddOn = "";
                secondaryWeaponUselessNameAddOn = "Useless ";
            }
            else
            {
                secondaryWeaponUselessNameAddOn = "";
            }
        }

        if (currentWeapon.fireRate > item.MinFireRate
            && currentWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && currentWeapon.damage > item.MinDamage
            && currentWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.05f))
        {
            equippedWeaponFireRateNameAddOn = "";
            equippedWeaponDamageNameAddOn = "";
            equippedWeaponBasicallyUselessNameAddOn = "Basically Useless: ";
        }
        else
        {
            equippedWeaponBasicallyUselessNameAddOn = "";
        }

        if (hotbar.secondaryWeapon != null)
        {
            if (secondaryWeapon.fireRate > item.MinFireRate
            && secondaryWeapon.fireRate <= item.MinFireRate + ((item.MaxFireRate - item.MinFireRate) * 0.05f)
            && secondaryWeapon.damage > item.MinDamage
            && secondaryWeapon.damage <= item.MinDamage + ((item.MaxDamage - item.MinDamage) * 0.05f))
            {
                secondaryWeaponFireRateNameAddOn = "";
                secondaryWeaponDamageNameAddOn = "";
                secondaryWeaponBasicallyUselessNameAddOn = "Basically Useless: ";
            }
            else
                secondaryWeaponBasicallyUselessNameAddOn = "";
        }
    }
}
