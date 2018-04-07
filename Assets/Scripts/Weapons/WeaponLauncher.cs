using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ranged weapon used to launch explosive projectiles
/// </summary>
public class WeaponLauncher : WeaponRanged
{
	/// <summary>
	/// Desired time between launching a projectile and hitting the ground
	/// </summary>
	[Tooltip("Desired time between launching a projectile and hitting the ground")]
	public float shotTime = 0.5f;

	public ExplosiveProjectile projectilePrefab;
	public override void Shoot (LayerMask targetLayers, Vector3 mouseTarget)
	{
		//some ugly copy-paste programming from WeaponRanged
		if (CurrentAmmo == 0)
        {
			audioSource.PlayOneShot (noAmmoClip);
			return;
		}

		CurrentAmmo--;

		if (shotParticles != null)
        {
			shotParticles.Play ();
		}

		if (audioSource != null && shotAudioClip != null)
        {
			audioSource.PlayOneShot (shotAudioClip);
		}

		//compute target for the rojectile
		Vector3 posDif = mouseTarget - barrelEnd.position;	
		if (posDif.magnitude > data.range)
        {
			mouseTarget = barrelEnd.position + posDif.normalized * data.range;
		}	

		//spawn the projectile  and compute initial velocity
		ExplosiveProjectile projectile = Instantiate<ExplosiveProjectile> (projectilePrefab);
		projectile.transform.position = barrelEnd.position;
		posDif = mouseTarget - barrelEnd.position;	
		float grav = Physics.gravity.y;

		Vector3 velocity = (posDif.XZ() / shotTime).To3D(-grav * shotTime * 0.5f + posDif.y/shotTime);

		//launch the projectile
		projectile.Launch (velocity, data.damage, targetLayers);
	}

	protected override HitGeometry GetHitGeometry (Vector3 targetPos)
	{
		//hit geometry is a sphere centered at target
		Vector3 posDif = targetPos - barrelEnd.position;	
		if (posDif.magnitude > data.range) {
			targetPos = barrelEnd.position + posDif.normalized * data.range;
		}
		return new GeometrySphere (targetPos, projectilePrefab.radius);
	}
}
