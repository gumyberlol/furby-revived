using System.Collections;
using Furby;
using UnityEngine;

public class FurblingModelLoader : MonoBehaviour
{
	public AnimationClip startAnimation;

	private void Start()
	{
		StartCoroutine(UpdateModel());
	}

	private IEnumerator UpdateModel()
	{
		yield return new WaitForEndOfFrame();
		bool done = false;
		do
		{
			BabyInstance baby = GetComponentInChildren<BabyInstance>();
			if (baby == null)
			{
				yield return new WaitForEndOfFrame();
				continue;
			}
			GameObject babyObj = baby.gameObject;
			Bounds b = default(Bounds);
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer r in componentsInChildren)
			{
				b.Encapsulate(r.bounds);
			}
			base.transform.localPosition = new Vector3(0f, b.extents.y * -0.5f, -10f);
			if (startAnimation != null)
			{
				Animation anim = babyObj.GetComponentInChildren<Animation>();
				anim.clip = startAnimation;
				anim.wrapMode = WrapMode.Loop;
				anim.playAutomatically = true;
				anim.Play(startAnimation.name);
			}
			done = true;
		}
		while (!done);
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateModel());
	}
}
