using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Prescription : System.Object
{
    public string description;
    public string startDate;
    public string endDate;
    public List<GameSetting> linkedGameSettings;
    public List<string> gameSettings;
    public bool isActive;
    public string notes;
    public float minActive;
    public float maxActive;

    public string GetDescription()
    {
        return description;
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }

    public string GetStartDate()
    {
        return startDate;
    }

    public void SetStartDate(string startDate)
    {
        this.startDate = startDate;
    }

    public string GetEndDate()
    {
        return endDate;
    }

    public void SetEndDate(string endDate)
    {
        this.endDate = endDate;
    }

    public List<string> GetGameSettings()
    {
        return gameSettings;
    }

    public void SetGameSettings(List<string> gameSettings)
    {
        this.gameSettings = gameSettings;
    }

    public List<GameSetting> GetLinkedGameSettings()
    {
        return this.linkedGameSettings;
    }

    public void setLinkedGameSettings(List<GameSetting> linkedGameSettings)
    {
        this.linkedGameSettings = linkedGameSettings;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;
    }

    public string GetNotes()
    {
        return notes;
    }

    public void SetNotes(string notes)
    {
        this.notes = notes;
    }

    public float GetMinActive()
    {
        return minActive;
    }

    public void SetMinActive(float minActive)
    {
        this.minActive = minActive;
    }

    public float GetMaxActive()
    {
        return maxActive;
    }

    public void SetMaxActive(float maxActive)
    {
        this.maxActive = maxActive;
    }

    public override string ToString()
    {
        return "Prescription{" +
                "description='" + description + '\'' +
                ", startDate='" + startDate + '\'' +
                ", endDate='" + endDate + '\'' +
                ", gameSettings=" + linkedGameSettings +
                ", isActive=" + isActive +
                ", notes='" + notes + '\'' +
                ", minActive=" + minActive +
                ", maxActive=" + maxActive +
                '}';
    }

}