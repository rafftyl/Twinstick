using UnityEngine;
using System.Collections;

/// <summary>
/// Unity style pseudo - singleton.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
    {
		get;
		private set;
	}

	protected virtual void Awake()
    {
		if (Instance == null)
        {
			Instance = GetComponent<T> ();
		} else
        {
			Destroy (gameObject);
		}
	}
}
