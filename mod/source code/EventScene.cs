using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventScene : MonoBehaviour
{
	private UnitInfo current;

	private UnitInfo target;

	private Attack attack;

	private Skill skill;

	private TextWriter tw;

	private Queue<Action> processQ = new Queue<Action>();

	private Queue<Action> eventQ = new Queue<Action>();

	public bool wait;

	public bool inCoroutine;

	private Transform battleBackground;

	private Transform battleWindow;

	public Transform playerMask;

	public Transform enemyMask;

	public Transform center;

	private Sprite messageBase;

	private Sprite battleBase;

	private DelayAction delayAction;

	private bool hit;

	private bool inBattle;

	public int stomachSize;

	public int targetWeight;

	public int damage;

	public string currentSpeech;

	public AudioChanger option;

	private void Awake()
	{
		delayAction = base.gameObject.AddComponent<DelayAction>();
		tw = GetComponentInChildren<TextWriter>();
		playerMask = base.transform.Find("BattleImages").Find("Player");
		enemyMask = base.transform.Find("BattleImages").Find("Enemy");
		center = base.transform.Find("BattleImages").Find("Center");
		battleBackground = base.transform.Find("BattleBackground");
		battleWindow = base.transform.Find("BattleWindow");
		messageBase = Resources.Load<Sprite>("Images/MessageWindow");
		battleBase = Resources.Load<Sprite>("Images/BattleWindow");
	}

	private void Start()
	{
		float num = Screen.currentResolution.width / Screen.currentResolution.height;
		if (num == 1f)
		{
			num = Screen.width / Screen.height;
		}
		if (Mathf.Approximately(num, 2f))
		{
			playerMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1501f);
			enemyMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1501f);
			playerMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(-751.1f, 0f);
			enemyMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(751.1f, 0f);
		}
	}

	public void Update()
	{
		if (!StaticRef.isTurn() && !wait && !inCoroutine && eventQ.Count > 0)
		{
			wait = true;
			eventQ.Dequeue()();
			StartCoroutine(WaitForInput());
		}
		if (!StaticRef.isTurn() && Input.GetMouseButtonDown(1) && processQ.Count > 0)
		{
			processQ.Dequeue()();
			while (eventQ.Count > 1)
			{
				eventQ.Dequeue();
			}
			eventQ.Dequeue()();
		}
		else if (!StaticRef.isTurn() && StaticRef.turnController.gameState != TurnController.GameState.Spawn && Input.GetMouseButtonDown(1))
		{
			tw.skip = true;
		}
	}

	private IEnumerator WaitForInput()
	{
		yield return new WaitUntil(() => ((Input.GetKeyDown("z") || Input.GetMouseButtonDown(0)) && !tw.isPrinting) || tw.skip);
		wait = false;
	}

	public void Setup(UnitInfo a, UnitInfo t, Attack atk)
	{
		current = a;
		target = t;
		attack = atk;
	}

	public void Setup(UnitInfo a, UnitInfo t, Skill s)
	{
		current = a;
		target = t;
		skill = s;
	}

	public void NewUnit(UnitInfo u, Transform mask)
	{
		Image component = mask.Find("Container").Find("Images").Find("Main")
			.GetComponent<Image>();
		Image component2 = mask.Find("Container").Find("Images").Find("2nd")
			.GetComponent<Image>();
		component.sprite = SpriteLoad.Load(u);
		RectTransform component3 = component.GetComponent<RectTransform>();
		component3.pivot = StaticRef.SpritePivotBattle(u.classID);
		component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1200f + StaticRef.SpriteSizeBattle(u.classID));
		component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1200f + StaticRef.SpriteSizeBattle(u.classID));
		component3.localPosition = new Vector3(130f, 420f, 0f);
		if (u.clas.hCap > 0)
		{
			component2.sprite = SpriteLoad.LoadSecond(u);
			component3 = component2.GetComponent<RectTransform>();
			component3.pivot = StaticRef.SpritePivotBattle(u.classID);
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1200f + StaticRef.SpriteSizeBattle(u.classID));
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1200f + StaticRef.SpriteSizeBattle(u.classID));
			component3.localPosition = new Vector3(130f, 420f, 0f);
			component2.color = Color.white;
		}
		else
		{
			component2.color = new Color(1f, 1f, 1f, 0f);
		}
		Transform obj = mask.Find("Container");
		obj.GetComponent<ExpressLib>().LoadExpressions(u);
		obj.GetComponent<ExpressLib>().SetRect(component3, 130f, 420f);
		obj.GetComponent<ExpressLib>().SetExpression("Normal");
		component.color = Color.white;
		tw.skip = false;
	}

	public void RunAttack(int id)
	{
		switch (id)
		{
		case -1001:
			StartCoroutine(GenericFeed(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case -11:
			StartCoroutine(GenericBattle(() => StaticRef.CalcVHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case -10:
			if (current.TotalSize() <= 2 && target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
			{
				current.speechLib.route = 1;
				StartCoroutine(GenericFeed(() => 100f, delegate
				{
					StandardBattleEnding();
				}));
				break;
			}
			StaticRef.Battle();
			Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				FadeInMessageWindow();
				ShowBattlers("Attack", "Normal");
				hit = true;
				current.speechLib.Set(current, target, attack);
				target.speechLib.Set(target, current, attack);
				Delay(0.5f, delegate
				{
					ProcessSpeech("BattleStart", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "Attack");
						playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						wait = false;
					});
					ProcessText("AttackDesc", current);
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						ChangeExpression(current, "Kiss");
						ChangeExpression(target, "Kiss");
						wait = false;
					});
					ProcessText("SkillActivation", current);
					List<UnitInfo> list2 = new List<UnitInfo>();
					list2.AddRange(current.containingUnits);
					current.secondStomach.Reverse();
					list2.AddRange(current.secondStomach);
					current.secondStomach.Reverse();
					foreach (UnitInfo u4 in list2)
					{
						if (u4.containedByUnit == current)
						{
							eventQ.Enqueue(delegate
							{
								if (target.MaxStomach() + 1 - target.TotalStomach() >= u4.TotalSize())
								{
									playerMask.Find("Container").GetComponent<Animation>().Play("Special");
									enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
									stomachSize = current.TotalStomach() - u4.TotalSize();
									CalcWeight(current);
									current.ProcessUncarry(u4);
									u4.ProcessUncarried();
									UpdateSprite(current, playerMask);
									stomachSize = target.TotalStomach() + u4.TotalSize();
									target.ProcessVContainment(u4);
									UpdateSprite(target, enemyMask);
									tw.LoadText("<SEse_chupa4><SEse_gulp1><SEeaten_by_monster2>" + u4.unitName + " slides from " + current.unitName + "'s stomach into " + target.unitName + "'s!");
								}
								else
								{
									wait = false;
								}
							});
						}
					}
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						wait = false;
					});
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
							tw.LoadText("<SESwing2>" + current.unitName + " kisses " + target.unitName + " once again...");
						}
						else
						{
							wait = false;
						}
					});
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							target.ProcessVContainment(current);
							stomachSize = target.TotalStomach() + current.TotalSize();
							CalcWeight(target);
							UpdateSprite(target, enemyMask);
							KillSprite(current, playerMask);
							StaticRef.turnController.moveComplete = true;
							tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>...then proceeds to force herself into her open mouth, sliding straight down her throat and into her stomach!");
						}
						else
						{
							wait = false;
						}
					});
					ProcessSpeech("Kiss", target);
					eventQ.Enqueue(delegate
					{
						TileMap.GetTile(target).iOccupied = target.team;
						StartCoroutine(HideBattle(delegate
						{
							StandardBattleEnding();
						}));
					});
				});
			});
			break;
		case 0:
			if (current.team != target.team)
			{
				StartCoroutine(GenericBattle(() => StaticRef.CalcVHit(), delegate
				{
					StandardBattleEnding();
				}));
			}
			else
			{
				StartCoroutine(GenericBattle(() => 100f, delegate
				{
					StandardBattleEnding();
				}));
			}
			break;
		case 2:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 3:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 4:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 5:
		{
			float num = 50f;
			float num2 = target.hp - current.hp;
			if (num2 > 0f)
			{
				num -= num2 / 5f;
			}
			if (target.TotalSize() > 2 || current.TotalStomach() == current.MaxStomach())
			{
				num = 0f;
			}
			MonoBehaviour.print("HITRATE " + num);
			if (Rndm.Chance(num))
			{
				current.speechLib.route = 0;
				StartCoroutine(GenericBattle(() => 100f, delegate
				{
					StandardBattleEnding();
				}));
				break;
			}
			if (current.TotalSize() <= 2 && target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
			{
				current.speechLib.route = 1;
				StaticRef.Battle();
				Delay(0.5f, delegate
				{
					UseBattleWindow();
					ShowWindow();
					FadeInMessageWindow();
					ShowBattlers("Attack", "Normal");
					hit = true;
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(target, current, attack);
					Delay(0.5f, delegate
					{
						ProcessSpeech("BattleStart", current);
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Attack");
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
							wait = false;
						});
						ProcessText("AttackDesc", current);
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							stomachSize = target.TotalStomach() + current.TotalSize();
							CalcWeight(target);
							target.ProcessVContainment(current);
							UpdateSprite(target, enemyMask);
							KillSprite(current, playerMask);
							if (target.isSafeContainer())
							{
								target.EndSafeDigestion();
							}
							MonoBehaviour.print(target.GetPredState().ToString() + " Pred State");
							ChangeExpression(target, "Normal");
							StaticRef.turnController.moveComplete = true;
							wait = false;
						});
						ProcessText("AttackHitAttacker", current);
						ProcessSpeech("AttackHitAttackerS", current);
						eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
					});
				});
				break;
			}
			StaticRef.Battle();
			Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				FadeInMessageWindow();
				ShowBattlers("Attack", "Normal");
				hit = true;
				current.speechLib.Set(current, target, attack);
				target.speechLib.Set(target, current, attack);
				Delay(0.5f, delegate
				{
					ProcessSpeech("BattleStart", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "Attack");
						playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						wait = false;
					});
					ProcessText("AttackDesc", current);
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						ChangeExpression(current, "Kiss");
						ChangeExpression(target, "Kiss");
						wait = false;
					});
					ProcessText("SkillActivation", current);
					List<UnitInfo> list = new List<UnitInfo>();
					list.AddRange(current.containingUnits);
					current.secondStomach.Reverse();
					list.AddRange(current.secondStomach);
					current.secondStomach.Reverse();
					foreach (UnitInfo u3 in list)
					{
						if (u3.containedByUnit == current)
						{
							eventQ.Enqueue(delegate
							{
								if (target.MaxStomach() + 1 - target.TotalStomach() >= u3.TotalSize())
								{
									playerMask.Find("Container").GetComponent<Animation>().Play("Special");
									enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
									stomachSize = current.TotalStomach() - u3.TotalSize();
									CalcWeight(current);
									current.ProcessUncarry(u3);
									u3.ProcessUncarried();
									UpdateSprite(current, playerMask);
									stomachSize = target.TotalStomach() + u3.TotalSize();
									target.ProcessVContainment(u3);
									UpdateSprite(target, enemyMask);
									tw.LoadText("<SEse_chupa4><SEse_gulp1><SEeaten_by_monster2>" + u3.unitName + " slides from " + current.unitName + "'s stomach into " + target.unitName + "'s!");
								}
								else
								{
									wait = false;
								}
							});
						}
					}
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						wait = false;
					});
					eventQ.Enqueue(delegate
					{
						tw.LoadSpeech("<BGGrowl><NF><NEPale><NEMouth\\Angry><NEEyes\\XD><NEBrows\\Down><NEGlasses>Urrruup! Haa... Haa... Bwuh... H-huh?`W-what just happened...", current);
					});
					if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadSpeech("<NF><NEPale><NEMouth\\Angry><NEEyes\\Narrow><NEBrows\\Angry><NEGlasses>N-nevermind...!`I've... *Huff* I-I've got you, now...!", current);
						});
					}
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
							tw.LoadText("<SESwing2>" + current.unitName + " makes one more attempt to eat " + target.unitName + "...");
						}
						else
						{
							wait = false;
						}
					});
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							target.ProcessVContainment(current);
							stomachSize = target.TotalStomach() + current.TotalSize();
							CalcWeight(target);
							UpdateSprite(target, enemyMask);
							KillSprite(current, playerMask);
							StaticRef.turnController.moveComplete = true;
							tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>...but slips and falls into her open mouth, sliding straight down her throat and into her stomach!");
						}
						else
						{
							wait = false;
						}
					});
					ProcessSpeech("Kiss", target);
					eventQ.Enqueue(delegate
					{
						TileMap.GetTile(target).iOccupied = target.team;
						StartCoroutine(HideBattle(delegate
						{
							StandardBattleEnding();
						}));
					});
				});
			});
			break;
		}
		case 6:
		{
			int x = Rndm.Rand(0, 14);
			StaticRef.Battle();
			Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				FadeInMessageWindow();
				ShowBattlers("Attack", "Normal");
				hit = true;
				current.speechLib.Set(current, target, attack);
				target.speechLib.Set(target, current, attack);
				Delay(0.5f, delegate
				{
					ProcessSpeech("BattleStart", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "Attack");
						playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						wait = false;
					});
					ProcessText("AttackDesc", current);
					if (x == 0)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was an empty vial! Nothing happens...");
						});
					}
					else if (x == 1)
					{
						eventQ.Enqueue(delegate
						{
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							target.CalculateDamage(-50);
							StaticRef.enemy.UpdateHealth();
							tw.LoadText("<SEglass_break>It was a health potion! 50 HP Recovered!");
						});
					}
					else if (x == 2)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							target.en = target.Men();
							StaticRef.enemy.UpdateEnergy();
							tw.LoadText("<SEglass_break>It was a vial of sticky white liquid! She recovers all her energy!");
						});
					}
					else if (x == 3)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							target.CalculateDamage(50);
							StaticRef.enemy.UpdateHealth();
							tw.LoadText("<SEglass_break>It was a vial of poison! 50 Damage!");
						});
					}
					else if (x == 4)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							target.en -= 50f;
							if (target.en < 0f)
							{
								target.en = 0f;
							}
							StaticRef.enemy.UpdateEnergy();
							tw.LoadText("<SEglass_break>It was a jar of Cornucopia's special seasoning! She loses 50 energy!");
						});
					}
					else if (x == 5)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break><SEStatus3>It was an evil sealing urn! A powerful vortex appears around her, sucking in all nearby units and sending them stumbling towards her!");
							foreach (Tile item2 in TileMap.GetTile(target).CheckAdjacent())
							{
								EventScene eventScene3 = this;
								UnitInfo u5 = TileMap.GetUnitInfo(item2);
								UnitInfo temp6 = current;
								StaticRef.QueueEvent(delegate
								{
									if (u5 != null && u5.isVorable(eventScene3.target) && Rndm.Chance(50f))
									{
										temp6.Pause(x: true);
										u5.Pause(x: true);
										StaticRef.turnController.gameState = TurnController.GameState.Transition;
										StaticRef.Message();
										u5.speechLib.Set(u5, eventScene3.target, SkillList.GetByID(0));
										eventScene3.target.speechLib.Set(eventScene3.target, u5, SkillList.GetByID(0));
										eventScene3.Setup(u5, eventScene3.target, SkillList.GetByID(0));
										eventScene3.RunSkill(0);
										eventScene3.Delay(() => !u5.Pause(), delegate
										{
											StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
											temp6.Pause(x: false);
										});
									}
									else
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp6.Pause(x: false);
									}
								});
							}
						});
					}
					else if (x == 6)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							if (target.containingUnits.Count > 0)
							{
								EventScene eventScene2 = this;
								tw.LoadText("<SEglass_break><SEStatus2>It was a jar of foul-smelling liquid! She becomes nauseous!");
								UnitInfo temp4 = current;
								UnitInfo temp5 = target;
								StaticRef.QueueEvent(delegate
								{
									temp4.Pause(x: true);
									temp5.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									StaticRef.Message();
									eventScene2.target.speechLib.Set(eventScene2.target, SkillList.GetByID(-501));
									eventScene2.Setup(eventScene2.target, eventScene2.current, SkillList.GetByID(-501));
									eventScene2.RunSkill(-501);
									eventScene2.Delay(() => !temp5.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp4.Pause(x: false);
									});
								});
							}
							else
							{
								tw.LoadText("<SEglass_break><SEStatus2>It was a jar of foul-smelling liquid! Nothing happens...");
							}
						});
					}
					else if (x == 7)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							target.AddStatus(StatusList.AddStatus(0, 2));
							tw.LoadText("<SEglass_break>It was a flashbang! The grenade explodes with blinding light, dizzying her!");
						});
					}
					else if (x == 8)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							if (target.containingUnits.Count > 0)
							{
								EventScene eventScene = this;
								target.containPeriod = 1f;
								tw.LoadText("<SEglass_break><SRheavyGrowl><SRemptyGrowl>It was a bottle of super enzymes! Her belly starts to rumble dangerously...");
								UnitInfo temp2 = current;
								UnitInfo temp3 = target;
								StaticRef.QueueEvent(delegate
								{
									temp2.Pause(x: true);
									temp3.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									eventScene.target.ForceDigestion();
									eventScene.Delay(() => !temp3.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp2.Pause(x: false);
									});
								});
							}
							else
							{
								tw.LoadText("<SEglass_break>It was a bottle of super enzymes! They have no effect whatsoever on her...");
							}
						});
					}
				else if (x == 9)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");							
							tw.LoadText("<SEglass_break>It was a nerve gas grenade! The grenade inflicts paralysis, reducing her stats by 20% and halving movement range!");
							target.dAtk -= (int)(target.atk * 0.2f);
							target.dDef -= (int)(target.def * 0.2f);
							target.dVatk -= (int)(target.vatk * 0.2f);
							target.dVres -= (int)(target.vres * 0.2f);
							target.dSkl -= (int)(target.skl * 0.2f);
							target.dSpd -= (int)(target.spd * 0.2f);
							target.dMove -= target.movement / 2;
							target.AddStatus(StatusList.AddStatus(143, 1));
						});
					}
				else if (x == 10)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of experimental polyphagia syrup! She is inflicted with extreme hunger, losing all energy!");
							if (target.en > 0f)
							{
							target.en -= target.en;
							}
							if (target.en < 0f)
							{
								target.en = 0f;
							}
							StaticRef.enemy.UpdateEnergy();
						});
					}
				else if (x == 11)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of slippery lubricant! She is covered with the lubricant, gaining a massive boost to VRES!");
							target.dVres += 1000f;
							target.AddStatus(StatusList.AddStatus(145, 1));
						});
					}
				else if (x == 12)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a cup of coffee! She gains 50 energy!");
							target.en += 50f;
							if (target.en > target.Men())
							{
								target.en = target.Men();
							}
							StaticRef.enemy.UpdateEnergy();
						});
					}
				else if (x == 13)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of nutritional supplement! She gains 50 nutritional value!");
							target.nutValue += 50;
						});
					}
				else if (x == 14)
					{
						eventQ.Enqueue(delegate
						{
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							if (target.containingUnits.Count > 0)
							{
							ChangeExpression(target, "VFail");
							EventScene eventScene = this;
							tw.LoadText("<SEglass_break>It was a bottle of liquid melatonin! She is inflicted with drowsiness!");
							target.skills.Add(200);
							target.skills.Add(99);
							target.containPeriod += 2f;
							if (target.skills.Contains(200))
							{
							UnitInfo temp2 = current;
								UnitInfo temp3 = target;
							StaticRef.QueueEvent(delegate
								{
									temp2.Pause(x: true);
									temp3.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									eventScene.target.ForceDigestion();
									eventScene.Delay(() => !temp3.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp2.Pause(x: false);
									});
								});
							}
							}
							else
							{
								ChangeExpression(target, "Damage");
								tw.LoadText("<SEglass_break>It was a bottle of liquid melatonin! They have no effect whatsoever on her...");
							}
						});
					}
					eventQ.Enqueue(delegate
					{
						StartCoroutine(HideBattle(delegate
						{
							StandardBattleEnding();
						}));
					});
				});
			});
			break;
		}
		case 7:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 8:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 10:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				if (target.hp <= 0f && current.TotalStomach() <= 1 && target.isVorable() && (Rndm.Chance(25f) || !target.isContainingAlly(current)))
				{
					Setup(current, target, AttackList.GetByID(11));
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(current, target, attack);
					wait = false;
					ProcessSpeech("BattleStart", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "Attack");
						playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						wait = false;
					});
					ProcessText("AttackDesc", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "VSuccess");
						ChangeExpression(target, "VEaten");
						playerMask.Find("Container").GetComponent<Animation>().Play("Click");
						enemyMask.Find("Container").GetComponent<Animation>().Play("Die");
						stomachSize = current.TotalStomach() + target.TotalSize();
						CalcWeight(current);
						UpdateSprite(current, playerMask);
						KillSprite(target, enemyMask);
						current.ProcessVContainment(target);
						MonoBehaviour.print(current.GetPredState().ToString() + " Pred State");
						target.skills.Add(99);
						wait = false;
					});
					ProcessText("AttackHitAttacker", current);
					ProcessText("AttackHitTarget", target);
					ProcessSpeech("AttackHitTargetS", target);
					ProcessSpeech("AttackHitAttackerS", current);
					eventQ.Enqueue(delegate
					{
						StandardBattleEnding();
					});
				}
				else if (target.hp <= 0f && target.containingUnits.Count > 0 && current.TotalStomach() <= 1)
				{
					UnitInfo x2 = current;
					UnitInfo y = target;
					StartCoroutine(HideBattle(delegate
					{
						target.ActionEnd();
						StaticRef.QueueEvent(delegate
						{
							if (y.hp <= 0f)
							{
								y.skills.Add(99);
								Setup(x2, y, AttackList.GetByID(11));
								StaticRef.turnController.ProcessAttack(inBattle: true);
								RunAttack(11);
							}
							else
							{
								StaticRef.Turn();
							}
						});
					}));
				}
				else
				{
					StandardBattleEnding();
				}
			}));
			break;
		case 11:
			StartCoroutine(GenericBattle(() => 100f, delegate
			{
				StandardBattleEnding();
			}));
			break;
		case 15:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), delegate
			{
				if (target.hp <= 0f && current.TotalStomach() + target.TotalSize() <= current.clas.mCap + 1 && target.isVorable())
				{
					List<UnitInfo> list6 = current.CombinedStomach();
					Rndm.Shuffle(list6);
					foreach (UnitInfo u6 in list6)
					{
						if (u6.containedByUnit == current && target.isVorable(u6))
						{
							Setup(current, target, AttackList.GetByID(11));
							current.speechLib.Set(current, target, attack);
							target.speechLib.Set(current, target, attack);
							wait = false;
							eventQ.Enqueue(delegate
							{
								tw.LoadSpeech("<NF><NEBlush><NEMouth\\Grin><NEEyes\\Normal><NEBrows\\Normal>What a delicious looking girl... Are you ready in there? Please be prepared to swallow her as she enters my stomach!", current);
							});
							eventQ.Enqueue(delegate
							{
								ChangeExpression(current, "Attack");
								playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
								wait = false;
							});
							eventQ.Enqueue(delegate
							{
								tw.LoadText("<SESESwing2>" + current.unitName + " scoops up her prey, using her axe like a spatula...");
							});
							eventQ.Enqueue(delegate
							{
								ChangeExpression(current, "VSuccess");
								ChangeExpression(target, "VEaten");
								playerMask.Find("Container").GetComponent<Animation>().Play("Click");
								enemyMask.Find("Container").GetComponent<Animation>().Play("Die");
								stomachSize = current.TotalStomach() + target.TotalSize();
								CalcWeight(current);
								UpdateSprite(current, playerMask);
								KillSprite(target, enemyMask);
								u6.ProcessVContainment(target);
								current.ProcessUncarry(u6);
								current.ProcessCarry(u6);
								if (target.team != current.team && u6.team == target.team)
							{
								current.EndSafeDigestion();
							}
								wait = false;
							});
							eventQ.Enqueue(delegate
							{
								tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>...and lets them slide down into her open mouth!");
							});
							eventQ.Enqueue(delegate
							{
								playerMask.Find("Container").GetComponent<Animation>().Play("Click");
								wait = false;
							});
							eventQ.Enqueue(delegate
							{
								tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>As they slip down her throat, they meet with " + u6.unitName + "'s maw headfirst, getting further slurped down into " + u6.unitName + "'s belly!");
							});
							eventQ.Enqueue(delegate
							{
								tw.LoadSpeech("<SRheavyGrowl><SRemptyGrowl><NF><NEBlush><NEMouth\\Grin><NEEyes\\Closed><NEBrows\\Normal>Heeheehee, those are the sounds of a happy customer~`Please, relax and enjoy your stay!", current);
							});

							eventQ.Enqueue(delegate
							{
								StandardBattleEnding();
							});
							return;
						}
					}
				}
				StandardBattleEnding();
			}));
			break;
		case 16:
			UseMessageWindow();
			ShowWindow();
			FadeInMessageWindow();
			NewUnit(current, center);
			center.GetComponent<UIFade>().FadeIn();
			if (current.GetPredState() != UnitInfo.PredState.V_Container)
			{
				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("<NF><NEMouth\\Grin><NEEyes\\Normal><NEBrows\\Angry>Orrrraaaa!! Everyone, with me!", current);
				});
				foreach (Tile item3 in TileMap.GetTile(current).CheckAdjacent())
				{
					UnitInfo u2 = TileMap.GetUnitInfo(item3);
					if (u2 != null && u2.team == current.team)
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadText("<SEpowerup1>" + u2.unitName + "'s strength has been bolstered by " + current.unitName + "'s cry!");
						});
						u2.dAtk += u2.atk * 0.1f;
						u2.dVatk += u2.vatk * 0.1f;
						u2.AddStatus(StatusList.GetByID(16));
					}
				}
			}
			else
			{
				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("<NF><NEMouth\\Drool><NEEyes\\Narrow><NEBrows\\Down>Orrr.... Mmmph...", current);
				});
				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("<SEse_longburp1><NF><NEBlush><NELines><NEMouth\\Burp><NEEyes\\Ahegao><NEBrows\\Down>Eeoooooouuuuuuuuurrrrrrrrpppp!!!!", current);
				});
				foreach (Tile item4 in TileMap.GetTile(current).CheckAdjacent())
				{
					UnitInfo u = TileMap.GetUnitInfo(item4);
					if (u != null)
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadText(u.unitName + "'s defenses have been shaken by " + current.unitName + "'s earth-shattering belch!");
						});
						u.dDef -= u.def * 0.2f;
						u.dVres -= u.vres * 0.2f;
						u.AddStatus(StatusList.GetByID(17));
					}
				}
			}
			EventQ().Enqueue(delegate
			{
				center.GetComponent<UIFade>().FadeOut();
				StandardBattleEnding();
			});
			break;
		case 20:
			StartCoroutine(GenericBattle(() => 100f, delegate
			{
				StartCoroutine(HideBattle(delegate
				{
					current.ForceHeal();
				}));
			}));
			break;
		case 21:
			StartCoroutine(GenericBattle(() => 100f, delegate
			{
				current.containPeriod = 1f;
				StartCoroutine(HideBattle(delegate
				{
					current.ForceDigestion();
				}));
			}));
			break;
			case 141:
			UseMessageWindow();
			ShowWindow();
			FadeInMessageWindow();
			NewUnit(current, center);
			center.GetComponent<UIFade>().FadeIn();

				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("Lord, restore the vigor of these valiant warriors.", current);
				});
				foreach (Tile item3 in TileMap.GetTile(current).CheckAdjacent())
				{
					UnitInfo u2 = TileMap.GetUnitInfo(item3);
					if (u2 != null && u2.team == current.team)
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadText("<SEpowerup1>" + u2.unitName + " was enveloped by " + current.unitName + "'s rays, restoring 50 energy!");
						});
						u2.en += 50f;
							if (u2.en > u2.Men())
							{
								u2.en = u2.Men();
							}
					}
				}

			EventQ().Enqueue(delegate
			{
				center.GetComponent<UIFade>().FadeOut();
				StandardBattleEnding();
			});
			break;
			
					case 142:
					
					StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), ()=>{if(hit) {
						
						target.dAtk -= (int)(target.atk * 0.2f); 
					target.dVatk -= (int)(target.vatk * 0.2f); 
					target.dSkl -= (int)(target.skl * 0.2f);
					target.dSpd -= (int)(target.spd * 0.2f);
					target.AddStatus(StatusList.AddStatus(142, 1));
					StandardBattleEnding();}
					else
					{
					eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
					}
					
					
					
					}));
			break;
			
		case 1000:
			StartCoroutine(NonstandardBattle(() => StaticRef.CalcHit(), delegate
			{
				eventQ.Enqueue(delegate
				{
					target.skills.Add(200);
					target.skills.Add(99);
					target.containPeriod += 2f;
					ChangeExpression(target, "Normal");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
					wait = false;
				});
			}, delegate
			{
				eventQ.Enqueue(delegate
				{
					ChangeExpression(current, "VFail");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Recoil");
					wait = false;
				});
			}, delegate
			{
				StartCoroutine(HideBattle(delegate
				{
					if (target.skills.Contains(200))
					{
						target.ForceDigestion();
					}
					else
					{
						StaticRef.Turn();
					}
				}));
			}));
			break;
		case 1002:
			StartCoroutine(NonstandardBattle(() => 100f, delegate
			{
				eventQ.Enqueue(delegate
				{
					enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
					wait = false;
				});
			}, delegate
			{
				eventQ.Enqueue(delegate
				{
					ChangeExpression(current, "VFail");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Recoil");
					wait = false;
				});
			}, delegate
			{
				StartCoroutine(HideBattle(delegate
				{
					MonoBehaviour.print("Finding Best target IN RANGE...");
					Tile tile = null;
					List<Tile> list3 = new List<Tile>();
					List<AttackRating> list4 = new List<AttackRating>();
					AttackRating attackRating = null;
					Attack byID = AttackList.GetByID(-11);
					Token component = target.GetComponent<Token>();
					TileMap.GetTile(target.transform.position).PulseGround(target, byID.minRange, byID.maxRange);
					foreach (Tile attackableTile in TileMap.AttackableTiles)
					{
						if (attackableTile.iOccupied == target.team)
						{
							UnitInfo unitInfo = TileMap.GetUnitInfo(attackableTile);
							if (unitInfo != null && byID.restrict(unitInfo))
							{
								list4.Add(new AttackRating(target.CalculateBattleRating(unitInfo, byID), unitInfo, byID));
							}
						}
					}
					foreach (AttackRating item5 in list4)
					{
						if (attackRating == null || item5.score > attackRating.score)
						{
							foreach (Tile item6 in TileMap.GetTile(item5.target).CheckInRange(item5.attack.minRange, item5.attack.maxRange))
							{
								if (TileMap.SelectableTiles.Contains(item6))
								{
									attackRating = item5;
								}
							}
						}
					}
					if (attackRating == null)
					{
						StaticRef.Turn();
					}
					else
					{
						TileMap.GetTile(target.transform.position).PulseGround(component.Unit(), 1, 1);
						list3.AddRange(TileMap.GetTile(attackRating.target).CheckInRange(1, 1));
						foreach (Tile item7 in list3)
						{
							if ((tile == null || item7.iDistance < tile.iDistance) && item7.bSelectable)
							{
								tile = item7;
							}
						}
						if (tile == null)
						{
							StaticRef.Turn();
						}
						else
						{
							MonoBehaviour.print("Target tile is " + tile);
							component.SetupAttack(TileMap.GetTile(attackRating.target), attackRating.attack);
							UnitInfo temp = current;
							bool move = StaticRef.turnController.moveComplete;
							bool isPlayer = target.isPlayer;
							target.isPlayer = false;
							component.state = Token.State.Attack;
							StaticRef.turnController.actionComplete = false;
							StaticRef.turnController.moveComplete = false;
							StaticRef.turnController.current = target;
							StaticRef.status.NewCharacter(target);
							component.PF().FindPath(tile, component.Unit().Movement(), component.Unit().team);
							StaticRef.QueueEvent(delegate
							{
								StaticRef.CurrentUnit().isPlayer = isPlayer;
								StaticRef.turnController.moveComplete = move;
								StaticRef.turnController.current = temp;
								StaticRef.status.NewCharacter(temp);
								StaticRef.sound.PlayStomachBGS();
								StaticRef.Turn();
							});
						}
					}
				}));
				MonoBehaviour.print("xxxxxxxxx");
			}));
			break;
		case 1003:
			StartCoroutine(NonstandardBattle(() => 100f, delegate
			{
				eventQ.Enqueue(delegate
				{
					enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
					wait = false;
				});
			}, delegate
			{
				eventQ.Enqueue(delegate
				{
					ChangeExpression(current, "VFail");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Recoil");
					wait = false;
				});
			}, delegate
			{
				StartCoroutine(HideBattle(delegate
				{
					MonoBehaviour.print("Finding Best target IN RANGE...");
					Tile tile4 = null;
					List<Tile> list5 = new List<Tile>();
					new List<AttackRating>();
					Attack byID2 = AttackList.GetByID(1004);
					Token component2 = target.GetComponent<Token>();
					target.dMove = 3f;
					TileMap.GetTile(target.transform.position).PulseGround(target, byID2.minRange, byID2.maxRange);
					TileMap.GetTile(target.transform.position).PulseGround(component2.Unit(), 1, 1);
					list5.AddRange(TileMap.GetTile(current).CheckInRange(1, 1));
					foreach (Tile item8 in list5)
					{
						if ((tile4 == null || item8.iDistance < tile4.iDistance) && item8.bSelectable)
						{
							tile4 = item8;
						}
					}
					if (tile4 == null)
					{
						StaticRef.Turn();
					}
					else
					{
						MonoBehaviour.print("Target tile is " + tile4);
						component2.SetupAttack(TileMap.GetTile(current), byID2);
						if (target != TileMap.GetTile(target.transform.position))
						{
							UnitInfo temp7 = current;
							bool move2 = StaticRef.turnController.moveComplete;
							bool isPlayer2 = target.isPlayer;
							target.isPlayer = false;
							component2.state = Token.State.Attack;
							StaticRef.turnController.actionComplete = false;
							StaticRef.turnController.moveComplete = false;
							StaticRef.turnController.current = target;
							StaticRef.status.NewCharacter(target);
							component2.PF().FindPath(tile4, component2.Unit().Movement(), component2.Unit().team);
							StaticRef.QueueEvent(delegate
							{
								StaticRef.CurrentUnit().isPlayer = isPlayer2;
								StaticRef.turnController.moveComplete = move2;
								StaticRef.turnController.current = temp7;
								StaticRef.status.NewCharacter(temp7);
								StaticRef.sound.PlayStomachBGS();
								StaticRef.Turn();
							});
						}
						else
						{
							component2.state = Token.State.Attack;
						}
					}
				}));
				MonoBehaviour.print("xxxxxxxxx");
			}));
			break;
		case 1004:
			if (current.TotalSize() <= 2 && target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
			{
				StaticRef.Battle();
				Delay(0.5f, delegate
				{
					UseBattleWindow();
					ShowWindow();
					FadeInMessageWindow();
					ShowBattlers("Normal", "Normal");
					hit = true;
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(target, current, attack);
					Delay(0.5f, delegate
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadText(current.unitName + " approaches the " + target.unitName + " in a daze...");
						});
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Kiss");
							ChangeExpression(target, "Kiss");
							playerMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEse_chupa1>" + target.unitName + " wraps her in an embrace and begins softly kissing her, forcing her deeper and deeper inside...");
						});
						eventQ.Enqueue(delegate
						{
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							stomachSize = target.TotalStomach() + current.TotalSize();
							CalcWeight(target);
							target.ProcessVContainment(current);
							UpdateSprite(target, enemyMask);
							KillSprite(current, playerMask);
							if (target.isSafeContainer())
							{
								target.EndSafeDigestion();
							}
							MonoBehaviour.print(target.GetPredState().ToString() + " Pred State");
							ChangeExpression(target, "Normal");
							StaticRef.turnController.moveComplete = true;
							tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>...until her mouth completely engulfs her! Her belly distorts as her helpless victim slides down her throat and settles inside her.");
						});
						eventQ.Enqueue(delegate
						{
							tw.LoadSpeech("<NF><NEBlush><NEMouth\\Happy><NEEyes\\ClosedUp><NEBrows\\Normal>Ah, what a nice, obedient girl... It's too bad I'll have to digest her, now. ♥", target);
						});
						eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
					});
				});
				break;
			}
			StaticRef.Battle();
			Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				FadeInMessageWindow();
				ShowBattlers("Attack", "Normal");
				hit = true;
				current.speechLib.Set(current, target, attack);
				target.speechLib.Set(target, current, attack);
				Delay(0.5f, delegate
				{
					eventQ.Enqueue(delegate
					{
						tw.LoadText(current.unitName + " approaches the " + target.unitName + " in a daze...");
					});
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						ChangeExpression(current, "Kiss");
						ChangeExpression(target, "Kiss");
						tw.LoadText("<SEse_chupa1>" + target.unitName + " wraps her in an embrace and begins softly kissing her!");
					});
					List<UnitInfo> list7 = new List<UnitInfo>();
					list7.AddRange(current.containingUnits);
					current.secondStomach.Reverse();
					list7.AddRange(current.secondStomach);
					current.secondStomach.Reverse();
					foreach (UnitInfo u7 in list7)
					{
						if (u7.containedByUnit == current)
						{
							eventQ.Enqueue(delegate
							{
								if (target.MaxStomach() + 1 - target.TotalStomach() >= u7.TotalSize())
								{
									playerMask.Find("Container").GetComponent<Animation>().Play("Special");
									enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
									stomachSize = current.TotalStomach() - u7.TotalSize();
									CalcWeight(current);
									current.ProcessUncarry(u7);
									u7.ProcessUncarried();
									UpdateSprite(current, playerMask);
									stomachSize = target.TotalStomach() + u7.TotalSize();
									target.ProcessVContainment(u7);
									UpdateSprite(target, enemyMask);
									tw.LoadText("<SEse_chupa4><SEse_gulp1><SEeaten_by_monster2>" + u7.unitName + " slides from " + current.unitName + "'s stomach into " + target.unitName + "'s!");
								}
								else
								{
									wait = false;
								}
							});
						}
					}
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							tw.LoadSpeech("<NF><NEBlush><NEMouth\\Happy><NEEyes\\ClosedUp><NEBrows\\Normal>Thaaaaat's it... What a good girl you are... ♥\nNow... come inside, sweetheart. ♥", target);
						}
						else
						{
							wait = false;
						}
					});
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							ChangeExpression(current, "Kiss");
							playerMask.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEse_chupa1>" + target.unitName + " kisses her prey once again, forcing them deeper and deeper inside...");
						}
						else
						{
							wait = false;
						}
					});
					eventQ.Enqueue(delegate
					{
						if (target.MaxStomach() + 1 - target.TotalStomach() >= current.TotalSize())
						{
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							target.ProcessVContainment(current);
							stomachSize = target.TotalStomach() + current.TotalSize();
							CalcWeight(target);
							UpdateSprite(target, enemyMask);
							KillSprite(current, playerMask);
							StaticRef.turnController.moveComplete = true;
							tw.LoadText("<SEse_gulp1><SEeaten_by_monster2>...until her mouth completely engulfs them! Her belly distorts as her helpless victim slides down her throat and settles inside her.");
						}
						else
						{
							wait = false;
						}
					});
					eventQ.Enqueue(delegate
					{
						if (current.containedByUnit == target)
						{
							tw.LoadSpeech("<NF><NEBlush><NEMouth\\Happy><NEEyes\\ClosedUp><NEBrows\\Normal>Ah, what a nice, obedient girl... It's too bad I'll have to digest her, now. ♥", target);
						}
						else
						{
							tw.LoadSpeech("<NF><NEBlush><NEMouth\\Happy><NEEyes\\ClosedUp><NEBrows\\Normal>Ah, what a nice, obedient girl... I'll come back for you later, okay? ♥", target);
						}
					});
					eventQ.Enqueue(delegate
					{
						TileMap.GetTile(target).iOccupied = target.team;
						StartCoroutine(HideBattle(delegate
						{
							StandardBattleEnding();
						}));
					});
				});
			});
			break;
			case 1141:
			
			StaticRef.Battle();
				Delay(0.5f, delegate
				{
					UseBattleWindow();
					ShowWindow();
					FadeInMessageWindow();
					ShowBattlers("Normal", "Normal");
					hit = true;
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(target, current, attack);
					Delay(0.5f, delegate
					{
						eventQ.Enqueue(delegate
						{
							tw.LoadSpeech("Feeling hungry, little baby? Don't worry, mommy will take care of all your needs ♥", current);
						});
						eventQ.Enqueue(delegate
						{
							tw.LoadText(current.unitName + " suddenly pulls " + target.unitName + " near her chest...");
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						});
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Kiss");
							ChangeExpression(target, "Kiss");
							playerMask.Find("Container").GetComponent<Animation>().Play("Special");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							tw.LoadText("<SEse_chupa1>Instinctively, " + target.unitName + " begins sucking on " + current.unitName + "'s nipples, releasing milk!");
						});
						
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "VSuccess");
							ChangeExpression(current, "VSuccess");
							target.en = target.Men();
							StaticRef.enemy.UpdateEnergy();
							tw.LoadText("<SEpowerup1>" + target.unitName + " recovers all of her energy after drinking the milk!");
						});
						
						ProcessSpeech("Nurse", target);
						
						eventQ.Enqueue(delegate
						{
							tw.LoadSpeech("<NF><NEBlush><NEMouth\\Happy><NEEyes\\ClosedUp><NEBrows\\Normal>Hee hee, hope you enjoyed the meal~ ♥", current);
							
							
							
						});
						
						
						eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
						
				
				});
				
			});
			
			
			
			break;
	
