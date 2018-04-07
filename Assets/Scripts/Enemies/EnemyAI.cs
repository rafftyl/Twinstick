using UnityEngine;
using System.Collections;

/// <summary>
/// The base class for all the enemies
/// </summary>
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))] 
public abstract class EnemyAI : Character
{
	/// <summary>
	/// Maximum player speed that can be perceived by this character - if the player moves faster, this character will lag behind while aiming 
	/// </summary>
	[Tooltip("Maximum player speed that can be perceived by this character - if the player moves faster, this character will lag behind while aiming")]
	public float positionPerceptionSpeed = 5f;

	/// <summary>
	/// Nav Mesh Agent for navigation 
	/// </summary>
	UnityEngine.AI.NavMeshAgent navmeshAgent;

	/// <summary>
	/// This character will attack when the distance from player is less than (1 - mRangeMargin) * weaponRange 
	/// </summary>
	float rangeMargin = 0.1f;

	Vector3 perceivedPlayerPosition;
	protected virtual void Update()
	{
		Player player = GameController.Instance.player;
		perceivedPlayerPosition = Vector3.MoveTowards (perceivedPlayerPosition, player.transform.position, positionPerceptionSpeed * Time.deltaTime);
		if (CurrentWeapon != null && Vector3.Distance (player.transform.position, transform.position) > CurrentWeapon.data.range * (1 - rangeMargin))
        { 
			navmeshAgent.SetDestination (GameController.Instance.player.transform.position);
		}
        else if(!isShootingLocked)
        {
			navmeshAgent.SetDestination (transform.position);
			Shoot (perceivedPlayerPosition);
		}

		transform.rotation = Quaternion.LookRotation ((perceivedPlayerPosition - transform.position).Flat ());
	}

	/// <summary>
	/// Initializes character's parameters. Derived from charac
	/// </summary>
	protected override void Init ()
	{
		base.Init ();
		if (CurrentWeapon != null)
        {
			CurrentWeapon.CurrentAmmo = 10000;
		}
		navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		navmeshAgent.updateRotation = false;	
		navmeshAgent.Warp (transform.position);
		perceivedPlayerPosition = GameController.Instance.player.transform.position;
	}

	/// <summary>
	/// This function is called on character's death
	/// </summary>
	protected override void Die()
    {
		base.Die ();
		GameEvents.EnemyKilled (this);
		//Debug.Log ("Enemy killed");
		Destroy (gameObject);
	}
}
