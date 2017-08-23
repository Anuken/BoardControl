using System;
using BoardControl;

namespace BoardControl{

	class Controllers{
		const int topLeft = 0, bottomLeft = 1, topRight = 2, bottomRight = 3, padLeft = 4, padRight = 5;

		public static Controller leftRightWASD = e=>{
			e.keys[EventType.pressed][padLeft] = Keys.A;
			e.keys[EventType.pressed][padRight] = Keys.D;
		};

		public static Controller upDownWASD = e =>{
			e.keys[EventType.pressed][padLeft] = Keys.S;
			e.keys[EventType.pressed][padRight] = Keys.W;
		};

		public static Controller WASD = e =>{
			e.keys[EventType.pressed][bottomLeft] = Keys.A;
			e.keys[EventType.pressed][bottomRight] = Keys.D;

			e.keys[EventType.pressed][topLeft] = Keys.S;
			e.keys[EventType.pressed][topRight] = Keys.W;
		};

		public static Controller riskOfRain = e =>{
			e.keys[EventType.pressed][bottomLeft] = Keys.LEFT;
			e.keys[EventType.pressed][bottomRight] = Keys.RIGHT;

			e.keys[EventType.pressed][topLeft] = Keys.A;
			e.keys[EventType.pressed][topRight] = Keys.UP;

			e.keys[EventType.tapped][topRight] = Keys.D;
			e.keys[EventType.tapped][topLeft] = Keys.S;

			e.keys[EventType.tapped][bottomRight] = Keys.LEFT_SHIFT;
			e.keys[EventType.tapped][bottomLeft] = Keys.LEFT_SHIFT;
		};
	}
}

