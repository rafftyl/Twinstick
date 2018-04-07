using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// User interface weapon slot.
/// </summary>
public class UIWeaponSlot : MonoBehaviour
{
	/// <summary>
	/// Image overriden with weapon icon
	/// </summary>
	[Tooltip("Image overriden with weapon icon")]
	public Image iconImage;
	public Image backgroundImage;

	/// <summary>
	/// The color of the active slot.
	/// </summary>
	[Tooltip("The color of the active slot.")]
	public Color highlightColor;

	/// <summary>
	/// The color of the inactive slot.
	/// </summary>
	[Tooltip("The color of the inactive slot.")]
	public Color normalColor;

	/// <summary>
	/// Text field containing the name of the weapon
	/// </summary>
	[Tooltip("Text field containing the name of the weapon")]
	public Text weaponName;

	/// <summary>
	/// The ammo slider
	/// </summary>
	[Tooltip("The ammo slider")]
	public Slider slider;
	Weapon weapon;

	void Update()
    {
		if (weapon != null)
        {
			slider.value = weapon.CurrentAmmo;
		}
	}

	public void SetWeapon(Weapon wp)
    {
		//activate ui elements and fill them with data from the weapon
		iconImage.gameObject.SetActive (true);
		slider.gameObject.SetActive (true);
		weaponName.gameObject.SetActive (true);
		weaponName.text = wp.data.weaponName;
		weapon = wp;
		slider.maxValue = wp.data.maxAmmo;
		slider.wholeNumbers = true;
		iconImage.sprite = wp.data.icon;
	}

	public void Highlight()
    {
		iconImage.color = backgroundImage.color = highlightColor;
	}

	public void Unhighlight()
    {
		iconImage.color = backgroundImage.color = normalColor;
	}

	public void Clear()
    {
		//deactivate unnecessary ui elements
		iconImage.gameObject.SetActive (false);
		slider.gameObject.SetActive (false);
		weaponName.gameObject.SetActive (false);
		Unhighlight ();
	}
}
