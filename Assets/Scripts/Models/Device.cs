using UnityEngine;
using System.Collections;

[System.Serializable]
public class Device : System.Object
{

	public string createDate;
	public bool deployed;
	public string lastActiveDate;
	public string notes;
	public string serial;
	public Patient patient;

    public Device() { }

public Device(string createDate, bool deployed, string lastActiveDate, string notes, string serial, Patient patient)
	{
		this.createDate = createDate;
		this.deployed = deployed;
		this.lastActiveDate = lastActiveDate;
		this.notes = notes;
		this.serial = serial;
		this.patient = patient;
	}

	public string GetCreateDate()
	{
		return createDate;
	}

	public void SetCreateDate(string createDate)
	{
		this.createDate = createDate;
	}

	public bool GetDeployed()
	{
		return deployed;
	}

	public void SetDeployed(bool deployed)
	{
		this.deployed = deployed;
	}

	public string GetLastActiveDate()
	{
		return lastActiveDate;
	}

	public void SetLastActiveDate(string lastActiveDate)
	{
		this.lastActiveDate = lastActiveDate;
	}

	public string GetNotes()
	{
		return notes;
	}

	public void SetNotes(string notes)
	{
		this.notes = notes;
	}

	public string GetSerial()
	{
		return serial;
	}

    public void SetSerial(string serial)
    {
        this.serial = serial;
    }

	public Patient getPatient()
	{
		return this.patient;
	}

	public void setPatient(Patient patient)
	{
		this.patient = patient;
	}

}
