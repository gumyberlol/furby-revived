using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyDataChannel : Singleton<FurbyDataChannel>
	{
		private const int kHeartbeatInterval = 15000;

		private const int kConnectInterval = 6125;

		private const int kMaxConnectionLevel = 5;
		

		
		[SerializeField]
		private Texture2D m_DebugTexture;

		private static readonly FurbyMessageType[] kMessageTypes = new FurbyMessageType[1024]
		{
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Unknown,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.EggGifting,
			FurbyMessageType.EggGifting,
			FurbyMessageType.EggGifting,
			FurbyMessageType.Command,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Unknown,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Translator,
			FurbyMessageType.Action,
			FurbyMessageType.Translator,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Translator,
			FurbyMessageType.Action,
			FurbyMessageType.Translator,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Translator,
			FurbyMessageType.Status,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Colour,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Slang,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Action,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.Translator,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.NamePhrase,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.NamePhrase,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.NamePhrase,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.Furbling,
			FurbyMessageType.Gaming,
			FurbyMessageType.Furbling,
			FurbyMessageType.Action,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Furbling,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.FurbyToFurby,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Sensor,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Furbling,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Command,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Sensor,
			FurbyMessageType.Sensor,
			FurbyMessageType.Gaming,
			FurbyMessageType.Gaming,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Action,
			FurbyMessageType.Gaming,
			FurbyMessageType.Unknown,
			FurbyMessageType.Command,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Gaming,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.LagacyPersonality,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Personality,
			FurbyMessageType.Personality,
			FurbyMessageType.Personality,
			FurbyMessageType.Personality,
			FurbyMessageType.Personality,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Unknown,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name,
			FurbyMessageType.Name
		};

		private static readonly ComAirChannel.TxWaitRxMode[] kTxWaitRxModeTypes;

		private ComAirChannel m_ComAirChannel = new ComAirChannel();

		private GameEventSubscription m_PauseEvents;

		private FurbyCommand m_ConnectTone = FurbyCommand.Application;

		private int m_ConnectionLevel;

		private long m_ConnectionTime = long.MinValue;

		private bool m_HeartBeatActive = true;

		private long m_HeartBeatTime = long.MinValue;

		private bool m_AutoConnect;

		private bool m_DisconnectDetection = true;

		private FurbyStatus m_FurbyStatus = default(FurbyStatus);

		private Queue<ComAirChannel.Tone> m_CompleteQueue = new Queue<ComAirChannel.Tone>();

		private long m_LastTone = -1L;

		private long m_LastToneTime = long.MinValue;

		private bool m_disableCommunications;

		public bool AutoConnect
		{
			get
			{
				return m_AutoConnect;
			}
			set
			{
				m_AutoConnect = value;
			}
		}

		public bool DisconnectDetection
		{
			get
			{
				return m_DisconnectDetection;
			}
			set
			{
				m_DisconnectDetection = value;
			}
		}

		public FurbyStatus FurbyStatus
		{
			get
			{
				return m_FurbyStatus;
			}
			set
			{
				m_FurbyStatus = value;
			}
		}

		public bool HeartBeatActive
		{
			get
			{
				return m_HeartBeatActive;
			}
		}

		public bool HasSynced
		{
			get
			{
				return m_FurbyStatus.Name != FurbyReceiveName.Null;
			}
		}

		public bool IsBusy
		{
			get
			{
				return m_ComAirChannel.IsBusy;
			}
		}

		public bool FurbyConnected
		{
			get
			{
				return m_ConnectionLevel > 0;
			}
		}

		public bool DisableCommunications
		{
			get
			{
				return m_disableCommunications;
			}
			set
			{
				if (m_disableCommunications != value)
				{
					Logging.Log("FurbyDataChannel.DisableCommunications = " + value);
					m_ComAirChannel.Active = !value;
					m_ConnectionLevel = 0;
					m_ConnectionTime = long.MinValue;
					m_disableCommunications = value;
				}
			}
		}

		public float ResponseInterval
		{
			get
			{
				return 0.001f * (float)(m_ComAirChannel.Ticks - m_LastToneTime);
			}
		}

		public event FurbyToneEvent ToneEvent = delegate
		{
		};

		public FurbyDataChannel()
		{
			m_FurbyStatus.Supported = true;
			m_FurbyStatus.Happyness = 50;
			m_FurbyStatus.Fullness = 50;
			m_FurbyStatus.Name = FurbyReceiveName.Null;
			m_FurbyStatus.Personality = FurbyPersonality.Gobbler;
			m_FurbyStatus.Pattern = FurbyPattern.Triangles;
			m_FurbyStatus.Sickness = 0;
		}

		static FurbyDataChannel()
		{
			ComAirChannel.TxWaitRxMode[] array = new ComAirChannel.TxWaitRxMode[1024];
			array[228] = ComAirChannel.TxWaitRxMode.Normal;
			array[229] = ComAirChannel.TxWaitRxMode.Normal;
			array[230] = ComAirChannel.TxWaitRxMode.Normal;
			array[232] = ComAirChannel.TxWaitRxMode.Normal;
			array[236] = ComAirChannel.TxWaitRxMode.Normal;
			array[350] = ComAirChannel.TxWaitRxMode.Normal;
			array[351] = ComAirChannel.TxWaitRxMode.Normal;
			array[352] = ComAirChannel.TxWaitRxMode.Normal;
			array[353] = ComAirChannel.TxWaitRxMode.Normal;
			array[354] = ComAirChannel.TxWaitRxMode.Normal;
			array[355] = ComAirChannel.TxWaitRxMode.Normal;
			array[356] = ComAirChannel.TxWaitRxMode.Normal;
			array[358] = ComAirChannel.TxWaitRxMode.Normal;
			array[359] = ComAirChannel.TxWaitRxMode.Normal;
			array[360] = ComAirChannel.TxWaitRxMode.Normal;
			array[361] = ComAirChannel.TxWaitRxMode.Normal;
			array[364] = ComAirChannel.TxWaitRxMode.Normal;
			array[365] = ComAirChannel.TxWaitRxMode.Normal;
			array[366] = ComAirChannel.TxWaitRxMode.Normal;
			array[368] = ComAirChannel.TxWaitRxMode.Normal;
			array[370] = ComAirChannel.TxWaitRxMode.Normal;
			array[371] = ComAirChannel.TxWaitRxMode.Normal;
			array[372] = ComAirChannel.TxWaitRxMode.Normal;
			array[373] = ComAirChannel.TxWaitRxMode.Normal;
			array[374] = ComAirChannel.TxWaitRxMode.Normal;
			array[376] = ComAirChannel.TxWaitRxMode.Normal;
			array[377] = ComAirChannel.TxWaitRxMode.Normal;
			array[378] = ComAirChannel.TxWaitRxMode.Normal;
			array[379] = ComAirChannel.TxWaitRxMode.Normal;
			array[380] = ComAirChannel.TxWaitRxMode.Normal;
			array[382] = ComAirChannel.TxWaitRxMode.Normal;
			array[383] = ComAirChannel.TxWaitRxMode.Normal;
			array[384] = ComAirChannel.TxWaitRxMode.Normal;
			array[385] = ComAirChannel.TxWaitRxMode.Normal;
			array[386] = ComAirChannel.TxWaitRxMode.Normal;
			array[387] = ComAirChannel.TxWaitRxMode.Normal;
			array[388] = ComAirChannel.TxWaitRxMode.Normal;
			array[389] = ComAirChannel.TxWaitRxMode.Normal;
			array[390] = ComAirChannel.TxWaitRxMode.Normal;
			array[391] = ComAirChannel.TxWaitRxMode.Normal;
			array[392] = ComAirChannel.TxWaitRxMode.Normal;
			array[393] = ComAirChannel.TxWaitRxMode.Normal;
			array[394] = ComAirChannel.TxWaitRxMode.Normal;
			array[395] = ComAirChannel.TxWaitRxMode.Normal;
			array[396] = ComAirChannel.TxWaitRxMode.Normal;
			array[397] = ComAirChannel.TxWaitRxMode.Normal;
			array[398] = ComAirChannel.TxWaitRxMode.Normal;
			array[399] = ComAirChannel.TxWaitRxMode.Normal;
			array[400] = ComAirChannel.TxWaitRxMode.Normal;
			array[401] = ComAirChannel.TxWaitRxMode.Normal;
			array[402] = ComAirChannel.TxWaitRxMode.Normal;
			array[403] = ComAirChannel.TxWaitRxMode.Normal;
			array[407] = ComAirChannel.TxWaitRxMode.Normal;
			array[408] = ComAirChannel.TxWaitRxMode.Normal;
			array[410] = ComAirChannel.TxWaitRxMode.Normal;
			array[412] = ComAirChannel.TxWaitRxMode.Normal;
			array[413] = ComAirChannel.TxWaitRxMode.Normal;
			array[414] = ComAirChannel.TxWaitRxMode.Normal;
			array[415] = ComAirChannel.TxWaitRxMode.Normal;
			array[416] = ComAirChannel.TxWaitRxMode.Normal;
			array[417] = ComAirChannel.TxWaitRxMode.Normal;
			array[418] = ComAirChannel.TxWaitRxMode.Normal;
			array[419] = ComAirChannel.TxWaitRxMode.Normal;
			array[420] = ComAirChannel.TxWaitRxMode.Normal;
			array[421] = ComAirChannel.TxWaitRxMode.Normal;
			array[422] = ComAirChannel.TxWaitRxMode.Normal;
			array[423] = ComAirChannel.TxWaitRxMode.Normal;
			array[424] = ComAirChannel.TxWaitRxMode.Normal;
			array[425] = ComAirChannel.TxWaitRxMode.Normal;
			array[427] = ComAirChannel.TxWaitRxMode.Boost;
			array[528] = ComAirChannel.TxWaitRxMode.Normal;
			array[529] = ComAirChannel.TxWaitRxMode.Normal;
			array[530] = ComAirChannel.TxWaitRxMode.Normal;
			array[531] = ComAirChannel.TxWaitRxMode.Normal;
			array[532] = ComAirChannel.TxWaitRxMode.Normal;
			array[533] = ComAirChannel.TxWaitRxMode.Normal;
			array[534] = ComAirChannel.TxWaitRxMode.Normal;
			array[535] = ComAirChannel.TxWaitRxMode.Normal;
			array[550] = ComAirChannel.TxWaitRxMode.Normal;
			array[551] = ComAirChannel.TxWaitRxMode.Normal;
			array[552] = ComAirChannel.TxWaitRxMode.Normal;
			array[553] = ComAirChannel.TxWaitRxMode.Normal;
			array[554] = ComAirChannel.TxWaitRxMode.Normal;
			array[590] = ComAirChannel.TxWaitRxMode.Normal;
			array[591] = ComAirChannel.TxWaitRxMode.Normal;
			array[592] = ComAirChannel.TxWaitRxMode.Normal;
			array[593] = ComAirChannel.TxWaitRxMode.Normal;
			array[594] = ComAirChannel.TxWaitRxMode.Normal;
			array[595] = ComAirChannel.TxWaitRxMode.Normal;
			array[596] = ComAirChannel.TxWaitRxMode.Normal;
			array[597] = ComAirChannel.TxWaitRxMode.Normal;
			array[598] = ComAirChannel.TxWaitRxMode.Normal;
			array[599] = ComAirChannel.TxWaitRxMode.Normal;
			array[600] = ComAirChannel.TxWaitRxMode.Normal;
			array[601] = ComAirChannel.TxWaitRxMode.Normal;
			array[602] = ComAirChannel.TxWaitRxMode.Normal;
			array[603] = ComAirChannel.TxWaitRxMode.Normal;
			array[604] = ComAirChannel.TxWaitRxMode.Normal;
			array[605] = ComAirChannel.TxWaitRxMode.Normal;
			array[606] = ComAirChannel.TxWaitRxMode.Normal;
			array[607] = ComAirChannel.TxWaitRxMode.Normal;
			array[608] = ComAirChannel.TxWaitRxMode.Normal;
			array[609] = ComAirChannel.TxWaitRxMode.Normal;
			array[721] = ComAirChannel.TxWaitRxMode.Normal;
			array[722] = ComAirChannel.TxWaitRxMode.Normal;
			array[723] = ComAirChannel.TxWaitRxMode.Normal;
			array[724] = ComAirChannel.TxWaitRxMode.Normal;
			array[727] = ComAirChannel.TxWaitRxMode.Normal;
			array[728] = ComAirChannel.TxWaitRxMode.Normal;
			array[729] = ComAirChannel.TxWaitRxMode.Normal;
			array[731] = ComAirChannel.TxWaitRxMode.Normal;
			array[733] = ComAirChannel.TxWaitRxMode.Normal;
			array[735] = ComAirChannel.TxWaitRxMode.Normal;
			array[745] = ComAirChannel.TxWaitRxMode.Normal;
			array[746] = ComAirChannel.TxWaitRxMode.Normal;
			array[747] = ComAirChannel.TxWaitRxMode.Normal;
			array[748] = ComAirChannel.TxWaitRxMode.Normal;
			array[749] = ComAirChannel.TxWaitRxMode.Normal;
			array[757] = ComAirChannel.TxWaitRxMode.Normal;
			array[758] = ComAirChannel.TxWaitRxMode.Normal;
			array[759] = ComAirChannel.TxWaitRxMode.Normal;
			array[767] = ComAirChannel.TxWaitRxMode.Normal;
			array[768] = ComAirChannel.TxWaitRxMode.Normal;
			array[769] = ComAirChannel.TxWaitRxMode.Normal;
			array[770] = ComAirChannel.TxWaitRxMode.Normal;
			array[771] = ComAirChannel.TxWaitRxMode.Normal;
			array[772] = ComAirChannel.TxWaitRxMode.Normal;
			array[773] = ComAirChannel.TxWaitRxMode.Normal;
			array[774] = ComAirChannel.TxWaitRxMode.Normal;
			array[775] = ComAirChannel.TxWaitRxMode.Normal;
			array[776] = ComAirChannel.TxWaitRxMode.Normal;
			array[777] = ComAirChannel.TxWaitRxMode.Normal;
			array[778] = ComAirChannel.TxWaitRxMode.Normal;
			array[779] = ComAirChannel.TxWaitRxMode.Normal;
			array[782] = ComAirChannel.TxWaitRxMode.Normal;
			array[783] = ComAirChannel.TxWaitRxMode.Normal;
			array[784] = ComAirChannel.TxWaitRxMode.Normal;
			array[785] = ComAirChannel.TxWaitRxMode.Normal;
			array[786] = ComAirChannel.TxWaitRxMode.Normal;
			array[787] = ComAirChannel.TxWaitRxMode.Normal;
			array[788] = ComAirChannel.TxWaitRxMode.Normal;
			array[789] = ComAirChannel.TxWaitRxMode.Normal;
			array[790] = ComAirChannel.TxWaitRxMode.Normal;
			array[791] = ComAirChannel.TxWaitRxMode.Normal;
			array[793] = ComAirChannel.TxWaitRxMode.Normal;
			array[795] = ComAirChannel.TxWaitRxMode.Normal;
			array[796] = ComAirChannel.TxWaitRxMode.Normal;
			array[797] = ComAirChannel.TxWaitRxMode.Normal;
			array[798] = ComAirChannel.TxWaitRxMode.Normal;
			array[799] = ComAirChannel.TxWaitRxMode.Normal;
			array[800] = ComAirChannel.TxWaitRxMode.Normal;
			array[801] = ComAirChannel.TxWaitRxMode.Normal;
			array[802] = ComAirChannel.TxWaitRxMode.Normal;
			array[803] = ComAirChannel.TxWaitRxMode.Normal;
			array[804] = ComAirChannel.TxWaitRxMode.Normal;
			array[805] = ComAirChannel.TxWaitRxMode.Normal;
			array[806] = ComAirChannel.TxWaitRxMode.Normal;
			array[807] = ComAirChannel.TxWaitRxMode.Normal;
			array[808] = ComAirChannel.TxWaitRxMode.Normal;
			array[809] = ComAirChannel.TxWaitRxMode.Normal;
			array[813] = ComAirChannel.TxWaitRxMode.Normal;
			array[814] = ComAirChannel.TxWaitRxMode.Normal;
			array[819] = ComAirChannel.TxWaitRxMode.Normal;
			array[820] = ComAirChannel.TxWaitRxMode.Normal;
			array[821] = ComAirChannel.TxWaitRxMode.Normal;
			array[822] = ComAirChannel.TxWaitRxMode.Normal;
			array[823] = ComAirChannel.TxWaitRxMode.Normal;
			array[824] = ComAirChannel.TxWaitRxMode.Normal;
			array[825] = ComAirChannel.TxWaitRxMode.Normal;
			array[826] = ComAirChannel.TxWaitRxMode.Normal;
			array[827] = ComAirChannel.TxWaitRxMode.Normal;
			array[828] = ComAirChannel.TxWaitRxMode.Normal;
			array[829] = ComAirChannel.TxWaitRxMode.Normal;
			array[830] = ComAirChannel.TxWaitRxMode.Normal;
			array[831] = ComAirChannel.TxWaitRxMode.Normal;
			array[832] = ComAirChannel.TxWaitRxMode.Normal;
			array[833] = ComAirChannel.TxWaitRxMode.Normal;
			array[834] = ComAirChannel.TxWaitRxMode.Normal;
			array[835] = ComAirChannel.TxWaitRxMode.Normal;
			array[836] = ComAirChannel.TxWaitRxMode.Normal;
			array[837] = ComAirChannel.TxWaitRxMode.Normal;
			array[838] = ComAirChannel.TxWaitRxMode.Normal;
			array[839] = ComAirChannel.TxWaitRxMode.Normal;
			array[840] = ComAirChannel.TxWaitRxMode.Normal;
			array[841] = ComAirChannel.TxWaitRxMode.Normal;
			array[842] = ComAirChannel.TxWaitRxMode.Normal;
			array[843] = ComAirChannel.TxWaitRxMode.Normal;
			array[844] = ComAirChannel.TxWaitRxMode.Normal;
			array[845] = ComAirChannel.TxWaitRxMode.Normal;
			array[848] = ComAirChannel.TxWaitRxMode.Normal;
			array[849] = ComAirChannel.TxWaitRxMode.Normal;
			array[850] = ComAirChannel.TxWaitRxMode.Normal;
			array[851] = ComAirChannel.TxWaitRxMode.Normal;
			array[852] = ComAirChannel.TxWaitRxMode.Normal;
			array[853] = ComAirChannel.TxWaitRxMode.Normal;
			array[854] = ComAirChannel.TxWaitRxMode.Normal;
			array[855] = ComAirChannel.TxWaitRxMode.Normal;
			array[856] = ComAirChannel.TxWaitRxMode.Normal;
			array[857] = ComAirChannel.TxWaitRxMode.Normal;
			array[858] = ComAirChannel.TxWaitRxMode.Normal;
			array[859] = ComAirChannel.TxWaitRxMode.Normal;
			array[860] = ComAirChannel.TxWaitRxMode.Normal;
			array[861] = ComAirChannel.TxWaitRxMode.Normal;
			array[862] = ComAirChannel.TxWaitRxMode.Normal;
			array[863] = ComAirChannel.TxWaitRxMode.Normal;
			array[864] = ComAirChannel.TxWaitRxMode.Normal;
			array[865] = ComAirChannel.TxWaitRxMode.Normal;
			array[866] = ComAirChannel.TxWaitRxMode.Normal;
			array[867] = ComAirChannel.TxWaitRxMode.Normal;
			array[868] = ComAirChannel.TxWaitRxMode.Normal;
			array[869] = ComAirChannel.TxWaitRxMode.Normal;
			array[870] = ComAirChannel.TxWaitRxMode.Normal;
			array[871] = ComAirChannel.TxWaitRxMode.Normal;
			array[872] = ComAirChannel.TxWaitRxMode.Normal;
			array[873] = ComAirChannel.TxWaitRxMode.Normal;
			array[874] = ComAirChannel.TxWaitRxMode.Normal;
			array[875] = ComAirChannel.TxWaitRxMode.Normal;
			array[876] = ComAirChannel.TxWaitRxMode.Normal;
			array[877] = ComAirChannel.TxWaitRxMode.Normal;
			array[878] = ComAirChannel.TxWaitRxMode.Normal;
			array[879] = ComAirChannel.TxWaitRxMode.Normal;
			array[880] = ComAirChannel.TxWaitRxMode.Normal;
			array[881] = ComAirChannel.TxWaitRxMode.Normal;
			array[882] = ComAirChannel.TxWaitRxMode.Normal;
			array[883] = ComAirChannel.TxWaitRxMode.Normal;
			array[884] = ComAirChannel.TxWaitRxMode.Normal;
			array[885] = ComAirChannel.TxWaitRxMode.Normal;
			array[886] = ComAirChannel.TxWaitRxMode.Normal;
			array[887] = ComAirChannel.TxWaitRxMode.Normal;
			array[888] = ComAirChannel.TxWaitRxMode.Normal;
			array[889] = ComAirChannel.TxWaitRxMode.Normal;
			array[890] = ComAirChannel.TxWaitRxMode.Normal;
			array[891] = ComAirChannel.TxWaitRxMode.Normal;
			array[892] = ComAirChannel.TxWaitRxMode.Normal;
			array[893] = ComAirChannel.TxWaitRxMode.Normal;
			array[894] = ComAirChannel.TxWaitRxMode.Normal;
			array[895] = ComAirChannel.TxWaitRxMode.Normal;
			array[897] = ComAirChannel.TxWaitRxMode.Normal;
			array[898] = ComAirChannel.TxWaitRxMode.Normal;
			kTxWaitRxModeTypes = array;
		}

		public void OnDebugPanelGUI(Enum eventType, GameObject gameObject, params object[] parameters)
		{
		}

		public void Start()
		{
			m_PauseEvents = new GameEventSubscription(OnScreenSwitched, FurbyScreenSwitchEvent.StartLevelLoad, FurbyScreenSwitchEvent.StartFadeup);
			ComAirChannel.ComAirTick += ChannelTick;
			ComAirChannel.CommsVolume = Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel;
			m_ComAirChannel.Start();
		}
		
		private void Awake()
		{
			m_disableCommunications = true;
			m_ConnectionLevel = 0;
		}

		public new void OnDestroy()
		{
			m_PauseEvents.Dispose();
			base.OnDestroy();
			m_ComAirChannel.Stop();
		}

		public void OnApplicationPause(bool pausing)
		{
			if (!DisableCommunications)
			{
				m_ComAirChannel.Active = !pausing;
			}
		}

		private void OnScreenSwitched(Enum eventType, GameObject gameObject, object[] parameters)
		{
			lock (this)
			{
				if ((FurbyScreenSwitchEvent)(object)eventType == FurbyScreenSwitchEvent.StartLevelLoad)
				{
					ClearQueue(m_CompleteQueue);
					m_ComAirChannel.Paused = true;
				}
				else
				{
					m_HeartBeatTime = Math.Min(m_HeartBeatTime, m_ComAirChannel.Ticks + 6125);
					m_ComAirChannel.Paused = false;
				}
			}
		}

		private static int ExtractBits(long bitField, int bitIndex, int bitCount)
		{
			long num = (1L << bitCount) - 1;
			bitField >>= bitIndex;
			bitField &= num;
			return Convert.ToInt32(bitField);
		}

		public static FurbyReceiveName FurbyNameSendToReceive(FurbyTransmitName name)
		{
			return EnumExtensions.Parse<FurbyReceiveName>(name.ToString());
		}

		public static FurbyTransmitName FurbyNameSendToReceive(FurbyReceiveName name)
		{
			return EnumExtensions.Parse<FurbyTransmitName>(name.ToString());
		}

		private void ChannelTick(ComAirChannel.Tone? result)
		{
			long ticks = m_ComAirChannel.Ticks;
			if (m_disableCommunications)
			{
				if (result.HasValue)
				{
					m_CompleteQueue.Enqueue(result.Value);
				}
				return;
			}
			lock (this)
			{
				if (result.HasValue)
				{
					ComAirChannel.Tone value = result.Value;
					FurbyMessageType messageType = GetMessageType(value.Inbound);
					int connectionLevel = Math.Max(0, m_ConnectionLevel - 1);
					switch (value.WaitMode)
					{
					case ComAirChannel.TxWaitRxMode.Normal:
						if (value.Acknowledged && IsResponseType(messageType))
						{
							m_LastToneTime = m_ComAirChannel.Ticks;
							m_HeartBeatTime = ticks + 15000;
							connectionLevel = 5;
						}
						m_CompleteQueue.Enqueue(value);
						break;
					case ComAirChannel.TxWaitRxMode.Boost:
						if (value.Acknowledged)
						{
							value.Inbound = SwizzleBoostFields(value.Inbound);
							FurbyScannedPersonality scanned = (FurbyScannedPersonality)ExtractBits(value.Inbound, 32, 3);
							m_FurbyStatus.Pattern = (FurbyPattern)ExtractBits(value.Inbound, 11, 6);
							m_FurbyStatus.Name = (FurbyReceiveName)ExtractBits(value.Inbound, 3, 8);
							m_FurbyStatus.Personality = PersonalityFromScanned(scanned);
							m_FurbyStatus.Sickness = ExtractBits(value.Inbound, 2, 1);
							m_FurbyStatus.Happyness = ExtractBits(value.Inbound, 28, 4) * 10;
							m_FurbyStatus.Fullness = ExtractBits(value.Inbound, 24, 4) * 10;
							m_FurbyStatus.Supported = true;
							m_FurbyStatus.Happyness = Mathf.Clamp(m_FurbyStatus.Happyness, 0, 100);
							m_FurbyStatus.Fullness = Mathf.Clamp(m_FurbyStatus.Fullness, 0, 100);
							m_LastToneTime = m_ComAirChannel.Ticks;
							m_HeartBeatTime = ticks + 15000;
							connectionLevel = 5;
						}
						m_CompleteQueue.Enqueue(value);
						break;
					default:
						connectionLevel = m_ConnectionLevel;
						break;
					}
					m_ConnectionLevel = connectionLevel;
					switch (messageType)
					{
					case FurbyMessageType.LagacyPersonality:
						m_FurbyStatus.Supported = false;
						break;
					case FurbyMessageType.FurbyToFurby:
						if (m_DisconnectDetection)
						{
							m_ConnectionLevel = Math.Max(0, m_ConnectionLevel - 1);
						}
						break;
					case FurbyMessageType.Personality:
						m_FurbyStatus.Personality = (FurbyPersonality)value.Inbound;
						m_FurbyStatus.Supported = true;
						break;
					}
					m_LastTone = value.Inbound;
				}
				else if (!FurbyConnected & AutoConnect)
				{
					if (ticks > m_ConnectionTime)
					{
						m_ConnectionTime = ticks + 6125;
						m_HeartBeatTime = ticks + 15000;
						PostMessage((int)m_ConnectTone, null);
					}
				}
				else if ((HeartBeatActive & AutoConnect) && ticks > m_HeartBeatTime)
				{
					m_HeartBeatTime = ticks + 15000;
					PostMessage(375, null);
				}
			}
		}

		private FurbyPersonality PersonalityFromScanned(FurbyScannedPersonality scanned)
		{
			FurbyPersonality result = FurbyPersonality.Base;
			switch (scanned)
			{
			case FurbyScannedPersonality.ToughGirl_ProfileScan:
				result = FurbyPersonality.ToughGirl;
				break;
			case FurbyScannedPersonality.Kooky_ProfileScan:
				result = FurbyPersonality.Kooky;
				break;
			case FurbyScannedPersonality.RockStar_ProfileScan:
				result = FurbyPersonality.Base;
				break;
			case FurbyScannedPersonality.SweetBelle_ProfileScan:
				result = FurbyPersonality.SweetBelle;
				break;
			case FurbyScannedPersonality.Gobbler_ProfileScan:
				result = FurbyPersonality.Gobbler;
				break;
			}
			return result;
		}

		public void Update()
		{
			lock (this)
			{
				foreach (ComAirChannel.Tone item in m_CompleteQueue)
				{
					if (item.Callback != null)
					{
						item.Callback(item.Acknowledged);
					}
				}
				if (ComAirChannel.Valid(m_LastTone))
				{
					FurbyMessageType messageType = GetMessageType(m_LastTone);
					FurbySensor furbySensor = (FurbySensor)m_LastTone;
					this.ToneEvent(messageType, m_LastTone);
					switch (messageType)
					{
					case FurbyMessageType.Personality:
						base.gameObject.SendGameEvent(FurbyDataEvent.PersonalityDataReceived);
						break;
					case FurbyMessageType.Sensor:
						base.gameObject.SendGameEvent(FurbyDataEvent.SensorDataReceived, furbySensor);
						base.gameObject.SendGameEvent(furbySensor);
						break;
					case FurbyMessageType.Profile:
						base.gameObject.SendGameEvent(FurbyDataEvent.FurbyDataReceived, m_FurbyStatus);
						break;
					}
					m_LastTone = -1L;
				}
				m_CompleteQueue.Clear();
			}
		}

		private static void ClearQueue(Queue<ComAirChannel.Tone> queue)
		{
			while (queue.Count != 0)
			{
				ComAirChannel.Tone tone = queue.Dequeue();
				if (tone.Callback != null)
				{
					tone.Callback(false);
				}
			}
		}

		private bool PostMessage(int data, FurbyReply callback)
		{
			ComAirChannel.TxWaitRxMode txWaitRxMode = kTxWaitRxModeTypes[data];
			ComAirChannel.Tone tone = new ComAirChannel.Tone(data, callback, txWaitRxMode);
			if (m_disableCommunications)
			{
				if (txWaitRxMode != ComAirChannel.TxWaitRxMode.None)
				{
					tone.Inbound = 0L;
					m_CompleteQueue.Enqueue(tone);
				}
				return true;
			}
			lock (this)
			{
				if (m_ComAirChannel.Transmit(tone))
				{
					return true;
				}
				return false;
			}
		}

		public void SetHeartBeatActive(bool active)
		{
			lock (this)
			{
				if (active != m_HeartBeatActive)
				{
					m_HeartBeatActive = active;
					m_HeartBeatTime = long.MinValue;
				}
			}
		}

		public void SetConnectionTone(FurbyCommand connectTone)
		{
			lock (this)
			{
				if (connectTone != m_ConnectTone)
				{
					ClearQueue(m_CompleteQueue);
					m_ConnectionLevel = 0;
					m_ConnectTone = connectTone;
					m_ConnectionTime = long.MinValue;
				}
			}
		}

		public bool PostHeartBeat()
		{
			return true;
		}

		public bool PostCommand(FurbyCommand cmdCode, FurbyReply furbyReply)
		{
			if (!AutoConnect | FurbyConnected)
			{
				return PostMessage((int)cmdCode, furbyReply);
			}
			return false;
		}

		public bool PostAction(FurbyAction msgCode, FurbyReply furbyReply)
		{
			if (!AutoConnect | FurbyConnected)
			{
				return PostMessage((int)msgCode, furbyReply);
			}
			return false;
		}

		public bool PostName(FurbyTransmitName msgCode, FurbyReply furbyReply)
		{
			if (!AutoConnect | FurbyConnected)
			{
				return PostMessage((int)msgCode, furbyReply);
			}
			return false;
		}

		public bool PostMessage(int msgCode)
		{
			if (!AutoConnect | FurbyConnected)
			{
				return PostMessage(msgCode, null);
			}
			return false;
		}

		public static FurbyMessageType GetMessageType(long code)
		{
			if (ComAirChannel.Valid(code))
			{
				if (!ComAirChannel.Boost(code))
				{
					return kMessageTypes[code & 0x3FF];
				}
				return FurbyMessageType.Profile;
			}
			return FurbyMessageType.Unknown;
		}

		public static string GetMessageName(long code)
		{
			switch (GetMessageType(code))
			{
			case FurbyMessageType.Action:
				return EnumExtensions.GetName<FurbyAction>(code);
			case FurbyMessageType.Gaming:
				return EnumExtensions.GetName<FurbyAction>(code);
			case FurbyMessageType.Personality:
				return EnumExtensions.GetName<FurbyPersonality>(code);
			case FurbyMessageType.Sensor:
				return EnumExtensions.GetName<FurbySensor>(code);
			case FurbyMessageType.Name:
				return EnumExtensions.GetName<FurbyTransmitName>(code);
			case FurbyMessageType.Command:
				return EnumExtensions.GetName<FurbyCommand>(code);
			case FurbyMessageType.Status:
				return EnumExtensions.GetName<FurbyCommand>(code);
			case FurbyMessageType.LagacyPersonality:
				return "Furby-1.0";
			case FurbyMessageType.Profile:
				return "Profile";
			case FurbyMessageType.Colour:
				return "Colour";
			default:
				return "?";
			}
		}

		public static long SwizzleBoostFields(long value)
		{
			long num = value & 0xFC003F;
			long num2 = value & -67662508096L;
			long num3 = value & 0xFC003F000L;
			return num2 | (num << 12) | (num3 >> 12);
		}

		public static bool IsResponseType(FurbyMessageType msgType)
		{
			switch (msgType)
			{
			case FurbyMessageType.Slang:
				return true;
			case FurbyMessageType.Personality:
				return true;
			case FurbyMessageType.NamePhrase:
				return true;
			default:
				return false;
			}
		}
	}
}
