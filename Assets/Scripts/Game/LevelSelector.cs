using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;




public class LevelSelector : MonoBehaviour
{

	public List<Button> levels = new List<Button>();
	public Button levelButton;
	public Transform buttonHolder;
	public Button backButton;

	private Button oldBackButton;

	public void PopulateButtons(int numberOfButtons)
	{
		Button[] buttons = buttonHolder.GetComponentsInChildren<Button>(true);

		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons [i] != null)
			{
				DestroyImmediate(buttons [i].gameObject);
			}
		}
		levels.Clear();

		for (int i = 0; i < numberOfButtons; i++)
		{
			Button newLevelButton = Instantiate(levelButton) as Button;
			newLevelButton.transform.SetParent(buttonHolder.transform);
			newLevelButton.transform.localScale = Vector3.one;
			levels.Add(newLevelButton);
			CreateLevelButton(i);
			newLevelButton.transform.localScale = Vector3.one;
		}

		oldBackButton = CreateBackButton();
	}

	public Button CreateBackButton()
	{
		Button newBackButton = Instantiate(backButton) as Button;
		newBackButton.transform.SetParent(buttonHolder.transform);
		newBackButton.transform.localScale = Vector3.one;

		newBackButton.onClick.AddListener(() =>
		{
			ButtonController.Instance.BackMenu();
		});

		newBackButton.gameObject.name = "Back Button";
		return newBackButton;
	}

	public void CreateLevelButton(int level)
	{
		levels [level].onClick.AddListener(() =>
		{
			ButtonController.Instance.LoadLevel(level);
		});
		levels [level].onClick.AddListener(() =>
		{
			MenuManager.Instance.CloseMenu(gameObject);
		});
		levels [level].gameObject.name = "Level " + (level + 1) + " Button";

		levels [level].GetComponentInChildren<Text>(true).text = "Level " + (level + 1);
	}

	public void ShowUnlockedLevels()
	{
		for (int i = GameController.Instance.playerSave.latestLevel + 1; i < levels.Count; i++)
		{
			levels [i].interactable = false;
		}
	}

}
