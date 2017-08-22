using System;
using BoardControl;

namespace BoardControl{

	class Controllers{
		const int
			KEY_W = 0x57,
			KEY_A = 0x41,
			KEY_S = 0x53,
			KEY_D = 0x44,
			
			KEY_LEFT = 0x25,
			KEY_UP = 0x26,
			KEY_RIGHT = 0x27,
			KEY_DOWN = 0x28
			;

		const int topLeft = 0, bottomLeft = 1, topRight = 2, bottomRight = 3, padLeft = 4, padRight = 5;

		public static Controller leftRightWASD = e=>{
			e.keys[padLeft] = KEY_A;
			e.keys[padRight] = KEY_D;
		};

		public static Controller upDownWASD = e =>{
			e.keys[padLeft] = KEY_S;
			e.keys[padRight] = KEY_W;
		};

		public static Controller WASD = e =>{
			e.keys[bottomLeft] = KEY_A;
			e.keys[bottomRight] = KEY_D;

			e.keys[topLeft] = KEY_S;
			e.keys[topRight] = KEY_W;
		};
	}
}

