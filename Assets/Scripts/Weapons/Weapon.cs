using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all weapons in game.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
	public WeaponData data;

	/// <summary>
	/// Weapon handle to be aligned with character's weapon handle
	/// </summary>
	[Tooltip("Weapon handle to be aligned with character's weapon handle")]
	public Transform handle;

	public int MaxAmmo
    { 
		get
        { 
			return data.maxAmmo;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this weapon is picked by a character.
	/// </summary>
	/// <value><c>true</c> if this weapon is picked; otherwise, <c>false</c>.</value>
	public bool IsPicked{ get; set; }

	int currentAmmo;
	public int CurrentAmmo
    { 
		get
        {
			return currentAmmo;
		}
		set
        {
			currentAmmo = Mathf.Min (value, MaxAmmo);
		}
	}

	protected virtual void Awake()
    {
		CurrentAmmo = MaxAmmo;
	}

	void OnTriggerEnter(Collider col)
    {
		//if the weapon is lying on the ground, and the player walks over it, pick it up
		if (!IsPicked)
        {
			Player player = col.GetComponent<Player> ();
			if (player != null)
            {
				player.PickUpWeapon (this);
			}
		}
	}

	/// <summary>
	/// Called after being  equipped by  a character.
	/// </summary>
	public virtual void OnEquip()
    {
		if (!IsPicked)
        {
			IsPicked = true;
		}
	}

	/// <summary>
	/// A function defining hit geometry for a weapon
	/// </summary>
	/// <returns>The hit geometry.</returns>
	/// <param name="targetPos">Target position (mouse position in  case of player, player position in case of enemies).</param>
	protected abstract HitGeometry GetHitGeometry (Vector3 targetPos);

	public virtual void Shoot(LayerMask targetLayers, Vector3 targetPos)
    {
		//get hits
		var hits = GetHitGeometry (targetPos).GetHits<IHittable> (targetLayers);
		if (hits == null) {
			return;
		}
		//apply damage
		foreach (var hit in hits) {
			hit.Apply (data.damage);
		}
	}

	/// <summary>
	/// Highlights enemies in weapon's range.
	/// </summary>
	/// <param name="targetLayers">Target layers.</param>
	public virtual void HighlightEnemies(LayerMask targetLayers, Vector3 targetPos, Color highlightColor)
    {
		var targets = GetHitGeometry (targetPos).GetHits<EnemyAI> (targetLayers);
		if (targets == null)
        {
			return;
		}
		foreach (var target in targets)
        {
			target.Hittable.Highlight(0.4f);
		}
	}

	void Update()
    {
		if (!IsPicked)
        {
			transform.Rotate (Vector3.up * 60 * Time.deltaTime);
		}
	}
}
