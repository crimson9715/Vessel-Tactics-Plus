using System.Collections.Generic;
using UnityEngine;

public static class SkillList
{
	public static List<Skill> skills;

	private static UnitInfo current;

	private static UnitInfo target;

	public static void Initialize()
	{
		skills = new List<Skill>();
		int id = 0;
		skills.Add(new Skill(-1005, "TwoBelly", "TwoBelly", "Has two stomachs.", "Trait", Skill.Type.Inactive, Skill.Visibility.Invisible, delegate
		{
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-1001, "Healer", "Healer", "Able to heal allied units by 'digesting' them.", "Trait", Skill.Type.Inactive, Skill.Visibility.Invisible, delegate
		{
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-1000, "Carrier", "Carrier", "Able to safely carry allied units inside her stomach. Can swallow twice as many units before becoming encumbered and receives lower penalties.", "Trait", Skill.Type.Inactive, Skill.Visibility.Invisible, delegate
		{
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-504, "ProcessENDecay", "ProcessENDecay", "", "", Skill.Type.TurnEnd, Skill.Visibility.Invisible, delegate
		{
			Debug.Log("Activating skill -504 " + current);
			if (current.en > 0f && current.GetPredState() != UnitInfo.PredState.V_Container && current.hp > 0f)
			{
				if (StaticRef.turnController.turnType == 2 || current.GetPredState() == UnitInfo.PredState.S_Container || current.GetPredState() == UnitInfo.PredState.H_Container)
				{
					current.en -= current.EnRate();
				}
				else if (StaticRef.turnController.turnType == 0)
				{
					current.en += 10f;
				}
			}
			if (current.en < 0f)
			{
				current.en = 0f;
			}
			else if (current.en > current.Men())
			{
				current.en = current.Men();
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-503, "ProcessHunger", "ProcessHunger", "", "", Skill.Type.TurnStart, Skill.Visibility.Invisible, delegate
		{
			id = -503;
			Debug.Log("Activating skill " + id);
			if (current.en <= 0f && current.GetPredState() != UnitInfo.PredState.V_Container && current.hp > 0f && current.containedByUnit == null)
			{
				StaticRef.turnController.gameState = TurnController.GameState.Transition;
				EventScene eventScene6 = StaticRef.eventScene;
				StaticRef.turnController.HideAll();
				StaticRef.Message();
				current.speechLib.Set(current, GetByID(id));
				eventScene6.Setup(current, null, GetByID(id));
				eventScene6.RunSkill(id);
			}
			else
			{
				current.Pause(x: false);
			}
		}));
		skills.Add(new Skill(-502, "ProcessHealFinish", "ProcessHealFinish", "", "", Skill.Type.Inactive, Skill.Visibility.Invisible, delegate
		{
			id = -502;
			Debug.Log("Activating skill -502");
			if (current.containingUnits.Count > 0)
			{
				StaticRef.turnController.gameState = TurnController.GameState.Transition;
				EventScene eventScene5 = StaticRef.eventScene;
				StaticRef.turnController.HideAll();
				StaticRef.Message();
				current.speechLib.Set(current, GetByID(id));
				eventScene5.Setup(current, null, GetByID(id));
				eventScene5.RunSkill(id);
			}
			else
			{
				current.Pause(x: false);
			}
		}));
		skills.Add(new Skill(-501, "ProcessKO", "ProcessKO", "", "", Skill.Type.ActionEnd, Skill.Visibility.Invisible, delegate
		{
			id = -501;
			Debug.Log("Activating skill -501 " + current.name);
			if (StaticRef.eventScene.GetLastAttack().type != 0)
			{
				current.Pause(x: false);
			}
			else if (current.hp <= 0f && current.containingUnits.Count > 0)
			{
				Debug.Log("Activating skill -501 123" + current.name);
				StaticRef.turnController.gameState = TurnController.GameState.Transition;
				EventScene eventScene4 = StaticRef.eventScene;
				StaticRef.turnController.HideAll();
				StaticRef.Message();
				current.speechLib.Set(current, GetByID(-501));
				eventScene4.Setup(current, null, GetByID(id));
				eventScene4.RunSkill(id);
			}
			else if (current.hp <= 0f)
			{
				Debug.Log("Activating skill -501 456 " + current.name);
				current.ProcessKO();
				current.Pause(x: false);
			}
			else
			{
				current.Pause(x: false);
			}
		}));
		skills.Add(new Skill(-500, "EndSafeDigestion", "EndSafeDigestion", "", "", Skill.Type.ActionEnd, Skill.Visibility.Invisible, delegate
		{
			Debug.Log("Activating skill -500");
			if (current.GetPredState() == UnitInfo.PredState.V_Container && current.skills.Contains(120))
			{
				current.skills.Remove(120);
			}
			else if (current.GetPredState() == UnitInfo.PredState.V_Container && current.skills.Contains(140))
			{
				current.skills.Remove(140);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-100, "Normal Stomach", "NormalStomach", "", "", Skill.Type.StomachSizer, Skill.Visibility.Invisible, delegate
		{
			Debug.Log("Activating skill -100");
			current.skills.Remove(100);
			current.skills.Remove(102);
			current.skills.Remove(103);
			current.skills.Remove(104);
			current.skills.Remove(105);
			if (current.TotalStomach() > current.MaxStomach())
			{
				Debug.Log("104");
				current.skills.Add(104);
				current.skills.Add(105);
				if (current == StaticRef.CurrentUnit())
				{
					StaticRef.turnController.moveComplete = true;
				}
			}
			else if (current.TotalStomach() == 1)
			{
				Debug.Log("100");
				current.skills.Add(100);
			}
			else if (current.TotalStomach() == 2)
			{
				Debug.Log("102");
				current.skills.Add(102);
			}
			else if (current.TotalStomach() == 3)
			{
				Debug.Log("103");
				current.skills.Add(103);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(-99, "Big Stomach", "BigStomach", "", "", Skill.Type.StomachSizer, Skill.Visibility.Invisible, delegate
		{
			Debug.Log("Activating skill -99");
			current.skills.Remove(122);
			current.skills.Remove(123);
			current.skills.Remove(124);
			current.skills.Remove(125);
			current.skills.Remove(126);
			current.skills.Remove(127);
			current.skills.Remove(128);
			current.skills.Remove(129);
			if (current.TotalStomach() > current.MaxStomach())
			{
				current.skills.Add(128);
				current.skills.Add(129);
				if (current == StaticRef.CurrentUnit())
				{
					StaticRef.turnController.moveComplete = true;
				}
			}
			else if (current.TotalStomach() >= 6)
			{
				current.skills.Add(127);
			}
			else if (current.TotalStomach() >= 5)
			{
				current.skills.Add(126);
			}
			else if (current.TotalStomach() >= 4)
			{
				current.skills.Add(125);
			}
			else if (current.TotalStomach() >= 3)
			{
				current.skills.Add(124);
			}
			else if (current.TotalStomach() >= 2)
			{
				current.skills.Add(123);
			}
			else if (current.TotalStomach() >= 1)
			{
				current.skills.Add(122);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(0, "Misfortune", "Misfortune", "Prone to incredible feats of clumsiness. Chance of stumbling and falling into a nearby unit's mouth at the end of turn.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Visible, delegate
		{
			id = 0;
			Debug.Log("Activating skill 0");
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			EventScene eventScene3 = StaticRef.eventScene;
			List<UnitInfo> list = new List<UnitInfo>();
			UnitInfo current = StaticRef.CurrentUnit();
			if (current.GetPreyState() != 0)
			{
				current.Pause(x: false);
			}
			else
			{
				foreach (Tile item in TileMap.GetTile(current.transform.position).CheckAdjacent())
				{
					UnitInfo unitInfo6 = null;
					if (TileMap.GetOccupant(item) != null)
					{
						unitInfo6 = TileMap.GetOccupant(item).GetComponent<UnitInfo>();
						if (unitInfo6 != null && unitInfo6.TotalStomach() <= unitInfo6.MaxStomach() + 1 - current.size - current.TotalStomach() && !unitInfo6.isSafeContainer() && unitInfo6.isAlive())
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
						current.speechLib.Set(current, target, GetByID(0));
						target.speechLib.Set(current, target, GetByID(0));
						eventScene3.Setup(current, target, GetByID(id));
						eventScene3.RunSkill(id);
					});
				}
			}
		}));
		skills.Add(new Skill(1, "Plump and Delicious!", "PlumpAndDelicious", "If digested by an enemy, increase their total Nutritional Value by 50 points. If digested by an ally, restore their HP by an additional 25%. At the start of her turn while digesting units, Cornucopia grants all contained units +25 Nutrition but loses 20 energy.", "Trait", Skill.Type.OnDigested, Skill.Visibility.Visible, delegate
		{
			UnitInfo containedByUnit = current.containedByUnit;
			while (containedByUnit.containedByUnit != null)
			{
				containedByUnit = containedByUnit.containedByUnit;
			}
			if (containedByUnit.team == current.team)
			{
				containedByUnit.hp += containedByUnit.Mhp() / 4f;
				if (containedByUnit.hp > containedByUnit.Mhp())
				{
					containedByUnit.hp = containedByUnit.Mhp();
				}
			}
			else
			{
				containedByUnit.nutValue += 50;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(2, "Oblivious", "Oblivious", "Extremely durable, but can't really be bothered to apply herself. Has virtually no Vore Resistance as a result.", "Trait", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(3, "Brunch Squad", "BrunchSquad", "Other vessels are used to treating this unit like snack food and will eat her first, if hungry.", "Trait", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(4, "Placeholder", "Placeholder", "This skill has yet to be implemented.", "Trait", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(5, "Keen Claws", "KeenClaws", "May occasionally deal critical hits.\nChance = (Skill x 25%).", "Trait", Skill.Type.DamageMod, Skill.Visibility.Visible, delegate
		{
			if (Rndm.Chance(StaticRef.CurrentUnit().Skl() / 4f) && StaticRef.turnController.gameState == TurnController.GameState.Battle)
			{
				StaticRef.damage *= 1.5f;
			}
		}));
		skills.Add(new Skill(6, "Overflowing Pockets", "OverflowingPockets", "Upon being digested by an enemy, trigger a random vial effect targeting them.", "Trait", Skill.Type.OnDigested, Skill.Visibility.Visible, delegate
		{
			id = 6;
			Debug.Log("Activating skill " + id);
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			StaticRef.turnController.HideAll();
			UnitInfo topPred2 = current.GetTopPred();
			if (current.team != topPred2.team)
			{
				StaticRef.Message();
				topPred2.speechLib.Set(topPred2, GetByID(id));
				StaticRef.eventScene.Setup(topPred2, null, GetByID(id));
				StaticRef.eventScene.RunSkill(id);
			}
			else
			{
				current.Pause(x: false);
			}
		}));
		skills.Add(new Skill(7, "Quantum Paradox", "QuantumParadox", "When failing a roll, has a 50% chance of re-rolling it with half the chance of succeeding. (This does not effect All-In.)", "Trait", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
			id = 7;
		}));
		skills.Add(new Skill(10, "Bionic Legs", "ReinforcedLegs", "Does not suffer any speed debuffs from eating other units. (Will still become overburdened if exceeding max limit.)", "Trait", Skill.Type.StomachSizer, Skill.Visibility.Visible, delegate
		{
			id = 10;
			Debug.Log("Activating skill " + id);
			current.skills.Remove(104);
			current.skills.Remove(105);
			if (current.TotalStomach() > current.MaxStomach())
			{
				Debug.Log("104");
				current.skills.Add(104);
				current.skills.Add(105);
				if (current == StaticRef.CurrentUnit())
				{
					StaticRef.turnController.moveComplete = true;
				}
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(11, "Protector of the Weak", "IllSaveYou", "Deals increased damage to enemies that have eaten allies. Additionally, Theano can safely carry allied units in her stomach without digesting them.", "Trait", Skill.Type.DamageMod, Skill.Visibility.Visible, delegate
		{
			if (StaticRef.TargetUnit().isContainingAlly(current))
			{
				StaticRef.damage *= 1.2f;
			}
		}));
		skills.Add(new Skill(12, "Determination", "Determination", "When HP is over 50% the Guts effect granted by it is doubled, vastly increasing Vore Resistance.", "Trait", Skill.Type.VResMod, Skill.Visibility.Visible, delegate
		{
			if (current.hp >= current.Mhp() / 2f)
			{
				StaticRef.vEva = StaticRef.vEva * current.CalculateEndurance() + current.hp * 0.05f;
			}
		}));
		skills.Add(new Skill(15, "Swollen With Pride", "IDidIt", "Grants an additional 200 Nutritional EXP if digested while containing a unit. At the end of and beginning of her turn, gains a 20% stat boost per unit contained.", "Trait", Skill.Type.PreDigested, Skill.Visibility.Visible, delegate
		{
					
			foreach (UnitInfo containingUnit in current.containingUnits)
			{

					current.nutValue += 200;
					break;

			}
		}));
		skills.Add(new Skill(16, "Determined", "Determined", "Focuses on becoming a strong predator with everything she has. Relatively low defensive stats, but high offensive stats and growths.", "Trait", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
			


					if (current.skills.Contains(100))
					{
					current.dAtk += (int)(current.atk * 0.2f);
			current.dDef += (int)(current.def * 0.2f);
			current.dVatk += (int)(current.vatk * 0.2f);
			current.dVres += (int)(current.vres * 0.2f);
			current.dSkl += (int)(current.skl * 0.2f);
			current.dSpd += (int)(current.spd * 0.2f);
			current.AddStatus(StatusList.AddStatus(149, 1));
					}
					if (current.skills.Contains(104))
					{
					current.dAtk += (int)(current.atk * 0.4f);
			current.dDef += (int)(current.def * 0.4f);
			current.dVatk += (int)(current.vatk * 0.4f);
			current.dVres += (int)(current.vres * 0.4f);
			current.dSkl += (int)(current.skl * 0.4f);
			current.dSpd += (int)(current.spd * 0.4f);
			current.AddStatus(StatusList.AddStatus(150, 1));
					}	
					current.Pause(x: false);
			
		}));
		skills.Add(new Skill(17, "PrideEnd", "PrideEnd", "Swollen with Pride's effect handler for the end of turn effect.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Invisible, delegate
		{
			


					if (current.skills.Contains(100))
					{
					current.dAtk += (int)(current.atk * 0.2f);
			current.dDef += (int)(current.def * 0.2f);
			current.dVatk += (int)(current.vatk * 0.2f);
			current.dVres += (int)(current.vres * 0.2f);
			current.dSkl += (int)(current.skl * 0.2f);
			current.dSpd += (int)(current.spd * 0.2f);
			current.AddStatus(StatusList.AddStatus(149, 1));
					}
					if (current.skills.Contains(104))
					{
					current.dAtk += (int)(current.atk * 0.4f);
			current.dDef += (int)(current.def * 0.4f);
			current.dVatk += (int)(current.vatk * 0.4f);
			current.dVres += (int)(current.vres * 0.4f);
			current.dSkl += (int)(current.skl * 0.4f);
			current.dSpd += (int)(current.spd * 0.4f);
			current.AddStatus(StatusList.AddStatus(150, 1));
					}	
					current.Pause(x: false);
			
		}));
		skills.Add(new Skill(20, "Sin of Gluttony", "SinofGluttony", "Does not lose energy each turn normally, instead loses it at an increased rate for each ally currently being healed.", "Trait", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
			current.enRate = 0f;
			if (current.GetPredState() == UnitInfo.PredState.H_Container)
			{
				foreach (UnitInfo containingUnit2 in current.containingUnits)
				{
					_ = containingUnit2;
					current.enRate += 10f;
				}
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(21, "Sanctuary", "Sanctuary", "Able to heal allied units by 'digesting' them. Increases allies max HP by 10% and restores 10% of max EN for each turn they spend inside her.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Visible, delegate
		{
			if (current.GetPredState() == UnitInfo.PredState.H_Container)
			{
				foreach (UnitInfo containingUnit3 in current.containingUnits)
				{
					Status status = containingUnit3.statuses.Find((Status x) => x.refID == 21);
					if (status != null)
					{
						containingUnit3.statuses.Remove(status);
						containingUnit3.AddStatus(StatusList.AddStatus(21, (int)(containingUnit3.mhp * 0.1f) + status.removalParam));
						containingUnit3.en += (containingUnit3.Men() * 0.1f);
						if (containingUnit3.en > containingUnit3.Men())
							{
								containingUnit3.en = containingUnit3.Men();
							}
					}
					else
					{
						containingUnit3.AddStatus(StatusList.AddStatus(21, (int)(containingUnit3.mhp * 0.1f)));
						containingUnit3.en += (containingUnit3.Men() * 0.1f);
						if (containingUnit3.en > containingUnit3.Men())
							{
								containingUnit3.en = containingUnit3.Men();
							}
					}
				}
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(22, "SanctuaryContain", "SanctuaryContain", "Able to heal allied units by 'digesting' them. Overheals allies by an additional 10% for each turn they spend inside her.", "Trait", Skill.Type.OnContain, Skill.Visibility.Invisible, delegate
		{
			if (current.GetPredState() == UnitInfo.PredState.H_Container && !target.skillTracker.ContainsKey(21))
			{
				target.skillTracker.Add(21, 0);
			}
		}));
		skills.Add(new Skill(23, "SanctuaryTick", "SanctuaryTick", "Able to heal allied units by 'digesting' them. Overheals allies by an additional 10% for each turn they spend inside her.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Invisible, delegate
		{
			if (current.GetPredState() == UnitInfo.PredState.H_Container)
			{
				foreach (UnitInfo containingUnit4 in current.containingUnits)
				{
					if (containingUnit4.skillTracker.ContainsKey(21))
					{
						target.skillTracker.TryGetValue(21, out var value);
						value++;
						target.skillTracker.Remove(21);
						target.skillTracker.Add(21, value);
					}
				}
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(24, "Narrow Escape", "NarrowEscape", "Allies healed by Covenant have their VRES increased by 20 points for 3 turns.", "Trait", Skill.Type.OnHeal, Skill.Visibility.Visible, delegate
		{
			foreach (UnitInfo containingUnit5 in current.containingUnits)
			{
				containingUnit5.AddStatus(StatusList.AddStatus(24, 3));
			}
		}));
		skills.Add(new Skill(25, "Heavyweight", "LowMetabolism", "Chateau absorbs the bodies of her prey much slower than other vessels. Gains Max HP / Max EN and loses SPD each weight level. Retains 100% of prey's nutritional value.", "Trait", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("HeavyWeight");
			for (int i = 0; i < current.weight; i++)
			{
				Debug.Log("Buffing stats");
				current.dSpd -= current.spd * 0.1f;
				current.dMhp += current.mhp * 0.05f;
				current.dMen += current.dMen * 0.1f;
			}
			if (current.hp >= current.mhp)
			{
				current.hp = current.Mhp();
			}
			if (current.en >= current.Men())
			{
				current.en = current.Men();
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(26, "Low Metabolism", "LowMetabolismDigest", "Chateau absorbs the bodies of her prey much slower than other vessels. Gains Max HP and loses SPD each weight level. Retains 100% of prey's nutritional value.", "Trait", Skill.Type.OnDigest, Skill.Visibility.Invisible, delegate
		{
			foreach (UnitInfo item2 in current.CombinedStomach())
			{
				current.nutValue += item2.nutValue / 2;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(27, "Emergency Rations", "EmergencyRations", "Upon being KO'd while carrying allies, begin digesting all allies inside and recover 15% HP for each one.", "Trait", Skill.Type.OnKO, Skill.Visibility.Visible, delegate
		{
			id = 27;
			Debug.Log("Activating skill " + id);
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			StaticRef.turnController.HideAll();
			if (current.GetPredState() == UnitInfo.PredState.S_Container)
			{
				StaticRef.Message();
				current.speechLib.Set(current, GetByID(id));
				StaticRef.eventScene.Setup(current, null, GetByID(id));
				StaticRef.eventScene.RunSkill(id);
			}
			else
			{
				current.Pause(x: false);
			}
		}));
		skills.Add(new Skill(28, "Bulwark", "Bulwark", "Chateau has a tremendous stomach capacity and can carry allied units without digesting them. In addition, Chateau will suffer from vore debuffs at half the normal rate.", "Trait", Skill.Type.StomachSizer, Skill.Visibility.Visible, delegate
		{
			id = 28;
			Debug.Log("Activating skill " + id);
			current.skills.Remove(122);
			current.skills.Remove(123);
			current.skills.Remove(124);
			current.skills.Remove(125);
			current.skills.Remove(126);
			current.skills.Remove(127);
			current.skills.Remove(128);
			current.skills.Remove(129);
			if (current.TotalStomach() > current.MaxStomach())
			{
				current.skills.Add(128);
				current.skills.Add(129);
				if (current == StaticRef.CurrentUnit())
				{
					StaticRef.turnController.moveComplete = true;
				}
			}
			else if (current.TotalStomach() >= 6)
			{
				current.skills.Add(127);
			}
			else if (current.TotalStomach() >= 5)
			{
				current.skills.Add(126);
			}
			else if (current.TotalStomach() >= 4)
			{
				current.skills.Add(125);
			}
			else if (current.TotalStomach() >= 3)
			{
				current.skills.Add(124);
			}
			else if (current.TotalStomach() >= 2)
			{
				current.skills.Add(123);
			}
			else if (current.TotalStomach() >= 1)
			{
				current.skills.Add(122);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(30, "Narrow Escape", "NarrowEscape", "Allies healed by Covenant have their VRES increased by 10 points for 3 turns.", "Trait", Skill.Type.OnSpeech, Skill.Visibility.Invisible, delegate
		{
			Setup(StaticRef.CurrentUnit(), StaticRef.TargetUnit());
			EventScene e = StaticRef.eventScene;
			Debug.Log("Inserting text");
			if (e.currentSpeech == "BattleStart" && current.cCap > 0 && current.clas.hCap >= current.hCap + current.containingUnits.ToArray()[0].TotalSize())
			{
				Debug.Log("Inserting text 2");
				Rndm.InsertToQueue(e.EventQ(), delegate
				{
					Debug.Log("Inserting text 3");
					current.SecondStomachMove();
					current.CalcCap();
					e.stomachSize = current.hCap;
					e.targetWeight = current.weight;
					e.UpdateSprite(current, e.playerMask);
					e.TW().LoadText("<SEse_chupa4><SEse_gulp1><SEeaten_by_monster2>The " + current.unitName + " sucks " + current.secondStomach.ToArray()[current.secondStomach.Count - 1].unitName + " deeper inside her to make way for her next meal!");
					e.wait = true;
				});
			}
		}));
		skills.Add(new Skill(50, "Possessed", "Possessed", "Possessed by a playful Will o' Wisp. Will attempt to feed themselves to a random nearby unit. Deal damage to bring them to their senses!", "Status", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(99, "Incapacitated", "Incapacitated", "HP has been reduced to 0. Cannot move or act until HP is restored and will not resist vore.", "Status", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
			id = 99;
			Debug.Log("Activating skill 99");
			if (current.hp <= 0f && current.containingUnits.Count > 0)
			{
				current.speechLib.Set(current, GetByID(104));
				current.isPlayer = false;
				StaticRef.turnController.actionComplete = false;
				StaticRef.turnController.moveComplete = true;
			}
			else if (current.hp <= 0f)
			{
				StaticRef.turnController.moveComplete = true;
				StaticRef.turnController.actionComplete = true;
			}
			else
			{
				current.skills.Remove(99);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(100, "Full", "Full", "Speed is halved.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 100");
			current.dSpd -= current.spd / 2f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(101, "Digesting", "Digesting", "This unit is currently being digested and cannot move or take any actions.", "Status", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(102, "Stuffed", "Stuffed", "Speed is reduced to zero.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 102");
			current.dSpd = -9999f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(103, "Bloated", "Bloated", "Speed is reduced to zero and movement is halved.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 103");
			current.dSpd = -9999f;
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(104, "Overburdened", "Overburdened", "Speed and movement are reduced to zero. Cannot move or take any actions until digestion is finished.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 104");
			current.dSpd = -9999f;
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(105, "Overburdened", "OverburdenedEvent", "Speed and movement are reduced to zero. Cannot move or take any actions until digestion is finished.", "Status", Skill.Type.TurnStart, Skill.Visibility.Invisible, delegate
		{
			id = 105;
			Debug.Log("Activating skill 105");
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			EventScene eventScene2 = StaticRef.eventScene;
			UnitInfo unitInfo3 = StaticRef.CurrentUnit();
			unitInfo3.isPlayer = false;
			if (unitInfo3.skills.Contains(99) || unitInfo3.en <= 0f)
			{
				unitInfo3.Pause(x: false);
			}
			else
			{
				StaticRef.turnController.HideAll();
				StaticRef.Message();
				unitInfo3.speechLib.Set(unitInfo3, GetByID(104));
				eventScene2.Setup(unitInfo3, null, GetByID(id));
				eventScene2.RunSkill(id);
			}
		}));
		skills.Add(new Skill(120, "Transporting", "Transporting", "Carrying allied units without digesting them. If an enemy is eaten, if digestion is commanded, or if afflicted with certain status effects, will digest all units inside her.", "State", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(121, "Transported", "Transported", "Being safely carried inside the stomach of an allied unit.", "State", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(122, "Sated", "CSated", "No effect.", "Status", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(123, "Full", "CFull", "Speed is decreased by 1/4.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 123");
			current.dSpd -= current.spd / 4f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(124, "Stuffed", "CStuffed", "Speed is decreased by 1/2.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 124");
			current.dSpd -= current.spd / 2f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(125, "Glutted", "CGlutted", "Speed is reduced by 3/4.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 125");
			current.dSpd -= current.spd * 3f / 4f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(126, "Bloated", "CBloated", "Speed is reduced to zero and movement is reduced by 1.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 126");
			current.dSpd -= 9999f;
			current.dMove -= 1f;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(127, "Gorged", "CGorged", "Speed is reduced to zero and movement is reduced by 1/2.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 127");
			current.dSpd -= 9999f;
			current.dMove -= current.movement / 2;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(128, "Overburdened", "COverburdened", "Speed and movement are reduced to zero. Cannot move or take any actions until digestion is finished.", "Status", Skill.Type.Buff, Skill.Visibility.Visible, delegate
		{
			Debug.Log("Activating skill 128");
			current.dSpd -= 9999f;
			current.dMove = -current.movement;
			current.Pause(x: false);
		}));
		skills.Add(new Skill(129, "Overburdened", "COverburdenedEvent", "Speed and movement are reduced to zero. Cannot move or take any actions until digestion is finished.", "Status", Skill.Type.TurnStart, Skill.Visibility.Invisible, delegate
		{
			id = 128;
			Debug.Log("Activating skill 129");
			StaticRef.turnController.gameState = TurnController.GameState.Transition;
			EventScene eventScene = StaticRef.eventScene;
			UnitInfo unitInfo2 = StaticRef.CurrentUnit();
			unitInfo2.isPlayer = false;
			if (unitInfo2.skills.Contains(99))
			{
				unitInfo2.Pause(x: false);
			}
			else
			{
				StaticRef.turnController.HideAll();
				StaticRef.Message();
				unitInfo2.speechLib.Set(unitInfo2, GetByID(128));
				eventScene.Setup(unitInfo2, null, GetByID(105));
				eventScene.RunSkill(105);
			}
		}));
		skills.Add(new Skill(140, "Healing", "Healing", "Healing allied units inside her stomach. If an enemy is eaten, if digestion is commanded, or if afflicted with certain status effects, will digest all units inside her.", "State", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(141, "Resting", "Resting", "Being healed inside the stomach of an allied unit.", "State", Skill.Type.Inactive, Skill.Visibility.Visible, delegate
		{
		}));
		skills.Add(new Skill(200, "Sleeping", "Sleeping", "Forced to begin digesting all units inside immediately. Incapacitated until digestion has finished.", "Status", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
			id = 200;
			Debug.Log("Activating skill 200");
			if (current.TotalStomach() > 0)
			{
				current.speechLib.Set(current, GetByID(104));
				current.isPlayer = false;
				StaticRef.turnController.actionComplete = false;
				StaticRef.turnController.moveComplete = true;
				current.skills.Add(99);
			}
			else
			{
				current.skills.Remove(200);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1000, "Serpentine Body", "Serpentine Body", "Has the lower body of a snake. Can be eaten normally while containing a single unit, however will occupy three spaces in predator's stomach. Suffers debuffs from vore at half the normal rate. Low movement, but high stats.", "Trait", Skill.Type.StomachSizer, Skill.Visibility.Visible, delegate
		{
			id = 1000;
			Debug.Log("Activating skill " + id);
			current.skills.Remove(122);
			current.skills.Remove(123);
			current.skills.Remove(124);
			current.skills.Remove(125);
			current.skills.Remove(126);
			current.skills.Remove(127);
			current.skills.Remove(128);
			current.skills.Remove(129);
			if (current.TotalStomach() > current.MaxStomach())
			{
				current.skills.Add(128);
				current.skills.Add(129);
				if (current == StaticRef.CurrentUnit())
				{
					StaticRef.turnController.moveComplete = true;
				}
			}
			else if (current.TotalStomach() >= 6)
			{
				current.skills.Add(127);
			}
			else if (current.TotalStomach() >= 5)
			{
				current.skills.Add(126);
			}
			else if (current.TotalStomach() >= 4)
			{
				current.skills.Add(125);
			}
			else if (current.TotalStomach() >= 3)
			{
				current.skills.Add(124);
			}
			else if (current.TotalStomach() >= 2)
			{
				current.skills.Add(123);
			}
			else if (current.TotalStomach() >= 1)
			{
				current.skills.Add(122);
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1001, "Sleeping Beauty", "SleepingBeauty", "Digest period decreases by one additional turn each round while asleep. Uses Hibernate instead of Digest when Overburdened.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Visible, delegate
		{
			if (current.skills.Contains(200) && current.containPeriod > 1f)
			{
				current.containPeriod -= 1f;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1005, "Jealous Predator", "JealousPredator", "When eaten by a target, will struggle with all her might until digested, reducing her predator's defense by half. Does not apply when eaten by a friendly unit that is currently safe until they start digesting. Will refuse to feed herself to other units.", "Trait", Skill.Type.ContainedBuff, Skill.Visibility.Visible, delegate
		{
			UnitInfo topPred = current.GetTopPred();
			if (topPred != null && !topPred.isSafeContainer() )
			{
				topPred.dDef -= topPred.def / 2f;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1006, "Might Makes Right", "MightMakesRight", "Vore hit rate is increased the higher Wild Lamia's level is than her opponent's. Will eat nearby medium or small sized allies if she cannot attack. Massive boost to vore rate when eating allies.", "Trait", Skill.Type.VHitMod, Skill.Visibility.Visible, delegate
		{
			if (StaticRef.CurrentUnit().nutLevel > StaticRef.TargetUnit().nutLevel)
			{
				StaticRef.vHit += (StaticRef.CurrentUnit().nutLevel - StaticRef.TargetUnit().nutLevel) * 5;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1007, "LamiaVoreBoost", "LamiaVoreBoost", "", "Trait", Skill.Type.VHitMod, Skill.Visibility.Invisible, delegate
		{
			if (StaticRef.CurrentUnit().team == StaticRef.TargetUnit().team)
			{
				StaticRef.vHit += 100f;
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1010, "Endobeautician", "CutieJunkie", "Gains +10% to a random stat after digesting a unit. May eat nearby medium or small sized allies if she cannot attack. Massive boost to vore rate when eating allies.", "Trait", Skill.Type.OnDigest, Skill.Visibility.Visible, delegate
		{
			foreach (UnitInfo item3 in current.endo)
			{

					switch (Rndm.Rand(1, 6))
					{
					case 1:
						current.atk = (int)(current.atk * 1.1f);
						break;
					case 2:
						current.def = (int)(current.def * 1.1f);
						break;
					case 3:
						current.vatk = (int)(current.vatk * 1.1f);
						break;
					case 4:
						current.vres = (int)(current.vres * 1.1f);
						break;
					case 5:
						current.skl = (int)(current.skl * 1.1f);
						break;
					case 6:
						current.spd = (int)(current.spd * 1.1f);
						break;
					}
				
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1011, "You go, girl!", "YouGoGirl", "At the end of Lamia's turn, she motivates all allies within 4 tiles, granting them +10% to all stats for 1 turn.", "Trait", Skill.Type.TurnEnd, Skill.Visibility.Visible, delegate
		{
			TileMap.GetTile(current).Pulse(4, 0);
			foreach (Tile selectableTile in TileMap.SelectableTiles)
			{
				UnitInfo unitInfo = TileMap.GetUnitInfo(selectableTile);
				if (unitInfo != null && unitInfo.team == current.team && unitInfo != current)
				{
					Status status = unitInfo.statuses.Find((Status x) => x.refID == 1011);
					if (status != null)
					{
						unitInfo.statuses.Remove(status);					

					unitInfo.AddStatus(StatusList.AddStatus(1011, 1));
					}
					else
					{
					unitInfo.dAtk += (int)(unitInfo.atk * 0.1f);
					unitInfo.dDef += (int)(unitInfo.def * 0.1f);
					unitInfo.dVatk += (int)(unitInfo.vatk * 0.1f);
					unitInfo.dVres += (int)(unitInfo.vres * 0.1f);
					unitInfo.dSkl += (int)(unitInfo.skl * 0.1f);
					unitInfo.dSpd += (int)(unitInfo.spd * 0.1f);
					unitInfo.AddStatus(StatusList.AddStatus(1011, 1));
					}
				}
			}
			current.Pause(x: false);
		}));
		skills.Add(new Skill(1012, "Seasoning", "Seasoning", "Cornucopia's unique absorption abilities gives her more control over her digestive system. Digestion ticks do not lower the digestion period unless done manually or unit is hungry. Instantly digests units while hungry, overburdened, or incapacitated.", "Trait", Skill.Type.TurnStart, Skill.Visibility.Visible, delegate
		{
				if (current.en > 0f && current.skills.Contains(100) && !(current.skills.Contains(104) && !(current.skills.Contains(99) )))
				{
					current.containPeriod += 1f;
					current.en -= 20f;
							if (current.en < 0f)
							{
								current.en = 0f;
							}
				}
				
				
				
				foreach (UnitInfo containingUnit3 in current.containingUnits)
				{
					containingUnit3.nutValue += 25;
				}
				
				if (current.en <= 0f)
				{
					current.containPeriod = 1f;
				}
				
				
				if (current.skills.Contains(104))
				{
					current.containPeriod = 1f;
				}
				
				if (current.skills.Contains(99))
				{
					current.containPeriod = 1f;
				}


			current.Pause(x: false);
		}));
	}

	public static Skill GetByID(int id)
	{
		return skills.Find((Skill x) => x.refID == id);
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
