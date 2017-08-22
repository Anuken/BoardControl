using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WiiDeviceLibrary;

namespace BoardControl{
	
	class MainClass{
		static int keyUpDelay = 16;
		static int doubleTapTime = 300;
		static float threshhold = 5f;

		static Controller controller = Controllers.riskOfRain;
		static ControlData data = new ControlData();

		static ProcessStartInfo startInfo = new ProcessStartInfo();

		static IDeviceProvider deviceProvider;
		static IBalanceBoard board;

		static float[] tare = new float[4];
		static Pad[] pads = {new Pad("top left"), new Pad("bottom left"), new Pad("top right"), new Pad("bottom right"), new Pad("left"), new Pad("right")};

		const uint KEY_RELEASE = 2;
		
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

			Console.WriteLine ("Waiting 2 seconds for tare...");
			Thread.Sleep (2000);
			Console.WriteLine("Tare done.");

			tare[0] = board.TopLeftWeight;
			tare[1] = board.BottomLeftWeight;

			tare[2] = board.TopRightWeight;
			tare[3] = board.BottomRightWeight;

			board.Updated += Updated;
		}

		static void Updated(object sender, EventArgs e){
			pads[0].value = board.TopLeftWeight - tare [0];
			pads[1].value = board.BottomLeftWeight - tare[1];

			pads[2].value = board.TopRightWeight - tare [2];
			pads[3].value = board.BottomRightWeight - tare[3];

			pads[4].value = pads[0].value + pads[1].value;
			pads[5].value = pads[2].value + pads[3].value;

			//debugging
			//Console.WriteLine (values[0] + " " + values[1] + " " + values[2] + " " + values[3]);

			//press one from each keygroup
			foreach(KeyGroup group in data.groups){
				pressOne (group.from, group.to);
			}
		}

		static void pressOne(int start, int end){
			//find the max
			int max = -1;

			for(int i = start; i < end; i ++){
				if(pads[i].value > threshhold && (max == -1 || pads[i].value > pads[max].value)){
					max = i;
				}
			}

			//unpress everything except the max
			for(int i = start; i < end; i ++){
				if (i != max && pads[i].down) {
					
					if (data.keys [i] != null) {
						KeyUp(data.keys[i]);
					}

					if(data.keysUp[i] != null){
						KeyDown (data.keysUp[i]);
						DelayKeyUp (data.keysUp[i]);
					}

					pads[i].down = false;
				}
			}

			//press max if needed
			if(max != -1 && !pads[max].down){

				Console.WriteLine (Milliseconds() - pads [max].lastDown);
				if(data.keysDoubleTap[max] != null && Milliseconds() - pads [max].lastDown < doubleTapTime){
					KeyDown (data.keysDoubleTap[max]);
					DelayKeyUp (data.keysDoubleTap[max]);
				}

				if (data.keys [max] != null) {
					KeyDown(data.keys[max]);
				}

				if(data.keysDown[max] != null){
					KeyDown (data.keysDown[max]);
					DelayKeyUp (data.keysDown[max]);
				}

				pads[max].down = true;
				pads [max].lastDown = Milliseconds();
			}

		}

		static void DeviceLost(object sender, DeviceInfoEventArgs arg){
			Console.WriteLine ("Lost device: " + arg.DeviceInfo.ToString());
		}

		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		static void KeyDown(Key key){
			Console.WriteLine("Key down: " + key);

			PressKey (key, true);
		}

		static void KeyUp(Key key){
			Console.WriteLine("Key up: " + key);

			PressKey (key, false);
		}

		static async Task DelayKeyUp(Key key){
			await Task.Delay(keyUpDelay);
			KeyUp (key);
		}

		static void PressKey(Key key, bool down){
			if (IsLinux ()) {
				Console.WriteLine ((down ? "keydown" : "keyup") + " " + key.name);
				executeBashCommand ((down ? "keydown" : "keyup") + " " + key.name);
			} else {
				keybd_event ((byte)key.keycode, 0, down ? 0 : KEY_RELEASE, new System.UIntPtr());
			}
		}

		static long Milliseconds(){
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		//requires xdotool to be installed
		static void executeBashCommand(string args){
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "/usr/bin/xdotool";
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = false;
			startInfo.Arguments = args;
			Process proc = new Process() { StartInfo = startInfo, };
			proc.Start();
			proc.WaitForExit ();
		}

		static bool IsLinux(){
			int p = (int) Environment.OSVersion.Platform;
			return (p == 4) || (p == 6) || (p == 128);
		}

	}
}
