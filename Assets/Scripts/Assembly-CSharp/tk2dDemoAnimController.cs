using System.Collections;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Demo/tk2dDemoAnimController")]
public class tk2dDemoAnimController : MonoBehaviour
{
	private tk2dAnimatedSprite animSprite;

	public tk2dTextMesh popupTextMesh;

	private void Start()
	{
		animSprite = GetComponent<tk2dAnimatedSprite>();
		animSprite.animationEventDelegate = AnimationEventDelegate;
		popupTextMesh.gameObject.SetActive(false);
	}

	private void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		string text = sprite.name + "\n" + clip.name + "\nINFO: " + frame.eventInfo;
		StartCoroutine(PopupText(text));
	}

	private IEnumerator PopupText(string text)
	{
		popupTextMesh.text = text;
		popupTextMesh.Commit();
		popupTextMesh.gameObject.SetActive(true);
		float fadeTime = 1f;
		Color c1 = popupTextMesh.color;
		Color c2 = popupTextMesh.color2;
		for (float f = 0f; f < fadeTime; f += Time.deltaTime)
		{
			c2.a = (c1.a = Mathf.Clamp01(2f * (1f - f / fadeTime)));
			popupTextMesh.color = c1;
			popupTextMesh.color2 = c2;
			popupTextMesh.Commit();
			yield return 0;
		}
		popupTextMesh.gameObject.SetActive(false);
	}
}
