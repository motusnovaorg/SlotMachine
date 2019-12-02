using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskPassword : MonoBehaviour
{
    string[] maskArray = new string[21];
    int maskIndex = 0;

    public InputField passwordInput;
    public Text maskOutput;

    // Start is called before the first frame update
    void Start()
    {
        maskArray[0] = "";
        string mask = "";

        for (int i = 1; i < 21; i++)
        {
            maskArray[i] = mask + "*";
            mask = mask + "*";
        }
    }

    public void PasswordMask()
    {

    }
}
