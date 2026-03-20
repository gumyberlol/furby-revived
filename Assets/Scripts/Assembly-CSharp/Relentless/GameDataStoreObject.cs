using System;
using System.Collections.Generic;
using Furby.Scripts.FurMail;
using UnityEngine;

namespace Relentless
{
	public class GameDataStoreObject : Singleton<GameDataStoreObject>
	{
		[SerializeField]
		private int m_numberOfSaveSlots = 3;

		private GameDataStore[] m_persistedDataStore;

		private GlobalGameDataStore m_globalPersistedDataStore;

		private int m_currentSaveSlot;

		private bool m_HasAGameLoaded;

		private GameDataStore[] PersistedDataStore
		{
			get
			{
				if (m_persistedDataStore == null)
				{
					InitializePersistedDataStore();
				}
				return m_persistedDataStore;
			}
		}

		public GlobalGameDataStore GlobalPersistedDataStore
		{
			get
			{
				if (m_globalPersistedDataStore == null)
				{
					InitializeGlobalPersistedDataStore();
				}
				return m_globalPersistedDataStore;
			}
		}

		public GameData Data
		{
			get
			{
				if (PersistedDataStore[m_currentSaveSlot].Data == null)
				{
					PersistedDataStore[m_currentSaveSlot].Load();
					PersistedDataStore[m_currentSaveSlot].Data.Loaded();
				}
				return PersistedDataStore[m_currentSaveSlot].Data;
			}
		}

		public GlobalGameData GlobalData
		{
			get
			{
				if (GlobalPersistedDataStore.Data == null)
				{
					GlobalPersistedDataStore.Load();
				}
				return GlobalPersistedDataStore.Data;
			}
		}

		public IEnumerator<GameData> GetEnumerator()
		{
			GameDataStore[] persistedDataStore = PersistedDataStore;
			foreach (GameDataStore slot in persistedDataStore)
			{
				yield return slot.Data;
			}
		}

		public void UnlockCrystal()
		{
			GlobalData.MakeSpringEligible();
			GlobalData.MakeCrystalEligible();
			GlobalData.UnlockCrystal();
			GameDataStore[] persistedDataStore = PersistedDataStore;
			foreach (GameDataStore gameDataStore in persistedDataStore)
			{
				gameDataStore.Data.Loaded();
			}
			Save();
		}

		public GameData GetSlot(int index)
		{
			return PersistedDataStore[index].Data;
		}

		public int GetNumSlots()
		{
			return m_numberOfSaveSlots;
		}

		public int GetCurrentSlotIndex()
		{
			return m_currentSaveSlot;
		}

		public bool HasAGameLoaded()
		{
			return m_HasAGameLoaded;
		}

		public void Clear()
		{
			for (int i = 0; i < m_numberOfSaveSlots; i++)
			{
				Clear(i);
				SingletonInstance<FurMailManager>.Instance.OnSaveSlotDeleted(i);
			}
		}

		public void Clear(int index)
		{
			PersistedDataStore[index].Clear();
			if (GlobalData.CrystalUnlocked)
			{
				PersistedDataStore[index].Data.Loaded();
			}
			SingletonInstance<FurMailManager>.Instance.OnSaveSlotDeleted(index);
		}

		public void InitializePersistedDataStore()
		{
			m_persistedDataStore = new GameDataStore[m_numberOfSaveSlots];
			for (int i = 0; i < m_numberOfSaveSlots; i++)
			{
				m_persistedDataStore[i] = new GameDataStore(i);
				try
				{
					m_persistedDataStore[i].Load();
					m_persistedDataStore[i].Data.Loaded();
				}
				catch (Exception exception)
				{
					Logging.LogException(exception);
					m_persistedDataStore[i] = new GameDataStore(i);
				}
				m_persistedDataStore[i].Data.TimeOfLastLoad = DateTime.Now.Ticks;
			}
		}

		public void InitializeGlobalPersistedDataStore()
		{
			m_globalPersistedDataStore = new GlobalGameDataStore();
			try
			{
				m_globalPersistedDataStore.Load();
			}
			catch (Exception exception)
			{
				Logging.LogException(exception);
				m_globalPersistedDataStore = new GlobalGameDataStore();
			}
		}

		public void Save()
		{
			if (PersistedDataStore[m_currentSaveSlot].Data != null)
			{
				try
				{
					if (PersistedDataStore[m_currentSaveSlot].Data.TimeOfLastSave < PersistedDataStore[m_currentSaveSlot].Data.TimeOfLastLoad)
					{
						PersistedDataStore[m_currentSaveSlot].Data.TimeSpentPlaying += DateTime.Now.Ticks - PersistedDataStore[m_currentSaveSlot].Data.TimeOfLastLoad;
					}
					else
					{
						PersistedDataStore[m_currentSaveSlot].Data.TimeSpentPlaying += DateTime.Now.Ticks - PersistedDataStore[m_currentSaveSlot].Data.TimeOfLastSave;
					}
					PersistedDataStore[m_currentSaveSlot].Data.TimeOfLastSave = DateTime.Now.Ticks;
					PersistedDataStore[m_currentSaveSlot].Save();
				}
				catch (Exception exception)
				{
					Logging.LogException(exception);
				}
			}
			try
			{
				if (m_globalPersistedDataStore != null)
				{
					m_globalPersistedDataStore.Data.TimeOfLastSave = DateTime.Now.Ticks;
					m_globalPersistedDataStore.Save();
				}
			}
			catch (Exception exception2)
			{
				Logging.LogException(exception2);
			}
		}

		public void SetSaveSlotIndex(int index)
		{
			int num = index;
			if (num == -1)
			{
				m_HasAGameLoaded = false;
				num = 0;
			}
			else
			{
				m_HasAGameLoaded = true;
			}
			m_currentSaveSlot = num;
			if (PersistedDataStore[m_currentSaveSlot].Data == null)
			{
				PersistedDataStore[m_currentSaveSlot].Load();
			}
			SingletonInstance<FurMailManager>.Instance.OnSaveSlotChanged(m_currentSaveSlot);
		}

		public new void OnDestroy()
		{
			base.OnDestroy();
			Save();
		}
	}
}
