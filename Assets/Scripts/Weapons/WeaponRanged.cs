using UnityEngine;
using System.Collections;

public abstract class WeaponRanged : Weapon
{
	/// <summary>
	/// The barrel end.
	/// </summary>
	[Tooltip("The barrel end.")]
	public Transform barrelEnd;

	/// <summary>
	/// The audio source used  to  play shot sounds.
	/// </summary>
	[Tooltip("The audio source used  to  play shot sounds.")]
	public AudioSource audioSource;
	public AudioClip shotAudioClip;

	/// <summary>
	/// Audio clip played when we have no ammo
	/// </summary>
	[Tooltip("Audio clip played when we have no ammo")]
	public AudioClip noAmmoClip;

	/// <summary>
	/// Particles shown after shooting
	/// </summary>
	[Tooltip("Particles shown after shooting")]
	public ParticleSystem shotParticles;

	protected override void Awake ()
	{
		base.Awake ();
		if (shotParticles != null)
        {			
			shotParticles.transform.position = barrelEnd.position;
			shotParticles.transform.rotation = barrelEnd.rotation;
			shotParticles.transform.SetParent (barrelEnd);
		}
	}

	public override void Shoot (LayerMask targetLayers, Vector3 mouseTarget)
	{
		if (CurrentAmmo == 0)
        {
			audioSource.PlayOneShot (noAmmoClip);
			return;
		}

		base.Shoot (targetLayers, mouseTarget);
		CurrentAmmo--;
		if (shotParticles != null)
        {
			shotParticles.Play ();
		}

		if (audioSource != null && shotAudioClip != null)
        {
			audioSource.PlayOneShot (shotAudioClip);
		}
	}

	protected override HitGeometry GetHitGeometry(Vector3 targetPos)
    {
		return new GeometryRay (barrelEnd.position, (targetPos - barrelEnd.position).Flat(), data.range); 
	}
}
