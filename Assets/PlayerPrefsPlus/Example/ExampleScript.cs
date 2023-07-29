using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerPrefsPlus;


public class ExampleScript : MonoBehaviour
{
    public InputField Text;
    public InputField Password;
    public InputField DeText;
    public InputField DePassword;

    void Start()
    {
        print("Welcome To The PlayerPrefsPlus Demo. Thank You For Your Purchase!");
    }


    public void Encrypt()
    {
        var encrypted = PPPlus.EncryptString(Text.text, Password.text);
        DeText.text = encrypted;
        Debug.Log("Succesfully Encrypted \"" + Text.text + "\" Into: " + encrypted);
    }

    public void Decrypt()
    {
        var decrypted = PPPlus.DecryptString(DeText.text, DePassword.text);
        Text.text = decrypted;
        Debug.Log("Succesfully Decrypted \"" + DeText.text + "\" Into: " + decrypted);
    }
}