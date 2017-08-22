using System;
using System.Threading;
using WiiDeviceLibrary;
using Gtk;

namespace OSUBoard{
	
	class MainClass{
		static int sleeptime = 10;
		static IDeviceProvider deviceProvider;

		static bool updating;
		static float[] values = new float[4];
		static float[] lastvalues = new float[4];
		
		public static void Main (string[] args){
			Console.WriteLine ("Starting..");
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

			updating = true;

			Thread thread = new Thread (()=>{

				while(updating){
					values[0] = board.TopLeftWeight;
					values[1] = board.BottomLeftWeight;

					values[2] = board.TopRightWeight;
					values[3] = board.BottomRightWeight;

					for(int i = 0; i < 4; i ++){
						if(Math.Abs(values[i] - lastvalues[i]) > 0.01){
							Console.WriteLine("Detected weight change: " + lastvalues[i] + " -> " + values[i]);
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

	}
}
