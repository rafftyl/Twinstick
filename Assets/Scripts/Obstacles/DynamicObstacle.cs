using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class for dealing with hit impacts.
/// </summary>
public class HitForceInfo
{
	/// <summary>
	/// Forced applied over Duration time.
	/// </summary>
	public Vector3 Force;

	/// <summary>
	/// Force application  point in local coordinates.
	/// </summary>
	public Vector3 LocalPoint;

	/// <summary>
	/// Duration of impact.
	/// </summary>
	public float Duration;
}

/// <summary>
/// Dynamic obstacle that moves upon hit.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DynamicObstacle : Obstacle
{
	Rigidbody attachedRigidbody;
	HitForceInfo hitForceInfo;
	float timer = 0;

	void Awake()
    {
		attachedRigidbody = GetComponent<Rigidbody> ();
	}

	public override void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		//initialize force info
		hitForceInfo = new HitForceInfo {
			Force = hitDirection * damage * 2,
			LocalPoint = transform.InverseTransformPoint (hitPoint),
			Duration = 0.1f
		};
		timer = 0;
	}

	void FixedUpdate()
    {
		//if a hit has been detected
		if (hitForceInfo != null)
        {
			timer += Time.fixedDeltaTime;
			//if the collision is finished, erase hit info...
			if (timer > hitForceInfo.Duration)
            {
				hitForceInfo = null;
				return;
			}

			//...otherwise, apply force
			attachedRigidbody.AddForceAtPosition (hitForceInfo.Force, transform.TransformPoint (hitForceInfo.LocalPoint));
		}
	}
}