case 1142:
			
			StaticRef.Battle();
				Delay(0.5f, delegate
				{
					UseBattleWindow();
					ShowWindow();
					FadeInMessageWindow();
					ShowBattlers("Normal", "Normal");
					hit = true;
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(target, current, attack);
					Delay(0.5f, delegate
					{
						
						ProcessSpeech("BattleStart", current);
						
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "VSuccess");
							tw.LoadText(current.unitName + " reaches for " + target.unitName + "'s engorged stomach...");
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						});
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Kiss");
							ChangeExpression(target, "VSuccess");
							playerMask.Find("Container").GetComponent<Animation>().Play("Special");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							tw.LoadText("<SRheavyGrowl><SRemptyGrowl>Chateau's massage lowers " + target.unitName + "'s digestion period by 1 turn!");
							target.containPeriod -= 1f;	
						});
						
						if (!(target.skills.Contains(99)))
						{
						ProcessSpeech("Massage", target);
						}
						
						if (target.skills.Contains(99))
						{
						ProcessSpeech("vTickDown", target);
						}
						
						if (target.containPeriod <= 1f)
						{
							EventScene eventScene = this;
							target.containPeriod = 1f;
								tw.LoadText("<SRheavyGrowl><SRemptyGrowl>Chateau's massage causes" + target.unitName + " to start digesting their meal!");
								UnitInfo temp2 = current;
								UnitInfo temp3 = target;
								StaticRef.QueueEvent(delegate
								{
									temp2.Pause(x: true);
									temp3.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									eventScene.target.ForceDigestion();
									eventScene.Delay(() => !temp3.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp2.Pause(x: false);
									});
						});
						
						}
						
						eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
						
				
				});
				
			});	
			
			
			
			break;
			
			case 1143:
			StartCoroutine(GenericBattle(() => StaticRef.CalcHit(), ()=>{if(hit) {
					
					target.dSkl -= (int)(target.skl * 0.2f);
					target.dSpd -= (int)(target.spd * 0.2f);
					target.dVres -= (int)(target.vres * 0.2f);
					target.dDef -= (int)(target.def * 0.2f);
					target.dMove -= target.movement / 2;
					target.AddStatus(StatusList.AddStatus(148, 2));
					StandardBattleEnding();}
					else
					{
					eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
					}
					
					
					
					}));		
					
			break;
			
			case 1144:
			
			StaticRef.Battle();
			Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				FadeInMessageWindow();
				ShowBattlers("Attack", "Normal");
				hit = true;
				current.speechLib.Set(current, target, attack);
				target.speechLib.Set(target, current, attack);
				Delay(0.5f, delegate
				{
					ProcessSpeech("BattleStart", current);
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "Attack");
						playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						wait = false;
					});
					ProcessText("AttackDesc", current);
					eventQ.Enqueue(delegate
					{
						playerMask.Find("Container").GetComponent<Animation>().Play("Special");
						ChangeExpression(current, "Kiss");
						ChangeExpression(target, "Kiss");
						wait = false;
					});
					ProcessText("SkillActivation", current);
					List<UnitInfo> list2 = new List<UnitInfo>();
					list2.AddRange(current.containingUnits);
					current.secondStomach.Reverse();
					list2.AddRange(current.secondStomach);
					current.secondStomach.Reverse();
					foreach (UnitInfo u4 in list2)
					{
						if (u4.containedByUnit == current)
						{
							eventQ.Enqueue(delegate
							{
								if (target.MaxStomach() + 1 - target.TotalStomach() >= u4.TotalSize())
								{
									playerMask.Find("Container").GetComponent<Animation>().Play("Special");
									enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
									stomachSize = current.TotalStomach() - u4.TotalSize();
									CalcWeight(current);
									current.ProcessUncarry(u4);
									u4.ProcessUncarried();
									UpdateSprite(current, playerMask);
									stomachSize = target.TotalStomach() + u4.TotalSize();
									target.ProcessVContainment(u4);
									UpdateSprite(target, enemyMask);
									tw.LoadText("<SEse_chupa4><SEse_gulp1><SEeaten_by_monster2>" + u4.unitName + " slides from " + current.unitName + "'s stomach into " + target.unitName + "'s!");
									current.containPeriod -= 1f;
								}
								else
								{
									wait = false;
								}
							});
						}
					}
					eventQ.Enqueue(delegate
					{
						ChangeExpression(current, "VSuccess");
						wait = false;
					});


					ProcessSpeech("Kiss", target);
					
					if (current.containPeriod <= 0f)
					{
						current.containPeriod = 0f;
					}
					
					if (current.containingUnits.Count <= 0f)
					{
						current.containPeriod = 0f;
					}
					
					
					if (target.skills.Contains(-1000) && target.GetPredState() != UnitInfo.PredState.V_Container)					
					{
						eventQ.Enqueue(delegate
						{
							if (target.containingUnits.Count > 0)
							{
								EventScene eventScene = this;
								UnitInfo temp2 = current;
								UnitInfo temp3 = target;
								StaticRef.QueueEvent(delegate
								{
									temp2.Pause(x: true);
									temp3.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									eventScene.target.ForceDigestion();
									eventScene.Delay(() => !temp3.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp2.Pause(x: false);
									});
								});
							}


						});
					}
					
						if (target.skills.Contains(-1001) && target.GetPredState() != UnitInfo.PredState.V_Container)					
					{
						eventQ.Enqueue(delegate
						{
							if (target.containingUnits.Count > 0)
							{
								EventScene eventScene = this;
								UnitInfo temp2 = current;
								UnitInfo temp3 = target;
								StaticRef.QueueEvent(delegate
								{
									temp2.Pause(x: true);
									temp3.Pause(x: true);
									StaticRef.turnController.gameState = TurnController.GameState.Transition;
									StaticRef.turnController.HideAll();
									eventScene.target.ForceDigestion();
									eventScene.Delay(() => !temp3.Pause(), delegate
									{
										StaticRef.turnController.gameState = TurnController.GameState.PlayerTurn;
										temp2.Pause(x: false);
									});
								});
							}


						});
					}

					eventQ.Enqueue(delegate
					{
						TileMap.GetTile(target).iOccupied = target.team;
						StartCoroutine(HideBattle(delegate
						{
						
						StandardBattleEnding();;
					
							
							
						}));
					});
				});
			});
			
			
			break;
			case 1146:
			
			StaticRef.Battle();
				Delay(0.5f, delegate
				{
					UseBattleWindow();
					ShowWindow();
					FadeInMessageWindow();
					ShowBattlers("Normal", "Normal");
					hit = true;
					current.speechLib.Set(current, target, attack);
					target.speechLib.Set(target, current, attack);
					Delay(0.5f, delegate
					{
						ProcessSpeech("BattleStart", current);
						
						eventQ.Enqueue(delegate
						{
							tw.LoadText(current.unitName + "'s hands reach for " + target.unitName + " to work her magic...");
							playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
						});
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "VSuccess");
							ChangeExpression(target, "VSuccess");
							playerMask.Find("Container").GetComponent<Animation>().Play("Special");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
							tw.LoadText("<SEpowerup1>After receiving " + target.unitName + "'s help, " + current.unitName + "'s skin begins to glow with radiance!");
							current.dAtk += (int)(current.atk * 0.2f);
						target.dDef += (int)(target.def * 0.2f);
						target.dVatk += (int)(target.vatk * 0.2f);
						target.dVres += (int)(target.vres * 0.2f);
						target.dSkl += (int)(target.skl * 0.2f);
						target.dSpd += (int)(target.spd * 0.2f);
						target.AddStatus(StatusList.AddStatus(151, 1));
						});
						
						eventQ.Enqueue(delegate
						{
							
							tw.LoadSpeech("<NF><NEStar><NEMouth\\Kiss><NEEyes\\Wink><NEBrows\\Normal>Come back any time~", current);
							
							
							
						});
						
						
						eventQ.Enqueue(delegate
						{
							StartCoroutine(HideBattle(delegate
							{
								StandardBattleEnding();
							}));
						});
						
				
				});
				
			});
			
			break;
			
		}	
	}

	public void RunSkill(int id)
	{
		inCoroutine = false;
		wait = false;
		hit = false;
		eventQ.Clear();
		switch (id)
		{
		case -503:
			GenericMessage("Hunger", delegate
			{
				current.isPlayer = false;
				current.Pause(x: false);
			});
			break;
		case -502:
			StartCoroutine(HealFinish());
			break;
		case -501:
			StartCoroutine(KO());
			break;
		case 0:
			Trip();
			break;
		case 6:
		{
			int x = Rndm.Rand(0, 11);
			StaticRef.Message();
			Delay(0.5f, delegate
			{
				UseMessageWindow();
				StaticRef.turnController.HideInitiative();
				FadeInBackground();
				NewUnit(current, center);
				center.GetComponent<UIFade>().FadeIn();
				ShowWindow();
				eventQ.Enqueue(delegate
				{
					tw.LoadText("<SRheavyBurp>" + current.unitName + " belches out a last memento from Schrodinger!");
				});
				Delay(0.5f, delegate
				{
					if (x == 0)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(target, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was an empty vial! Nothing happens...");
						});
					}
					else if (x == 1)
					{
						eventQ.Enqueue(delegate
						{
							center.Find("Container").GetComponent<Animation>().Play("Special");
							current.CalculateDamage(-50);
							tw.LoadText("<SEglass_break>It was a health potion! 50 HP Recovered!");
						});
					}
					else if (x == 2)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "VSuccess");
							enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
							current.en = current.Men();
							tw.LoadText("<SEglass_break>It was a vial of sticky white liquid! She recovers all energy!");
						});
					}
					else if (x == 3)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							current.CalculateDamage(50);
							tw.LoadText("<SEglass_break>It was a vial of poison! 50 Damage!");
						});
					}
					else if (x == 4)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							current.en -= 50f;
							if (current.en < 0f)
							{
								current.en = 0f;
							}
							tw.LoadText("<SEglass_break>It was a jar of Cornucopia's special seasoning! She loses 50 energy!");
						});
					}
					else if (x == 5)
					{
						eventQ.Enqueue(delegate
						{
							EventScene eventScene = this;
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break><SEStatus3>It was an evil sealing urn! A powerful vortex appears around her, sucking in all nearby units and sending them stumbling towards her!");
							UnitInfo temp = current;
							List<Tile> list = TileMap.GetTile(current).CheckAdjacent();
							Rndm.Shuffle(list);
							foreach (Tile item in list)
							{
								UnitInfo u = TileMap.GetUnitInfo(item);
								StaticRef.InsertEvent(delegate
								{
									if (u != null && u.isVorable(temp) && Rndm.Chance(50f))
									{
										temp.Pause(x: true);
										u.Pause(x: true);
										StaticRef.turnController.gameState = TurnController.GameState.Transition;
										StaticRef.Message();
										u.speechLib.Set(u, temp, SkillList.GetByID(0));
										eventScene.current.speechLib.Set(temp, u, SkillList.GetByID(0));
										eventScene.Setup(u, temp, SkillList.GetByID(0));
										eventScene.RunSkill(0);
										eventScene.Delay(() => !u.Pause(), delegate
										{
											StaticRef.Turn();
											temp.Pause(x: false);
										});
									}
									else
									{
										StaticRef.Turn();
										temp.Pause(x: false);
									}
								});
							}
						});
					}
					else if (x == 6)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							current.AddStatus(StatusList.AddStatus(0, 2));
							tw.LoadText("<SEglass_break>It was a flashbang! The grenade explodes with blinding light, dizzying her!");
						});
					}
					else if (x == 7)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");							
							tw.LoadText("<SEglass_break>It was a nerve gas grenade! The grenade inflicts paralysis, reducing her stats by 20% and halving movement range!");
							current.dAtk -= (int)(current.atk * 0.2f);
							current.dDef -= (int)(current.def * 0.2f);
							current.dVatk -= (int)(current.vatk * 0.2f);
							current.dVres -= (int)(current.vres * 0.2f);
							current.dSkl -= (int)(current.skl * 0.2f);
							current.dSpd -= (int)(current.spd * 0.2f);
							current.dMove -= current.movement / 2;
							current.AddStatus(StatusList.AddStatus(143, 1));
						});
						
					}
					else if (x == 8)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of experimental polyphagia syrup! She is inflicted with extreme hunger, losing all energy!");
							if (current.en > 0f)
							{
							current.en -= current.en;
							}
							if (current.en < 0f)
							{
								current.en = 0f;
							}
						});
					}
					else if (x == 9)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "VSuccess");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of slippery lubricant! She is covered with the lubricant, gaining a massive boost to VRES!");
							current.dVres += 1000f;
							current.AddStatus(StatusList.AddStatus(145, 1));
						});
					}
					else if (x == 10)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "VSuccess");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a cup of coffee! She gains 50 energy!");
							current.en += 50f;
							if (current.en > current.Men())
							{
								current.en = current.Men();
							}
						});
					}
					else if (x == 11)
					{
						eventQ.Enqueue(delegate
						{
							ChangeExpression(current, "Damage");
							center.Find("Container").GetComponent<Animation>().Play("Special");
							tw.LoadText("<SEglass_break>It was a bottle of nutritional supplement! She gains 50 nutritional value!");
							current.nutValue += 50;
						});
					}
					eventQ.Enqueue(delegate
					{
						StartCoroutine(HideSpeech(delegate
						{
							current.Pause(x: false);
							StaticRef.Waiting();
						}));
					});
				});
			});
			break;
		}
		case 27:
			StaticRef.Message();
			Delay(0.5f, delegate
			{
				UseMessageWindow();
				StaticRef.turnController.HideInitiative();
				FadeInBackground();
				NewUnit(current, center);
				ChangeExpression(current, "KO");
				center.GetComponent<UIFade>().FadeIn();
				ShowWindow();
				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("Urrup... N-no... I can't lose now... M-master trusted me to protect them!", current);
				});
				eventQ.Enqueue(delegate
				{
					tw.LoadSpeech("<SRheavyGrowl><SRemptyGrowl><NF><NEMouth\\Drool><NEEyes\\Normal><NEBrows\\Angry>Orrrraaaa!! I-I shall not give up so easily!", current);
				});
				UnitInfo unitInfo = current.containingUnits.ToArray()[0];
				unitInfo.speechLib.Set(unitInfo, current, SkillList.GetByID(27));
				ProcessSpeech("Betrayed", unitInfo);
				current.skills.Remove(99);
				int num = 0;
				foreach (UnitInfo item2 in current.CombinedStomach())
				{
					if (item2.containedByUnit == current)
					{
						num++;
					}
				}
				current.hp += current.Mhp() * 0.15f * (float)num;
				eventQ.Enqueue(delegate
				{
					StartCoroutine(HideSpeech(delegate
					{
						current.Pause(x: false);
						current.ForceDigestion();
					}));
				});
			});
			break;
		case 105:
			ShowSpeech();
			ProcessSpeech("SkillActivation", current);
			eventQ.Enqueue(delegate
			{
				tw.LoadText(current.unitName + " is too full to move whatsoever!");
			});
			eventQ.Enqueue(delegate
			{
				StartCoroutine(HideSpeech(delegate
				{
					StaticRef.turnController.moveComplete = true;
					current.Pause(x: false);
				}));
				wait = false;
			});
			break;
		}
	}

	private IEnumerator GenericBattle(Func<float> toHit, Action onFinish)
	{
		StaticRef.Battle();
		yield return new WaitForSeconds(0.5f);
		UseBattleWindow();
		ShowWindow();
		FadeInMessageWindow();
		ShowBattlers();
		hit = Rndm.Chance(toHit());
		if (attack.damage != null)
		{
			damage = attack.damage();
		}
		current.speechLib.Set(current, target, attack);
		target.speechLib.Set(target, current, attack);
		yield return new WaitForSeconds(0.5f);
		processQ.Clear();
		if (hit)
		{
			if (attack.type == Attack.Type.VAttack)
			{
				processQ.Enqueue(delegate
				{
					ChangeExpression(current, "VSuccess");
					ChangeExpression(target, "VEaten");
					playerMask.Find("Container").GetComponent<Animation>().Play("Click");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Die");
					stomachSize = current.TotalStomach() + target.TotalSize();
					CalcWeight(current);
					current.ProcessVContainment(target);
					UpdateSprite(current, playerMask);
					KillSprite(target, enemyMask);
					MonoBehaviour.print(current.GetPredState().ToString() + " Pred State");
					wait = false;
				});
			}
			else
			{
				processQ.Enqueue(delegate
				{
					ChangeExpression(target, "Damage");
					enemyMask.Find("Container").GetComponent<Animation>().Play("Special");
					target.CalculateDamage(damage);
					StaticRef.enemy.UpdateHealth();
					wait = false;
				});
			}
		}
		else
		{
			processQ.Enqueue(delegate
			{
				ChangeExpression(current, "VFail");
				enemyMask.Find("Container").GetComponent<Animation>().Play("Recoil");
				wait = false;
			});
		}
		ProcessSpeech("BattleStart", current);
		ProcessSpeech("Targeted", target);
		eventQ.Enqueue(delegate
		{
			ChangeExpression(current, "Attack");
			playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
			wait = false;
		});
		ProcessText("AttackDesc", current);
		if (hit)
		{
			if (attack.type == Attack.Type.VAttack)
			{
				eventQ.Enqueue(delegate
				{
					processQ.Dequeue()();
				});
			}
			else
			{
				eventQ.Enqueue(delegate
				{
					processQ.Dequeue()();
				});
			}
			ProcessText("AttackHitAttacker", current);
			ProcessText("AttackHitTarget", target);
			ProcessSpeech("AttackHitTargetS", target);
			ProcessSpeech("AttackHitAttackerS", current);
		}
		else
		{
			eventQ.Enqueue(delegate
			{
				processQ.Dequeue()();
			});
			ProcessText("AttackMissAttacker", current);
			ProcessText("AttackMissTarget", target);
			ProcessSpeech("AttackMissTargetS", target);
			ProcessSpeech("AttackMissAttackerS", current);
		}
		eventQ.Enqueue(delegate
		{
			onFinish();
		});
	}

	private IEnumerator NonstandardBattle(Func<float> toHit, Action onHit, Action onFail, Action onFinish)
	{
		StaticRef.Battle();
		yield return new WaitForSeconds(0.5f);
		UseBattleWindow();
		ShowWindow();
		FadeInMessageWindow();
		ShowBattlers();
		hit = Rndm.Chance(toHit());
		if (attack.damage != null)
		{
			damage = attack.damage();
		}
		current.speechLib.Set(current, target, attack);
		target.speechLib.Set(target, current, attack);
		yield return new WaitForSeconds(0.5f);
		ProcessSpeech("BattleStart", current);
		ProcessSpeech("Targeted", target);
		eventQ.Enqueue(delegate
		{
			ChangeExpression(current, "Attack");
			playerMask.Find("Container").GetComponent<Animation>().Play("Attack");
			wait = false;
		});
		ProcessText("AttackDesc", current);
		if (hit)
		{
			onHit();
			ProcessText("AttackHitAttacker", current);
			ProcessText("AttackHitTarget", target);
			ProcessSpeech("AttackHitTargetS", target);
			ProcessSpeech("AttackHitAttackerS", current);
		}
		else
		{
			onFail();
			ProcessText("AttackMissAttacker", current);
			ProcessText("AttackMissTarget", target);
			ProcessSpeech("AttackMissTargetS", target);
			ProcessSpeech("AttackMissAttackerS", current);
		}
		eventQ.Enqueue(delegate
		{
			onFinish();
		});
	}

	private IEnumerator GenericFeed(Func<float> toHit, Action onFinish)
	{
		StaticRef.Battle();
		yield return new WaitForSeconds(0.5f);
		UseBattleWindow();
		ShowWindow();
		FadeInMessageWindow();
		ShowBattlers("Attack", "Normal");
		hit = true;
		current.speechLib.Set(current, target, attack);
		target.speechLib.Set(target, current, attack);
		yield return new WaitForSeconds(0.5f);
		ProcessSpeech("BattleStart", current);
		ProcessSpeech("Targeted", target);
		ProcessText("AttackDesc", current);
		eventQ.Enqueue(delegate
		{
			enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
			stomachSize = target.TotalStomach() + current.TotalSize();
			CalcWeight(target);
			target.ProcessVContainment(current);
			UpdateSprite(target, enemyMask);
			KillSprite(current, playerMask);
			if (target.isSafeContainer())
			{
				target.EndSafeDigestion();
			}
			MonoBehaviour.print(target.GetPredState().ToString() + " Pred State");
			ChangeExpression(target, "Normal");
			StaticRef.turnController.moveComplete = true;
			wait = false;
		});
		ProcessText("AttackHitAttacker", current);
		ProcessText("AttackHitTarget", target);
		ProcessSpeech("AttackHitAttackerS", current);
		ProcessSpeech("AttackHitTargetS", target);
		eventQ.Enqueue(delegate
		{
			StartCoroutine(HideBattle(onFinish));
		});
	}

	private void GenericMessage(string message, Action onFinish)
	{
		UseMessageWindow();
		ShowWindow();
		FadeInMessageWindow();
		NewUnit(current, center);
		center.GetComponent<UIFade>().FadeIn();
		ProcessSpeech(message, current);
		EventQ().Enqueue(delegate
		{
			center.GetComponent<UIFade>().FadeOut();
			FadeOutMessageWindow();
			HideWindow();
			Cleanup();
			Delay(0.5f, delegate
			{
				onFinish();
			});
		});
	}

	public void StoryScene(Action onFinish)
	{
		UseMessageWindow();
		ShowWindow();
		FadeInMessageWindow();
		EventQ().Enqueue(delegate
		{
			FadeOutMessageWindow();
			HideWindow();
			Cleanup();
			Delay(0.5f, delegate
			{
				onFinish();
			});
		});
	}

	private IEnumerator HideBattle(Action onFinish)
	{
		HideWindow();
		HideBattlers();
		FadeOutMessageWindow();
		StaticRef.status.GetComponent<Animator>().Play("BattleHide");
		StaticRef.enemy.GetComponent<Animator>().Play("BattleHide");
		Cleanup();
		yield return new WaitForSeconds(0.5f);
		onFinish();
		wait = false;
	}

	public void ProcessContainment(UnitInfo a)
	{
		StaticRef.Message();
		current = a;
		current.speechLib.Set(current);
		ShowSpeech();
		Delay(0.2f, delegate
		{
			center.Find("Container").GetComponent<Animation>().Play("Special");
		});
		ProcessSpeech("vTickDown", current);
		if (current.GetPredState() == UnitInfo.PredState.V_Container)
		{
			eventQ.Enqueue(delegate
			{
				tw.LoadText(current.unitName + "'s stomach growls loudly!\n" + Mathf.Round(current.containPeriod) + " turns remaining!");
			});
		}
		else if (current.GetPredState() == UnitInfo.PredState.H_Container)
		{
			eventQ.Enqueue(delegate
			{
				tw.LoadText(current.unitName + "'s gurgles and blurps calmly.\n" + Mathf.Round(current.containPeriod) + " turns remaining.");
			});
		}
		eventQ.Enqueue(delegate
		{
			StartCoroutine(HideSpeech(delegate
			{
				current.Pause(x: false);
				StaticRef.Waiting();
			}));
		});
	}

	public void CompleteContainment(UnitInfo a)
	{
		StaticRef.Message();
		current = a;
		current.speechLib.Set(current);
		ShowSpeech();
		ProcessSpeech("vComplete", current);
		eventQ.Enqueue(delegate
		{
			ChangeExpression(current, "Digest");
			FadeOutMessageWindowOnly();
			HideWindow();
			wait = true;
			StartCoroutine(delayAction.ExecuteAfterTime(1f, delegate
			{
				wait = false;
			}));
		});
		eventQ.Enqueue(delegate
		{
			StartCoroutine(DigestAnimation(current, center, current.TotalStomach()));
		});
		eventQ.Enqueue(delegate
		{
			tw.Clear();
			ShowWindow();
			FadeInMessageWindowOnly();
			tw.LoadText(current.unitName + " digests everyone inside her!");
			current.ProcessDigestAll();
			current.CalcCap();
			StaticRef.sound.PlayStomachBGS();
		});
		ProcessSpeech("vAftermath", current);
		eventQ.Enqueue(delegate
		{
			StaticRef.QueueEvent(delegate
			{
				StaticRef.status.NewCharacter(current);
				StaticRef.Turn();
			});
			center.GetComponent<UIFade>().FadeOut();
			if (current.blevel > 0)
			{
				FadeOutMessageWindowOnly();
				LevelUp(current);
			}
			else
			{
				FadeOutMessageWindow();
				HideWindow();
				Cleanup();
				current.Pause(x: false);
				StaticRef.Waiting();
			}
		});
	}

	public void LevelUp(UnitInfo a)
	{
		StaticRef.Message();
		current = a;
		tw.skip = false;
		UseMessageWindow();
		ShowWindow();
		StaticRef.sound.LevelUp();
		StaticRef.status.NewCharacter(current);
		StartCoroutine(delayAction.ExecuteAfterTime(0.5f, delegate
		{
			FadeInBackground();
			StaticRef.level.GetComponent<LevelUpWindow>().SetupWindow(a);
			StaticRef.level.GetComponent<LevelUpWindow>().ShowWindow();
			StartCoroutine(delayAction.ExecuteAfterTime(1f, delegate
			{
				StartCoroutine(StaticRef.level.GetComponent<LevelUpWindow>().LevelUpAnimation(a));
				wait = true;
				StartCoroutine(delayAction.ExecuteWhenTrue(() => !StaticRef.level.GetComponent<LevelUpWindow>().animating, delegate
				{
					wait = false;
					ShowWindow();
					FadeInMessageWindowOnly();
					ProcessSpeech("vLevelUp", current);
					eventQ.Enqueue(delegate
					{
						StaticRef.level.GetComponent<LevelUpWindow>().HideWindow();
						FadeOutMessageWindow();
						HideWindow();
						Cleanup();
						current.Pause(x: false);
						StaticRef.Waiting();
					});
				}));
			}));
		}));
	}

	public void DigestAll(List<UnitInfo> list)
	{
		StaticRef.Message();
		foreach (UnitInfo u in list)
		{
			u.speechLib.Set(u);
			eventQ.Enqueue(delegate
			{
				MonoBehaviour.print("ADDING " + u.unitName + " GKJHNSJKLEGHRFJESGHIUKFGHBEIRWUGHuiohwjgeio*");
				Camera.main.GetComponent<CameraFollow>().GoToTarget(u.gameObject);
				NewUnit(u, center);
				inCoroutine = true;
				StartCoroutine(delayAction.ExecuteAfterTime(0.5f, delegate
				{
					inCoroutine = false;
					wait = false;
				}));
			});
			eventQ.Enqueue(delegate
			{
				UseMessageWindow();
				ShowWindow();
				FadeInMessageWindow();
				center.GetComponent<UIFade>().FadeIn();
				wait = false;
			});
			ProcessSpeech("vComplete", u);
			eventQ.Enqueue(delegate
			{
				ChangeExpression(u, "Digest");
				FadeOutMessageWindowOnly();
				HideWindow();
				wait = true;
				StartCoroutine(delayAction.ExecuteAfterTime(1f, delegate
				{
					wait = false;
				}));
			});
			eventQ.Enqueue(delegate
			{
				StartCoroutine(DigestAnimation(u, center, u.TotalStomach()));
			});
			eventQ.Enqueue(delegate
			{
				tw.Clear();
				ShowWindow();
				FadeInMessageWindowOnly();
				tw.LoadText(u.unitName + " digests everyone inside her!");
				u.ProcessDigestAll();
				u.CalcCap();
				StaticRef.sound.PlayStomachBGS();
			});
			ProcessSpeech("vAftermath", u);
			eventQ.Enqueue(delegate
			{
				center.GetComponent<UIFade>().FadeOut();
				FadeOutMessageWindow();
				HideWindow();
				inCoroutine = true;
				StartCoroutine(delayAction.ExecuteAfterTime(0.5f, delegate
				{
					inCoroutine = false;
					wait = false;
				}));
			});
		}
		eventQ.Enqueue(delegate
		{
			StaticRef.Turn();
		});
	}

	private IEnumerator HealFinish()
	{
		ShowSpeech();
		ProcessSpeech("SkillActivation", current);
		eventQ.Enqueue(delegate
		{
			SkillList.SetCurrent(current);
			foreach (int skill in current.skills)
			{
				Skill byID = SkillList.GetByID(skill);
				if (byID.type == Skill.Type.OnHeal)
				{
					byID.action();
				}
			}
			FadeOutMessageWindowOnly();
			HideWindow();
			wait = false;
		});
		StaticRef.sound.StopBGS();
		stomachSize = current.TotalStomach();
		yield return new WaitForSeconds(0.5f);
		foreach (UnitInfo u in current.containingUnits)
		{
			eventQ.Enqueue(delegate
			{
				if (u.containedByUnit == current)
				{
					inCoroutine = true;
					List<Tile> list = new List<Tile>();
					TileMap.GetTile(current).PulseGround(u);
					foreach (Tile item in TileMap.GetTile(current).CheckAdjacent((int)current.GetComponent<Token>().heading))
					{
						if (item.iOccupied == 0 && item.iDistance < 100)
						{
							u.transform.parent = current.transform.parent;
							u.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 1f, item.transform.position.z);
							u.ProcessHealComplete();
							item.iOccupied = u.team;
							break;
						}
						if (item.iDistance < 100)
						{
							list.Add(item);
						}
					}
					while (u.GetPreyState() != 0 && u.GetPreyState() != UnitInfo.PreyState.Defeated)
					{
						Tile[] array = list.ToArray();
						list.Clear();
						Tile[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							foreach (Tile item2 in array2[i].CheckAdjacent())
							{
								if (item2.iOccupied == 0 && item2.iDistance < 100)
								{
									u.transform.parent = current.transform.parent;
									u.transform.position = new Vector3(item2.transform.position.x, item2.transform.position.y + 1f, item2.transform.position.z);
									u.ProcessHealComplete();
									item2.iOccupied = u.team;
									break;
								}
								if (item2.iDistance < 100)
								{
									list.Add(item2);
								}
							}
						}
					}
					StaticRef.sound.se.PlayOneShot(Resources.Load<AudioClip>("Sounds/se_buchu1"));
					stomachSize -= u.TotalSize();
					targetWeight = current.weight;
					UpdateSprite(current, center);
					TileMap.ResetOccupied();
					Delay(0.75f, delegate
					{
						wait = false;
						inCoroutine = false;
					});
				}
				else
				{
					wait = false;
				}
			});
		}
		eventQ.Enqueue(delegate
		{
			current.ProcessEmptyStomach();
			ShowWindow();
			FadeInMessageWindowOnly();
			tw.LoadText("Everyone has been healed inside " + current.unitName + "'s stomach!");
		});
		eventQ.Enqueue(delegate
		{
			StaticRef.status.NewCharacter(current);
			StartCoroutine(HideSpeech(delegate
			{
				current.Pause(x: false);
			}));
		});
	}

	private IEnumerator KO()
	{
		inCoroutine = true;
		SkillList.SetCurrent(current);
		StartCoroutine(current.OnKO());
		yield return new WaitWhile(() => inCoroutine);
		if (current.hp > 0f)
		{
			yield break;
		}
		UseMessageWindow();
		StaticRef.turnController.HideInitiative();
		FadeInBackground();
		NewUnit(current, center);
		ChangeExpression(current, "KO");
		center.GetComponent<UIFade>().FadeIn();
		stomachSize = current.TotalStomach();
		List<UnitInfo> list = new List<UnitInfo>();
		list.AddRange(current.containingUnits);
		current.secondStomach.Reverse();
		list.AddRange(current.secondStomach);
		current.secondStomach.Reverse();
		yield return new WaitForSeconds(0.5f);
		foreach (UnitInfo u in list)
		{
			eventQ.Enqueue(delegate
			{
				if (u.containedByUnit == current)
				{
					inCoroutine = true;
					List<Tile> list2 = new List<Tile>();
					TileMap.GetTile(current).PulseGround(u);
					foreach (Tile item in TileMap.GetTile(current).CheckAdjacent((int)current.GetComponent<Token>().heading))
					{
						if (TileMap.GetUnitInfo(item) == null && item.iDistance < 100)
						{
							u.transform.parent = current.transform.parent;
							u.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 1f, item.transform.position.z);
							u.ProcessEscape();
							item.iOccupied = u.team;
							break;
						}
						if (item.iDistance < 100)
						{
							list2.Add(item);
						}
					}
					while (u.GetPreyState() != 0 && u.GetPreyState() != UnitInfo.PreyState.Defeated)
					{
						Tile[] array = list2.ToArray();
						list2.Clear();
						Tile[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							foreach (Tile item2 in array2[i].CheckAdjacent())
							{
								if (TileMap.GetUnitInfo(item2) == null && item2.iDistance < 100)
								{
									u.transform.parent = current.transform.parent;
									u.transform.position = new Vector3(item2.transform.position.x, item2.transform.position.y + 1f, item2.transform.position.z);
									u.ProcessEscape();
									item2.iOccupied = u.team;
									break;
								}
								if (item2.iDistance < 100)
								{
									list2.Add(item2);
								}
							}
						}
					}
					center.Find("Container").GetComponent<Animation>().Play("Special");
					StaticRef.sound.se.PlayOneShot(Resources.Load<AudioClip>("Sounds/se_chupa4"));
					stomachSize -= u.TotalSize();
					targetWeight = current.weight;
					current.ProcessUncarry(u);
					UpdateSprite(current, center);
					TileMap.ResetOccupied();
					Delay(0.75f, delegate
					{
						wait = false;
						inCoroutine = false;
					});
				}
				else
				{
					wait = false;
				}
			});
		}
		eventQ.Enqueue(delegate
		{
			current.ProcessKO();
			ShowWindow();
			FadeInMessageWindowOnly();
			tw.LoadText("Everyone has been rescued from " + current.unitName + "'s stomach!");
		});
		ProcessSpeech("SkillActivation", current);
		eventQ.Enqueue(delegate
		{
			StartCoroutine(HideSpeech(delegate
			{
				current.Pause(x: false);
			}));
		});
	}

	private void Trip()
	{
		ShowSpeech("SkillActivation");
		ProcessSpeech("SkillActivation", current);
		eventQ.Enqueue(delegate
		{
			tw.LoadText(current.unitName + " trips and stumbles forward...");
		});
		eventQ.Enqueue(delegate
		{
			center.GetComponent<UIFade>().FadeOut();
			FadeOutMessageWindow();
			HideWindow();
			StaticRef.Delay(0.5f, delegate
			{
				UseBattleWindow();
				ShowWindow();
				ShowBattlers("Attack", "Targeted");
				FadeInMessageWindow();
				StaticRef.Delay(0.5f, delegate
				{
					ProcessText("PreBattle", current);
					ProcessSpeech("BattleStart", current);
					ProcessSpeech("Targeted", target);
					ProcessText("AttackDesc", current);
					eventQ.Enqueue(delegate
					{
						enemyMask.Find("Container").GetComponent<Animation>().Play("Click");
						ChangeExpression(target, "VSuccess");
						stomachSize = target.TotalStomach() + current.TotalSize();
						CalcWeight(target);
						target.ProcessVContainment(current);
						current.ProcessVContained(target);
						UpdateSprite(target, enemyMask);
						KillSprite(current, playerMask);
						current.transform.parent = target.transform;
						wait = false;
					});
					ProcessText("AttackHitAttacker", current);
					ProcessText("AttackHitTarget", target);
					ProcessSpeech("AttackHitTargetS", target);
					ProcessSpeech("AttackHitAttackerS", current);
					eventQ.Enqueue(delegate
					{
						skill = null;
						StartCoroutine(HideBattle(delegate
						{
							current.Pause(x: false);
						}));
						wait = false;
					});
				});
				wait = false;
			});
		});
	}

	private IEnumerator DigestAnimation(UnitInfo c, Transform mask, int stomachSize)
	{
		inCoroutine = true;
		Image main = mask.Find("Container").Find("Images").Find("Main")
			.GetComponent<Image>();
		mask.Find("Container").Find("Images").Find("2nd")
			.GetComponent<Image>();
		GameObject g;
		for (int i = stomachSize; i > 1; i--)
		{
			g = CreateTemp(c, i - 1, c.IncWeight(), mask);
			StaticRef.sound.RandomHeavyGrowl();
			StaticRef.sound.RandomEmptyGrowl();
			main.transform.parent.GetComponent<UIFade>().FadeOut();
			g.GetComponent<UIFade>().FadeIn();
			yield return new WaitForSeconds(1.5f);
			CopyTemp(g, mask);
		}
		g = CreateTemp(c, -1, c.IncWeight(), mask);
		StaticRef.sound.RandomHeavyGrowl();
		StaticRef.sound.RandomEmptyGrowl();
		main.transform.parent.GetComponent<UIFade>().FadeOut();
		g.GetComponent<UIFade>().FadeIn();
		yield return new WaitForSeconds(1.5f);
		CopyTemp(g, mask);
		g = CreateTemp(c, 0, c.weight, mask);
		StaticRef.sound.RandomEmptyGrowl();
		main.transform.parent.GetComponent<UIFade>().FadeOut();
		g.GetComponent<UIFade>().FadeIn();
		yield return new WaitForSeconds(1.5f);
		CopyTemp(g, mask);
		inCoroutine = false;
		wait = false;
	}

	private void ShakeAnimation(Image[] image)
	{
		image[0].transform.parent.GetComponent<Animation>().Play("Special");
	}

	public void ProcessSpeech(string t, UnitInfo u)
	{
		List<string> speech = u.speechLib.GetSpeech(t);
		if (speech == null)
		{
			return;
		}
		foreach (string s in speech)
		{
			Debug.Log(s);
			eventQ.Enqueue(delegate
			{
				SkillList.SetCurrent(u);
				currentSpeech = t;
				foreach (int skill in u.skills)
				{
					Skill byID = SkillList.GetByID(skill);
					if (byID.type == Skill.Type.OnSpeech)
					{
						byID.action();
					}
				}
				wait = false;
			});
			eventQ.Enqueue(delegate
			{
				tw.LoadSpeech(s, u);
				wait = true;
			});
		}
	}

	public void ProcessText(string t, UnitInfo u)
	{
		List<string> speech = u.speechLib.GetSpeech(t);
		if (speech == null)
		{
			return;
		}
		foreach (string s in speech)
		{
			Debug.Log(s);
			eventQ.Enqueue(delegate
			{
				SkillList.SetCurrent(u);
				currentSpeech = t;
				foreach (int skill in u.skills)
				{
					Skill byID = SkillList.GetByID(skill);
					if (byID.type == Skill.Type.OnSpeech)
					{
						byID.action();
					}
				}
				wait = false;
			});
			eventQ.Enqueue(delegate
			{
				tw.LoadText(s);
				wait = true;
			});
		}
	}

	public void Cleanup()
	{
		inCoroutine = false;
		wait = false;
		hit = false;
		tw.skip = false;
		skill = null;
		eventQ.Clear();
		StaticRef.turnController.ShowInitiative();
	}

	public void ChangeExpression(UnitInfo u, string s)
	{
		Transform transform = ((!inBattle) ? center : ((!(u == current)) ? enemyMask : playerMask));
		transform = transform.Find("Container");
		transform.GetComponent<ExpressLib>().SetExpression(s);
	}

	public void NewExpression(UnitInfo u)
	{
		Transform transform = ((!inBattle) ? center : ((!(u == current)) ? enemyMask : playerMask));
		transform = transform.Find("Container");
		transform.GetComponent<ExpressLib>().NewExpression();
	}

	public void AddExpression(UnitInfo u, string s)
	{
		Transform transform = ((!inBattle) ? center : ((!(u == current)) ? enemyMask : playerMask));
		transform = transform.Find("Container");
		transform.GetComponent<ExpressLib>().AddExpression(s);
	}

	public void UseMessageWindow()
	{
		inBattle = false;
		battleWindow.Find("Background").GetComponent<Image>().sprite = messageBase;
	}

	public void UseBattleWindow()
	{
		inBattle = true;
		battleWindow.Find("Background").GetComponent<Image>().sprite = battleBase;
	}

	public void FadeInMessageWindow()
	{
		battleBackground.GetComponent<UIFade>().FadeIn();
		battleWindow.GetComponent<UIFade>().FadeIn();
	}

	public void FadeOutMessageWindow()
	{
		battleBackground.GetComponent<UIFade>().FadeOut();
		battleWindow.GetComponent<UIFade>().FadeOut();
	}

	public void FadeOutMessageWindowOnly()
	{
		battleWindow.GetComponent<UIFade>().FadeOut();
	}

	public void FadeInMessageWindowOnly()
	{
		battleWindow.GetComponent<UIFade>().FadeIn();
	}

	public void FadeInBackground()
	{
		battleBackground.GetComponent<UIFade>().FadeIn();
	}

	public void FadeOutBackground()
	{
		battleBackground.GetComponent<UIFade>().FadeOut();
	}

	public void ShowWindow()
	{
		StaticRef.turnController.HideInitiative();
		battleWindow.GetComponent<UIFade>().FadeIn();
		tw.Clear();
	}

	public void HideWindow()
	{
		battleWindow.GetComponent<UIFade>().FadeOut();
		tw.Clear();
	}

	private void StandardBattleEnding()
	{
		StartCoroutine(HideBattle(delegate
		{
			StaticRef.ActionEnd();
			StaticRef.QueueEvent(delegate
			{
				if (target != null)
				{
					target.ActionEnd();
				}
				else
				{
					StaticRef.Turn();
				}
			});
		}));
	}

	private void ShowBattlers()
	{
		NewUnit(current, playerMask);
		NewUnit(target, enemyMask);
		playerMask.GetComponent<UIFade>().FadeIn();
		enemyMask.GetComponent<UIFade>().FadeIn();
		playerMask.Find("Container").GetComponent<Animation>().Play("Show");
		enemyMask.Find("Container").GetComponent<Animation>().Play("Show");
	}

	private void ShowBattlers(string currentFace, string targetFace)
	{
		NewUnit(current, playerMask);
		NewUnit(target, enemyMask);
		ChangeExpression(current, currentFace);
		ChangeExpression(target, targetFace);
		playerMask.GetComponent<UIFade>().FadeIn();
		enemyMask.GetComponent<UIFade>().FadeIn();
		playerMask.Find("Container").GetComponent<Animation>().Play("Show");
		enemyMask.Find("Container").GetComponent<Animation>().Play("Show");
	}

	private void HideBattlers()
	{
		playerMask.GetComponent<UIFade>().FadeOut();
		playerMask.Find("Container").GetComponent<Animation>().Play("Hide");
		enemyMask.GetComponent<UIFade>().FadeOut();
		enemyMask.Find("Container").GetComponent<Animation>().Play("Hide");
	}

	private void ShowSpeech()
	{
		UseMessageWindow();
		ShowWindow();
		FadeInMessageWindow();
		NewUnit(current, center);
		center.GetComponent<UIFade>().FadeIn();
	}

	private void ShowSpeech(string face)
	{
		UseMessageWindow();
		ShowWindow();
		FadeInMessageWindow();
		NewUnit(current, center);
		ChangeExpression(current, face);
		center.GetComponent<UIFade>().FadeIn();
	}

	private IEnumerator HideSpeech(Action onFinish)
	{
		center.GetComponent<UIFade>().FadeOut();
		FadeOutMessageWindow();
		HideWindow();
		Cleanup();
		yield return new WaitForSeconds(0.5f);
		onFinish();
	}

	private GameObject CreateTemp(UnitInfo u, int x, int y, Transform mask)
	{
		GameObject gameObject = mask.Find("Container").Find("Images").gameObject;
		gameObject = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
		Image component = gameObject.transform.Find("Main").GetComponent<Image>();
		Image component2 = gameObject.transform.Find("2nd").GetComponent<Image>();
		if (x < u.StomachSize(u.containingUnits))
		{
			gameObject.transform.SetSiblingIndex(0);
		}
		else
		{
			gameObject.transform.SetSiblingIndex(1);
		}
		gameObject.name = "Temp";
		gameObject.GetComponent<CanvasGroup>().alpha = 0f;
		if (x >= u.cCap && u.clas.hCap > 0)
		{
			x -= u.cCap;
			component.sprite = SpriteLoad.Load(u);
			if (x < u.hCap)
			{
				CanvasGroup canvasGroup = component.gameObject.AddComponent<CanvasGroup>();
				canvasGroup.ignoreParentGroups = true;
				canvasGroup.alpha = 1f;
			}
			component2.sprite = SpriteLoad.LoadSecond(u.clas.refName, x);
		}
		else
		{
			component.sprite = SpriteLoad.Load(u.clas.refName, x, y);
			if (u.clas.hCap > 0)
			{
				component2.sprite = SpriteLoad.LoadSecond(u.clas.refName, 0);
			}
		}
		return gameObject;
	}

	private void CopyTemp(GameObject g, Transform mask)
	{
		Image component = mask.Find("Container").Find("Images").Find("Main")
			.GetComponent<Image>();
		Image component2 = mask.Find("Container").Find("Images").Find("2nd")
			.GetComponent<Image>();
		component.sprite = g.transform.Find("Main").GetComponent<Image>().sprite;
		component2.sprite = g.transform.Find("2nd").GetComponent<Image>().sprite;
		component.transform.parent.GetComponent<CanvasGroup>().alpha = 1f;
		component.transform.parent.localScale = Vector3.one;
		UnityEngine.Object.DestroyImmediate(g);
	}

	private IEnumerator UpdateAnimation(UnitInfo c, Transform mask)
	{
		Image component = mask.Find("Container").Find("Images").Find("Main")
			.GetComponent<Image>();
		mask.Find("Container").Find("Images").Find("2nd")
			.GetComponent<Image>();
		GameObject g = CreateTemp(c, stomachSize, targetWeight, mask);
		component.transform.parent.GetComponent<UIFade>().FadeOut();
		g.GetComponent<UIFade>().FadeIn();
		yield return new WaitForSeconds(1f);
		CopyTemp(g, mask);
	}

	public void UpdateSprite(UnitInfo c, Transform mask)
	{
		StartCoroutine(UpdateAnimation(c, mask));
	}

	private void KillSprite(UnitInfo c, Transform t)
	{
		t.GetComponent<UIFade>().FadeOut();
	}

	private void CalcWeight(UnitInfo u)
	{
		targetWeight = u.weight;
	}

	private void DoubleQueue(Action a)
	{
	}

	private List<UnitInfo> MergeStomachs(UnitInfo u)
	{
		List<UnitInfo> list = new List<UnitInfo>();
		list.AddRange(u.containingUnits);
		list.AddRange(u.secondStomach);
		return list;
	}

	public Queue<Action> EventQ()
	{
		return eventQ;
	}

	public TextWriter TW()
	{
		return tw;
	}

	public Attack GetLastAttack()
	{
		return attack;
	}

	public Skill GetSkill()
	{
		return skill;
	}

	public void Delay(float delay, Action act)
	{
		StartCoroutine(delayAction.ExecuteAfterTime(delay, delegate
		{
			act();
		}));
	}

	public void Delay(Func<bool> delayUntil, Action act)
	{
		StartCoroutine(delayAction.ExecuteWhenTrue(delayUntil, delegate
		{
			act();
		}));
	}
}
