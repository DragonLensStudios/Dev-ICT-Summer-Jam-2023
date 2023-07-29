using System.Collections.Generic;
using UnityEngine;
using PlayerPrefsPlus;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

//Programmed By SamarthCat
//Sorry For Lack Of Comments
namespace PlayerPrefsPlus
{
    /// <summary>
    /// The Main Class For PlayerPrefsPlus, "PlayerPrefsPlus.PPPlus"
    /// </summary>
    public class PPPlus
    {
        #region Callbacks

        /// <summary>
        /// When a player pref is set this is called.
        /// </summary>
        public static Action<string, object> OnPrefChanged;

        #endregion

        #region Normal PlayerPrefs

        /// <summary>
        /// Stores An Int In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetInt(string Key, int value)
        {
            PlayerPrefs.SetInt(Key, value);
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets An Int From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static int GetInt(string Key)
        {
            return PlayerPrefs.GetInt(Key);
        }

        /// <summary>
        /// Stores A Float In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetFloat(string Key, float value)
        {
            PlayerPrefs.SetFloat(Key, value);
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets A Float From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static float GetFloat(string Key)
        {
            return PlayerPrefs.GetFloat(Key);
        }

        /// <summary>
        /// Stores A String In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetString(string Key, string value)
        {
            PlayerPrefs.SetString(Key, value);
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets A String From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static string GetString(string Key)
        {
            return PlayerPrefs.GetString(Key);
        }

        #endregion

        #region Get And Set Booleans

        /// <summary>
        /// Sets A Boolean Value In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetBool(string Key, bool value)
        {
            var intVal = 0;
            if (value)
            {
                intVal = 1;
            }

            PlayerPrefs.SetInt(Key, intVal);
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets A Boolean Value From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Boolean Value Found In PlayerPrefs</returns>
        public static bool GetBool(string Key)
        {
            var intVal = PlayerPrefs.GetInt(Key);
            if (intVal == 0)
            {
                return false;
            }
            else if (intVal == 1)
            {
                return true;
            }
            else
            {
                Debug.LogError("The Value Returned Was " + intVal +
                               ", Which Cannot Be Converted To A Boolean Value. False Will Be Returned");
                return false;
            }
        }

        #endregion

        #region Get And Set Vectors

        /// <summary>
        /// Sets A Vector2 Value In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetVector2(string Key, Vector2 value)
        {
            PlayerPrefs.SetString(Key, value.x.ToString() + "," + value.y.ToString());
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets A Vector2 Value From PlayerPrefs, Can Also Pull From PlayerPref Keys With Vector3 Values
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Vector2 Value Found In PlayerPrefs</returns>
        public static Vector2 GetVector2(string Key)
        {
            string[] values = PlayerPrefs.GetString(Key).Split(',');
            try
            {
                return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
            }
            catch
            {
                Debug.LogError("The Given Key Contains An Invalid Value Of \"" + PlayerPrefs.GetString(Key) + "\"");
                return Vector2.zero;
            }
        }

        /// <summary>
        /// Sets A Vector3 Value In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Set</param>
        public static void SetVector3(string Key, Vector3 value)
        {
            PlayerPrefs.SetString(Key, value.x.ToString() + "," + value.y.ToString());
            PlayerPrefs.SetFloat(Key + "3", value.z);
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Gets A Vector3 Value From PlayerPrefs, Cannot Pull From PlayerPref Keys With Vector2 Values
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Vector3 Value Found In PlayerPrefs</returns>
        public static Vector3 GetVector3(string Key)
        {
            string[] values = PlayerPrefs.GetString(Key).Split(',');
            try
            {
                return new Vector3(float.Parse(values[0]), float.Parse(values[1]), PlayerPrefs.GetFloat(Key + "3"));
            }
            catch
            {
                Debug.LogError("The Given Key Contains An Invalid Value Of \"" + PlayerPrefs.GetString(Key) + "\"");
                return Vector3.zero;
            }
        }

        #endregion

        #region Get And Set Arrays

        /// <summary>
        /// Stores An Array Of Strings In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="Values">The Values To Set</param>
        public static void SetStringArray(string Key, string[] Values)
        {
            PlayerPrefs.SetInt(Key + "_Array_Length", Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                PlayerPrefs.SetString(Key + "_Array_" + i, Values[i]);
            }
        }

        /// <summary>
        /// Gets An Array Of Strings From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Array Of Strings Found In PlayerPrefs</returns>
        public static string[] GetStringArray(string Key)
        {
            List<string> toReturn = new List<string>();
            for (int i = 0; i < PlayerPrefs.GetInt(Key + "_Array_Length"); i++)
            {
                toReturn.Add(PlayerPrefs.GetString(Key + "_Array_" + i));
            }

            string[] toReturnFinal = toReturn.ToArray();
            return toReturnFinal;
        }

        /// <summary>
        /// Stores An Array Of Integers In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="Values">The Values To Set</param>
        public static void SetIntArray(string Key, int[] Values)
        {
            PlayerPrefs.SetInt(Key + "_Array_Length", Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                PlayerPrefs.SetInt(Key + "_Array_" + i, Values[i]);
            }
        }

        /// <summary>
        /// Gets An Array Of Integers From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Array Of Integers Found In PlayerPrefs</returns>
        public static int[] GetIntArray(string Key)
        {
            List<int> toReturn = new List<int>();
            for (int i = 0; i < PlayerPrefs.GetInt(Key + "_Array_Length"); i++)
            {
                toReturn.Add(PlayerPrefs.GetInt(Key + "_Array_" + i));
            }

            int[] toReturnFinal = toReturn.ToArray();
            return toReturnFinal;
        }

        /// <summary>
        /// Stores An Array Of Floats In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="Values">The Values To Set</param>
        public static void SetFloatArray(string Key, float[] Values)
        {
            PlayerPrefs.SetInt(Key + "_Array_Length", Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                PlayerPrefs.SetFloat(Key + "_Array_" + i, Values[i]);
            }
        }

        /// <summary>
        /// Gets An Array Of Floats From PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <returns>The Array Of Floats Found In PlayerPrefs</returns>
        public static float[] GetFloatArray(string Key)
        {
            List<float> toReturn = new List<float>();
            for (int i = 0; i < PlayerPrefs.GetInt(Key + "_Array_Length"); i++)
            {
                toReturn.Add(PlayerPrefs.GetInt(Key + "_Array_" + i));
            }

            float[] toReturnFinal = toReturn.ToArray();
            return toReturnFinal;
        }

        #endregion

        #region Get And Set Anything

        /// <summary>
        /// Stores Anything In PlayerPrefs(Make Sure It Is Serializable)
        /// </summary>
        /// <typeparam name="T">The Type Of Object</typeparam>
        /// <param name="value">The Variable To Set(MUST BE SERIALIZABLE)</param>
        /// <param name="Key">The Key To Set</param>
        public static void SetAnything<T>(T value, string Key)
        {
            if (value == null)
            {
                return;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Path.Combine(Application.persistentDataPath, Key + ".PPPlus"));
                bf.Serialize(file, value);
                file.Close();
                OnPrefChanged?.Invoke(Key, value);
            }
            catch (Exception ex)
            {
                Debug.LogError("An Unkown Exception Occured: " + ex.Message);
            }
        }


        /// <summary>
        /// Gets Anything From PlayerPrefs(As Long As It Was Stored With The "SetAnything()" Method)
        /// </summary>
        /// <typeparam name="T">The Type Of Object</typeparam>
        /// <param name="Key">The Key To Get</param>
        /// <returns>Anything That Is Serializable</returns>
        public static T GetAnything<T>(string Key)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return default(T);
            }

            T objectOut = default(T);

            try
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath, Key + ".PPPlus")))
                {
                    Debug.LogError("That Key Doesn't Exist, Silly");
                    return default(T);
                }

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Path.Combine(Application.persistentDataPath, Key + ".PPPlus"),
                    FileMode.Open);
                T saveObj = (T)bf.Deserialize(file);
                file.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError("An Unkown Exception Occured: " + ex.Message);
            }

