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
			Main.TimeSyncSelected,
			Main.WeatherSyncSelected,
			Main.NotificationsDisplaySelected,
			Main.UpdateCheckingSelected,
			Main.ApplySelected,
			Main.ExitSelected,
			Main.CheckSelected,
			Main.CrossSelected
		};

		private static Texture[] UnselectedTextures = new Texture[]
		{
			Main.TimeSyncUnselected,
			Main.WeatherSyncUnselected,
			Main.NotificationsDisplayUnselected,
			Main.UpdateCheckingUnselected,
			Main.ApplyUnselected,
			Main.ExitUnselected,
			Main.CheckUnselected,
			Main.CrossUnselected
		};

		private static void main()
		{
			try
			{
				General.ShowLogStart();
				if (Main.settings.Exists())
				{
					Main.apiUrl = Main.settings.ReadString("WEATHER SETTINGS", "APIURL", "http://api.openweathermap.org/data/2.5/weather?q={CITY}&units={Metric/Imperial}&APPID={OpenWeatherMap AppID}");
					Main.WeatherFreq = Conversions.ToString(Main.settings.ReadInt32("WEATHER SETTINGS", "NOTIFICATIONS_FREQ", 10));
					Main.weatherStates[0] = Main.settings.ReadBoolean("WEATHER TYPES", "ExtraSunny", true);
					Main.weatherStates[1] = Main.settings.ReadBoolean("WEATHER TYPES", "Clear", true);
					Main.weatherStates[2] = Main.settings.ReadBoolean("WEATHER TYPES", "Clouds", true);
					Main.weatherStates[3] = Main.settings.ReadBoolean("WEATHER TYPES", "Smog", true);
					Main.weatherStates[4] = Main.settings.ReadBoolean("WEATHER TYPES", "Foggy", true);
					Main.weatherStates[5] = Main.settings.ReadBoolean("WEATHER TYPES", "Overcast", true);
					Main.weatherStates[6] = Main.settings.ReadBoolean("WEATHER TYPES", "Rain", true);
					Main.weatherStates[7] = Main.settings.ReadBoolean("WEATHER TYPES", "Thunder", true);
					Main.weatherStates[8] = Main.settings.ReadBoolean("WEATHER TYPES", "Clearing", true);
					Main.weatherStates[9] = Main.settings.ReadBoolean("WEATHER TYPES", "Neutral", true);
					Main.weatherStates[10] = Main.settings.ReadBoolean("WEATHER TYPES", "Snow", true);
					Main.weatherStates[11] = Main.settings.ReadBoolean("WEATHER TYPES", "Blizzard", true);
					Main.weatherStates[12] = Main.settings.ReadBoolean("WEATHER TYPES", "Snowlight", true);
					Main.weatherStates[13] = Main.settings.ReadBoolean("WEATHER TYPES", "Xmas", true);
					Main.menuStates[1] = Main.settings.ReadBoolean("GENERAL SETTINGS", "WeatherSync", true);
					Main.menuStates[0] = Main.settings.ReadBoolean("GENERAL SETTINGS", "TimeSync", true);
					Main.menuStates[2] = Main.settings.ReadBoolean("GENERAL SETTINGS", "ShowNotifications", true);
					Main.menuStates[3] = Main.settings.ReadBoolean("GENERAL SETTINGS", "CheckForUpdates", true);
					try
					{
						Main.menuKey = (Keys)Conversions.ToInteger(new KeysConverter().ConvertFromInvariantString(Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5")));
					}
					catch (Exception arg_27E_0)
					{
						ProjectData.SetProjectError(arg_27E_0);
						Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5") + " is not recognized as key! ~n~Menu key set to NumPad5 by default");
						Game.LogVerbose("ERROR : " + Main.settings.ReadString("KEYS SETTINGS", "OpenMenu", "NumPad5") + " is not recognized as key! ~n~Menu key set to NumPad5 by default");
						Main.menuKey = Keys.NumPad5;
						ProjectData.ClearProjectError();
					}
					Game.Console.Print("Current version : " + Main.PluginVersion);
					Game.Console.Print("Up to date version : " + General.openURI(Main.updateUrl, ref Main.internetDownloadsError));
					Game.Console.Print("FREQ : " + Main.WeatherFreq);
					Game.Console.Print("WeatherSync : " + Main.menuStates[1].ToString());
					Game.Console.Print("TimeSync : " + Main.menuStates[0].ToString());
					Game.Console.Print("ShowNotifications : " + Main.menuStates[2].ToString());
					Game.Console.Print("CheckForUpdates : " + Main.menuStates[3].ToString());
					Game.Console.Print("OpenMenu key : " + Main.menuKey.ToString());
					Game.Console.Print(string.Format("API URL: {0}", Main.apiUrl));
					Game.Console.Print("-----------------------------------------------------------");
				}
				else
				{
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong with your config file!~n~File doesn't exist!");
				}
				try
				{
					Main.WeatherFreq = Conversions.ToString(checked(Conversions.ToInteger(Main.WeatherFreq) * 60000));
					if (Conversions.ToDouble(Main.WeatherFreq) < 10.0)
					{
						Main.WeatherFreq = Conversions.ToString(10);
					}
				}
				catch (Exception expr_48B)
				{
					ProjectData.SetProjectError(expr_48B);
					Exception ex = expr_48B;
					Main.WeatherFreq = "600000";
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong with your config file!");
					Game.LogVerbose("ERROR : WeatherFreq is NaN, " + ex.Message);
					ProjectData.ClearProjectError();
				}
				Game.FrameRender += new EventHandler<GraphicsEventArgs>(Main.GameFrameRender);
				Main.showMenu = false;
				Main.menuID = 0;
				GameFiber.StartNew(new ThreadStart(Main.voidLoop), "VoidLoop");
				if (Main.menuStates[3])
				{
					Main.updateGamefiber = GameFiber.StartNew(new ThreadStart(Main.CheckUpdate), "CheckUpdate");
				}
				if (Main.menuStates[1])
				{
					Main.weatherGamefiber = GameFiber.StartNew(new ThreadStart(Main.WeatherLoop), "WeatherLoop");
				}
				if (Main.menuStates[0])
				{
					Main.timeGamefiber = GameFiber.StartNew(new ThreadStart(Main.TimeLoop), "TimeLoop");
				}
			}
			catch (Exception expr_579)
			{
				ProjectData.SetProjectError(expr_579);
				Exception ex2 = expr_579;
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
			Main.updateWebResult = General.openURI(Main.updateUrl, ref Main.internetDownloadsError);
		}

		private static void weatherGather()
		{
			WebClient expr_05 = new WebClient();
			Main.weatherWebResult = expr_05.DownloadString(Main.apiUrl);
			expr_05.Dispose();
		}

		private static void CheckUpdate()
		{
			try
			{
				while (true)
				{
					Main.UpdatesCheckingStatus = true;
					if (!Main.menuStates[3])
					{
						break;
					}
					Main.updateThread = new Thread(new ThreadStart(Main.updateGather));
					Main.updateThread.IsBackground = true;
					if (General.netIsOk("www.google.com"))
					{
						Main.updateThread.Start();
						while (Main.updateThread.IsAlive)
						{
							GameFiber.Sleep(1);
						}
						if (Main.internetDownloadsError)
						{
							Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Update version info can't be downloaded! (more info in console log)");
							Game.LogVerbose("ERROR : " + Main.updateWebResult);
						}
						else if (Operators.CompareString(Main.updateWebResult, Main.PluginVersion, false) != 0)
						{
							Game.DisplayNotification("char_bugstars", "char_bugstars", "~g~~h~Real Time Sync Reborn", "~g~~h~INFO", string.Concat(new string[]
							{
								"New version (~g~",
								Main.updateWebResult,
								"~s~) available!~n~Installed version : ~r~",
								Main.PluginVersion,
								"~n~~s~Download it on www.lcpdfr.com!"
							}));
							Game.LogVerbose(string.Concat(new string[]
							{
								"New version (",
								Main.updateWebResult,
								") is available! Installed version : ",
								Main.PluginVersion,
								". Download the new version on www.lcpdfr.com!"
							}));
						}
						Main.internetDownloadsError = false;
						GameFiber.Sleep(Main.CheckUpdatesFreq);
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
				Main.UpdatesCheckingStatus = false;
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
						Main.weatherSyncStatus = true;
						if (!Main.menuStates[1])
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
							Main.weatherThread = new Thread(new ThreadStart(Main.weatherGather));
							Main.weatherThread.IsBackground = true;
							Main.weatherThread.Start();
							while (Main.weatherThread.IsAlive)
							{
								GameFiber.Sleep(1);
							}
							if (!Main.weatherWebResult.Contains("Error"))
							{
								weather weather = javaScriptSerializer.Deserialize<weather>(Main.weatherWebResult);
								if (Main.menuStates[2])
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
						GameFiber.Sleep(Conversions.ToInteger(Main.WeatherFreq));
					}
				}
				catch (Exception expr_2C6)
				{
					ProjectData.SetProjectError(expr_2C6);
					Exception ex = expr_2C6;
					Main.weatherSyncStatus = false;
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
					Main.timeSyncStatus = true;
					if (!Main.menuStates[0])
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
				Main.timeSyncStatus = false;
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
					if (Game.IsKeyDown(Main.menuKey))
					{
						if (!Main.showMenu)
						{
							Main.showMenu = true;
						}
						else
						{
							Main.showMenu = false;
						}
					}
					if (Main.showMenu)
					{
						Game.DisableControlAction(1, GameControl.Phone, true);
						if (Game.IsKeyDown(Keys.Down))
						{
							General.menuDown(ref Main.menuID);
						}
						if (Game.IsKeyDown(Keys.Up))
						{
							General.menuUp(ref Main.menuID);
						}
						if (Game.IsKeyDown(Keys.Left) | Game.IsKeyDown(Keys.Right))
						{
							General.selectSyncStates(Main.menuID, ref Main.menuStates);
						}
						General.menuSelectionBot(e, Main.menuID, Main.menuStates, Main.header, Main.selectedTextures, Main.UnselectedTextures);
						if (Game.IsKeyDown(Keys.Return))
						{
							int num = Main.menuID;
							if (num != 4)
							{
								if (num == 5)
								{
									Main.showMenu = false;
								}
							}
							else
							{
								Main.settings.Write("GENERAL SETTINGS", "TimeSync", Main.menuStates[0]);
								Main.settings.Write("GENERAL SETTINGS", "WeatherSync", Main.menuStates[1]);
								Main.settings.Write("GENERAL SETTINGS", "ShowNotifications", Main.menuStates[2]);
								Main.settings.Write("GENERAL SETTINGS", "CheckForUpdates", Main.menuStates[3]);
								try
								{
									if (Main.menuStates[0])
									{
										if (!Main.timeSyncStatus)
										{
											Main.timeGamefiber = GameFiber.StartNew(new ThreadStart(Main.TimeLoop), "TimeLoop");
										}
									}
									else
									{
										Main.timeGamefiber.Abort();
									}
								}
								catch (Exception arg_166_0)
								{
									ProjectData.SetProjectError(arg_166_0);
									ProjectData.ClearProjectError();
								}
								try
								{
									if (Main.menuStates[1])
									{
										if (!Main.weatherSyncStatus)
										{
											Main.weatherGamefiber = GameFiber.StartNew(new ThreadStart(Main.WeatherLoop), "WeatherLoop");
										}
									}
									else
									{
										Main.weatherGamefiber.Abort();
									}
								}
								catch (Exception arg_1AC_0)
								{
									ProjectData.SetProjectError(arg_1AC_0);
									ProjectData.ClearProjectError();
								}
								try
								{
									if (Main.menuStates[3])
									{
										if (!Main.UpdatesCheckingStatus)
										{
											Main.updateGamefiber = GameFiber.StartNew(new ThreadStart(Main.CheckUpdate), "CheckUpdate");
										}
									}
									else
									{
										Main.updateGamefiber.Abort();
									}
								}
								catch (Exception arg_1F2_0)
								{
									ProjectData.SetProjectError(arg_1F2_0);
									ProjectData.ClearProjectError();
								}
								string text = "";
								int num2 = Main.menuStates.Count<bool>() - 1;
								for (int i = 0; i <= num2; i++)
								{
									switch (i)
									{
									case 0:
										if (Main.menuStates[i])
										{
											text += "Time : ~g~True~n~";
										}
										else
										{
											text += "Time : ~r~False~n~";
										}
										break;
									case 1:
										if (Main.menuStates[i])
										{
											text += "~s~Weather : ~g~True~n~";
										}
										else
										{
											text += "~s~Weather : ~r~False~n~";
										}
										break;
									case 2:
										if (Main.menuStates[i])
										{
											text += "~s~Notifications : ~g~True~n~";
										}
										else
										{
											text += "~s~Notifications : ~r~False~n~";
										}
										break;
									case 3:
										if (Main.menuStates[i])
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
				catch (Exception expr_2F4)
				{
					ProjectData.SetProjectError(expr_2F4);
					Exception ex = expr_2F4;
					Game.LogVerbose("ERROR : " + ex.Message);
					Game.DisplayNotification("char_bugstars", "char_bugstars", "~r~~h~Real Time Sync Reborn", "~r~~h~ERROR", "Oh no! Something went wrong!");
					ProjectData.ClearProjectError();
				}
			}
		}
	}
}
