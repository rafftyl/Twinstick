using UnityEngine;
using System.Collections;

/// <summary>
/// Explosive projectile used by WeaponLauncher.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ExplosiveProjectile : MonoBehaviour
{	
	public ParticleSystem explosionPrefab;
	public ExplosionSound explosionSoundPrefab;
	public float radius = 3;

	int damage;
	LayerMask hitMask;
	Rigidbody attachedRigidbody;

	public float Mass
    {
		get
        {
			if (attachedRigidbody == null)
            {
				attachedRigidbody = GetComponent<Rigidbody> ();
			}

			return attachedRigidbody.mass;
		}
	}

	/// <summary>
	/// Launches the projectile with the specified velocity , explosion damage and hit mask.
	/// </summary>
	/// <param name="startVel">Start velocity.</param>
	/// <param name="explosionDamage">Explosion damage.</param>
	/// <param name="hitMask">Hit mask.</param>
	public void Launch(Vector3 startVel, int explosionDamage, LayerMask hitMask)
    {
		if (attachedRigidbody == null)
        {
			attachedRigidbody = GetComponent<Rigidbody> ();
		}
		this.hitMask = hitMask;
		damage = explosionDamage;
		attachedRigidbody.velocity = startVel;
	}

	void OnTriggerEnter(Collider  col)
    {
		//check hits, apply damage,  play particles and sound
		GeometrySphere ExplosionSphere = new GeometrySphere (transform.position, radius);
		var hits = ExplosionSphere.GetHits<IHittable> (hitMask);
		foreach (var hit in hits)
        {
			hit.Apply (damage);
		}
		if (explosionPrefab != null)
        {
			ParticleSystem explosion = Instantiate<ParticleSystem> (explosionPrefab);
			explosion.transform.position = transform.position;
			explosion.Play ();
		}
		if (explosionSoundPrefab != null)
        {
			ExplosionSound sound = Instantiate<ExplosionSound> (explosionSoundPrefab);
			sound.transform.position = transform.position;
			sound.Activate ();
		}

		//destroy the projectile
		Destroy (gameObject);
	}
}
