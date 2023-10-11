using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadController : MonoBehaviour
{
	private GameObject unitButton;

	public GameObject[] sqButtons = new GameObject[10];

	public UnitStorage[] squad1 = new UnitStorage[10];

	public ActionStack undo;

	private int selectedIndex;

	private int cost;

	private int maxCost = 999;

	private Dictionary<Guid, GameObject> buttons = new Dictionary<Guid, GameObject>();

	private List<UnitStorage> units = new List<UnitStorage>();

	private Transform parent;

	private void Start()
	{
		unitButton = Resources.Load<GameObject>("Prefab/UnitButton");
		parent = base.transform.Find("UnitSelect").Find("Mask").Find("Container");
		CreateUnits();
		if ((bool)GameObject.FindGameObjectWithTag("EternalCanvas"))
		{
			List<UnitStorage> list = new List<UnitStorage>();
			list.AddRange(GameObject.FindGameObjectWithTag("EternalCanvas").GetComponent<ChangeScene>().units);
			for (int i = 0; i < list.Count; i++)
			{
				selectedIndex = i;
				AddUnitLight(list.ToArray()[i]);
			}
		}
	}

	public void CreateUnits()
	{
		Transform[] array = base.transform.Find("UnitSelect").Find("Mask").Find("Container")
			.Cast<Transform>()
			.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
		units.Clear();
		buttons.Clear();
		float num = -700f;
		float num2 = 310f;
		float num3 = 0f;
		parent.localPosition = new Vector2(0f, 0f);
		List<UnitStorage> list = StaticRef.data.SearchActive((UnitStorage u) => u.squad == 0 && u.clas.rarity < 11 && cost + UnitCost(u) <= maxCost);
		list = StaticRef.StandardUnitSort(list);
		CreateNullSquare(num, num2);
		num += 200f;
		num3 += 1f;
		foreach (UnitStorage u2 in list)
		{
			Sprite sprite = SpriteLoad.LoadSmall(u2);
			GameObject gameObject = UnityEngine.Object.Instantiate(unitButton, parent);
			gameObject.name = u2.unitName;
			gameObject.transform.localPosition = new Vector3(num, num2, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/Background" + u2.GetClass().rarity);
			gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.99f;
			gameObject.GetComponent<UnitButton>().unit = u2;
			gameObject.GetComponent<UnitButton>().weight = u2.weight;
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				AddUnit(u2);
			});
			RectTransform component = gameObject.transform.Find("Mask").Find("Portrait").GetComponent<RectTransform>();
			component.pivot = StaticRef.SpritePivot(u2.classID);
			StaticRef.SpriteSize(component, u2.classID);
			component.localPosition = new Vector3(0f, 30.5f, 0f);
			component.GetComponent<Image>().sprite = sprite;
			if (u2.GetClass().role == 0)
			{
				gameObject.transform.Find("Class").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/ClassPred");
			}
			if (u2.GetClass().role == 1)
			{
				gameObject.transform.Find("Class").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/ClassFood");
			}
			if (u2.squad > 0)
			{
				gameObject.transform.Find("Squad").gameObject.SetActive(value: true);
			}
			else
			{
				gameObject.transform.Find("Squad").gameObject.SetActive(value: false);
			}
			if (u2.locked)
			{
				gameObject.transform.Find("Lock").gameObject.SetActive(value: true);
			}
			else
			{
				gameObject.transform.Find("Lock").gameObject.SetActive(value: false);
			}
			gameObject.transform.Find("Overlay").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/Overlay" + u2.GetClass().rarity);
			gameObject.transform.Find("Stomach").GetComponent<TMP_Text>().text = u2.cCap + u2.hCap + " / " + u2.MaxStomach();
			gameObject.transform.Find("Level").GetComponent<TMP_Text>().text = u2.GetClass().level.ToString() ?? "";
			gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = u2.unitName;
			buttons.Add(u2.refID, gameObject);
			if (u2.isSecretary)
			{
				gameObject.GetComponent<UnitButton>().sec.SetActive(value: true);
			}
			if (num3 >= 7f)
			{
				num = -700f;
				num2 -= 300f;
				num3 = 0f;
			}
			else
			{
				num += 200f;
				num3 += 1f;
			}
		}
		units.AddRange(list);
		base.transform.Find("DragBox").GetComponent<DragAndSwipe>().SetMaxY(StaticRef.data.ReturnActive().Count + 1, 8, 300f, -600f);
	}

	public void ShowUnits()
	{
		for (int i = 0; i < 10; i++)
		{
			List<RectTransform> list = new List<RectTransform>();
			list.AddRange(sqButtons[i].GetComponentsInChildren<RectTransform>());
			list.Reverse();
			list.Remove(sqButtons[i].GetComponent<RectTransform>());
			foreach (RectTransform item in list)
			{
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
			if (squad1[i] != null)
			{
				UnitStorage unitStorage = squad1[i];
				Sprite sprite = SpriteLoad.LoadSmall(unitStorage);
				GameObject gameObject = UnityEngine.Object.Instantiate(unitButton, sqButtons[i].transform);
				gameObject.name = unitStorage.unitName;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				while (gameObject.transform.childCount > 0)
				{
					Transform child = gameObject.transform.GetChild(0);
					child.parent = sqButtons[i].transform;
					child.localScale = Vector3.one;
				}
				UnityEngine.Object.Destroy(gameObject);
				gameObject = sqButtons[i];
				gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/Background" + unitStorage.GetClass().rarity);
				gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.99f;
				RectTransform component = gameObject.transform.Find("Mask").Find("Portrait").GetComponent<RectTransform>();
				component.pivot = StaticRef.SpritePivot(unitStorage.classID);
				StaticRef.SpriteSize(component, unitStorage.classID);
				component.localPosition = new Vector3(0f, 30.5f, 0f);
				component.GetComponent<Image>().sprite = sprite;
				if (unitStorage.GetClass().role == 0)
				{
					gameObject.transform.Find("Class").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/ClassPred");
				}
				if (unitStorage.GetClass().role == 1)
				{
					gameObject.transform.Find("Class").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/ClassFood");
				}
				if (unitStorage.squad > 0)
				{
					gameObject.transform.Find("Squad").gameObject.SetActive(value: true);
				}
				else
				{
					gameObject.transform.Find("Squad").gameObject.SetActive(value: false);
				}
				if (unitStorage.locked)
				{
					gameObject.transform.Find("Lock").gameObject.SetActive(value: true);
				}
				else
				{
					gameObject.transform.Find("Lock").gameObject.SetActive(value: false);
				}
				gameObject.transform.Find("Overlay").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/Overlay" + unitStorage.GetClass().rarity);
				gameObject.transform.Find("Stomach").GetComponent<TMP_Text>().text = unitStorage.cCap + unitStorage.hCap + " / " + unitStorage.MaxStomach();
				gameObject.transform.Find("Level").GetComponent<TMP_Text>().text = unitStorage.GetClass().level.ToString() ?? "";
				gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = unitStorage.unitName;
				UnityEngine.Object.Destroy(gameObject.transform.Find("Secretary").gameObject);
				UnityEngine.Object.Destroy(gameObject.transform.Find("Number").gameObject);
			}
			else
			{
				sqButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/AddUnit");
			}
		}
	}

	public void ShowUnitSelect(int x)
	{
		StaticRef.sound.MenuConfirm();
		selectedIndex = x;
		if (squad1[selectedIndex] != null)
		{
			cost -= UnitCost(squad1[selectedIndex]);
		}
		CreateUnits();
		base.transform.Find("SquadSelect").GetComponent<UIFade>().FadeOut();
		base.transform.Find("UnitSelect").GetComponent<UIFade>().FadeIn();
		undo.Push(delegate
		{
			HideUnitSelect();
		});
	}

	public void HideUnitSelect()
	{
		StaticRef.sound.MenuCancel();
		if (squad1[selectedIndex] != null)
		{
			cost += UnitCost(squad1[selectedIndex]);
		}
		base.transform.Find("UnitSelect").GetComponent<UIFade>().FadeOut();
		base.transform.Find("SquadSelect").GetComponent<UIFade>().FadeIn();
	}

	public void AddUnit(UnitStorage u)
	{
		StaticRef.sound.MenuConfirm();
		if (squad1[selectedIndex] != null)
		{
			squad1[selectedIndex].squad = 0;
		}
		squad1[selectedIndex] = u;
		if (u != null)
		{
			u.squad = 1;
		}
		cost += UnitCost(u);
		UpdateCost();
		ShowUnits();
		base.transform.Find("UnitSelect").GetComponent<UIFade>().FadeOut();
		base.transform.Find("SquadSelect").GetComponent<UIFade>().FadeIn();
		undo.Pop();
	}

	public void AddUnitLight(UnitStorage u)
	{
		if (squad1[selectedIndex] != null)
		{
			squad1[selectedIndex].squad = 0;
		}
		squad1[selectedIndex] = u;
		if (u != null)
		{
			u.squad = 1;
		}
		cost += UnitCost(u);
		UpdateCost();
		ShowUnits();
	}

	public void Embark(string s)
	{
		if (cost <= 0)
		{
			StaticRef.sound.MenuCancel();
			return;
		}
		StaticRef.sound.MenuConfirm();
		GameObject.FindGameObjectWithTag("EternalCanvas").GetComponent<ChangeScene>().units.Clear();
		GameObject.FindGameObjectWithTag("EternalCanvas").GetComponent<ChangeScene>().units.AddRange(squad1);
		StaticRef.menuController.LoadScene(s);
	}

	private void UpdateCost()
	{
		base.transform.Find("SquadSelect").Find("Cost").GetComponent<TMP_Text>()
			.text = "Cost: " + cost + "/" + maxCost;
	}

	private int UnitCost(UnitStorage u)
	{
		if (u == null)
		{
			return 0;
		}
		return u.clas.rarity switch
		{
			3 => 10, 
			4 => 20, 
			5 => 30,
			10 => 50,
			_ => 0, 
		};
	}

	private void CreateNullSquare(float x, float y)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(unitButton, parent);
		gameObject.name = "Null";
		gameObject.transform.localPosition = new Vector3(x, y, 0f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
		gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CWindow/AddUnit");
		gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.99f;
		gameObject.GetComponent<Button>().onClick.AddListener(delegate
		{
			AddUnit(null);
		});
		List<RectTransform> list = new List<RectTransform>();
		list.AddRange(gameObject.GetComponentsInChildren<RectTransform>());
		list.Reverse();
		list.Remove(gameObject.GetComponent<RectTransform>());
		foreach (RectTransform item in list)
		{
			UnityEngine.Object.DestroyImmediate(item.gameObject);
		}
	}
}
