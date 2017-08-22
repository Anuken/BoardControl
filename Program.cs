using System;
using System.Threading;
using WiiDeviceLibrary;
using System.Runtime.InteropServices;

namespace BoardControl{
	
	class MainClass{
		static Controller controller = Controllers.leftRightWASD;
		static ControlData data = new ControlData();

		static int fps = 5;
		static int sleeptime = (int)(1f/fps*1000);
		static string[] padnames = {"top left", "bottom left", "top right", "bottom right"};
		static float threshhold = 5f;

		static IDeviceProvider deviceProvider;
		static IBalanceBoard board;

		static float[] tare = new float[4];
		static float[] values = new float[2];
		static bool[] down = new bool[2];

		const int KEY_RELEASE = 2;
		
		public static void Main (string[] args){
			Console.WriteLine ("Starting..");

			controller (data);

			deviceProvider = DeviceProviderRegistry.CreateSupportedDeviceProvider();
			Console.WriteLine ("Found provider: " + deviceProvider.ToString());

			deviceProvider.DeviceFound += DeviceFound;
			deviceProvider.DeviceLost += DeviceLost;


			Console.WriteLine ("Discovering device...");

			try{
				deviceProvider.StartDiscovering();
			}catch{
				Console.WriteLine("Exception occured, re-discovering...");
				deviceProvider.StopDiscovering();
				deviceProvider.StartDiscovering();
			}
		}

		static void DeviceFound(object sender, DeviceInfoEventArgs args){ 
			IDevice device = deviceProvider.Connect(args.DeviceInfo);
			Console.WriteLine ("Found device: " + device.ToString());

			board = (IBalanceBoard)device;

			tare[0] = board.TopLeftWeight;
			tare[1] = board.BottomLeftWeight;

			tare[2] = board.TopRightWeight;
			tare[3] = board.BottomRightWeight;

			board.Updated += Updated;
		}

		static void Updated(object sender, EventArgs e){
			values[0] = board.TopLeftWeight - tare[0]
			 + board.BottomLeftWeight - tare[1];

			values[1] = board.TopRightWeight - tare[2]
				+ board.BottomRightWeight - tare[3];

			float left = values[0];
			float right = values[1];

			if (data.keys[0] != -1){
				if (left > threshhold && left > right){
					if (!down[0]){
						KeyDown(data.keys[0]);
						down[0] = true;
					}
				}else if(down[0]){
                	KeyUp(data.keys[0]);
					down[0] = false;
				}
			}

			if (data.keys[1] != -1){
				if (right > threshhold && right > left){
					if (!down[1]){
						KeyDown(data.keys[1]);
						down[1] = true;
					}
				}else if (down[1]){
                    KeyUp(data.keys[1]);
					down[1] = false;
				}
			}
		}

		static void DeviceLost(object sender, DeviceInfoEventArgs arg){
			Console.WriteLine ("Lost device: " + arg.DeviceInfo.ToString());
		}

		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		static void KeyDown(int i){
			Console.WriteLine("Key down: " + i);
			keybd_event ((byte)i, 0, 0, new System.UIntPtr());
		}

		static void KeyUp(int i){
			Console.WriteLine("Key up: " + i);
			keybd_event ((byte)i, 0, KEY_RELEASE, new System.UIntPtr());
		}

	}
}
