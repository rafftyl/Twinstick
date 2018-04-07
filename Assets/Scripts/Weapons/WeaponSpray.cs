using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSpray : WeaponRanged
{
	public float sprayAngle;

	protected override HitGeometry GetHitGeometry (Vector3 targetPos)
	{
		//sphere fragment
		return new GeometrySubsphere (transform.position, data.range, sprayAngle, targetPos - transform.position);
	}
}
