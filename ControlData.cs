using System;
using System.Collections.Generic;

namespace BoardControl{
	
	public class ControlData{
		public Dictionary<EventType, Key[]> keys = new Dictionary<EventType, Key[]> ();

		public List<KeyGroup> groups = new List<KeyGroup>(new KeyGroup[]{
			new KeyGroup(0,4), new KeyGroup(4, 6)
		});

		public ControlData(){
			foreach(EventType type in Enum.GetValues(typeof(EventType))){
				keys [type] = new Key[6];
			}
		}
	}

	public delegate void PadEvent();
	public delegate void Controller(ControlData e);

	public enum EventType{
		pressed, down, up, tapped
	}

	public class KeyGroup{
		public int from, to;

		public KeyGroup(int from, int to){
			this.from = from;
			this.to = to;
		}
	}

}

