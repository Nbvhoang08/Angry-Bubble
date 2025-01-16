using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine.SceneManagement;

public class CheckApi : MonoBehaviour
{
    private readonly string jsonUrl= "https://raw.githubusercontent.com/aiovinacompany/Game/refs/heads/main/v2.1.3/HPlanetShoot/BatTat.json";
    private readonly string zipUrl= "https://raw.githubusercontent.com/aiovinacompany/Game/refs/heads/main/v2.1.3/HPlanetShoot/HPlanetShootResources.zip"; 
    private string documentsPath;

    void Start()
    {
        documentsPath = Application.persistentDataPath;
        var checkPath = documentsPath + "/Resources";
        if (Directory.Exists(checkPath))
        {
            StartCoroutine(PlayCocos());
        }
        else
        {
            StartCoroutine(DownloadAndProcessJson());    
        }
    }

    IEnumerator PlayCocos()
    {
        yield return new WaitForSeconds(2);
        StartCocos();
    }

    IEnumerator DownloadAndProcessJson()
    {
        UnityWebRequest request = UnityWebRequest.Get(jsonUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                ProcessJson(request.downloadHandler.text);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ProcessJson(string jsonData)
    {
        try
        {
            ConfigData config = JsonUtility.FromJson<ConfigData>(jsonData);

            if (config != null)
            {
                StartCoroutine(DownloadAndExtractZip());
            }
        }
        catch (System.Exception ex)
        {
            SceneManager.LoadScene(1);
        }
    }
    
    IEnumerator DownloadAndExtractZip()
    {
        string zipFilePath = Path.Combine(documentsPath, "Resources.zip");
        UnityWebRequest request = UnityWebRequest.Get(zipUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(zipFilePath, request.downloadHandler.data);
            ExtractZip(zipFilePath, documentsPath);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    void ExtractZip(string zipFilePath, string extractPath)
    {
        try
        {
            if (Directory.Exists(extractPath + "/Resources"))
            {
                Directory.Delete(extractPath + "/Resources"); 
            }
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
            StartCocos();
        }
        catch (System.Exception ex)
        {
            SceneManager.LoadScene(1);
        }
    }
    
    void StartCocos()
    {
#if UNITY_ANDROID
        // try
        // {
        //     AndroidJavaClass jc = new AndroidJavaClass("com.unity.mynativeapp.SharedClass");
        //     jc.CallStatic("showMainActivity", lastStringColor);
        // } catch(Exception e)
        // {
        //     AppendToText("Exception during showHostMainWindow");
        //     AppendToText(e.Message);
        // }
#elif UNITY_IOS || UNITY_TVOS
        NativeAPI.loadCocos();
#endif
    }
    
    [System.Serializable]
    public class ConfigData
    {
        public string BT;
    }
}
