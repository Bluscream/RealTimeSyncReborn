using Microsoft.VisualBasic.CompilerServices;
using Rage;
using Rage.Native;
using RealTimeSyncReborn.functions;
using RealTimeSyncReborn.json;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace RealTimeSyncReborn.Plugin
{
	[StandardModule]
	public sealed class Main
	{
		private static string apiUrl = null;

		private static string updateUrl = "https://goo.gl/lUxv6N";

		private static string PluginVersion = "1.1a";

		private static string WeatherFreq;

		private static int CheckUpdatesFreq = 1800000;

		private static Keys menuKey;

		private static bool timeSyncStatus = false;

		private static bool weatherSyncStatus = false;

		private static bool UpdatesCheckingStatus = false;

		private static bool showMenu;

		private static int menuID;

        public static bool[] weatherStates = new bool[14];

		private static bool[] menuStates = new bool[4];

		private static InitializationFile settings = new InitializationFile("Plugins\\RealTimeSyncReborn.ini");

		private static Thread updateThread;

		private static GameFiber updateGamefiber;

		private static Thread weatherThread;

		private static GameFiber weatherGamefiber;

		private static GameFiber timeGamefiber;

		private static string updateWebResult;

		private static string weatherWebResult;

		private static bool internetDownloadsError = false;

		private static Texture header = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\header.png");

		private static Texture TimeSyncSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\TimeSyncSelected.png");

		private static Texture TimeSyncUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\TimeSyncUnselected.png");

		private static Texture WeatherSyncSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\WeatherSyncSelected.png");

		private static Texture WeatherSyncUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\WeatherSyncUnselected.png");

		private static Texture NotificationsDisplaySelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\NotificationDisplaySelected.png");

		private static Texture NotificationsDisplayUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\NotificationDisplayUnselected.png");

		private static Texture UpdateCheckingSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\UpdateCheckingSelected.png");

		private static Texture UpdateCheckingUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\UpdateCheckingUnselected.png");

		private static Texture ApplySelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\ApplySelected.png");

		private static Texture ApplyUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\ApplyUnselected.png");

		private static Texture ExitSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\ExitSelected.png");

		private static Texture ExitUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\ExitUnselected.png");

		private static Texture CheckSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\CheckSelected.png");

		private static Texture CheckUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\CheckUnselected.png");

		private static Texture CrossSelected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\CrossSelected.png");

		private static Texture CrossUnselected = Game.CreateTextureFromFile("Plugins\\RealTimeSyncReborn\\imgs\\menu\\CrossUnselected.png");

		private static Texture[] selectedTextures = new Texture[]
		{
			RealTimeSyncReborn.Plugin.Main.TimeSyncSelected,
			RealTimeSyncReborn.Plugin.Main.WeatherSyncSelected,
			RealTimeSyncReborn.Plugin.Main.NotificationsDisplaySelected,
			RealTimeSyncReborn.Plugin.Main.UpdateCheckingSelected,
			RealTimeSyncReborn.Plugin.Main.ApplySelected,
			RealTimeSyncReborn.Plugin.Main.ExitSelected,
			RealTimeSyncReborn.Plugin.Main.CheckSelected,
			RealTimeSyncReborn.Plugin.Main.CrossSelected
		};

		private static Texture[] UnselectedTextures = new Texture[]
		{
			RealTimeSyncReborn.Plugin.Main.TimeSyncUnselected,
			RealTimeSyncReborn.Plugin.Main.WeatherSyncUnselected,
			RealTimeSyncReborn.Plugin.Main.NotificationsDisplayUnselected,
			RealTimeSyncReborn.Plugin.Main.UpdateCheckingUnselected,
			RealTimeSyncReborn.Plugin.Main.ApplyUnselected,
			RealTimeSyncReborn.Plugin.Main.ExitUnselected,
			RealTimeSyncReborn.Plugin.Main.CheckUnselected,
			RealTimeSyncReborn.Plugin.Main.CrossUnselected
		};

		private static void main()
		{
			try
			{
				General.ShowLogStart();
				if (RealTimeSyncReborn.Plugin.Main.settings.Exists())
				{
                    RealTimeSyncReborn.Plugin.Main.apiUrl = RealTimeSyncReborn.Plugin.Main.settings.ReadString("WEATHER SETTINGS", "APIURL", "http://api.openweathermap.org/data/2.5/weather?q={CITY}&units={Metric/Imperial}&APPID={OpenWeatherMap AppID}");
					RealTimeSyncReborn.Plugin.Main.WeatherFreq = Conversions.ToString(RealTimeSyncReborn.Plugin.Main.settings.ReadInt32("WEATHER SETTINGS", "NOTIFICATIONS_FREQ", 10));
                    RealTimeSyncReborn.Plugin.Main.weatherStates[0] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "ExtraSunny", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[1] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Clear", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[2] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Clouds", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[3] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Smog", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[4] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Foggy", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[5] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Overcast", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[6] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Rain", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[7] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Thunder", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[8] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Clearing", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[9] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Neutral", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[10] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Snow", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[11] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Blizzard", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[12] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Snowlight", true);
                    RealTimeSyncReborn.Plugin.Main.weatherStates[13] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("WEATHER TYPES", "Xmas", true);
                    RealTimeSyncReborn.Plugin.Main.menuStates[1] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("GENERAL SETTINGS", "WeatherSync", true);
					RealTimeSyncReborn.Plugin.Main.menuStates[0] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("GENERAL SETTINGS", "TimeSync", true);
					RealTimeSyncReborn.Plugin.Main.menuStates[2] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("GENERAL SETTINGS", "ShowNotifications", true);
					RealTimeSyncReborn.Plugin.Main.menuStates[3] = RealTimeSyncReborn.Plugin.Main.settings.ReadBoolean("GENERAL SETTINGS", "CheckForUpdates", true);
					try
					{
						RealTimeSyncReborn.Plugin.Main.menuKey = (Keys)Conversions.ToInteger(new KeysConverter().ConvertFromInvariantString(RealTimeSyncReborn.Plugin.Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5")));
					}
					catch (Exception expr_12D)
					{
						ProjectData.SetProjectError(expr_12D);
						Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", RealTimeSyncReborn.Plugin.Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5") + " is not recognized as key! ~n~Menu key set to NumPad5 by default");
						Game.LogVerbose("ERROR : " + RealTimeSyncReborn.Plugin.Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5") + " is not recognized as key! ~n~Menu key set to NumPad5 by default");
						RealTimeSyncReborn.Plugin.Main.menuKey = Keys.NumPad5;
						ProjectData.ClearProjectError();
					}
					Game.Console.Print("Current version : " + RealTimeSyncReborn.Plugin.Main.PluginVersion);
					Game.Console.Print("Up to date version : " + General.openURI(RealTimeSyncReborn.Plugin.Main.updateUrl, ref RealTimeSyncReborn.Plugin.Main.internetDownloadsError));
					Game.Console.Print("FREQ : " + RealTimeSyncReborn.Plugin.Main.WeatherFreq);
					Game.Console.Print("WeatherSync : " + RealTimeSyncReborn.Plugin.Main.menuStates[1].ToString());
					Game.Console.Print("TimeSync : " + RealTimeSyncReborn.Plugin.Main.menuStates[0].ToString());
					Game.Console.Print("ShowNotifications : " + RealTimeSyncReborn.Plugin.Main.menuStates[2].ToString());
					Game.Console.Print("CheckForUpdates : " + RealTimeSyncReborn.Plugin.Main.menuStates[3].ToString());
					Game.Console.Print("OpenMenu key : " + RealTimeSyncReborn.Plugin.Main.menuKey.ToString());
                    Game.Console.Print($"API URL: {RealTimeSyncReborn.Plugin.Main.apiUrl}");
                    Game.Console.Print("-----------------------------------------------------------");
				}
				else
				{
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong with your config file!~n~File doesn't exist!");
				}
				try
				{
					RealTimeSyncReborn.Plugin.Main.WeatherFreq = Conversions.ToString(checked(Conversions.ToInteger(RealTimeSyncReborn.Plugin.Main.WeatherFreq) * 60000));
					if (Conversions.ToDouble(RealTimeSyncReborn.Plugin.Main.WeatherFreq) < 10.0)
					{
						RealTimeSyncReborn.Plugin.Main.WeatherFreq = Conversions.ToString(10);
					}
				}
				catch (Exception expr_36E)
				{
					ProjectData.SetProjectError(expr_36E);
					Exception ex = expr_36E;
					RealTimeSyncReborn.Plugin.Main.WeatherFreq = "600000";
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong with your config file!");
					Game.LogVerbose("ERROR : WeatherFreq is NaN, " + ex.Message);
					ProjectData.ClearProjectError();
				}
				Game.FrameRender += new EventHandler<GraphicsEventArgs>(RealTimeSyncReborn.Plugin.Main.GameFrameRender);
				RealTimeSyncReborn.Plugin.Main.showMenu = false;
				RealTimeSyncReborn.Plugin.Main.menuID = 0;
				GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.voidLoop), "VoidLoop");
				if (RealTimeSyncReborn.Plugin.Main.menuStates[3])
				{
					RealTimeSyncReborn.Plugin.Main.updateGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.CheckUpdate), "CheckUpdate");
				}
				if (RealTimeSyncReborn.Plugin.Main.menuStates[1])
				{
					RealTimeSyncReborn.Plugin.Main.weatherGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.WeatherLoop), "WeatherLoop");
				}
				if (RealTimeSyncReborn.Plugin.Main.menuStates[0])
				{
					RealTimeSyncReborn.Plugin.Main.timeGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.TimeLoop), "TimeLoop");
				}
			}
			catch (Exception expr_49C)
			{
				ProjectData.SetProjectError(expr_49C);
				Exception ex2 = expr_49C;
				Game.LogVerbose("ERROR : " + ex2.Message);
				ProjectData.ClearProjectError();
			}
		}

		private static void voidLoop()
		{
			try
			{
				GameFiber.Sleep(100000000);
			}
			catch (Exception expr_0C)
			{
				ProjectData.SetProjectError(expr_0C);
				Exception ex = expr_0C;
				Game.LogVerbose("VoidLoop : " + ex.Message);
				ProjectData.ClearProjectError();
			}
		}

		private static void updateGather()
		{
			RealTimeSyncReborn.Plugin.Main.updateWebResult = General.openURI(RealTimeSyncReborn.Plugin.Main.updateUrl, ref RealTimeSyncReborn.Plugin.Main.internetDownloadsError);
		}

		private static void weatherGather()
		{
			WebClient expr_17 = new WebClient();
			RealTimeSyncReborn.Plugin.Main.weatherWebResult = expr_17.DownloadString(RealTimeSyncReborn.Plugin.Main.apiUrl);
			expr_17.Dispose();
		}

		private static void CheckUpdate()
		{
			try
			{
				while (true)
				{
					RealTimeSyncReborn.Plugin.Main.UpdatesCheckingStatus = true;
					if (!RealTimeSyncReborn.Plugin.Main.menuStates[3])
					{
						break;
					}
					RealTimeSyncReborn.Plugin.Main.updateThread = new Thread(new ThreadStart(RealTimeSyncReborn.Plugin.Main.updateGather));
					RealTimeSyncReborn.Plugin.Main.updateThread.IsBackground = true;
					if (General.netIsOk("www.google.com"))
					{
						RealTimeSyncReborn.Plugin.Main.updateThread.Start();
						while (RealTimeSyncReborn.Plugin.Main.updateThread.IsAlive)
						{
							GameFiber.Sleep(1);
						}
						if (RealTimeSyncReborn.Plugin.Main.internetDownloadsError)
						{
							Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Update version info can't be downloaded! (more info in console log)");
							Game.LogVerbose("ERROR : " + RealTimeSyncReborn.Plugin.Main.updateWebResult);
						}
						else if (Operators.CompareString(RealTimeSyncReborn.Plugin.Main.updateWebResult, RealTimeSyncReborn.Plugin.Main.PluginVersion, false) != 0)
						{
							Game.DisplayNotification("char_bugstars", "char_bugstars", "~g~~h~Real Time Sync Reborn", "~g~~h~INFO", string.Concat(new string[]
							{
								"New version (~g~",
								RealTimeSyncReborn.Plugin.Main.updateWebResult,
								"~s~) available!~n~Installed version : ~r~",
								RealTimeSyncReborn.Plugin.Main.PluginVersion,
								"~n~~s~Download it on www.lcpdfr.com!"
							}));
							Game.LogVerbose(string.Concat(new string[]
							{
								"New version (",
								RealTimeSyncReborn.Plugin.Main.updateWebResult,
								") is available! Installed version : ",
								RealTimeSyncReborn.Plugin.Main.PluginVersion,
								". Download the new version on www.lcpdfr.com!"
							}));
						}
						RealTimeSyncReborn.Plugin.Main.internetDownloadsError = false;
						GameFiber.Sleep(RealTimeSyncReborn.Plugin.Main.CheckUpdatesFreq);
					}
					else
					{
						Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! It seems you're not connected to the internet!");
					}
				}
			}
			catch (Exception expr_174)
			{
				ProjectData.SetProjectError(expr_174);
				Exception ex = expr_174;
				RealTimeSyncReborn.Plugin.Main.UpdatesCheckingStatus = false;
				Game.LogVerbose("CheckUpdate : " + ex.Message);
				ProjectData.ClearProjectError();
			}
		}

		private static void WeatherLoop()
		{
			checked
			{
				try
				{
					while (true)
					{
						RealTimeSyncReborn.Plugin.Main.weatherSyncStatus = true;
						if (!RealTimeSyncReborn.Plugin.Main.menuStates[1])
						{
							break;
						}
						if (General.netIsOk("www.google.com"))
						{
							int num = (int)NativeFunction.CallByName<uint>("GET_CLOCK_HOURS", new NativeArgument[0]);
							int num2 = (int)NativeFunction.CallByName<uint>("GET_CLOCK_MINUTES", new NativeArgument[0]);
							string text = "°";
							string text2;
							if (num < 10)
							{
								text2 = "0" + num.ToString();
							}
							else
							{
								text2 = num.ToString();
							}
							string text3;
							if (num2 < 10)
							{
								text3 = "0" + num2.ToString();
							}
							else
							{
								text3 = num2.ToString();
							}
							JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
							RealTimeSyncReborn.Plugin.Main.weatherThread = new Thread(new ThreadStart(RealTimeSyncReborn.Plugin.Main.weatherGather));
							RealTimeSyncReborn.Plugin.Main.weatherThread.IsBackground = true;
							RealTimeSyncReborn.Plugin.Main.weatherThread.Start();
							while (RealTimeSyncReborn.Plugin.Main.weatherThread.IsAlive)
							{
								GameFiber.Sleep(1);
							}
							if (!RealTimeSyncReborn.Plugin.Main.weatherWebResult.Contains("Error"))
							{
								weather weather = javaScriptSerializer.Deserialize<weather>(RealTimeSyncReborn.Plugin.Main.weatherWebResult);
								if (RealTimeSyncReborn.Plugin.Main.menuStates[2])
								{
									Game.DisplayNotification("web_fruit", "web_fruit", "~y~~h~WEATHER CHANNEL", "~y~~h~FORECAST", string.Concat(new string[]
									{
										"It's ",
										text2,
										":",
										text3,
										"~n~",
										weather.name,
										", ",
										weather.sys.country,
										", ",
										weather.Weather[0].description,
										", ",
										Math.Round(weather.main.temp).ToString(),
										text,
										" ~n~Game weather set to ",
										General.setWeather(weather.Weather[0].id)
									}));
								}
								Game.Console.Print(string.Concat(new string[]
								{
									"WEATHER CHANNEL FORECAST It's ",
									text2,
									":",
									text3,
									", ",
									weather.name,
									", ",
									weather.sys.country,
									", ",
									weather.Weather[0].description,
									", ",
									Math.Round(weather.main.temp).ToString(),
									text,
									", Game weather set to ",
									General.setWeather(weather.Weather[0].id)
								}));
							}
							else
							{
								Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Location can't be found!");
							}
						}
						GameFiber.Sleep(Conversions.ToInteger(RealTimeSyncReborn.Plugin.Main.WeatherFreq));
					}
				}
				catch (Exception expr_2FB)
				{
					ProjectData.SetProjectError(expr_2FB);
					Exception ex = expr_2FB;
					RealTimeSyncReborn.Plugin.Main.weatherSyncStatus = false;
					Game.LogVerbose("WeatherLoop : " + ex.Message);
					if (ex.HResult == -2146233079)
					{
						Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Weather feature needs internet to work properly!");
					}
					ProjectData.ClearProjectError();
				}
			}
		}

		private static void TimeLoop()
		{
			try
			{
				while (true)
				{
					RealTimeSyncReborn.Plugin.Main.timeSyncStatus = true;
					if (!RealTimeSyncReborn.Plugin.Main.menuStates[0])
					{
						break;
					}
					NativeFunction.CallByName<uint>("SET_CLOCK_TIME", new NativeArgument[]
					{
						DateTime.Now.Hour,
						DateTime.Now.Minute,
						DateTime.Now.Second
					});
					GameFiber.Sleep(100);
				}
			}
			catch (Exception expr_6A)
			{
				ProjectData.SetProjectError(expr_6A);
				Exception ex = expr_6A;
				RealTimeSyncReborn.Plugin.Main.timeSyncStatus = false;
				Game.LogVerbose("TimeLoop : " + ex.Message);
				ProjectData.ClearProjectError();
			}
		}

		private static void GameFrameRender(object sender, GraphicsEventArgs e)
		{
			checked
			{
				try
				{
					if (Game.IsKeyDown(RealTimeSyncReborn.Plugin.Main.menuKey))
					{
						if (!RealTimeSyncReborn.Plugin.Main.showMenu)
						{
							RealTimeSyncReborn.Plugin.Main.showMenu = true;
						}
						else
						{
							RealTimeSyncReborn.Plugin.Main.showMenu = false;
						}
					}
					if (RealTimeSyncReborn.Plugin.Main.showMenu)
					{
						Game.DisableControlAction(1, GameControl.Phone, true);
						if (Game.IsKeyDown(Keys.Down))
						{
							General.menuDown(ref RealTimeSyncReborn.Plugin.Main.menuID);
						}
						if (Game.IsKeyDown(Keys.Up))
						{
							General.menuUp(ref RealTimeSyncReborn.Plugin.Main.menuID);
						}
						if (Game.IsKeyDown(Keys.Left) | Game.IsKeyDown(Keys.Right))
						{
							General.selectSyncStates(RealTimeSyncReborn.Plugin.Main.menuID, ref RealTimeSyncReborn.Plugin.Main.menuStates);
						}
						General.menuSelectionBot(e, RealTimeSyncReborn.Plugin.Main.menuID, RealTimeSyncReborn.Plugin.Main.menuStates, RealTimeSyncReborn.Plugin.Main.header, RealTimeSyncReborn.Plugin.Main.selectedTextures, RealTimeSyncReborn.Plugin.Main.UnselectedTextures);
						if (Game.IsKeyDown(Keys.Return))
						{
							int num = RealTimeSyncReborn.Plugin.Main.menuID;
							if (num != 4)
							{
								if (num == 5)
								{
									RealTimeSyncReborn.Plugin.Main.showMenu = false;
								}
							}
							else
							{
								RealTimeSyncReborn.Plugin.Main.settings.Write("GENERAL SETTINGS", "TimeSync", RealTimeSyncReborn.Plugin.Main.menuStates[0]);
								RealTimeSyncReborn.Plugin.Main.settings.Write("GENERAL SETTINGS", "WeatherSync", RealTimeSyncReborn.Plugin.Main.menuStates[1]);
								RealTimeSyncReborn.Plugin.Main.settings.Write("GENERAL SETTINGS", "ShowNotifications", RealTimeSyncReborn.Plugin.Main.menuStates[2]);
								RealTimeSyncReborn.Plugin.Main.settings.Write("GENERAL SETTINGS", "CheckForUpdates", RealTimeSyncReborn.Plugin.Main.menuStates[3]);
								try
								{
									if (RealTimeSyncReborn.Plugin.Main.menuStates[0])
									{
										if (!RealTimeSyncReborn.Plugin.Main.timeSyncStatus)
										{
											RealTimeSyncReborn.Plugin.Main.timeGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.TimeLoop), "TimeLoop");
										}
									}
									else
									{
										RealTimeSyncReborn.Plugin.Main.timeGamefiber.Abort();
									}
								}
								catch (Exception expr_160)
								{
									ProjectData.SetProjectError(expr_160);
									ProjectData.ClearProjectError();
								}
								try
								{
									if (RealTimeSyncReborn.Plugin.Main.menuStates[1])
									{
										if (!RealTimeSyncReborn.Plugin.Main.weatherSyncStatus)
										{
											RealTimeSyncReborn.Plugin.Main.weatherGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.WeatherLoop), "WeatherLoop");
										}
									}
									else
									{
										RealTimeSyncReborn.Plugin.Main.weatherGamefiber.Abort();
									}
								}
								catch (Exception expr_1A8)
								{
									ProjectData.SetProjectError(expr_1A8);
									ProjectData.ClearProjectError();
								}
								try
								{
									if (RealTimeSyncReborn.Plugin.Main.menuStates[3])
									{
										if (!RealTimeSyncReborn.Plugin.Main.UpdatesCheckingStatus)
										{
											RealTimeSyncReborn.Plugin.Main.updateGamefiber = GameFiber.StartNew(new ThreadStart(RealTimeSyncReborn.Plugin.Main.CheckUpdate), "CheckUpdate");
										}
									}
									else
									{
										RealTimeSyncReborn.Plugin.Main.updateGamefiber.Abort();
									}
								}
								catch (Exception expr_1F0)
								{
									ProjectData.SetProjectError(expr_1F0);
									ProjectData.ClearProjectError();
								}
								string text = "";
								int num2 = RealTimeSyncReborn.Plugin.Main.menuStates.Count<bool>() - 1;
								for (int i = 0; i <= num2; i++)
								{
									switch (i)
									{
									case 0:
										if (RealTimeSyncReborn.Plugin.Main.menuStates[i])
										{
											text += "Time : ~g~True~n~";
										}
										else
										{
											text += "Time : ~r~False~n~";
										}
										break;
									case 1:
										if (RealTimeSyncReborn.Plugin.Main.menuStates[i])
										{
											text += "~s~Weather : ~g~True~n~";
										}
										else
										{
											text += "~s~Weather : ~r~False~n~";
										}
										break;
									case 2:
										if (RealTimeSyncReborn.Plugin.Main.menuStates[i])
										{
											text += "~s~Notifications : ~g~True~n~";
										}
										else
										{
											text += "~s~Notifications : ~r~False~n~";
										}
										break;
									case 3:
										if (RealTimeSyncReborn.Plugin.Main.menuStates[i])
										{
											text += "~s~Updates : ~g~True";
										}
										else
										{
											text += "~s~Updates : ~r~False";
										}
										break;
									}
								}
								Game.DisplayNotification("char_bugstars", "char_bugstars", "~g~~h~Real Time Sync Reborn", "~g~~h~CONFIG INFO", text);
							}
						}
					}
					else
					{
						General.enableGameControls();
					}
				}
				catch (Exception expr_308)
				{
					ProjectData.SetProjectError(expr_308);
					Exception ex = expr_308;
					Game.LogVerbose("ERROR : " + ex.Message);
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong!");
					ProjectData.ClearProjectError();
				}
			}
		}
	}
}
