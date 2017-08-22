using System;
using System.Threading;
using WiiDeviceLibrary;
using Gtk;

namespace OSUBoard{
	
	class MainClass{
		static int sleeptime = 10;
		static IDeviceProvider deviceProvider;

		static bool updating;
		
		
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

			IBalanceBoard board = (IBalanceBoard)

			updating = true;

			Thread thread = new Thread (()=>{

				while(updating){
					float topl = board.TopLeftWeight, topr = board.TopRightWeight, 
						botl = board.BottomLeftWeight, botr = board.BottomRightWeight;

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
