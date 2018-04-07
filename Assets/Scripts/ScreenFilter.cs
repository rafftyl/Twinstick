using UnityEngine;
using System.Collections;

/// <summary>
/// A script for applying screen filter (ScreenFilter.shader)
/// </summary>
public class ScreenFilter : Singleton<ScreenFilter>
{
	public Shader screenFilterEffect;
	public Color color = Color.red;
	public float duration = 0.1f;
	[Range(0,1)]
	public float strength = 0.4f;
	Material material;
	bool isApplied;

	public void Apply()
    {
		StartCoroutine (FilterCoroutine ());
	}

	IEnumerator FilterCoroutine()
    {
		isApplied = true;
		yield return new WaitForSeconds (duration);
		isApplied = false;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		if (isApplied)
        {
			if (material == null)
            {
				material = new Material (screenFilterEffect);
                if(material == null)
                {
                    Debug.LogWarning("Cannot initialize screen filter material!");
                }
			}
			material.SetFloat ("_Strength", strength);
			material.SetColor ("_Color", color);
			Graphics.Blit (source, destination, material);
		}
        else
        {
            Graphics.Blit(source, destination);
        }
	}
}
