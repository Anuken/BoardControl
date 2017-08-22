using System;
using System.Threading;
using WiiDeviceLibrary;
using System.Runtime.InteropServices;

namespace BoardControl{
	
	class MainClass{
		static Controller controller = Controllers.test;
		static ControlData data = new ControlData();

		static int fps = 5;
		static int sleeptime = (int)(1f/fps*1000);
		static string[] padnames = {"top left", "bottom left", "top right", "bottom right"};
		static float threshhold = 4;

		static IDeviceProvider deviceProvider;

		static bool updating;
		static float[] values = new float[4];
		static float[] lastvalues = new float[4];

		const int KEY_RELEASE = 2;
		
		public static void Main (string[] args){
			Console.WriteLine ("Starting..");

			controller (data);

			deviceProvider = DeviceProviderRegistry.CreateSupportedDeviceProvider();
			Console.WriteLine ("Found provider: " + deviceProvider.ToString());

			deviceProvider.DeviceFound += DeviceFound;
			deviceProvider.DeviceLost += DeviceLost;

			Console.WriteLine ("Discovering device...");
			deviceProvider.StartDiscovering();

		}

		static void DeviceFound(object sender, DeviceInfoEventArgs args){ 
			IDevice device = deviceProvider.Connect(args.DeviceInfo);
			Console.WriteLine ("Found device: " + device.ToString());

			IBalanceBoard board = (IBalanceBoard)device;

			for(int i = 0; i < 4; i ++){
				lastvalues[i] = values[i];
			}

			updating = true;

			Thread thread = new Thread (()=>{

				while(updating){
					values[0] = board.TopLeftWeight;
					values[1] = board.BottomLeftWeight;

					values[2] = board.TopRightWeight;
					values[3] = board.BottomRightWeight;

					for(int i = 0; i < 4; i ++){
						if(values[i] - lastvalues[i] > threshhold){
							Console.WriteLine("Key down: " + padnames[i] + ": " + lastvalues[i] + " -> " + values[i]);

							if(data.keys[i] != -1){
								KeyDown(data.keys[i]);
							}
						}

						if(values[i] - lastvalues[i] < threshhold){
							Console.WriteLine("Key up: " + padnames[i] + ": " + lastvalues[i] + " -> " + values[i]);

							if(data.keys[i] != -1){
								KeyUp(data.keys[i]);
							}
						}
					}

					for(int i = 0; i < 4; i ++){
						lastvalues[i] = values[i];
					}

					Thread.Sleep(sleeptime);
				}
			});

			thread.Start();
		}

		static void DeviceLost(object sender, DeviceInfoEventArgs arg){
			Console.WriteLine ("Lost device: " + arg.DeviceInfo.ToString());

			updating = false;
		}

		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		static void KeyDown(int i){
			keybd_event ((byte)i, 0, 0, new System.UIntPtr());
		}

		static void KeyUp(int i){
			keybd_event ((byte)i, 0, KEY_RELEASE, new System.UIntPtr());
		}

	}
}
