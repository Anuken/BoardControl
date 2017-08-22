using System;
using System.Collections.Generic;

namespace BoardControl{
	
	public class ControlData{
		public int[] keys = {-1, -1, -1, -1, -1, -1};
		public int[] keysDown = {-1, -1, -1, -1, -1, -1};
		public int[] keysUp = {-1, -1, -1, -1, -1, -1};

		public List<KeyGroup> groups = new List<KeyGroup>(new KeyGroup[]{
			new KeyGroup(0,4), new KeyGroup(4, 6)
		});
	}

	public delegate void PadEvent();
	public delegate void Controller(ControlData e);

	public class KeyGroup{
		public int from, to;

		public KeyGroup(int from, int to){
			this.from = from;
			this.to = to;
		}
	}

}

