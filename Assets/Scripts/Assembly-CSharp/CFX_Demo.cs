using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFX_Demo : MonoBehaviour
{
	public bool orderedSpawns = true;

	public float step = 1f;

	public float range = 5f;

	private float order = -5f;

	public Material groundMat;

	public Material waterMat;

	public GameObject[] ParticleExamples;

	private Dictionary<string, float> ParticlesYOffsetD = new Dictionary<string, float>
	{
		{ "CFX_ElectricGround", 0.15f },
		{ "CFX_ElectricityBall", 1f },
		{ "CFX_ElectricityBolt", 1f },
		{ "CFX_Explosion", 2f },
		{ "CFX_SmallExplosion", 1.5f },
		{ "CFX_SmokeExplosion", 2.5f },
		{ "CFX_Flame", 1f },
		{ "CFX_DoubleFlame", 1f },
		{ "CFX_Hit", 1f },
		{ "CFX_CircularLightWall", 0.05f },
		{ "CFX_LightWall", 0.05f },
		{ "CFX_Flash", 2f },
		{ "CFX_Poof", 1.5f },
		{ "CFX_Virus", 1f },
		{ "CFX_SmokePuffs", 2f },
		{ "CFX_Slash", 1f },
		{ "CFX_Splash", 0.05f },
		{ "CFX_Fountain", 0.05f },
		{ "CFX_Ripple", 0.05f },
		{ "CFX_Magic", 2f }
	};

	private int exampleIndex;

	private string randomSpawnsDelay = "0.5";

	private bool randomSpawns;

	private bool slowMo;

	private void OnMouseDown()
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (base.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
		{
			GameObject gameObject = spawnParticle();
			gameObject.transform.position = hitInfo.point + gameObject.transform.position;
		}
	}

	private GameObject spawnParticle()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(ParticleExamples[exampleIndex]);
		float y = 0f;
		foreach (KeyValuePair<string, float> item in ParticlesYOffsetD)
		{
			if (gameObject.name.StartsWith(item.Key))
			{
				y = item.Value;
				break;
			}
		}
		gameObject.transform.position = new Vector3(0f, y, 0f);
		return gameObject;
	}

	private IEnumerator RandomSpawnsCoroutine()
	{
		while (true)
		{
			GameObject particles = spawnParticle();
			if (orderedSpawns)
			{
				particles.transform.position = base.transform.position + new Vector3(order, particles.transform.position.y, 0f);
				order -= step;
				if (order < 0f - range)
				{
					order = range;
				}
			}
			else
			{
				particles.transform.position = base.transform.position + new Vector3(Random.Range(0f - range, range), 0f, Random.Range(0f - range, range)) + new Vector3(0f, particles.transform.position.y, 0f);
			}
			yield return new WaitForSeconds(float.Parse(randomSpawnsDelay));
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			prevParticle();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextParticle();
		}
	}

	private void prevParticle()
	{
		exampleIndex--;
		if (exampleIndex < 0)
		{
			exampleIndex = ParticleExamples.Length - 1;
		}
		if (ParticleExamples[exampleIndex].name.Contains("Splash") || ParticleExamples[exampleIndex].name == "CFX_Ripple" || ParticleExamples[exampleIndex].name == "CFX_Fountain")
		{
			base.renderer.material = waterMat;
		}
		else
		{
			base.renderer.material = groundMat;
		}
	}

	private void nextParticle()
	{
		exampleIndex++;
		if (exampleIndex >= ParticleExamples.Length)
		{
			exampleIndex = 0;
		}
		if (ParticleExamples[exampleIndex].name.Contains("Splash") || ParticleExamples[exampleIndex].name == "CFX_Ripple" || ParticleExamples[exampleIndex].name == "CFX_Fountain")
		{
			base.renderer.material = waterMat;
		}
		else
		{
			base.renderer.material = groundMat;
		}
	}
}
