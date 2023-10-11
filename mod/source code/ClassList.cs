using System;
using System.Collections.Generic;
using UnityEngine;

public static class ClassList
{
	public static void Initialize(List<Class> c)
	{
		ClassList.classes = new List<Class>();
		ClassList.classes.AddRange(c);
	}

	public static void Initialize()
	{
		Debug.Log("CREATING CLASS LIST");
		ClassList.classes = new List<Class>();
		ClassList.classes.Add(new Class("Commander", "Commander", "c000", "", "Commander", "Commander", 0, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 4f, UnitInfo.Move.Ground, 0f, 0f, 0, 0, 1, 0, 1, 90, 1, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new Dice(0, 0f), new int[1], new int[]
		{
			0,
			1
		}));
		ClassList.classes.Add(new Class("Cornucopia", "Five-star Meal", "c010", "Everyone's favorite meal! By being digested, Cornucopia can fatten up enemies or heal allies in a pinch. An indispensable asset to have in battle.", "Chief", "Darling", 10, new Vector2(0.5f, 0.81f), new Vector2(0.54f, 0.94f), new Vector3(0.52f, 0.94f, 50f), 330f, 240f, 45f, 50f, 35f, 5f, 50f, 35f, 660f, 280f, 100f, 100f, 100f, 100f, 100f, 70f, 4f, UnitInfo.Move.Ground, 1f, 75f, 1, 0, 1, 3, 1, 90, 1, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(30, 75f), new Dice(10, 50f), new Dice(5, 30f), new Dice(3, 30f), new Dice(20, 50f), new Dice(3, 30f), new Dice(3, 10f), new Dice(3, 10f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			4,
			1144,
			1145
		}, new int[]
		{
			-504,
			-503,
			-100,
			1,
			2,
			3,
			1012
		}));
		ClassList.classes.Add(new Class("Schrodinger", "Lab Cat", "c011", "A predator who makes use of random chance-based skills to turn the tides of battle. While powerful, they each carry the risk of backfiring spectacularly.", "Commyander", "Myaster", 11, new Vector3(0.355f, 0.86f), new Vector3(0.43f, 0.975f), new Vector3(0.43f, 0.975f), 200f, 100f, 50f, 35f, 60f, 45f, 50f, 60f, 400f, 140f, 100f, 70f, 120f, 90f, 100f, 120f, 4f, UnitInfo.Move.Ground, 1f, 100f, 3, 0, 1, 4, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(20, 50f), new Dice(10, 50f), new Dice(5, 50f), new Dice(3, 30f), new Dice(5, 50f), new Dice(3, 30f), new Dice(5, 50f), new Dice(5, 75f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			3,
			5,
			6
		}, new int[]
		{
			-504,
			-503,
			-100,
			5,
			6,
			7
		}));
		ClassList.classes.Add(new Class("Covenant", "Holy Maiden?", "c012", "A sister of the church who can heal and buff allies by swallowing them. Her true nature is quite gluttonous, so be careful not to tempt her too much.", "Commander", "Father", 12, new Vector3(0.2f, 0.87f), new Vector3(0.3f, 0.975f), new Vector3(0.33f, 0.93f, 100f), 220f, 150f, 55f, 50f, 60f, 70f, 50f, 50f, 440f, 240f, 70f, 100f, 100f, 140f, 100f, 100f, 4f, UnitInfo.Move.Ground, 1f, 150f, 3, 0, 1, 5, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(10, 50f), new Dice(10, 50f), new Dice(5, 50f), new Dice(5, 50f), new Dice(5, 50f), new Dice(5, 50f), new Dice(3, 30f), new Dice(3, 30f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			20,
			21,
			22,
			140,
			141
		}, new int[]
		{
			-1001,
			-504,
			-503,
			-500,
			-100,
			20,
			21,
			24
		}));
		ClassList.classes.Add(new Class("Sampo", "Prideful Snack", "c013", "A girl who wants nothing more than to prove herself as a predator. The more she swells with pride, the more delicious she becomes.", "Commander", "Master", 13, new Vector3(0.436f, 0.82f), new Vector3(0.505f, 0.94f), new Vector3(0.58f, 0.96f, 50f), 160f, 120f, 45f, 25f, 55f, 35f, 50f, 50f, 280f, 120f, 100f, 50f, 110f, 70f, 100f, 100f, 4f, UnitInfo.Move.Ground, 1f, 75f, 1, 0, 1, 3, 1, 90, 1, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(20, 70f), new Dice(10, 50f), new Dice(5, 70f), new Dice(5, 50f), new Dice(5, 70f), new Dice(5, 50f), new Dice(5, 50f), new Dice(5, 50f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			7,
			8,
			1144
		}, new int[]
		{
			-504,
			-503,
			-100,
			15,
			16,
			17,
			3
		}));
		ClassList.classes.Add(new Class("Theano", "Peerless Blade", "c014", "A bodyguard who protects humanity at all costs. She deals bonus damage to predatory enemies, making her excellent at rescuing allies.", "Commander", "Master", 14, new Vector3(0.345f, 0.855f), new Vector3(0.43f, 0.975f), new Vector3(0.43f, 0.96f, 50f), 240f, 120f, 55f, 35f, 60f, 60f, 60f, 60f, 480f, 250f, 110f, 60f, 120f, 120f, 120f, 120f, 4f, UnitInfo.Move.Ground, 1f, 150f, 2, 0, 1, 5, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(10, 50f), new Dice(5, 50f), new Dice(5, 50f), new Dice(3, 30f), new Dice(5, 50f), new Dice(5, 50f), new Dice(5, 50f), new Dice(5, 50f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			10,
			12
		}, new int[]
		{
			-1000,
			-504,
			-503,
			10,
			11,
			12
		}));
		ClassList.classes.Add(new Class("Chateau", "Maid Knight", "c015", "A chivalrous maid dedicated to protecting her weaker allies. In a pinch, she can also digest them for a boost of power.", "Master", "Master", 15, new Vector3(0.385f, 0.77f, 150f), new Vector3(0.456f, 0.854f, 300f), new Vector3(0.5f, 0.82f, 350f), 280f, 150f, 60f, 60f, 50f, 40f, 40f, 50f, 560f, 300f, 120f, 130f, 70f, 50f, 80f, 80f, 3f, UnitInfo.Move.Ground, 1f, 150f, 5, 0, 1, 5, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(15, 50f), new Dice(10, 50f), new Dice(5, 50f), new Dice(5, 30f), new Dice(5, 50f), new Dice(5, 50f), new Dice(3, 50f), new Dice(5, 50f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			15,
			16,
			1142,
			1144
		}, new int[]
		{
			-1000,
			-504,
			-503,
			-99,
			25,
			26,
			27,
			28
		}));
		ClassList.classes.Add(new Class("Lamia", "Lamia", "m010", "A powerful unit that can lend an extremely powerful buff to nearby allies with her presence alone, or devour them to gain extra buffs.", "Human", "Darling", 100, new Vector3(0.45f, 0.81f), new Vector3(0.5f, 0.94f), new Vector3(0.55f, 0.85f, 50f), 360f, 100f, 70f, 50f, 70f, 80f, 50f, 40f, 480f, 200f, 100f, 100f, 100f, 100f, 100f, 100f, 3f, UnitInfo.Move.Ground, 1f, 300f, 1, 4, 2, 10, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(10, 50f), new Dice(10, 50f), new Dice(10, 50f), new Dice(3, 30f), new Dice(8, 30f), new Dice(8, 50f), new Dice(8, 50f), new Dice(8, 50f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			1146
		}, new int[]
		{
			-1005,
			-504,
			-503,
			1000,
			1010,
			1011
		}));
		ClassList.classes.Add(new Class("Wild Lamia", "Wild Lamia", "m011", "A hyper aggresive unit that specializes in powering up by eating other units. In the field, she'll eat any ally she can get her hands on.", "Meat", "Darling", 101, new Vector3(0.46f, 0.83f), new Vector3(0.52f, 0.94f), new Vector3(0.55f, 0.85f, 50f), 320f, 60f, 80f, 30f, 80f, 45f, 55f, 45f, 480f, 200f, 100f, 100f, 100f, 100f, 100f, 100f, 3f, UnitInfo.Move.Ground, 1f, 300f, 1, 4, 2, 10, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(10, 50f), new Dice(10, 50f), new Dice(10, 50f), new Dice(3, 30f), new Dice(8, 30f), new Dice(8, 50f), new Dice(8, 50f), new Dice(8, 50f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			1143
		}, new int[]
		{
			-1005,
			-504,
			-503,
			1000,
			1005,
			1006
		}));
		ClassList.classes.Add(new Class("Dusk Lamia", "Dusk Lamia", "m012", "A gentle woman whose powers over sleep allow her to quickly disarm and devour nearby predators.", "Dear", "Darling", 102, new Vector3(0.45f, 0.83f), new Vector3(0.52f, 0.94f), new Vector3(0.55f, 0.85f, 50f), 400f, 160f, 50f, 45f, 60f, 90f, 70f, 50f, 480f, 200f, 100f, 100f, 100f, 100f, 100f, 100f, 3f, UnitInfo.Move.Ground, 1f, 300f, 2, 4, 2, 10, 1, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, new Dice(20, 50f), new Dice(10, 50f), new Dice(6, 50f), new Dice(3, 30f), new Dice(8, 50f), new Dice(10, 40f), new Dice(5, 25f), new Dice(5, 25f), new int[]
		{
			-1001,
			-1000,
			0,
			1,
			2,
			1000,
			1001,
			1002,
			1003,
			1141
		}, new int[]
		{
			-1005,
			-504,
			-503,
			1000,
			1001
		}));
	}

