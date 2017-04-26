using System;
using System.Runtime.CompilerServices;

namespace RealTimeSyncReborn.json
{
	public class weather
	{
		[CompilerGenerated]
		private weatherCoord _coord;

		[CompilerGenerated]
		private weatherSys _sys;

		[CompilerGenerated]
		private weatherWeather[] _weather;

		[CompilerGenerated]
		private string _base;

		[CompilerGenerated]
		private weatherMain _main;

		[CompilerGenerated]
		private weatherWind _wind;

		[CompilerGenerated]
		private weatherClouds _clouds;

		[CompilerGenerated]
		private int _dt;

		[CompilerGenerated]
		private int _id;

		[CompilerGenerated]
		private string _name;

		[CompilerGenerated]
		private int _cod;

		public weatherCoord coord
		{
			get;
			set;
		}

		public weatherSys sys
		{
			get;
			set;
		}

		public weatherWeather[] Weather
		{
			get;
			set;
		}

		public string @base
		{
			get;
			set;
		}

		public weatherMain main
		{
			get;
			set;
		}

		public weatherWind wind
		{
			get;
			set;
		}

		public weatherClouds clouds
		{
			get;
			set;
		}

		public int dt
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public int cod
		{
			get;
			set;
		}
	}
}
