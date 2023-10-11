using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class AttackList
{
	public static void Initialize()
	{
		AttackList.attacks = new List<Attack>();
		int id = 0;
		AttackList.attacks.Add(new Attack(-1001, "Feed", "Feed", "Null", 0f, 0f, 1, 1, 0f, "Single", "Feed", Attack.Type.VOther, Attack.Visibility.Invisible, delegate()
		{
			id = -1001;
			AttackList.StandardSetup(-1001);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Feed";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + 100 + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Turns to digest: ",
				(int)AttackList.target.containPeriod,
				" → ",
				AttackList.target.PreviewContainment(AttackList.current)
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				StaticRef.turnController.moveComplete = true;
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(-1001);
			if (AttackList.current.TotalSize() <= 2)
			{
				foreach (Tile item in TileMap.GetTile(AttackList.current).CheckAdjacent())
				{
					if (TileMap.GetUnit(item) != null && TileMap.GetUnit(item).skills.Contains(99))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(-1001);
			return u.skills.Contains(99) && u.MaxStomach() - u.TotalStomach() >= AttackList.current.TotalSize() - 1;
		}));
		AttackList.attacks.Add(new Attack(-1000, "Enter", "Enter", "Null", 0f, 0f, 1, 1, 0f, "Single", "Carry", Attack.Type.VOther, Attack.Visibility.Invisible, delegate()
		{
			AttackList.StandardSetup(-1000);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Enter";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + 100 + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "No Digestion";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.sound.se.PlayOneShot(Resources.Load<AudioClip>("Sounds/se_gulp1"));
				AttackList.target.ProcessCarry(AttackList.current);
				AttackList.current.GetComponent<Token>().SContainedBy(AttackList.target);
				StaticRef.turnController.actionComplete = true;
				StaticRef.turnController.moveComplete = true;
				TileMap.PaintClearTiles();
				StaticRef.turnController.MovementEnd();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(-1000);
			if (AttackList.current.isVorable())
			{
				foreach (Tile t in TileMap.GetTile(AttackList.current).CheckAdjacent())
				{
					UnitInfo unit = TileMap.GetUnit(t);
					if (unit != null && unit.team == AttackList.current.team && unit.skills.Contains(-1000) && unit.isSafeContainer())
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(-1000);
			return u.MaxStomach() - u.TotalStomach() >= AttackList.current.TotalSize() - 1 && u.skills.Contains(-1000) && u.isAlive();
		}));
		AttackList.attacks.Add(new Attack(-11, "Swallow", "Hunger", "Devour a single unit with a low chance of success.", 0f, 0f, 1, 1, 120f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Invisible, delegate()
		{
			id = -11;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Swallow";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcVHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = AttackList.StandardTurnsToDigest();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(-11);
			return AttackList.current.TotalStomach() < AttackList.current.MaxStomach();
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(-11);
			return u.isVorable(AttackList.current);
		}));
		AttackList.attacks.Add(new Attack(-10, "Kiss", "Kiss", "0", 0f, 0f, 1, 1, 100f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Invisible, delegate()
		{
			id = -10;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Kiss";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Feed Everything";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(-10);
			return true;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(-10);
			return u.TotalStomach() < u.MaxStomach() + 1;
		}));
		AttackList.attacks.Add(new Attack(0, "Swallow", "Contain", "Devour a single unit with a low chance of success.", 0f, 0f, 1, 1, 70f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Invisible, delegate()
		{
			id = 0;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Swallow";
			if (AttackList.current.team == AttackList.target.team)
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcVHit() + "%";
			}
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = AttackList.StandardTurnsToDigest();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(0);
			return AttackList.current.TotalStomach() < AttackList.current.MaxStomach();
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(0);
			return u.isVorable(AttackList.current);
		}));
		AttackList.attacks.Add(new Attack(1, "Digest", "Digest", "Take some time to focus on digestion. Speeds up digestion slightly.", 0f, 0f, 0, 0, 100f, "Self", "", Attack.Type.VOther, Attack.Visibility.Invisible, delegate()
		{
			AttackList.StandardSetup(1);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Digest";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Success Rate: 100%";
			if (AttackList.current.isSafeContainer())
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.PreviewFullDigestion(),
					" → ",
					AttackList.current.PreviewFullDigestion() - 1
				});
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.containPeriod,
					" → ",
					AttackList.current.containPeriod - 1f
				});
			}
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(false);
				StaticRef.turnController.current.ForceDigestion();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(1);
			return AttackList.current.StateIsContainer();
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(2, "Attack", "Attack", "Attacks with equipped weapon", 1.1f, 0f, 1, 1, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Invisible, delegate()
		{
			id = 2;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Attack";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), () => true, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(3, "Pounce", "Pounce", "Schrodinger leaps on a faraway target with cat-like agility.", 1f, 0f, 2, 2, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 3;
			AttackList.StandardSetup(id);
			float num5 = 0.6f;
			float num6 = 1.4f;
			for (int i = 0; i < AttackList.current.TotalStomach(); i++)
			{
				num5 += 0.15f;
				num6 += 0.05f;
			}
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Pounce";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				(int)((float)StaticRef.CalcMinDamage() * num5),
				" - ",
				(int)((float)StaticRef.CalcMaxDamage() * num6)
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(3);
			float num3 = 0.6f;
			float num4 = 1.4f;
			for (int i = 0; i < AttackList.current.TotalStomach(); i++)
			{
				num3 += 0.15f;
				num4 += 0.05f;
			}
			return (int)((float)StaticRef.CalcDamage() * Rndm.Rand(num3, num4));
		}, () => true, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(4, "Rapid Fire", "RapidFire", "Cornucopia fires a burst from her SMG, dealing minor damage.", 1f, 0f, 1, 3, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 4;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Rapid Fire";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), () => true, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(5, "All-In", "AllIn", "Devour a single unit with a low chance of success.", 0f, 20f, 1, 1, 0f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Visible, delegate()
		{
			id = 5;
			AttackList.StandardSetup(id);
			float num = 50f;
			float num2 = AttackList.target.hp - AttackList.current.hp;
			if (num2 > 0f)
			{
				num -= num2 / 5f;
			}
			if (AttackList.target.TotalSize() > 2 || AttackList.current.TotalStomach() == AttackList.current.MaxStomach())
			{
				num = 0f;
			}
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "All-In";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + num + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = AttackList.StandardTurnsToDigest() + "?";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(0);
			return AttackList.current.TotalStomach() <= AttackList.current.MaxStomach() && AttackList.current.en >= 20f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(0);
			return u.TotalStomach() < u.MaxStomach() + 1;
		}));
		AttackList.attacks.Add(new Attack(6, "Vial Toss", "VialToss", "Schrodinger throws a vial that does a random effect. 15 unique effects are possible.", 0f, 10f, 1, 3, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 6;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Damage: ???";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(6);
			return AttackList.current.en >= 10f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(7, "Pinpoint Fire", "PinpointFire", "Sampo takes aim and fires her pistol at the enemy, dealing minor damage.", 1.1f, 0f, 1, 2, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 7;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Pinpoint Fire";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), () => true, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(8, "Point-Blank", "PointBlank", "Sampo fires her gun at point-blank range.", 1.4f, 10f, 1, 1, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 8;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Point-Blank";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(10);
			return AttackList.current.en >= 10f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(10, "Neutralize", "Neutralize", "If this attack defeats the target, all units inside will be rescued and then, if possible, Theano will immediately devour the target. Under certain circumstances, has a small chance of failing.", 1.35f, 15f, 1, 1, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 10;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Neutralize";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(10);
			return AttackList.current.en >= 15f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(11, "NeutralizeEat", "NeutralizeEat", "", 0f, 0f, 0, 0, 0f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Invisible, delegate()
		{
		}, () => true, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(12, "Dragon Kick", "DragonKick", "Deals massive damage to a single target with a low success rate.", 1.85f, 40f, 1, 1, 70f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 12;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Dragon Kick";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(2);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(12);
			return AttackList.current.en >= 40f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(15, "Order Up!", "OrderUp", "If this attack defeats the target and Chateau is carrying an ally, the target will be fed to a random ally inside Chateau, if possible. Allies too large to be vorable can no longer leave Chateau's stomach.", 1.3f, 20f, 1, 1, 90f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 15;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Order Up!";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(15);
			return AttackList.current.en >= 25f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(16, "Battle Cry", "WarCry", "Chateau lets loose a bellowing war cry, bolstering adjacent allies' ATK and VATK for one turn. If Chateau is currently digesting prey, she instead, decreases the ATK and VATK of all adjacent units for two turns.", 0f, 15f, 1, 1, 100f, "Self", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 16;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			if (AttackList.current.GetPredState() != UnitInfo.PredState.V_Container)
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Buff Adjacent";
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Debuff Adjacent";
			}
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(false);
				AttackList.eventScene.Setup(AttackList.current, null, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(16);
			return AttackList.current.en >= 15f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(141, "Radiance", "Radiance", "Covenant releases soothing beams that replenishes 50 energy for adjacent friendly units.", 0f, 50f, 1, 1, 100f, "Self", "", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 141;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Restore Adjacent";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(false);
				AttackList.eventScene.Setup(AttackList.current, null, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(141);
			return AttackList.current.en >= 50f;
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(20, "Respite", "Respite", "By consuming a large amount of energy, devour another unit and immediately heal them.", 0f, 40f, 1, 1, 100f, "Single", "", Attack.Type.VAttack, Attack.Visibility.Visible, delegate()
		{
			id = 20;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Respite";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Turns to heal: Instant";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= 40f;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(20);
			return AttackList.current.containingUnits.Count <= 0 && AttackList.current.en >= 40f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(20);
			return u.isVorable(AttackList.current) && AttackList.current.isSafeContainment(u);
		}));
		AttackList.attacks.Add(new Attack(21, "Communion", "Communion", "Covenant directs intense energy inwards, digesting an ally at an incredible rate. Consumes a nearby allied unit and then instantly digests them to recover all energy.", 0f, 0f, 1, 1, 100f, "Single", "Deadly", Attack.Type.VAttack, Attack.Visibility.Visible, delegate()
		{
			id = 21;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Communion";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Turns to digest: Instant";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.GetByID(id).enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.current.SetPredState(UnitInfo.PredState.V_Container);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(21);
			return AttackList.current.containingUnits.Count <= 0 && AttackList.current.en >= AttackList.GetByID(21).enCost;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(21);
			return u.isVorable(AttackList.current) && u.team == AttackList.current.team;
		}));
		AttackList.attacks.Add(new Attack(22, "Holy Light", "Holy Light", "Covenant emits an intense beam of energy towards a foe. Deals moderate damage to a single opponent at range and inflicts Charred, reducing their ATK, VATK, SPD, and SKL by 20% for 1 turn.", 1.3f, 10f, 2, 3, 95f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate
		{
			id = 22;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Holy Light";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Damage: " + StaticRef.CalcMinDamage() + " - " + StaticRef.CalcMaxDamage();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				current.en -= GetByID(id).enCost;
				StaticRef.turnController.ProcessAttack(inBattle: true);
				eventScene.Setup(current, target, attack);
				eventScene.RunAttack(142);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), () => current.en >= GetByID(22).enCost, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(120, "CarrierDigest", "CarrierDigest", "Take some time to focus on digestion. Speeds up digestion slightly.", 0f, 0f, 0, 0, 100f, "Self", "", Attack.Type.VOther, Attack.Visibility.Invisible, delegate()
		{
			AttackList.StandardSetup(120);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Can I Digest Them?";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Success Rate: 100%";
			if (AttackList.current.isSafeContainer())
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.PreviewFullDigestion(),
					" → ",
					AttackList.current.PreviewFullDigestion() - 1
				});
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.containPeriod,
					" → ",
					AttackList.current.containPeriod - 1f
				});
			}
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(false);
				StaticRef.turnController.current.ForceDigestion();
			});
			StaticRef.confirm.Find("No").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("No").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ActionEnd();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
		}, () => false, (UnitInfo u) => false));
		AttackList.attacks.Add(new Attack(140, "Heal", "Heal", "Expend some energy to instantly heal all contained units.", 0f, 20f, 0, 0, 100f, "Self", "", Attack.Type.VOther, Attack.Visibility.Invisible, delegate()
		{
			AttackList.StandardSetup(140);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Heal";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Success Rate: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Turns to heal: ",
				AttackList.current.containPeriod,
				" → ",
				0
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				AttackList.current.en -= 20f;
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(false);
				StaticRef.turnController.current.ForceHeal();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(140);
			return AttackList.current.StateIsContainer() && AttackList.current.en >= 20f && AttackList.current.isSafeContainer();
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(1000, "Sleep", "Sleep", "Casts a magic spell, putting a single unit to sleep and slightly delaying their digestion. If target is currently safely containing units, speeds up digestion instead.", 0f, 15f, 1, 5, 75f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1000;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Inflicts Sleep";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1000);
			return AttackList.current.en >= 15f;
		}, (UnitInfo u) => u.TotalStomach() > 0 && !u.skills.Contains(99)));
		AttackList.attacks.Add(new Attack(1001, "Hibernate", "Hibernate", "Puts self to sleep, speeding up digestion and advancing digestion by one turn.", 0f, 0f, 0, 0, 100f, "Self", "", Attack.Type.VOther, Attack.Visibility.Visible, delegate()
		{
			id = 1001;
			AttackList.StandardSetup(1001);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Success Rate: 100%";
			if (AttackList.current.isSafeContainer())
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.PreviewFullDigestion(),
					" → ",
					AttackList.current.PreviewFullDigestion() - 1
				});
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.containPeriod,
					" → ",
					AttackList.current.containPeriod - 1f
				});
			}
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(false);
				AttackList.current.skills.Add(99);
				AttackList.current.skills.Add(200);
				StaticRef.turnController.moveComplete = true;
				StaticRef.turnController.current.ForceDigestion();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(1001);
			return AttackList.current.StateIsContainer();
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(1002, "Somnibalism", "Somnibalism", "Commands a sleeping unit within range to devour one of her allies.", 0f, 20f, 1, 8, 100f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1002;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Take Control";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1002);
			return AttackList.current.en >= 20f;
		}, (UnitInfo u) => u.TotalStomach() < u.MaxStomach() && u.skills.Contains(200)));
		AttackList.attacks.Add(new Attack(1003, "Dream Eater", "DreamEater", "Commands a sleeping unit within range to feed herself to the user. If she is too full to be vored, she will first transfer over her prey, and then, if possible, feed herself to the user as well.", 0f, 20f, 1, 3, 100f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1003;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = AttackList.GetByID(id).name;
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Take Control";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1003);
			return AttackList.current.en >= 20f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(1003);
			return u.skills.Contains(200) && u.CanFeedAll();
		}));
		AttackList.attacks.Add(new Attack(1141, "Nurse", "Nurse", "Feeds a friendly unit milk, restoring their energy completely. Cannot be used on sleeping or incapacitated units.", 0f, 40f, 1, 1, 100f, "Single", "", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1141;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Nurse";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Energy Restore: Full";
			
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1141);
			return AttackList.current.en >= 40f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(1141);
			return u.team == AttackList.current.team;
			return !u.skills.Contains(99);
		}));
		AttackList.attacks.Add(new Attack(1142, "Massage", "Massage", "Chateau provides a massage for a friendly target currently digesting units, reducing their digestion period by 1 turn. Cannot be used on targets that are currently safely containing units.", 0f, 10f, 1, 1, 100f, "Single", "", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1142;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Massage";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Turns to digest: ",
					AttackList.target.containPeriod,
					" → ",
					AttackList.target.containPeriod - 1f
			});
			
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1142);
			return AttackList.current.en >= 10f;
			return AttackList.target.hp >= 0f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(1142);
			return u.TotalStomach() > 0 && u.team == AttackList.current.team;
			return !u.skills.Contains(120);
			return !u.skills.Contains(140);
		}));
		AttackList.attacks.Add(new Attack(1143, "Bite", "Bite", "Deals high damage to a single target with a low success rate. Inflicts Injured, reducing target's SPD, VRES, DEF, and SKL by 20% and halving movement range for 1 turn.", 1.5f, 20f, 1, 1, 70f, "Single", "Generic", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1143;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Bite";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: " + StaticRef.CalcHit() + "%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
			{
				"Damage: ",
				StaticRef.CalcMinDamage(),
				" - ",
				StaticRef.CalcMaxDamage()
			});
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(1143);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1143);
			return AttackList.current.en >= 20f;
		}, (UnitInfo u) => true));
				AttackList.attacks.Add(new Attack(1144, "Kiss", "PlayerKiss", "Transfers the contents of the current unit's stomach to the target.", 0f, 0f, 1, 1, 100f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Visible, delegate()
		{
			id = 1144;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Kiss";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Feed Everything";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(1144);
			return true;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(1144);
			return AttackList.current.TotalStomach() > 0 && u.TotalStomach() < u.MaxStomach() + 1;
		}));
		AttackList.attacks.Add(new Attack(1145, "Absorb", "Absorb", "Instantly digests the contents of the current unit's stomach.", 0f, 0f, 0, 0, 100f, "Self", "", Attack.Type.VOther, Attack.Visibility.Visible, delegate()
		{
			AttackList.StandardSetup(1145);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Absorb";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Success Rate: 100%";
			if (AttackList.current.isSafeContainer())
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.PreviewFullDigestion(),
					" → ",
					0f
				});
			}
			else
			{
				StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = string.Concat(new object[]
				{
					"Turns to digest: ",
					AttackList.current.containPeriod,
					" → ",
					0f
				});
			}
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(false);
				AttackList.current.containPeriod = 1f;
				StaticRef.turnController.current.ForceDigestion();
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(1145);
			return AttackList.current.StateIsContainer();
		}, (UnitInfo u) => true));
		AttackList.attacks.Add(new Attack(1146, "Beautify", "Beautify", "Lamia uses her keen sense of fashion to glamorize a teammate, giving them a massive boost to their confidence which grants a 20% stat boost for 1 turn. Does not stack and cannot be used on incapacitated units.", 0f, 10f, 1, 1, 100f, "Single", "", Attack.Type.Attack, Attack.Visibility.Visible, delegate()
		{
			id = 1146;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Beautify";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Stat Boost: 20%";
			
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				AttackList.current.en -= AttackList.attack.enCost;
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, () => StaticRef.CalcDamage(), delegate()
		{
			AttackList.StandardSetup(1146);
			return AttackList.current.en >= 10f;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(1146);
			return u.team == AttackList.current.team;
			return !u.skills.Contains(99);
		}));
		AttackList.attacks.Add(new Attack(1004, "Kiss", "KissDream", "0", 0f, 0f, 1, 1, 100f, "Single", "Generic", Attack.Type.VAttack, Attack.Visibility.Invisible, delegate()
		{
			id = 1004;
			AttackList.StandardSetup(id);
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[0].text = "Kiss";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[1].text = "Hit: 100%";
			StaticRef.confirm.GetComponentsInChildren<TMP_Text>()[2].text = "Feed Everything";
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
			StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate()
			{
				StaticRef.confirm.transform.localScale = new Vector3(0f, 0f, 0f);
				StaticRef.turnController.ProcessAttack(true);
				AttackList.eventScene.Setup(AttackList.current, AttackList.target, AttackList.attack);
				AttackList.eventScene.RunAttack(id);
			});
			StaticRef.confirm.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!StaticRef.CurrentUnit().isPlayer)
			{
				StaticRef.Delay(1f, delegate()
				{
					StaticRef.confirm.Find("Yes").GetComponent<Button>().onClick.Invoke();
				});
			}
		}, delegate()
		{
			AttackList.StandardSetup(-10);
			return true;
		}, delegate(UnitInfo u)
		{
			AttackList.StandardSetup(-10);
			return u.TotalStomach() < u.MaxStomach() + 1;
		}));
	}

	public static Attack GetByID(int id)
	{
		return AttackList.attacks.Find((Attack x) => x.refID == id);
	}

	private static void StandardSetup(int x)
	{
		AttackList.eventScene = StaticRef.eventScene;
		AttackList.current = StaticRef.CurrentUnit();
		AttackList.target = StaticRef.TargetUnit();
		AttackList.attack = AttackList.GetByID(x);
	}

	private static string StandardTurnsToDigest()
	{
		if (AttackList.current.skills.Contains(-1000) && AttackList.target.team == 1)
		{
			return "No Digestion";
		}
		if (AttackList.current.skills.Contains(-1001) && AttackList.target.team == 1)
		{
			return string.Concat(new object[]
			{
				"Turns to heal: ",
				(int)StaticRef.CurrentUnit().containPeriod,
				" → ",
				StaticRef.CurrentUnit().PreviewContainment(StaticRef.TargetUnit())
			});
		}
		if (AttackList.current.isSafeContainer())
		{
			return string.Concat(new object[]
			{
				"Turns to digest: ",
				0,
				" → ",
				AttackList.current.PreviewSafeDigestionEnd(AttackList.target)
			});
		}
		return string.Concat(new object[]
		{
			"Turns to digest: ",
			(int)StaticRef.CurrentUnit().containPeriod,
			" → ",
			StaticRef.CurrentUnit().PreviewContainment(StaticRef.TargetUnit())
		});
	}

	public static List<Attack> attacks;

	private static EventScene eventScene;

	private static UnitInfo current;

	private static UnitInfo target;

	private static Attack attack;
}
