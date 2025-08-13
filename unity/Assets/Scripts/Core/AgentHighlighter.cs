using UnityEngine;
using System.Collections;

public class AgentHighlighter : MonoBehaviour
{
	private Renderer _renderer;
	private Color _originalColor = Color.white;

	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		if (_renderer != null)
		{
			_originalColor = _renderer.material.color;
		}
	}

	public void Flash(bool success)
	{
		if (_renderer == null) return;
		StopAllCoroutines();
		StartCoroutine(FlashRoutine(success));
	}

	private IEnumerator FlashRoutine(bool success)
	{
		Color c = success ? Color.green : Color.red;
		_renderer.material.color = c;
		yield return new WaitForSeconds(0.25f);
		_renderer.material.color = _originalColor;
	}
}


