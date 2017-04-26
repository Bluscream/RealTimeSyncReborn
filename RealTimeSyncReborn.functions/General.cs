using Microsoft.VisualBasic.CompilerServices;
using Rage;
using Rage.Native;
using RealTimeSyncReborn.My;
using System;
using System.IO;
using System.Net;

namespace RealTimeSyncReborn.functions
{
	[StandardModule]
	public sealed class General
	{
		public static string setWeather(int weatherID)
		{
			int num = int.Parse(Conversions.ToString(weatherID.ToString().ToCharArray()[0]));
			string result = "Clear";
			int num2 = 30;
			switch (num)
			{
			    case 2:
                    if (RealTimeSyncReborn.Plugin.Main.weatherStates[7])
                        World.TransitionToWeather(WeatherType.Thunder, (float)num2);
				    result = "Thunder";
				    return result;
			    case 3:
			    case 5:
			    {
				    int num3 = weatherID;
				    if (num3 == 500 || num3 == 501 || num3 == 521)
				    {
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[8])
                            World.TransitionToWeather(WeatherType.Clearing, (float)num2);
					    result = "Clearing";
					    return result;
				    }
                    if (RealTimeSyncReborn.Plugin.Main.weatherStates[6])
                        World.TransitionToWeather(WeatherType.Rain, (float)num2);
				    result = "Rain";
				    return result;
			    }
			    case 6:
			    {
				    int num4 = weatherID;
				    switch (num4)
				    {
				    case 600:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[12])
                            World.TransitionToWeather(WeatherType.Snowlight, (float)num2);
					    result = "Snowlight";
					    return result;
				    case 601:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[10])
                            World.TransitionToWeather(WeatherType.Snow, (float)num2);
					    result = "Snow";
					    return result;
				    case 602:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[13])
                            World.TransitionToWeather(WeatherType.Xmas, (float)num2);
					    result = "Xmas";
					    return result;
				    default:
					    if (num4 != 622)
                        {
                            if (RealTimeSyncReborn.Plugin.Main.weatherStates[13])
                                World.TransitionToWeather(WeatherType.Xmas, (float)num2);
						    result = "Xmas";
						    return result;
                        }
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[11])
                            World.TransitionToWeather(WeatherType.Blizzard, (float)num2);
					    result = "Blizzard";
					    return result;
				    }
				    break;
			    }
			    case 7:
                    if (RealTimeSyncReborn.Plugin.Main.weatherStates[4])
                        World.TransitionToWeather(WeatherType.Foggy, (float)num2);
				    result = "Foggy";
				    return result;
			    case 8:
				    switch (weatherID)
				    {
				    case 800:
				    case 801:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[1])
                            World.TransitionToWeather(WeatherType.Clear, (float)num2);
					    result = "Clear";
					    return result;
				    case 802:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[2])
                            World.TransitionToWeather(WeatherType.Clouds, (float)num2);
					    result = "Clouds";
					    return result;
				    case 803:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[8])
                            World.TransitionToWeather(WeatherType.Clearing, (float)num2);
					    result = "Overcast";
					    return result;
				    case 804:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[5])
                            World.TransitionToWeather(WeatherType.Overcast, (float)num2);
					    result = "Overcast";
					    return result;
				    default:
					    return result;
				    }
				    break;
			    case 9:
			    {
				    int num5 = weatherID;
				    switch (num5)
				    {
				    case 900:
				    case 901:
				    case 902:
				    case 906:
					    break;
				    case 903:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[5])
                            World.TransitionToWeather(WeatherType.Overcast, (float)num2);
					    result = "Overcast";
					    return result;
				    case 904:
                        if (RealTimeSyncReborn.Plugin.Main.weatherStates[0])
                            World.TransitionToWeather(WeatherType.ExtraSunny, (float)num2);
					    result = "ExtraSunny";
					    return result;
				    case 905:
					    goto IL_22C;
				    default:
					    switch (num5)
					    {
					    case 960:
					    case 961:
					    case 962:
						    break;
					    default:
						    goto IL_22C;
					    }
					    break;
                    }
                    if (RealTimeSyncReborn.Plugin.Main.weatherStates[7])
                        World.TransitionToWeather(WeatherType.Thunder, (float)num2);
				    result = "Thunder";
				    return result;
				    IL_22C:
                    if (RealTimeSyncReborn.Plugin.Main.weatherStates[1])
                        World.TransitionToWeather(WeatherType.Clear, (float)num2);
				    result = "Clear";
				    return result;
			    }
            }
            if (RealTimeSyncReborn.Plugin.Main.weatherStates[1])
                World.TransitionToWeather(WeatherType.Clear, (float)num2);
			result = "Clear";
			return result;
		}

		public static bool netIsOk(string address)
		{
			return MyProject.Computer.Network.IsAvailable & MyProject.Computer.Network.Ping(address);
		}

		public static string openFile(string filePath)
		{
			StreamReader expr_06 = new StreamReader(filePath);
			string result = expr_06.ReadToEnd();
			expr_06.Dispose();
			return result;
		}

		public static string openURI(string uri, ref bool errorVar)
		{
			string result;
			try
			{
				WebClient webClient = new WebClient();
				webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
				StreamReader arg_2A_0 = new StreamReader(webClient.OpenRead(uri));
				webClient.Dispose();
				result = arg_2A_0.ReadToEnd();
			}
			catch (Exception expr_32)
			{
				ProjectData.SetProjectError(expr_32);
				Exception ex = expr_32;
				errorVar = true;
				result = ex.Message;
				ProjectData.ClearProjectError();
			}
			return result;
		}

		public static void ShowLogStart()
		{
			Game.Console.Print("-----------------------------------------------------------");
			Game.Console.Print("Real Time Sync Reborn running");
			Game.Console.Print("Copyright © 2015, Sp4s12");
			Game.Console.Print("-----------------------------------------------------------");
		}

		public static void menuSelectionBot(GraphicsEventArgs e, int ID, bool[] states, Texture headerTexture, Texture[] SelectedTextures, Texture[] UnselectedTextures)
		{
			int num = 121;
			e.Graphics.DrawTexture(headerTexture, 0f, 35f, 400f, 345f);
			int num2 = 0;
			checked
			{
				do
				{
					if (ID == num2)
					{
						e.Graphics.DrawTexture(SelectedTextures[num2], 0f, 35f, 400f, 345f);
					}
					else
					{
						e.Graphics.DrawTexture(UnselectedTextures[num2], 0f, 35f, 400f, 345f);
					}
					num2++;
				}
				while (num2 <= 5);
				int num3 = 0;
				do
				{
					if (states[num3])
					{
						if (ID == num3)
						{
							e.Graphics.DrawTexture(SelectedTextures[6], 333f, (float)(num + 45 * num3), 24f, 24f);
						}
						else
						{
							e.Graphics.DrawTexture(UnselectedTextures[6], 333f, (float)(num + 45 * num3), 24f, 24f);
						}
					}
					else if (ID == num3)
					{
						e.Graphics.DrawTexture(SelectedTextures[7], 333f, (float)(num + 45 * num3), 24f, 24f);
					}
					else
					{
						e.Graphics.DrawTexture(UnselectedTextures[7], 333f, (float)(num + 45 * num3), 24f, 24f);
					}
					num3++;
				}
				while (num3 <= 3);
			}
		}

		public static void selectSyncStates(int ID, ref bool[] states)
		{
			switch (ID)
			{
			case 0:
				if (states[0])
				{
					states[0] = false;
					return;
				}
				states[0] = true;
				return;
			case 1:
				if (states[1])
				{
					states[1] = false;
					return;
				}
				states[1] = true;
				return;
			case 2:
				if (states[2])
				{
					states[2] = false;
					return;
				}
				states[2] = true;
				return;
			case 3:
				if (states[3])
				{
					states[3] = false;
					return;
				}
				states[3] = true;
				return;
			default:
				return;
			}
		}

		public static void menuDown(ref int ID)
		{
			switch (ID)
			{
			case 0:
				ID = 1;
				return;
			case 1:
				ID = 2;
				return;
			case 2:
				ID = 3;
				return;
			case 3:
				ID = 4;
				return;
			case 4:
				ID = 5;
				return;
			case 5:
				ID = 0;
				return;
			default:
				return;
			}
		}

		public static void menuUp(ref int ID)
		{
			switch (ID)
			{
			case 0:
				ID = 5;
				return;
			case 1:
				ID = 0;
				return;
			case 2:
				ID = 1;
				return;
			case 3:
				ID = 2;
				return;
			case 4:
				ID = 3;
				return;
			case 5:
				ID = 4;
				return;
			default:
				return;
			}
		}

		public static void enableGameControls()
		{
			NativeFunction.CallByName<bool>("ENABLE_ALL_CONTROL_ACTIONS", new NativeArgument[0]);
		}
	}
}
