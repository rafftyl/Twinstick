using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponMelee : Weapon
{
	protected override HitGeometry GetHitGeometry(Vector3 targetPos)
    {
		return new GeometryRay (transform.position, (targetPos - transform.position).Flat(), data.range); 
	}
}
