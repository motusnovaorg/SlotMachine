using UnityEngine;
using System.Collections;

[System.Serializable]
public class Game : System.Object
{
	public string gameSettingUUID;

	public string name;

	// GameConstants Description or Name for Unity games.
	public string description;

	// AROM for the game.
	public float minAngle;
	public float maxAngle;

	// Assist for game
	public float assistPressure;
	public float assistPressureHysteresis;

	// Timers for game
	public int restTime;
	public int activeTime;

	// # of cycles for patient to complete
	public int cycles;

	// Does this game work for users with a low Range of Motion?
	public bool isLowROMGame;

    public int priority;
    public string scene;

    //Empty constructor.  I'm not sure if we'll need one.
	public Game() { }

    //Less empty constructor.
	public Game(string gameSettingUUID,
				string name, float minAngle, float maxAngle,
				float assistPressure, float assistPressureHysteresis,
				int restTime, int activeTime, int cycles)
	{
		this.gameSettingUUID = gameSettingUUID;
		this.name = name;
		this.description = null;
		this.minAngle = minAngle;
		this.maxAngle = maxAngle;
		this.assistPressure = assistPressure;
		this.assistPressureHysteresis = assistPressureHysteresis;
		this.restTime = restTime;
		this.activeTime = activeTime;
		this.cycles = cycles;
		this.isLowROMGame = false;
	}

	public string GetGameSettingUUID()
	{
		return gameSettingUUID;
	}

	public void SetGameSettingUUID(string gameSettingUUID)
	{
		this.gameSettingUUID = gameSettingUUID;
	}

	public string GetName()
	{
		return name;
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public float GetMinAngle()
	{
		return minAngle;
	}

	public float GetMaxAngle()
	{
		return maxAngle;
	}

	public void SetMinAngle(float minAngle)
	{
		this.minAngle = minAngle;
	}

	public void SetMaxAngle(float maxAngle)
	{
		this.maxAngle = maxAngle;
	}

	public float GetAssistPressure()
	{
		return assistPressure;
	}

	public void SetAssistPressure(float assistPressure)
	{
		this.assistPressure = assistPressure;
	}

	public float GetAssistPressureHysteresis()
	{
		return assistPressureHysteresis;
	}

	public void SetAssistPressureHysteresis(float assistPressureHysteresis)
	{
		this.assistPressureHysteresis = assistPressureHysteresis;
	}

	public int GetRestTime()
	{
		return restTime;
	}

	public void SetRestTime(int restTime)
	{
		this.restTime = restTime;
	}

	public int GetActiveTime()
	{
		return activeTime;
	}

	public void SetActiveTime(int activeTime)
	{
		this.activeTime = activeTime;
	}

	public int GetCycles()
	{
		return cycles;
	}

	public void SetCycles(int cycles)
	{
		this.cycles = cycles;
	}

	public string GetDescription()
	{
		return description;
	}

	public void SetDescription(string description)
	{
		this.description = description;
	}

	public bool IsLowROMGame()
	{
		return isLowROMGame;
	}

	public void SetLowROMGame(bool lowROMGame)
	{
		this.isLowROMGame = lowROMGame;
	}

    public void SetPriority(int priority)
    {
        this.priority = priority;
    }
    public int GetPriority(int priority)
    {
        return this.priority;
    }
    public void SetScene(string scene)
    {
        this.scene = scene;
    }
    public string GetScene()
    {
        return this.scene;
    }

    public override string ToString()
    {
        string outputString = "";
        outputString = outputString + "Active time: " + this.activeTime + "\n";
        outputString = outputString + "Cycles: " + this.cycles + "\n";
        outputString = outputString + "Description: " + this.description + "\n";
        outputString = outputString + "Game Name: " + this.name + "\n";
        outputString = outputString + "Rest time: " + this.restTime + "\n";
        outputString = outputString + "Priority: " + this.priority + "\n";
        outputString = outputString + "Scene: " + this.scene;
        return outputString;
    }
}
