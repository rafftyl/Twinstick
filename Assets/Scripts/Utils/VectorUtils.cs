using UnityEngine;
using System.Collections;

public static class VectorUtils {
	public static Vector2 XZ(this Vector3 vec)
    {
		return new Vector2(vec.x, vec.z);
	}

	public static Vector3 Flat(this Vector3 vec)
    {
		return new Vector3(vec.x,0, vec.z);
	}

	public static Vector3 To3D(this Vector2 vec, float height = 0)
    {
		return new Vector3 (vec.x, height, vec.y);
	}

	public static Vector3 ParallelComponent(this Vector3 vec, Vector3  other)
    {
		return other.normalized * Vector3.Dot (vec, other);
	}

	public static Vector3 PerpendicularComponent(this Vector3 vec, Vector3 other)
    {
		return other - ParallelComponent (vec, other);
	}
		
	public static void Decompose(this Vector3  vec, Vector3 other, out Vector3 parallel, out Vector3 perpendicular)
    {
		parallel = ParallelComponent (vec, other);
		perpendicular = other - parallel;
	}

	public static Vector3 Reflect(this Vector3 vec, Vector3 other)
    {
		Vector3 par, perp;
		Decompose (vec, other, out par, out perp);
		return perp - par;
	}
}
