using UnityEngine;
using System.Collections;

/// <summary>
/// User interface element containing weapon slots
/// </summary>
public class UIWeaponPanel : MonoBehaviour
{
	public UIWeaponSlot[] weaponSlots;
	void OnEnable()
    {
		GameEvents.OnPlayerInventoryChanged += RefreshSlots;
	}

	void OnDisable()
    {
		GameEvents.OnPlayerInventoryChanged -= RefreshSlots;
	}

	void RefreshSlots(Player player)
    {
		for (int i = 0; i < weaponSlots.Length; ++i)
        {
			if (i < player.weapons.Count)
            {
				weaponSlots [i].SetWeapon (player.weapons [i]);
				if (player.CurrentWeapon == player.weapons [i])
                {
					weaponSlots [i].Highlight(); //highlight active weapon
				}
                else
                {
					weaponSlots [i].Unhighlight();//unhighlight inactive weapon
				}
			}
            else
            {
				weaponSlots [i].Clear();//if there's no weapon, clear the slot
			}
		}
	}
}