            return objectOut;
        }

        #endregion

        #region Get And Set Encrypted

        /// <summary>
        /// Encrypts A String, But Returns The Encrypted Value Instead Of Saving It
        /// </summary>
        /// <param name="value">The Value To Encrypt</param>
        /// <param name="Password">The Password Can Be Anything You Like, Just Don't Tell Anyone :)</param>
        /// <returns>An Encrypted String Based On Your Password</returns>
        public static string EncryptString(string value, string Password)
        {
            string EncryptionKey = Password;
            byte[] clearBytes = Encoding.Unicode.GetBytes(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]
                {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    value = Convert.ToBase64String(ms.ToArray());
                }
            }

            return value;
        }

        /// <summary>
        /// Decrypts A String And Returns The Decrypted Value
        /// </summary>
        /// <param name="value">The Encrypted Value Of The String</param>
        /// <param name="Password">The Password That You Used To Encrypt The String</param>
        /// <returns>A Decrypted String Based On Your Password</returns>
        public static string DecryptString(string value, string Password)
        {
            string EncryptionKey = Password;
            value = value.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]
                {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    value = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return value;
        }

        /// <summary>
        /// Encrypts A String And Stores It In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Encrypt</param>
        /// <param name="Password">The Password Can Be Anything You Like, Just Don't Tell Anyone ;)</param>
        public static void SetStringEncrypted(string Key, string value, string Password)
        {
            PlayerPrefs.SetString(Key, EncryptString(value, Password));
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Decrypts A String From PlayerPrefs And Returns The Decrypted Value
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <param name="Password">The Password That You Used To Encrypt The String</param>
        /// <returns>A Decrypted String Based On Your Password</returns>
        public static string GetStringEncrypted(string Key, string Password)
        {
            return DecryptString(PlayerPrefs.GetString(Key), Password);
        }

        /// <summary>
        /// Encrypts An Integer And Stores It In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Encrypt</param>
        /// <param name="Password">The Password Can Be Anything You Like, Just Don't Tell Anyone ;)</param>
        public static void SetIntEncrypted(string Key, int value, string Password)
        {
            PlayerPrefs.SetString(Key, EncryptString(value.ToString(), Password));
            OnPrefChanged?.Invoke(Key, value);
        }

        /// <summary>
        /// Decrypts An Integer From PlayerPrefs And Returns The Decrypted Value
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <param name="Password">The Password That You Used To Encrypt The Integer</param>
        /// <returns>A Decrypted Integer Based On Your Password</returns>
        public static int GetIntEncrypted(string Key, string Password)
        {
            return int.Parse(DecryptString(PlayerPrefs.GetString(Key), Password));
        }

        /// <summary>
        /// Encrypts A Float And Stores It In PlayerPrefs
        /// </summary>
        /// <param name="Key">The Key To Set</param>
        /// <param name="value">The Value To Encrypt</param>
        /// <param name="Password">The Password Can Be Anything You Like, Just Don't Tell Anyone ;)</param>
        public static void SetFloatEncrypted(string Key, float value, string Password)
        {
            PlayerPrefs.SetString(Key, EncryptString(value.ToString(), Password));
        }

        /// <summary>
        /// Decrypts A Float From PlayerPrefs And Returns The Decrypted Value
        /// </summary>
        /// <param name="Key">The Key To Get</param>
        /// <param name="Password">The Password That You Used To Encrypt The Float</param>
        /// <returns>A Decrypted Float Based On Your Password</returns>
        public static float GetFloatEncrypted(string Key, string Password)
        {
            return float.Parse(DecryptString(PlayerPrefs.GetString(Key), Password));
        }

        #endregion

        #region Misc

        /// <summary>
        /// Deletes All PlayerPrefs(Cannot Be Undone)
        /// </summary>
        /// <param name="areYouSure">If You Are Sure, All PlayerPrefs Will Be Deleted. If Not, I Will Choose Whether To Delete Them Or Not</param>
        public static void ClearAllPrefs(bool areYouSure)
        {
            if (areYouSure)
            {
                PlayerPrefs.DeleteAll();
                Debug.LogWarning("Deleted All PlayerPref Keys, You Can No Longer Access Them.");
            }
            else
            {
                Debug.Log(
                    "You Are Not Sure If You Should Delete All The Prefs, You Are Under Intense Pressure. Should You Do It? Should You Not Do It?");
                var delete = UnityEngine.Random.Range(0, 1);
                if (delete == 1)
                {
                    Debug.LogError(
                        "Your Shoelaces Are Untied, You Trip Up On Them, You Fall, You Hit The Button. All The PlayerPrefs Were Deleted");
                    ClearAllPrefs(true);
                }
                else
                {
                    Debug.Log(
                        "Congratulations, You Resisted The Urge To Press The Button, All Your PlayerPrefs Remain.");
                }
            }
        }

        #endregion
    }
}