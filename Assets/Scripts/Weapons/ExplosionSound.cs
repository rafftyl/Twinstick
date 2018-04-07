using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ExplosionSound : MonoBehaviour
{
	AudioSource audioSource;
	public void Activate()
    {
		audioSource = GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.Play ();
		StartCoroutine (DestroyCoroutine ());
	}

	IEnumerator DestroyCoroutine()
    {
		while (audioSource.isPlaying)
        {
			yield return new WaitForEndOfFrame ();
		}
		Destroy (gameObject);
	}
}
