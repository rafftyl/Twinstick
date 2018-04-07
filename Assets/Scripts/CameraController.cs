using UnityEngine;
using System.Collections;

/// <summary>
/// Camera controller.
/// </summary>
public class CameraController : MonoBehaviour
{
	/// <summary>
	/// Camera target - probably the player
	/// </summary>
	public Transform target;

	public float minCamDistance = 7;
	public float maxCamDistance = 15;

	public float minDistanceSpeed = 0;
	public float maxDistanceSpeed = 5; 

	public float maxDistanceChangeSpeed = 3; 

	/// <summary>
	/// Camera  angle
	/// </summary>
	[Range(0f, 90f)]
	public float camAngle = 70f;

	Vector3 previousTargetPos;
	float currentCamDistance;

	void SetCamPosition(float deltaTime)
    {
		//calculate target's speed and remember his position
		float targetSpeed = Vector3.Distance (target.position, previousTargetPos)/deltaTime;
		previousTargetPos = target.position;

		//the faster he moves, the more he sees
		float targetCamDistance = Mathf.Lerp (minCamDistance, maxCamDistance, (targetSpeed - minDistanceSpeed) / (maxDistanceSpeed - minDistanceSpeed));
		float distDelta = targetCamDistance - currentCamDistance;

		float minMaxDif = maxCamDistance -  minCamDistance;
		float distChangeSpeed = Mathf.Lerp(0, maxDistanceChangeSpeed, distDelta * distDelta/(minMaxDif*minMaxDif));

		currentCamDistance += Mathf.Sign (distDelta) * distChangeSpeed * deltaTime;

		transform.position = target.position + Quaternion.Euler (new Vector3 (camAngle, 0, 0)) * (Vector3.back * currentCamDistance);
		transform.LookAt (target.position);
	}

	void Start()
    {
		currentCamDistance = minCamDistance;
		previousTargetPos = target.position;
	}

	void LateUpdate()
    {
		SetCamPosition(Time.deltaTime);
	}


}
