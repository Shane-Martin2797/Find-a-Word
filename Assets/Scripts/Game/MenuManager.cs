using UnityEngine;
using System.Collections;

public class MenuManager : SingletonBehaviour<MenuManager>
{
	GameObject currentMenu;
	GameObject previousMenu = null;

	public void OpenMenu(GameObject menu)
	{
		if (previousMenu != null)
		{
			if (menu == previousMenu)
			{
				BackMenu();
				return;
			}
		}

		UpdateMenus(menu);
	}

	public void CloseCurrentMenu()
	{
		if (currentMenu != null)
		{
			currentMenu.SetActive(false);
		}
	}

	public void CloseMenu(GameObject menu)
	{
		if (menu != null)
		{
			menu.SetActive(false);
		}
	}


	public void UpdateMenus(GameObject menu)
	{
		if (currentMenu != menu)
		{
			previousMenu = currentMenu;
			if (previousMenu != null)
			{
				previousMenu.SetActive(false);
			}

			currentMenu = menu;
			if (currentMenu != null)
			{
				currentMenu.SetActive(true);
			}
		}
	}

	public void BackMenu()
	{
		if (previousMenu == currentMenu)
		{
			return;
		}

		GameObject cur = currentMenu;

		previousMenu.SetActive(true);
		currentMenu.SetActive(false);

		currentMenu = previousMenu;
		previousMenu = cur;
	}

	public void ClearMenus()
	{
		currentMenu.SetActive(false);
		previousMenu.SetActive(false);
		currentMenu = null;
		previousMenu = null;
	}

}
