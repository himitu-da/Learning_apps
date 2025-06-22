using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveLoadService
{
    private static readonly string key = "JMeDIxvbFKUdaMakIMxyUPtSuKAsae3b"; // 32文字のキーを使用
    private static readonly string iv = "IIBSdmgGczUCLOwn";   // 16文字のIVを使用

    /// <summary>
    /// JsonとPathを入れれば良い感じに保存してくれる。ディレクトリは自動作成しない。
    /// </summary>
    /// <param name="full_path"></param>
    /// <param name="json"></param>
    public void Save(string full_path, string json)
    {
        try
        {
            string encrypted_json = EncryptString(json);
            File.WriteAllText(full_path, encrypted_json);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Pathを入れればJson（string）が返ってくる。存在しない場合はnull。
    /// </summary>
    /// <param name="full_path"></param>
    /// <returns></returns>
    public string Load(string full_path)
    {
        try
        {
            string encrypted_json = File.ReadAllText(full_path);
            string json = DecryptString(encrypted_json);
            return json;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public string EncryptString(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public string DecryptString(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
