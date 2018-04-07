using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A struct containg hit data.
/// </summary>
public struct Hit<T> where T : class, IHittable
{
	public Hit(T hittable, Vector3 hitPoint, Vector3  hitDir)
    {
		Hittable = hittable;
		HitPoint = hitPoint;
		HitDirection = hitDir;
	}
	public T Hittable;
	public Vector3 HitPoint;
	public Vector3 HitDirection;

	public void Apply(int damage)
    {
		Hittable.OnHit (damage, HitPoint, HitDirection);
	}
}

/// <summary>
/// Base class for hit checking
/// </summary>
public abstract class HitGeometry
{	

	protected abstract List<Hit<IHittable>> GetRaycastHits (LayerMask targetMask);

	/// <summary>
	/// Gets hittables of a specified derived type
	/// </summary>
	/// <returns>The hits.</returns>
	/// <param name="targetMask">Target mask.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual List<Hit<T>> GetHits<T>(LayerMask targetMask) where T: class, IHittable
    {
		var hits = GetRaycastHits (targetMask);
		if (hits == null)
        {
			return null;
		}

		List<Hit<T>> result =  new List<Hit<T>>();
		foreach (var hit in hits)
        {			
			if (hit.Hittable is T)
            {
				result.Add (new Hit<T>(hit.Hittable as T, hit.HitPoint, hit.HitDirection));
			}
		}

		return result;
	}
}

/// <summary>
/// Spherical hit geometry
/// </summary>
public class GeometrySphere : HitGeometry
{
	protected Vector3 center;
	protected float radius;
	public GeometrySphere(Vector3 center, float radius)
    {
		this.center = center;
		this.radius = radius;
	}
	protected override List<Hit<IHittable>> GetRaycastHits (LayerMask targetMask)
	{
		Collider[] collidersInSphere = Physics.OverlapSphere (center, radius, targetMask, QueryTriggerInteraction.Ignore);
		List<Hit<IHittable>> result = new List<Hit<IHittable>>();
		foreach (Collider col in collidersInSphere)
        {
			IHittable hittable = col.GetComponent<IHittable> ();
			if (hittable == null)
            {
				continue;
			}
			result.Add (new Hit<IHittable> (hittable, col.transform.position, (col.transform.position - center).normalized));		
		}

		return result;
	}
}

/// <summary>
/// Sphere fragment hit geometry
/// </summary>
public class GeometrySubsphere : GeometrySphere
{	
	float maxAngle;
	Vector3 forward;
	public GeometrySubsphere(Vector3 center, float radius, float maxAngle, Vector3 direction) : base(center,radius)
    {		
		this.maxAngle = maxAngle;
		forward = direction;
	}
	protected override List<Hit<IHittable>>  GetRaycastHits (LayerMask targetMask)
	{
		//Check hittables within a sphere, but also check if the angle is less then specified maximum value
		Collider[] collidersInSphere = Physics.OverlapSphere (center, radius, targetMask, QueryTriggerInteraction.Ignore);
		List<Hit<IHittable>>  result = new List<Hit<IHittable>> ();
		foreach (Collider col in collidersInSphere)
        {
			IHittable hittable = col.GetComponent<IHittable> ();
			if (hittable == null)
            {
				continue;
			}

			Vector3 posDif = col.transform.position - center;
			if (Vector3.Angle (forward, posDif) < maxAngle)
            {
				RaycastHit hit;
				if (Physics.Raycast (center, posDif,out hit, posDif.magnitude, targetMask, QueryTriggerInteraction.Ignore))
                {
					if (hit.collider == col)
                    {
						result.Add (new Hit<IHittable> (hittable, hit.point, (hit.point - center).normalized));
					}
				}
			}
		}

		return result;
	}
}

/// <summary>
/// Simple ray
/// </summary>
public class GeometryRay : HitGeometry
{
	Vector3 origin, direction;
	float range;
	public GeometryRay(Vector3 origin, Vector3 direction, float range)
    {
		this.range = range;
		this.origin = origin;
		this.direction = direction;
	}
		
	protected override List<Hit<IHittable>>  GetRaycastHits (LayerMask targetMask)
	{
		RaycastHit hit;
		if (Physics.Raycast (origin, direction, out hit, range, targetMask, QueryTriggerInteraction.Ignore))
        {
			IHittable hittable = hit.collider.GetComponent<IHittable> ();
			if (hittable == null)
            {
				return null;
			}

			return new List<Hit<IHittable>>  { new Hit<IHittable> (hittable, hit.point, direction) };
		}
		return null;
	}
}
