using System;
using BoardControl;

namespace BoardControl{

	class Controllers{
		const int
			UP = 0x57,
			LEFT = 0x41,
			DOWN = 0x53,
			RIGHT = 0x44;
		const int topLeft = 0, bottomLeft = 1, topRight = 2, bottomRight = 3;

		public static Controller test = e=>{
			e.keys[topLeft] = LEFT;
			e.keys[topRight] = RIGHT;
		};
	}
}

