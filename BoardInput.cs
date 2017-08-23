using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WiiDeviceLibrary;

namespace BoardControl{
	
	class BoardInput{
		const int keyUpDelay = 16;
		const int doubleTapTime = 300;
		const float threshhold = 5f;
		const uint KEY_RELEASE = 2;

		public ControlData data;

		ProcessStartInfo startInfo = new ProcessStartInfo();
		IDeviceProvider deviceProvider;
		IBalanceBoard board;

		public event EventHandler<DeviceInfoEventArgs> connected, disconnected;

		float[] tare = new float[4];

		public Pad[] pads = {new Pad("top left"), new Pad("bottom left"), new Pad("top right"), new Pad("bottom right"), new Pad("left"), new Pad("right")};


		public void Pair(ControlData data){
			this.data = data;

			Console.WriteLine ("Starting..");

			deviceProvider = DeviceProviderRegistry.CreateSupportedDeviceProvider();
			Console.WriteLine ("Found provider: " + deviceProvider.ToString());

			deviceProvider.DeviceFound += DeviceFound;
			deviceProvider.DeviceLost += DeviceLost;

			Console.WriteLine ("Discovering device...");

			deviceProvider.StartDiscovering();
		}

		public void Disconnect(){
			deviceProvider.StopDiscovering ();
			if(board != null)
				board.Disconnect ();
		}

		void DeviceFound(object sender, DeviceInfoEventArgs args){ 
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

			connected (sender, args);
		}

		void DeviceLost(object sender, DeviceInfoEventArgs args){
			Console.WriteLine ("Lost device: " + args.DeviceInfo.ToString());
			disconnected (sender, args);
		}

		void Updated(object sender, EventArgs e){
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

		void pressOne(int start, int end){
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
					
					if (data.keys[EventType.pressed][i] != null) {
						KeyUp(data.keys[EventType.pressed][i]);
					}

					if(data.keys[EventType.up][i] != null){
						KeyDown (data.keys[EventType.up][i]);
						DelayKeyUp (data.keys[EventType.up][i]);
					}

					pads[i].down = false;
				}
			}

			//press max if needed
			if(max != -1 && !pads[max].down){

				Console.WriteLine (Milliseconds() - pads [max].lastDown);
				if(data.keys[EventType.tapped][max] != null && Milliseconds() - pads [max].lastDown < doubleTapTime){
					KeyDown (data.keys[EventType.tapped][max]);
					DelayKeyUp (data.keys[EventType.tapped][max]);
				}

				if (data.keys [EventType.pressed][max] != null) {
					KeyDown(data.keys[EventType.pressed][max]);
				}

				if(data.keys[EventType.down][max] != null){
					KeyDown (data.keys[EventType.down][max]);
					DelayKeyUp (data.keys[EventType.down][max]);
				}

				pads[max].down = true;
				pads [max].lastDown = Milliseconds();
			}

		}

		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		void KeyDown(Key key){
			Console.WriteLine("Key down: " + key);

			PressKey (key, true);
		}

		void KeyUp(Key key){
			Console.WriteLine("Key up: " + key);

			PressKey (key, false);
		}

		async Task DelayKeyUp(Key key){
			await Task.Delay(keyUpDelay);
			KeyUp (key);
		}

		void PressKey(Key key, bool down){
			if (IsLinux ()) {
				Console.WriteLine ((down ? "keydown" : "keyup") + " " + key.name);
				executeBashCommand ((down ? "keydown" : "keyup") + " " + key.name);
			} else {
				keybd_event ((byte)key.keycode, 0, down ? 0 : KEY_RELEASE, new System.UIntPtr());
			}
		}

		//requires xdotool to be installed
		void executeBashCommand(string args){
			startInfo.FileName = "/usr/bin/xdotool";
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = false;
			startInfo.Arguments = args;
			Process proc = new Process() { StartInfo = startInfo};
			proc.Start();
			proc.WaitForExit ();
		}

		static long Milliseconds(){
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		static bool IsLinux(){
			int p = (int) Environment.OSVersion.Platform;
			return (p == 4) || (p == 6) || (p == 128);
		}

	}
}
