using System;

namespace BoardControl{
	
	public class Pad{
		public float value;
		public bool down;
		public long lastDown;
		public string name;

		public Pad(string name){
			this.name = name;
		}
	}
}

