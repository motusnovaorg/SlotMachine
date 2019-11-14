using UnityEngine;
using System.Collections;

[System.Serializable]
public class Update : System.Object
{
	public string[] gameList;

	public string[] GetGameList()
	{
		return this.gameList;
	}
	public void SetGameList(string[] gameList)
	{
		this.gameList = gameList;
	}
}