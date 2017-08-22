using System;

namespace BoardControl{
	
	public class ControlData{
		public int[] keys = {-1, -1};
	}

	public delegate void PadDown();
	public delegate void Controller(ControlData e);
}

