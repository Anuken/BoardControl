using System;
using System.Collections.Generic;

namespace BoardControl{

	public class Keys{
		public static List<Key> All = new List<Key>();

		public static Key

		W = new Key ("w", 0x57),
		A = new Key ("a", 0x41),
		S = new Key ("s", 0x53),
		D = new Key ("d", 0x44),

		LEFT = new Key ("Left", 0x25),
		UP = new Key ("Up", 0x26),
		RIGHT = new Key ("Right", 0x27),
		DOWN = new Key ("Down", 0x28),

		LEFT_SHIFT = new Key ("shift", 0x10),
		SPACE = new Key ("space", 0x20)
		;
	}
	
	public class Key{
		//name, used for linux
		public String name;
		//keycode, used for windows
		public int keycode;

		public Key(String name, int keycode){
			this.name = name;
			this.keycode = keycode;
			Keys.All.Add (this);
		}

		public override string ToString (){
			return name;
		}
	}
}