	public static Class GetByID(int id)
	{
		return ClassList.classes.Find((Class x) => x.refID == id);
	}

	public static void LevelUp(int id, float xp)
	{
		Class @class = ClassList.classes.Find((Class x) => x.refID == id);
		@class.exp += xp;
		if (@class.exp >= ClassList.CalcEXPToNextLevel(id))
		{
			@class.exp -= ClassList.CalcEXPToNextLevel(id);
			@class.level++;
		}
	}

	public static float CalcEXPToNextLevel(int id)
	{
		Class @class = ClassList.classes.Find((Class x) => x.refID == id);
		float num = (float)(20 + @class.rarity * 5);
		for (int i = 1; i < @class.level; i++)
		{
			num += 5f;
			num += (float)(@class.rarity / 2);
		}
		return num;
	}

	public static float CalcEXPToNextLevel(int id, int level)
	{
		Class @class = ClassList.classes.Find((Class x) => x.refID == id);
		float num = (float)(20 + @class.rarity * 5);
		for (int i = 1; i < level; i++)
		{
			num += 5f;
			num += (float)(@class.rarity / 2);
		}
		return num;
	}

	public static float CalcStat(float lv, float s, float ms)
	{
		float num = s;
		lv -= 1f;
		int num2 = 0;
		while ((float)num2 < lv)
		{
			if (num2 < 20)
			{
				num += (ms - s) * 0.025f;
			}
			else if (num2 < 40)
			{
				num += (ms - s) * 0.015f;
			}
			else
			{
				num += (ms - s) * 0.00333f;
			}
			num2++;
		}
		return num;
	}

	public static List<Class> classes;
}
