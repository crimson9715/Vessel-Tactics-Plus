using System.Collections.Generic;
using UnityEngine;

public static class StatusList
{
	public static List<Status> statuses;

	private static UnitInfo current;

	private static UnitInfo target;

	public static void Initialize()
	{
		statuses = new List<Status>();
		int id = 0;
		statuses.Add(new Status(0, "Dizzy", "Dizzy", "Prone to incredible feats of clumsiness. Chance of stumbling and falling into a nearby unit's mouth at the end of turn.", "Status", Skill.Type.TurnEnd, Status.Removal.TurnEnd, 0, Status.Visibility.Visible, delegate
		{
			id = 0;
			Debug.Log("Activating skill 0");
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			EventScene eventScene = StaticRef.eventScene;
			List<UnitInfo> list = new List<UnitInfo>();
			UnitInfo current = StaticRef.CurrentUnit();
			Status status = current.statuses.Find((Status x) => x.refID == id);
			if (--status.removalParam <= 0)
			{
				current.statuses.Remove(status);
			}
			if (current.GetPreyState() != 0)
			{
				current.Pause(x: false);
			}
			else
			{
				foreach (Tile item in TileMap.GetTile(current.transform.position).CheckAdjacent())
				{
					UnitInfo unitInfo = null;
					if (TileMap.GetOccupant(item) != null)
					{
						unitInfo = TileMap.GetOccupant(item).GetComponent<UnitInfo>();
						if (unitInfo != null && unitInfo.TotalStomach() <= unitInfo.MaxStomach() + 1 - current.size - current.TotalStomach() && !unitInfo.isSafeContainer() && unitInfo.isAlive())
						{
							list.Add(TileMap.GetOccupant(item).GetComponent<UnitInfo>());
						}
					}
				}
				if (current.TotalStomach() > 1 || list.Count <= 0 || !Rndm.Chance(100f))
				{
					current.Pause(x: false);
				}
				else
				{
					Rndm.SortPlayer(list, !current.isPlayer);
					UnitInfo target = list.ToArray()[0];
					StaticRef.turnController.HideAll();
					StaticRef.Delay(0.5f, delegate
					{
						StaticRef.Message();
						current.speechLib.Set(current, target, SkillList.GetByID(0));
						target.speechLib.Set(current, target, SkillList.GetByID(0));
						eventScene.Setup(current, target, SkillList.GetByID(id));
						eventScene.RunSkill(id);
					});
				}
			}
		}));
		statuses.Add(new Status(16, "Battle Cry", "BattleCry", "Attack has been strengthened by Chateau.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 16;
			current.dAtk += (int)(current.atk * 0.2f);
			current.dVatk += (int)(current.vatk * 0.2f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(17, "Shattering Belch", "BattleCry2", "Defense has been shaken by Chateau.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 2, Status.Visibility.Visible, delegate
		{
			id = 17;
			current.dDef -= (int)(current.def * 0.25f);
			current.dVres -= (int)(current.vres * 0.25f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(21, "Sanctuary", "Sanctuary", "Has been granted extra max HP after being healed by Covenant.", "Buff", Skill.Type.Buff, Status.Removal.Damage, 0, Status.Visibility.Visible, delegate
		{
			id = 21;
			current.dMhp += current.statuses.Find((Status x) => x.refID == 21).removalParam;
			if (current.hp == current.Mhp() - (float)current.statuses.Find((Status x) => x.refID == 21).removalParam)
			{
				current.hp = current.Mhp();
			}
			current.Pause(x: false);
		}));
		statuses.Add(new Status(24, "Narrow Escape", "NarrowEscape", "Narrowly escaped becoming Covenant's lunch. +20 VRES.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 3, Status.Visibility.Visible, delegate
		{
			id = 24;
			current.dVres += 20f;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(142, "Charred", "Charred", "Burned by Covenant's holy rays. Lowers ATK, VATK, SPD, and SKL by 20%.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 142;
			current.dAtk -= (int)(current.atk * 0.2f);
			current.dVatk -= (int)(current.vatk * 0.2f);
			current.dSkl -= (int)(current.skl * 0.2f);
			current.dSpd -= (int)(current.spd * 0.2f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(143, "Paralysis", "Paralysis", "Inflicted by nerve gas, reducing all stats by 20% and halving movement range.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 143;
			current.dAtk -= (int)(current.atk * 0.2f);
			current.dDef -= (int)(current.def * 0.2f);
			current.dVatk -= (int)(current.vatk * 0.2f);
			current.dVres -= (int)(current.vres * 0.2f);
			current.dSkl -= (int)(current.skl * 0.2f);
			current.dSpd -= (int)(current.spd * 0.2f);
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(144, "Polyphagia", "Polyphagia", "Inflicted with extreme hunger. Increases VATK by 50%, but reduces energy to 0 when inflicted.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 3, Status.Visibility.Visible, delegate
		{
			id = 144;
			current.dVatk += (int)(current.vatk * 0.5f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(145, "Lubricated", "Lubricated", "Covered in slippery lubricant. +1000 VRES.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 145;
			current.dVres += 1000f;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(146, "Caffeinated", "Caffeinated", "Full of energy after drinking coffee. 50% increase to SPD and SKL.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 146;
			current.dSkl += (int)(current.skl * 0.5f);
			current.dSpd += (int)(current.spd * 0.5f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(147, "Massaged", "Massaged", "Massaged by Chateau's skillful hands. Speeds up digestion by 1 turn.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 147;
			current.containPeriod -= 1f;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(148, "Injured", "Injured", "Injured after a powerful bite attack. Decreases SKL, SPD, DEF, and VRES by 20%, and halves movement speed.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 2, Status.Visibility.Visible, delegate
		{
			id = 148;
			current.dSkl -= (int)(current.skl * 0.2f);
			current.dSpd -= (int)(current.spd * 0.2f);
			current.dVres -= (int)(current.vres * 0.2f);
			current.dDef -= (int)(current.def * 0.2f);
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(148, "Injured", "Injured", "Injured after a powerful bite attack. Decreases SKL, SPD, DEF, and VRES by 20%, and halves movement speed.", "Status", Skill.Type.Buff, Status.Removal.TurnEnd, 2, Status.Visibility.Visible, delegate
		{
			id = 148;
			current.dSkl -= (int)(current.skl * 0.2f);
			current.dSpd -= (int)(current.spd * 0.2f);
			current.dVres -= (int)(current.vres * 0.2f);
			current.dDef -= (int)(current.def * 0.2f);
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		statuses.Add(new Status(149, "Prideful", "Prideful", "Pride has swollen after eating a unit. 20% boost to all stats.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 149;
			current.dAtk += (int)(current.atk * 0.2f);
			current.dDef += (int)(current.def * 0.2f);
			current.dVatk += (int)(current.vatk * 0.2f);
			current.dVres += (int)(current.vres * 0.2f);
			current.dSkl += (int)(current.skl * 0.2f);
			current.dSpd += (int)(current.spd * 0.2f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(150, "Egotistical", "Egotistical", "Pride has reached beyond its limits after eating too much. 40% boost to all stats.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 150;
			current.dAtk += (int)(current.atk * 0.4f);
			current.dDef += (int)(current.def * 0.4f);
			current.dVatk += (int)(current.vatk * 0.4f);
			current.dVres += (int)(current.vres * 0.4f);
			current.dSkl += (int)(current.skl * 0.4f);
			current.dSpd += (int)(current.spd * 0.4f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(151, "Beautiful", "Beautiful", "Unit has received a massive boost to her confidence after being worked on by Lamia. 20% boost to all stats.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 151;
			current.dAtk += (int)(current.atk * 0.2f);
			current.dDef += (int)(current.def * 0.2f);
			current.dVatk += (int)(current.vatk * 0.2f);
			current.dVres += (int)(current.vres * 0.2f);
			current.dSkl += (int)(current.skl * 0.2f);
			current.dSpd += (int)(current.spd * 0.2f);
			current.Pause(x: false);
		}));
		statuses.Add(new Status(1011, "You go, girl!", "YouGoGirl", "Motivated to do her best! Gains +10% to all stats.", "Buff", Skill.Type.Buff, Status.Removal.TurnEnd, 1, Status.Visibility.Visible, delegate
		{
			id = 1011;
			current.dAtk += (int)(current.atk * 0.1f);
			current.dDef += (int)(current.def * 0.1f);
			current.dVatk += (int)(current.vatk * 0.1f);
			current.dVres += (int)(current.vres * 0.1f);
			current.dSkl += (int)(current.skl * 0.1f);
			current.dSpd += (int)(current.spd * 0.1f);
			current.Pause(x: false);
		}));
	}

	public static Status GetByID(int id)
	{
		return statuses.Find((Status x) => x.refID == id);
	}

	public static Status AddStatus(int id, int rp)
	{
		return new Status(GetByID(id), rp);
	}

	public static void SetCurrent(UnitInfo c)
	{
		current = c;
	}

	public static void Setup(UnitInfo c, UnitInfo t)
	{
		current = c;
		target = t;
	}
}
