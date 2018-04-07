using UnityEngine;
using System.Collections;

/// <summary>
/// Player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : Character
{
	public float maxMovementSpeed = 10;
	public float movementAcceleration = 5;

	/// <summary>
	/// The main camera
	/// </summary>
	Camera mainCam;

	/// <summary>
	/// Attached rigidbody
	/// </summary>
	Rigidbody attachedRigidbody; 

	/// <summary>
	/// Equips the next weapon in the inventory.
	/// </summary>
	protected void EquipNextWeapon()
    {
		int nextIndex = currentWeaponIndex + 1;
		if (nextIndex == weapons.Count)
        {
			nextIndex = 0;
		}
		EquipWeapon (nextIndex);
	}

	/// <summary>
	/// Picks up a weapon.
	/// </summary>
	/// <param name="weapon">Weapon.</param>
	public void PickUpWeapon(Weapon weapon)
    {
		//check if you already have this type of weapon
		int index = weapons.FindIndex 
        (
            inventoryWeapon => 
            {
			    return  weapon.data == inventoryWeapon.data;
		    }
        );

		if (index > -1) //if you do, grab ammo and destroy the weapon
        {
			Weapon wp = weapons [index];
			if (wp.CurrentAmmo < wp.MaxAmmo)
            {
				wp.CurrentAmmo += weapon.CurrentAmmo;	
				Destroy (weapon.gameObject);
			}
		}
        else
        { //if not, pick it up
			weapons.Add (weapon);
			weapon.IsPicked = true;
			EquipWeapon (weapons.Count - 1);
		}

		//call an event (for ui to intercept)
		GameEvents.PlayerInventoryChanged (this);
	}

	protected override void EquipWeapon (int weaponIndex)
	{
		base.EquipWeapon (weaponIndex);
		GameEvents.PlayerInventoryChanged (this);
	}

	Vector3 mouseTarget;
	void FixedUpdate ()
	{
		//get axes
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		Vector3 direction = transform.forward;

		//add force in the right direction and limit player's speed after that
		attachedRigidbody.AddForce ((new Vector3 (horizontal, 0, vertical)).normalized * movementAcceleration * attachedRigidbody.mass);
		if (attachedRigidbody.velocity.magnitude > maxMovementSpeed) {
			attachedRigidbody.velocity = attachedRigidbody.velocity.normalized * maxMovementSpeed;
		}

		//set rotation based  on mouse position
		RaycastHit hit;
		if (Physics.Raycast (mainCam.ScreenPointToRay (Input.mousePosition), out hit, GameController.Instance.groundRaycastDist, GameController.Instance.groundLayers)) {
			direction = (hit.point - transform.position).Flat ().normalized;
		}
		Quaternion targetRotation = Quaternion.LookRotation (direction);

		attachedRigidbody.MoveRotation(targetRotation);
		mouseTarget = hit.point;
	}

	void Update()
    {
		if (Input.GetKeyDown (KeyCode.Q))
        {
			EquipNextWeapon ();
		}

		if (CurrentWeapon != null)
        {
			CurrentWeapon.HighlightEnemies (hittableLayers, mouseTarget, Color.red);
		}

		if (CurrentWeapon != null && Input.GetMouseButtonDown (0))
        {
			Shoot (mouseTarget);
		}
	}

	protected override void Init ()
	{
		base.Init ();
		attachedRigidbody = GetComponent<Rigidbody> ();
		//set angular drag to infinity to avoid jerky rotation
		attachedRigidbody.angularDrag = Mathf.Infinity;
		mainCam = Camera.main;
		//call hit event to initialize  ui health bar
		GameEvents.PlayerHit (this);
	}

	protected override void Die ()
	{
		base.Die ();
		GameController.Instance.ResetGame ();
	}

	public override void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		base.OnHit (damage, hitPoint, hitDirection);
		//apply red screen filter
		ScreenFilter.Instance.Apply ();
		GameEvents.PlayerHit (this);
	}
}
