using UnityEngine;
using System.Collections;

/// <summary>
/// Explosive obstacle that explodes after being hit.
/// </summary>
public class ExplosiveObstacle : DynamicObstacle
{
	/// <summary>
	/// Layers affected by explosion
	/// </summary>
	[Tooltip("Layers affected by explosion")]
	public LayerMask hitMask;

	/// <summary>
	/// The explosion particle system prefab.
	/// </summary>
	[Tooltip("The explosion particle system prefab.")]
	public ParticleSystem explosionPrefab;

	/// <summary>
	/// The explosion sound prefab.
	/// </summary>
	[Tooltip("The explosion sound prefab.")]
	public ExplosionSound explosionSoundPrefab;

	/// <summary>
	/// Fire particle system.
	/// </summary>
	[Tooltip("Fire particle system.")]
	public ParticleSystem fireParticles;

	/// <summary>
	/// The explosion radius.
	/// </summary>
	[Tooltip("The explosion radius.")]
	public float explosionRadius = 3;

	/// <summary>
	/// Delay between being hit and exploding
	/// </summary>
	[Tooltip("Delay between being hit and exploding")]
	public float explosionTime = 3;

	/// <summary>
	/// Explosion damage.
	/// </summary>
	[Tooltip("Explosion damage")]
	public int explosionDamage = 50;

	bool isExplosionStarted = false;
	public override void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		base.OnHit (damage, hitPoint, hitDirection);
		//enable fire particles
		if (!isExplosionStarted)
        {
			if (fireParticles != null)
            {
				fireParticles.Play ();
			}
			isExplosionStarted = true;
			StartCoroutine (ExplosionCoroutine ());
		}
	}

	IEnumerator ExplosionCoroutine()
    {
		//wait for the time  specified
		yield return new WaitForSeconds (explosionTime);

		//get hittables within a sphere
		GeometrySphere ExplosionSphere = new GeometrySphere (transform.position, explosionRadius);
		var hits = ExplosionSphere.GetHits<IHittable> (hitMask);

		//apply hits to each hittable
		foreach (var hit in hits)
        {
			hit.Apply (explosionDamage);
		}

		//if there's an explosion prefab, do your thing
		if (explosionPrefab != null)
        {
			ParticleSystem explosion = Instantiate<ParticleSystem> (explosionPrefab);
			explosion.transform.position = transform.position;
			explosion.Play ();
		}

		//if there's a sound source prefab, instantiate it and play the sound
		if (explosionSoundPrefab != null)
        {
			ExplosionSound sound = Instantiate<ExplosionSound> (explosionSoundPrefab);
			sound.transform.position = transform.position;
			sound.Activate ();
		}

		//destroy yourself
		Destroy (gameObject);
	}
}
