using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbucksWallet : Singleton<FurbucksWallet>
	{
		public delegate void BalanceChangedDelegate(FurbucksWallet wallet, int balanceDelta);

		public BalanceChangedDelegate BalanceChangedCallback;

		private float furbucksToAdd;

		private GameEventSubscription m_debugPanelSub;

		public int Balance
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.FurbucksBalance;
			}
			set
			{
				int furbucksBalance = Singleton<GameDataStoreObject>.Instance.Data.FurbucksBalance;
				Singleton<GameDataStoreObject>.Instance.Data.FurbucksBalance = value;
				Singleton<GameDataStoreObject>.Instance.Save();
				int num = value - furbucksBalance;
				SendMessage("OnBalanceChanged", num, SendMessageOptions.DontRequireReceiver);
				if (BalanceChangedCallback != null)
				{
					BalanceChangedCallback(this, num);
				}
			}
		}

		private void OnEnable()
		{
			m_debugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		public override void OnDestroy()
		{
			m_debugPanelSub.Dispose();
			base.OnDestroy();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Furbucks"))
			{
				GUILayout.Label("[Status]", RelentlessGUIStyles.Style_Header);
				GUILayout.Label("Current Balance: $" + Balance.ToString("#,#"), RelentlessGUIStyles.Style_Column);
				GUILayout.Label(string.Empty);
				GUILayout.Label("[Controls]", RelentlessGUIStyles.Style_Header);
				if (GUILayout.Button("Set to Zero", GUILayout.ExpandWidth(true)))
				{
					Balance = 0;
				}
				if (GUILayout.Button("Add 1,000", GUILayout.ExpandWidth(true)))
				{
					Balance += 1000;
				}
				if (GUILayout.Button("Add 10,000", GUILayout.ExpandWidth(true)))
				{
					Balance += 10000;
				}
				if (GUILayout.Button("Add 50,000", GUILayout.ExpandWidth(true)))
				{
					Balance += 50000;
				}
				if (GUILayout.Button("Add 100,000", GUILayout.ExpandWidth(true)))
				{
					Balance += 100000;
				}
				GUILayout.Label(string.Empty);
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("0");
				furbucksToAdd = GUILayout.HorizontalSlider(furbucksToAdd, 0f, 1000f);
				GUILayout.Label("100,000");
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Add " + (int)furbucksToAdd * 100, GUILayout.ExpandWidth(true)))
				{
					Balance += (int)furbucksToAdd * 100;
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}
	}
}
