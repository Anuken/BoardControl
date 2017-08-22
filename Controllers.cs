using System;
using BoardControl;

namespace BoardControl{

	class Controllers{
		const int
			W = 0x57,
			A = 0x41,
			S = 0x53,
			D = 0x44;
		const int padLeft = 0, padRight = 1;

		public static Controller leftRightWASD = e=>{
			e.keys[padLeft] = A;
			e.keys[padRight] = D;
		};

		public static Controller upDownWASD = e =>{
			e.keys[padLeft] = S;
			e.keys[padRight] = W;

		};
}

