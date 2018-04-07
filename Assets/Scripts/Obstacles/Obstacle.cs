using UnityEngine;
using System.Collections;

public abstract class Obstacle : MonoBehaviour,IHittable
{
	public abstract void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection);
}
