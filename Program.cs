using System;
using System.Threading;
using System.Threading.Tasks;
using WiiDeviceLibrary;
using System.Runtime.InteropServices;

namespace BoardControl{
	
	class MainClass{
		const int keyUpDelay = 16;
		const int fps = 5;
		const int sleeptime = (int)(1f/fps*1000);
		const string[] padnames = {"top left", "bottom left", "top right", "bottom right"};
		const float threshhold = 5f;

		static Controller controller = Controllers.WASD;
		static ControlData data = new ControlData();

		static IDeviceProvider deviceProvider;
		static IBalanceBoard board;

		static float[] tare = new float[4];
		static float[] values = new float[6];
		static bool[] down = new bool[6];

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
			values[0] = board.TopLeftWeight - tare [0];
			values[1] = board.BottomLeftWeight - tare[1];

			values[2] = board.TopRightWeight - tare [2];
			values[3] = board.BottomRightWeight - tare[3];

			values[4] = values[0] + values[1];
			values[5] = values[2] + values[3];

			//press one from each keygroup
			foreach(KeyGroup group in data.groups){
				pressOne (group.from, group.to);
			}
		}

		static void pressOne(int start, int end){
			//find the max
			int max = -1;

			for(int i = start; i < end; i ++){
				if(values[i] > threshhold && (max == -1 || values[i] > values[max])){
					max = i;
				}
			}

			//unpress everything except the max
			for(int i = start; i < end; i ++){
				if (i != max && down [i]) {
					
					if (data.keys [i] != -1) {
						KeyUp(data.keys[i]);
					}

					if(data.keysUp[i] != -1){
						KeyDown (data.keysUp[i]);
						DelayKeyUp (data.keysUp[i]);
					}
				}
			}

			//press max if needed
			if(max != -1 && !down[max]){
				if (data.keys [max] != -1) {
					KeyDown(data.keys[max]);
				}

				if(data.keysDown[max] != -1){
					KeyDown (data.keysDown[max]);
					DelayKeyUp (data.keysDown[max]);
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

		static async Task DelayKeyUp(int i){
			await Task.Delay(keyUpDelay);
			KeyUp (i);
		}

	}
}
