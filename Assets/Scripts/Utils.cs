using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Security.Cryptography.X509Certificates;

/// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html
/// https://docs.unity3d.com/ScriptReference/Networking.CertificateHandler.ValidateCertificate.html

namespace Utils_NS
{

    public class Utils : MonoBehaviour
    {

        public static void DebugLog(string debugLogString)
        {
            if (debugLogsEnabled)
            {
                UnityEngine.Debug.Log(debugLogString);
            }
        }

        public static void SuspendToBackgroundOnAndroid()
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").
               GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }

        static bool wwwDebugLogs = true;
        static bool debugLogsEnabled = true;
        //static bool errorDlg = false;

        public delegate void RequestResultCallback(string result);
        public delegate void BinaryRequestResultCallback(byte[] result);

        static string protocol = "http://";

        public static void reset()
        {
            protocol = "http://";
        }

        static string sendDCCCommandUri(string ipAddress) { return protocol + ipAddress + "/ec"; }

        class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        WaitForEndOfFrame waitEOF = new WaitForEndOfFrame();

        public void SendDeviceCommand(string ipAddress, int channel, string cmd, RequestResultCallback callback)
        {
            StartCoroutine(ExecuteGetRequestV1(sendDCCCommandUri(ipAddress) + "?" + "ch=" + channel + "&dcc=" + cmd, callback, true));

            //StartCoroutine(ExecuteGetRequestV1("https://www.example.com", callback, true));
        }

        IEnumerator ExecuteGetRequestV2(string uri, RequestResultCallback callback, bool debug = false)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }
        }

        IEnumerator ExecuteGetRequestV1(string uri, RequestResultCallback callback, bool debug = false)
        {
            yield return waitEOF;

            if (wwwDebugLogs || debug) UnityEngine.Debug.Log("Send request:" + uri);

            using (var w = UnityWebRequest.Get(uri))
            {
                w.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
                yield return w.SendWebRequest();
                if (w.result == UnityWebRequest.Result.ConnectionError || w.result == UnityWebRequest.Result.ProtocolError)
                {
                    UnityEngine.Debug.Log(w.error);
                    UnityEngine.Debug.Log("Error: " + ((w.result == UnityWebRequest.Result.ProtocolError) ? "HTTP error" : "Network error"));
                    UnityEngine.Debug.Log("URI was: " + uri);
                    string result = "{ \"code\": " + ((w.result == UnityWebRequest.Result.ConnectionError) ? 999 : w.responseCode) + " , \"error\" : \"" + w.error + "\" }";
                    if (wwwDebugLogs || debug) UnityEngine.Debug.Log("Result: " + result);
                    //if (errorDlg) PopupManager.instance.alertDialog("lblError", w.error + "\n\nURI: " + uri);
                    if (callback != null)
                    {
                        callback(result);
                    }
                }
                else
                {
                    if (wwwDebugLogs || debug) UnityEngine.Debug.Log("Result: " + w.downloadHandler.text);
                    if (callback != null)
                    {
                        string result = "{ \"code\": 0 , \"error\" : \"No error\" }";
                        callback(result);
                        //callback(w.downloadHandler.text.Trim());
                    }
                }
            }
        }

        IEnumerator executePostRequest(string uri, RequestResultCallback callback, bool debug = false)
        {
            yield return waitEOF;

            if (wwwDebugLogs || debug)
                UnityEngine.Debug.Log("Post request:" + uri);

            WWWForm form = new WWWForm();
            using (var w = UnityWebRequest.Post(uri, form))
            {
                w.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
                yield return w.SendWebRequest();
                if (w.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.Log(w.error);
                    UnityEngine.Debug.Log("Error: " + ((w.result == UnityWebRequest.Result.ProtocolError) ? "HTTP error" : "Network error"));
                    UnityEngine.Debug.Log("URI was: " + uri);
                    string result = "{ \"code\": " + ((w.result == UnityWebRequest.Result.ConnectionError) ? 999 : w.responseCode) + " , \"error\" : \"" + w.error + "\" }";
                    if (wwwDebugLogs || debug) UnityEngine.Debug.Log("Result: " + result);
                    //if (errorDlg) PopupManager.instance.alertDialog("lblError", w.error + "\n\nURI: " + uri);
                    if (callback != null)
                    {
                        callback(result);
                    }
                }
                else
                {
                    if (wwwDebugLogs || debug) UnityEngine.Debug.Log("Result: " + w.downloadHandler.text);
                    if (callback != null)
                    {
                        string result = "{ \"code\": 0 , \"error\" : \"No error\" }";
                        callback(result);
                        //callback(w.downloadHandler.text.Trim());
                    }
                }
            }
        }

    }
}
