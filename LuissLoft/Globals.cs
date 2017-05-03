using System;
using System.Collections;
using System.Collections.Generic;

namespace LuissLoft
{
	public class Globals
	{
		public Globals()
		{
		}

		public const string RestApiV1 = "http://rest.luiss.visual1993.com/";
		//public const string RestApiV1 = "http://127.0.0.1:8889/";
		public const string DefaultThumb = "loftLogo";
		public static TimeSpan DurataMinimaEvento = TimeSpan.FromMinutes(30);



		public static IEnumerable<LuogoStringaClass> Luoghi = new List<LuogoStringaClass> {
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.Intero,LuogoStringa= "Intero LOFT", IsCorretto=true},
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.Cinema,LuogoStringa= "Cinema", IsCorretto=true},
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.Centrale,LuogoStringa= "Centrale", IsCorretto=true},
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.TeloVerde,LuogoStringa= "Telo verde", IsCorretto=true},
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.Intero,LuogoStringa= "Intero"},
			new LuogoStringaClass{LuogoEnum= LuoghiEnum.Centrale,LuogoStringa= "Central"},
		};
	}

		public enum LuoghiEnum
	{
		Intero = 0,
		Cinema = 1,
		TeloVerde = 2,
		Centrale = 3
	};
	public class LuogoStringaClass
	{
		public LuoghiEnum LuogoEnum;
		public string LuogoStringa;
		public bool IsCorretto = false;
	}
}
