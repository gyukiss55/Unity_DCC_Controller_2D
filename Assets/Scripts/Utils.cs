using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Security.Cryptography.X509Certificates;

using DCC_Controller_NS;

/// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html
/// https://docs.unity3d.com/ScriptReference/Networking.CertificateHandler.ValidateCertificate.html

namespace Utils_NS
{

    public class Utils : MonoBehaviour
    {

         static bool wwwDebugLogs = true;
        static bool debugLogsEnabled = true;
        //static bool errorDlg = false;

        public delegate void RequestResultCallback(string result);
        public delegate void BinaryRequestResultCallback(byte[] result);

        static string protocol = "http://";

        WaitForEndOfFrame waitEOF = new WaitForEndOfFrame();

        Color colorOn = new Color(1, 0.92f, 0.016f, 1); // yellow
        Color colorOff = new Color(0, 1, 1, 1); // cyan
        Color colorHighOn = new Color(1, 1, 0.03f, 1); // yellow
        Color colorHighOff = new Color(0.2f, 1, 1, 1); // cyan
        Color colorDarkOn = new Color(0.8f, 0.92f, 0.016f, 1); // yellow
        Color colorDarkOff = new Color(0, 0.8f, 0.8f, 1); // cyan


        public void UpdateButtonColor(Button tmp_button, bool status)
        {
            ColorBlock cb = tmp_button.colors;
            if (status)
            {
                Color c = colorOn;
                cb.normalColor = c;
                cb.selectedColor = c;
                cb.highlightedColor = colorHighOn;
                cb.pressedColor = colorDarkOn;
            }
            else
            {
                Color c = colorOff;
                cb.normalColor = c;
                cb.selectedColor = c;
                cb.highlightedColor = colorHighOff;
                cb.pressedColor = colorDarkOff;
            }
            tmp_button.colors = cb;
        }

        static string sendDCCCommandUri(string ipAddress) { return protocol + ipAddress + "/ec"; }

        class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        public void SendDeviceCommand(string ipAddress, int channel, string cmd, RequestResultCallback callback)
        {
            StartCoroutine(ExecuteGetRequest(sendDCCCommandUri(ipAddress) + "?" + "ch=" + channel + "&dcc=" + cmd, callback, true));
        }

        IEnumerator ExecuteGetRequest(string uri, RequestResultCallback callback, bool debug = false)
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

    }
}
