using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for every character in game - player and his opponents
/// </summary>
public abstract class Character : MonoBehaviour, IHittable
{
	/// <summary>
	/// Maximum health points of the character
	/// </summary>
	[Tooltip("Maximum health points of the character")]
	public int maxHealth;

	/// <summary>
	/// Layers containing objects that can be hit by this character
	/// </summary>
	[Tooltip("Layers containing objects that can be hit by this character")]
	public LayerMask hittableLayers;

	/// <summary>
	/// Renderers to be highlighted when the character is hit or aimed at - materials of these renderers have to use RimHighlighted shader
	/// </summary>
	[Tooltip("Renderers to be highlighted when the character is hit or aimed at - materials of these renderers have to use RimHighlighted shader")]
	public Renderer[] highlightedRenderers;

	/// <summary>
	/// Blood particle system prefab
	/// </summary>
	[Tooltip("Blood particle system prefab")]
	public ParticleSystem bloodParticlePrefab;

	/// <summary>
	/// The transform that this character's weapon will be parented to
	/// </summary>
	[Tooltip("The transform that this character's weapon will be parented to")]
	public Transform weaponHandle;

	/// <summary>
	/// Character's weapon inventory.
	/// </summary>
	[Tooltip("Character's weapon inventory.")]
	public List<Weapon> weapons;

	/// <summary>
	/// Instanced blood particles (taken from bloodParticlePrefab)
	/// </summary>
	ParticleSystem bloodParticles;
	Animator animator;

	/// <summary>
	/// A flag used to determine if the character is ready to shoot 
	/// </summary>
	protected bool isShootingLocked = false;

	/// <summary>
	/// The index of the m current weapon.
	/// </summary>
	protected int currentWeaponIndex;

	/// <summary>
	/// Helper flag for highlighting renderers
	/// </summary>
	protected bool wasHighlighted = false;
	protected float highlightTime;
	protected float highlightTimer;


	/// <summary>
	/// Gets the current weapon.
	/// </summary>
	/// <value>The current weapon.</value>
	public Weapon CurrentWeapon
    {
		get
        {			
			return (currentWeaponIndex < weapons.Count && currentWeaponIndex >-1) ? weapons [currentWeaponIndex] : null;
		}
	}

	/// <summary>
	/// Gets or sets current health.
	/// </summary>
	/// <value>Current health.</value>
	public int CurrentHealth { get; protected set; }

	/// <summary>
	/// Equips the weapon under given index in the weapon list.
	/// </summary>
	/// <param name="weaponIndex">Weapon index.</param>
	protected virtual void EquipWeapon(int weaponIndex)
    {
		if (weaponIndex > -1 && weaponIndex < weapons.Count)
        {
			//disable current weapon
			if (CurrentWeapon != null)
            {
				CurrentWeapon.gameObject.SetActive (false);
			}
			//set, position and parent the specified weapon
			currentWeaponIndex = weaponIndex;
			CurrentWeapon.transform.rotation = weaponHandle.rotation;
			CurrentWeapon.transform.position = weaponHandle.position + (CurrentWeapon.transform.position - CurrentWeapon.handle.position);
			CurrentWeapon.transform.SetParent (weaponHandle);
			CurrentWeapon.gameObject.SetActive (true);

			//let the weapon know that it's been equipped
			CurrentWeapon.OnEquip ();
		}
	}

	/// <summary>
	/// This function is called on character's death
	/// </summary>
	protected virtual void Die ()
    {
		//Debug.Log (name + " died");
	}

	/// <summary>
	/// Highlight the renderers with highlightStrength for highlightTime.
	/// </summary>
	/// <param name="highlightStrength">Highlight strength.</param>
	/// <param name="highlightTime">Highlight time.</param>
	public void Highlight(float highlightStrength = 1, float highlightTime = 0)
    {
		//if the renderer's are not highlighted or we want to increase the highlight time
		if (!wasHighlighted || highlightTime > this.highlightTime)
        {
			foreach (Renderer r in highlightedRenderers)
            {
				r.material.SetFloat ("_HighlightStrength", highlightStrength); //_HighlightStrength is defined  in RimHighlight shader
			}
            highlightTimer = 0;
			this.highlightTime = highlightTime;
            wasHighlighted = true;
		}
	}

	/// <summary>
	/// Unhighlight the renderers
	/// </summary>
	protected void Unhighlight()
    {
		foreach (Renderer r in highlightedRenderers)
        {
			r.material.SetFloat ("_HighlightStrength", 0);
		}
	}

	/// <summary>
	/// Initializes character's parameters
	/// </summary>
	protected virtual void Init()
    {
		CurrentHealth = maxHealth;
		EquipWeapon (0); //equip the first weapon in the character's inventory
		animator = GetComponent<Animator> ();
	}

	void Start()
    {
		Init ();
	}

	/// <summary>
	/// Shoots in the direction of targetPos
	/// </summary>
	/// <param name="targetPos">Target position.</param>
	protected virtual void Shoot(Vector3 targetPos)
    {
		if (!isShootingLocked)
        {
			CurrentWeapon.Shoot (hittableLayers, targetPos);
			PlayAttackAnimation ();
			StartCoroutine (ShootLockCoroutine ());
		}
	}
		
	IEnumerator ShootLockCoroutine()
    {
		isShootingLocked = true;
		yield return new WaitForSeconds (CurrentWeapon.data.recoilTime);
		isShootingLocked = false;
	}

	/// <summary>
	/// Called when the character gets hit
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="hitPoint">Hit point.</param>
	/// <param name="hitDirection">Hit direction.</param>


	protected void PlayAttackAnimation()
    {
		if (animator != null)
        {
			animator.SetTrigger ("Attack");
		}
	}

	void LateUpdate()
    {
		if (!wasHighlighted)
        {
			Unhighlight ();
			return;
		}

		highlightTimer += Time.deltaTime;
		if (highlightTimer > highlightTime)
        {
			wasHighlighted = false;
		}
	}

	//IHittable implementation:
	public virtual void OnHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
		CurrentHealth -= damage;
		//Debug.Log (name + " has been hit for " + damage.ToString () + " damage.");
		if (CurrentHealth <= 0)
        {
			Die();
		}
		Highlight (1, 0.1f);

		//if there is a blood particle  system assigned, instantiate it's clone and play
		if (bloodParticlePrefab != null)
        {
			if (bloodParticles == null)
            {
				bloodParticles = Instantiate<ParticleSystem> (bloodParticlePrefab);
				bloodParticles.transform.SetParent (transform);
			}

			bloodParticles.transform.position = hitPoint;
			bloodParticles.transform.rotation = Quaternion.LookRotation (hitDirection);
			bloodParticles.Play ();
		}
	}
}
