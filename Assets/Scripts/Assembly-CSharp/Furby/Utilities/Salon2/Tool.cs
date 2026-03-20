using System;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Tool : MonoBehaviour
	{
		public delegate void Handler();

		public delegate void EffectApplicationHandler(GameObject effect);

		public delegate void ProgressAppliedHandler(ToolEffect effect, float f);

		[SerializeField]
		private ParticleSystem m_startingSparkles;

		[SerializeField]
		private Transform m_itemPrefabSpawnPoint;

		private SalonItem m_salonItem;

		private SalonStage m_stage;

		public SalonItem SalonItem
		{
			get
			{
				return m_salonItem;
			}
		}

		public SalonStage Stage
		{
			get
			{
				return m_stage;
			}
		}

		public event Handler Clicked;

		public event Handler Pressed;

		public event Handler Released;

		public event Handler ScrubbingStarted;

		public event Handler ScrubbingStopped;

		public event Handler Destroyed;

		public event EffectApplicationHandler EffectApplied;

		public event ProgressAppliedHandler ProgressApplied;

		public void SetupFrom(SalonItem item, SalonStage stage)
		{
			base.gameObject.name = string.Format("Tool: {0}", item.Name);
			m_salonItem = item;
			m_stage = stage;
			GameObject gameObject = UnityEngine.Object.Instantiate(item.Prefab) as GameObject;
			Transform transform = gameObject.transform;
			transform.parent = m_itemPrefabSpawnPoint;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.gameObject.layer = base.gameObject.layer;
			foreach (Transform item2 in transform)
			{
				item2.gameObject.layer = base.gameObject.layer;
			}
			if (item.PressParticles != null)
			{
				SetupPressParticles(gameObject, item.PressParticles);
			}
			Handler stopParticles = null;
			stopParticles = delegate
			{
				Tool tool = this;
				tool.Clicked = (Handler)Delegate.Remove(tool.Clicked, stopParticles);
				m_startingSparkles.Stop();
			};
			this.Clicked = (Handler)Delegate.Combine(this.Clicked, stopParticles);
			Handler tellStage = null;
			tellStage = delegate
			{
				Tool tool = this;
				tool.ScrubbingStarted = (Handler)Delegate.Remove(tool.ScrubbingStarted, tellStage);
				stage.OnInitialContact(base.gameObject);
			};
			this.ScrubbingStarted = (Handler)Delegate.Combine(this.ScrubbingStarted, tellStage);
			if (item.PressEvent != SalonItemUseEvent.NONE)
			{
				Handler b = delegate
				{
					base.gameObject.SendGameEvent(item.PressEvent);
				};
				Handler b2 = delegate
				{
					base.gameObject.SendGameEvent(item.ReleaseEvent);
				};
				this.Pressed = (Handler)Delegate.Combine(this.Pressed, b);
				this.Released = (Handler)Delegate.Combine(this.Released, b2);
				this.Destroyed = (Handler)Delegate.Combine(this.Destroyed, b2);
			}
		}

		private void SetupPressParticles(GameObject item, GameObject particlesPrefab)
		{
			Transform transform = null;
			Transform[] componentsInChildren = item.GetComponentsInChildren<Transform>();
			foreach (Transform transform2 in componentsInChildren)
			{
				if (transform2.gameObject.name == "VFX_locator")
				{
					transform = transform2;
				}
			}
			if (transform == null)
			{
				throw new ApplicationException(string.Format("Salon prefab {0} needs a VFX_locator node.", item.name));
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(particlesPrefab) as GameObject;
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = particleSystems;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Stop();
			}
			this.Pressed = (Handler)Delegate.Combine(this.Pressed, (Handler)delegate
			{
				ParticleSystem[] array2 = particleSystems;
				foreach (ParticleSystem particleSystem2 in array2)
				{
					particleSystem2.Play();
				}
			});
			this.Released = (Handler)Delegate.Combine(this.Released, (Handler)delegate
			{
				ParticleSystem[] array2 = particleSystems;
				foreach (ParticleSystem particleSystem2 in array2)
				{
					particleSystem2.Stop();
				}
			});
		}

		public void OnPress(bool down)
		{
			if (down)
			{
				if (this.Pressed != null)
				{
					this.Pressed();
				}
				if (this.Clicked != null)
				{
					this.Clicked();
				}
			}
			else if (this.Released != null)
			{
				this.Released();
			}
		}

		public void OnDestroy()
		{
			if (this.Destroyed != null)
			{
				this.Destroyed();
			}
		}

		public Collider GetCollider()
		{
			return base.gameObject.GetComponent<SphereCollider>();
		}

		public void OnScrubbingStart()
		{
			Stage.OnRubStart(base.gameObject);
			if (this.ScrubbingStarted != null)
			{
				this.ScrubbingStarted();
			}
		}

		public void OnScrubbingStop()
		{
			Stage.OnRubStop(base.gameObject);
			if (this.ScrubbingStopped != null)
			{
				this.ScrubbingStopped();
			}
		}

		public void OnPointScrubbed(Transform t)
		{
			ApplyEffect(t);
			Stage.OnPointRubbed(base.gameObject);
		}

		private void ApplyEffect(Transform t)
		{
			Transform transform = t.Find("Tool effect");
			if (transform != null)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
			GameObject effectPrefab = m_salonItem.EffectPrefab;
			if (effectPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(effectPrefab, t.position, t.rotation) as GameObject;
				gameObject.transform.parent = t;
				gameObject.name = "Tool effect";
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
				foreach (Transform transform2 in componentsInChildren)
				{
					transform2.gameObject.layer = t.gameObject.layer;
				}
				if (this.EffectApplied != null)
				{
					this.EffectApplied(gameObject);
				}
			}
		}

		public void Progress(Transform t, float f)
		{
			Stage.OnProgression(base.gameObject, f);
			ToolEffect toolEffect = t.gameObject.GetComponentInChildren<ToolEffect>();
			if (toolEffect == null)
			{
				GameObject progressionEffectPrefab = m_salonItem.ProgressionEffectPrefab;
				if (progressionEffectPrefab != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(progressionEffectPrefab, t.position, t.rotation) as GameObject;
					gameObject.transform.parent = t;
					toolEffect = gameObject.AddComponent<ToolEffect>();
					foreach (AnimationState item in toolEffect.animation)
					{
						item.speed = 0f;
					}
					toolEffect.animation.Play();
				}
			}
			if (!(toolEffect != null))
			{
				return;
			}
			Animation animation = toolEffect.animation;
			foreach (AnimationState item2 in animation)
			{
				item2.normalizedTime = f;
			}
			animation.Sample();
			if (this.ProgressApplied != null)
			{
				this.ProgressApplied(toolEffect, f);
			}
		}
	}
}
