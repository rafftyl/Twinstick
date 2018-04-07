using UnityEngine;
using System.Collections;

/// <summary>
/// A singleton spawning weapons in defined spawn points
/// </summary>
public class WeaponFactory : Singleton<WeaponFactory>
{
	/// <summary>
	/// Weapons that can be spawned. 
	/// </summary>
	[Tooltip("Weapons that can be spawned.")]
	public Weapon[] weapons;

	/// <summary>
	/// Weapon spawn points.
	/// </summary>
	[Tooltip("Weapon spawn points.")]
	public Transform[] weaponSpawnPoints;

	/// <summary>
	/// Time between weapon spawn attempts
	/// </summary>
	[Tooltip("Time between weapon spawn attempts.")]
	public float weaponSpawnPeriod;

	/// <summary>
	/// Layers checked before spawning a weapon
	/// </summary>
	[Tooltip("Layers checked before spawning a weapon.")]
	public LayerMask collisionCheckLayers;
	public float collisionCheckRadius;

	protected override void Awake ()
	{
		base.Awake ();
		if (weaponSpawnPoints.Length > 0 && weapons.Length > 0)
        {
			StartCoroutine (SpawnCoroutine ());
		}
	}

	IEnumerator SpawnCoroutine()
    {
		while (true)
        {			
			yield return new WaitForSeconds (weaponSpawnPeriod);
			Transform spawnPoint = weaponSpawnPoints [Random.Range (0, weaponSpawnPoints.Length)];
			if (!Physics.CheckSphere (spawnPoint.position, collisionCheckRadius, collisionCheckLayers,QueryTriggerInteraction.Collide))
            {
				Weapon wp = weapons [Random.Range (0, weapons.Length)];
				Instantiate (wp.gameObject, spawnPoint.position, spawnPoint.rotation);
			}
		}
	}

}
