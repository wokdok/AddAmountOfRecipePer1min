using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AddAmountOfRecipePer1min
{

    [HarmonyPatch(typeof(UIItemTip))]
	[HarmonyPatch("SetTip")]
	static class Patch_UIItemTip_SetTip
	{
		static void Prefix(UIItemTip __instance, int __0, int __1, Vector2 __2, Transform __3)
		{
			ref UIRecipeEntry[] recipeEntryArr = ref AccessTools.FieldRefAccess<UIItemTip, UIRecipeEntry[]>(__instance, "recipeEntryArr");
			
			if (recipeEntryArr != null)
			{
				for (int k = 0; k < recipeEntryArr.Length; k++)
				{
					if (recipeEntryArr[k] != null)
					{
						recipeEntryArr[k].gameObject.SetActive(false);
					}
				}
			}

		}
		static void Postfix(UIItemTip __instance, int __0, int __1, Vector2 __2, Transform __3)
        {
			int itemId = __0;
			
			int num;
			int id;

			if (itemId > 0)
			{
				num = itemId;
				id = 0;
			}
			else if (itemId < 0)
			{
				num = 0;
				id = -itemId;
			}
			else
			{
				num = 0;
				id = 0;
			}

			ItemProto itemProto = LDB.items.Select(num);
			RecipeProto recipeProto = LDB.recipes.Select(id);
			List<RecipeProto>  tmp_recipeList = new List<RecipeProto>();
			if (recipeProto != null)
			{
				tmp_recipeList.Add(null);
				tmp_recipeList[0] = recipeProto;
			}
			List<RecipeProto> list = (itemProto != null) ? itemProto.recipes : ((recipeProto != null) ? tmp_recipeList : null);
			ref UIRecipeEntry recipeEntry = ref AccessTools.FieldRefAccess<UIItemTip, UIRecipeEntry>(__instance, "recipeEntry");
			ref UIRecipeEntry[] recipeEntryArr = ref AccessTools.FieldRefAccess<UIItemTip, UIRecipeEntry[]>(__instance, "recipeEntryArr");
			Array.Resize(ref recipeEntryArr, 64);

			if (list != null && list.Count > 0)
			{
				float recipeMaxWidth = 0;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Type == ERecipeType.Fractionate)
					{
						continue;
					}

					float recipeWidth = ((list[j].Results.Length + list[j].Items.Length) * 40 + 40) + 20;
					if(recipeWidth > recipeMaxWidth)
                    {
						recipeMaxWidth = recipeWidth;
                    }
				}


				for (int j = 0; j < list.Count; j++)
				{

					if (recipeEntryArr[j + 32] == null)
					{
						recipeEntryArr[j + 32] = UnityEngine.Object.Instantiate<UIRecipeEntry>(recipeEntry, __instance.transform);
					}

					if (list[j].Type == ERecipeType.Fractionate)
					{
						recipeEntryArr[j + 32].gameObject.SetActive(false);
						continue;
					}
					setRecipe1Min(recipeEntryArr[j + 32], list[j]);
					recipeEntryArr[j + 32].rectTrans.anchoredPosition = new Vector2((float)(recipeEntryArr[j].rectTrans.anchoredPosition.x + recipeMaxWidth), recipeEntryArr[j].rectTrans.anchoredPosition.y);
					recipeEntryArr[j + 32].gameObject.SetActive(true);
				}
				ref RectTransform trans = ref AccessTools.FieldRefAccess<UIItemTip, RectTransform>(__instance, "trans");
				if (trans.sizeDelta.x < recipeMaxWidth * 2)
                {
					trans.sizeDelta = new Vector2((float)(recipeMaxWidth * 2), (float)trans.sizeDelta.y);
					trans.SetParent(UIRoot.instance.itemTipTransform, true);
					Rect rect = UIRoot.instance.itemTipTransform.rect;
					float num22 = (float)Mathf.RoundToInt(rect.width);
					float num23 = (float)Mathf.RoundToInt(rect.height);
					float num24 = trans.anchorMin.x * num22 + trans.anchoredPosition.x;
					float num25 = trans.anchorMin.y * num23 + trans.anchoredPosition.y;
					Rect rect2 = trans.rect;
					rect2.x += num24;
					rect2.y += num25;
					Vector2 zero = Vector2.zero;
					if (rect2.xMin < 0f)
					{
						zero.x -= rect2.xMin;
					}
					if (rect2.yMin < 0f)
					{
						zero.y -= rect2.yMin;
					}
					if (rect2.xMax > num22)
					{
						zero.x -= rect2.xMax - num22;
					}
					if (rect2.yMax > num23)
					{
						zero.y -= rect2.yMax - num23;
					}
					trans.anchoredPosition = trans.anchoredPosition + zero;
					trans.anchoredPosition = new Vector2((float)((int)trans.anchoredPosition.x), (float)((int)trans.anchoredPosition.y));
					trans.localScale = new Vector3(1f, 1f, 1f);
				}
				
			}
		}

		static void setRecipe1Min(UIRecipeEntry uiRecipeEntry, RecipeProto recipeProto)
		{

			int num = 0;
			int num2 = 0;
			int num3 = 0;
			double buildSpeed = 1.0;
			if(uiRecipeEntry.timeText.rectTransform.sizeDelta.y > 37){
				uiRecipeEntry.timeText.rectTransform.sizeDelta = new Vector2(uiRecipeEntry.timeText.rectTransform.sizeDelta.x, 37);
			}
			uiRecipeEntry.timeText.text = "1min";
			if (recipeProto.Type == ERecipeType.Assemble)
            {
				uiRecipeEntry.timeText.rectTransform.sizeDelta = new Vector2(uiRecipeEntry.timeText.rectTransform.sizeDelta.x, uiRecipeEntry.timeText.rectTransform.sizeDelta.y + 30);

				if (GameMain.data.history.TechUnlocked(1203))
				{
					buildSpeed = 1.5;
					uiRecipeEntry.timeText.text = "1min\n(mk3)";
					uiRecipeEntry.timeText.lineSpacing = (float)0.7;

				} else if (GameMain.data.history.TechUnlocked(1202))
				{
					uiRecipeEntry.timeText.text = "1min\n(mk2)";
				} else
                {
					buildSpeed = 0.75;
					uiRecipeEntry.timeText.text = "1min\n(mk1)";
				}
			}

			while (num3 < recipeProto.Results.Length && num < 7)
			{
				ItemProto itemProto = LDB.items.Select(recipeProto.Results[num3]);
				if (itemProto != null)
				{
					uiRecipeEntry.icons[num].sprite = itemProto.iconSprite;
				}
				else
				{
					uiRecipeEntry.icons[num].sprite = null;
				}
				uiRecipeEntry.countTexts[num].text = (60f / (float)recipeProto.TimeSpend * 60f * recipeProto.ResultCounts[num3] * buildSpeed).ToString();
				uiRecipeEntry.icons[num].rectTransform.anchoredPosition = new Vector2((float)num2, 0f);
				uiRecipeEntry.icons[num].gameObject.SetActive(true);
				num++;
				num2 += 40;
				num3++;
			}
			uiRecipeEntry.arrow.anchoredPosition = new Vector2((float)num2, -27f);
			num2 += 40;
			int num4 = 0;
			while (num4 < recipeProto.Items.Length && num < 7)
			{
				ItemProto itemProto2 = LDB.items.Select(recipeProto.Items[num4]);
				if (itemProto2 != null)
				{
					uiRecipeEntry.icons[num].sprite = itemProto2.iconSprite;
				}
				else
				{
					uiRecipeEntry.icons[num].sprite = null;
				}
				uiRecipeEntry.countTexts[num].text = (60f / (float)recipeProto.TimeSpend * 60f * recipeProto.ItemCounts[num4] * buildSpeed).ToString();

				uiRecipeEntry.icons[num].rectTransform.anchoredPosition = new Vector2((float)num2, 0f);
				uiRecipeEntry.icons[num].gameObject.SetActive(true);
				num++;
				num2 += 40;
				num4++;
			}
			for (int i = num; i < 7; i++)
			{
				uiRecipeEntry.icons[i].gameObject.SetActive(false);
			}
		}
	}

	[HarmonyPatch(typeof(UIItemTip))]
	[HarmonyPatch("OnDisable")]
	static class Patch_UIItemTip_OnDisable
	{
		static void Postfix(UIItemTip __instance)
		{
			ref UIRecipeEntry[] recipeEntryArr = ref AccessTools.FieldRefAccess<UIItemTip, UIRecipeEntry[]>(__instance, "recipeEntryArr");

			if (recipeEntryArr != null)
			{
				for (int k = 0; k < recipeEntryArr.Length; k++)
				{
					if (recipeEntryArr[k] != null)
					{
						recipeEntryArr[k].gameObject.SetActive(false);
					}
				}
			}
		}
	}
}