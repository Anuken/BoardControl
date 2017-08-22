using System;
using BoardControl;

namespace BoardControl{

	class Controllers{
		const int topLeft = 0, bottomLeft = 1, topRight = 2, bottomRight = 3, padLeft = 4, padRight = 5;

		public static Controller leftRightWASD = e=>{
			e.keys[padLeft] = Keys.A;
			e.keys[padRight] = Keys.D;
		};

		public static Controller upDownWASD = e =>{
			e.keys[padLeft] = Keys.S;
			e.keys[padRight] = Keys.W;
		};

		public static Controller WASD = e =>{
			e.keys[bottomLeft] = Keys.A;
			e.keys[bottomRight] = Keys.D;

			e.keys[topLeft] = Keys.S;
			e.keys[topRight] = Keys.W;
		};

		public static Controller riskOfRain = e =>{
			e.keys[bottomLeft] = Keys.LEFT;
			e.keys[bottomRight] = Keys.RIGHT;

			e.keys[topLeft] = Keys.A;
			e.keys[topRight] = Keys.UP;

			e.keysDoubleTap[topRight] = Keys.D;
			e.keysDoubleTap[topLeft] = Keys.S;

			e.keysDoubleTap[bottomRight] = Keys.LEFT_SHIFT;
			e.keysDoubleTap[bottomLeft] = Keys.LEFT_SHIFT;

		};
	}
}

