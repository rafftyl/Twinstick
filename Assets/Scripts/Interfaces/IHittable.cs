using UnityEngine;
using System.Collections;

//The interface used by each hittable object - player, enemy or obstacle
public interface IHittable
{
	/// <summary>
	/// Behaviour after being hit
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="hitPoint">Hit point.</param>
	/// <param name="hitDirection">Hit direction.</param>
	void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection);
}
