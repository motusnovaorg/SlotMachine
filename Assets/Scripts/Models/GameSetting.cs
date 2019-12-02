using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSetting : System.Object
{
    public int activeTime;
    public int cycles;
    public string description;
    public string gameId;
    public int maxOffset;
    public int minOffset;
    public int restTime;
    public string uuid;

    public int GetActiveTime() {
        return activeTime;
    }

    public void SetActiveTime(int activeTime) {
        this.activeTime = activeTime;
    }

    public int GetCycles() {
        return cycles;
    }

    public void SetCycles(int cycles) {
        this.cycles = cycles;
    }

    public string GetDescription() {
        return description;
    }

    public void SetDescription(string description) {
        this.description = description;
    }

    public string GetGameId() {
        return gameId;
    }

    public void SetGameId(string gameId) {
        this.gameId = gameId;
    }

    public int GetMaxOffset() {
        return maxOffset;
    }

    public void SetMaxOffset(int maxOffset) {
        this.maxOffset = maxOffset;
    }

    public int GetMinOffset() {
        return minOffset;
    }

    public void SetMinOffset(int minOffset) {
        this.minOffset = minOffset;
    }

    public int GetRestTime() {
        return restTime;
    }

    public void SetRestTime(int restTime) {
        this.restTime = restTime;
    }

    public string GetUuid() {
        return uuid;
    }

    public void SetUuid(string uuid) {
        this.uuid = uuid;
    }

    public override string ToString()
    {
        string outputString = "";
        outputString = outputString + "Active time: " + this.activeTime + "\n";
        outputString = outputString + "Cycles: " + this.cycles + "\n";
        outputString = outputString + "Description: " + this.description + "\n";
        outputString = outputString + "Game ID: " + this.gameId + "\n";
        outputString = outputString + "Rest time: " + this.restTime + "\n";
        return outputString;
    }
}
