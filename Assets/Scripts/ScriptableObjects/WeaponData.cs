using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Scriptable object containing basic weapon data (needed for inventory and UI purposes)
/// </summary>
[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
	public string weaponName;
	public string weaponDescription;
	public Sprite icon;
	public int damage;
	public float recoilTime = 1;
	public float range = 20;
	public int maxAmmo;
}
