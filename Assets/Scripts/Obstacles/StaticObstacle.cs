using UnityEngine;
using System.Collections;

/// <summary>
/// Static obstacle - deforms when hit. Has to contain renderers using materials with Deformable shader
/// </summary>
public class StaticObstacle : Obstacle
{
	/// <summary>
	/// Duration of deformation after being hit
	/// </summary>
	public float deformationDecayTime = 2;

	/// <summary>
	/// The deformed renderers.
	/// </summary>
	MeshRenderer[] deformedRenderers;

	bool isHit;
	float strength = 0;

	void Awake()
    {
		deformedRenderers = GetComponentsInChildren<MeshRenderer> ();
	}

	public override void OnHit (int damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		//start deforming the surface
		if (!isHit)
        {
			strength = 1;
			foreach (MeshRenderer rend in deformedRenderers)
            {
				rend.material.SetVector ("_DeformationCenter", hitPoint);
				rend.material.SetFloat ("_DeformationTimestamp", Time.time);
				rend.material.SetVector ("_DeformationDirection", hitDirection);
			}
			isHit = true;
		}
	}

	void Update()
    {
		//decrease deformation strength over time and disable deformation after deformationDecayTie
		if (isHit)
        {
			strength -= 1 / deformationDecayTime * Time.deltaTime;
			if (strength > 0)
            {
				foreach (MeshRenderer rend in deformedRenderers)
                {
					rend.material.SetFloat ("_DeformationStrength", strength);
				}
			}
            else
            {
				foreach (MeshRenderer rend in deformedRenderers)
                {
					rend.material.SetFloat ("_DeformationStrength", 0);
				}
				isHit = false;
			}
		}
	}
}
