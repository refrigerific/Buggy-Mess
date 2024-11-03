using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<WeaponBase> weapons = new List<WeaponBase>();
    private int currentWeaponIndex = 0;

    private void Start()
    {
        DeactivateAllWeapons(); // Ensure all weapons are inactive initially
        if (weapons.Count > 0)
        {
            EquipWeapon(0); // Equip the first weapon at the start
        }
    }

    public void AddWeapon(WeaponBase weapon)
    {
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);
            weapon.gameObject.SetActive(false); // Disable new weapon by default
            Debug.Log($"{weapon.weaponName} added to inventory!");
        }
    }

    public void RemoveWeapon(WeaponBase weapon)
    {
        if (weapons.Contains(weapon))
        {
            if (weapons[currentWeaponIndex] == weapon)
            {
                weapon.CancelReload();
                weapon.gameObject.SetActive(false); // Disable if it's the current weapon
            }
            weapons.Remove(weapon);
            Debug.Log($"{weapon.weaponName} removed from inventory!");
        }
    }

    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            // Cancel reload on the current weapon before switching
            weapons[currentWeaponIndex].CancelReload();
            weapons[currentWeaponIndex].gameObject.SetActive(false);

            // Update the current weapon index and enable the new weapon
            currentWeaponIndex = index;
            weapons[currentWeaponIndex].gameObject.SetActive(true);

            Debug.Log($"Equipped {weapons[currentWeaponIndex].weaponName}");
        }
    }

    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(nextIndex);
    }

    public void PreviousWeapon()
    {
        int previousIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(previousIndex);
    }

    public WeaponBase GetCurrentWeapon()
    {
        return weapons[currentWeaponIndex];
    }

    private void DeactivateAllWeapons()
    {
        foreach (WeaponBase weapon in weapons)
        {
            weapon.CancelReload(); // Cancel any ongoing reloads
            weapon.gameObject.SetActive(false);
        }
    }
}
