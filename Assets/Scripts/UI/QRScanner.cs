using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class QRScanner : MonoBehaviour
{
    WebCamTexture webcamTexture;
    RawImage renderer = null;
    string QrCode = string.Empty;

    [SerializeField]
    RectTransform qr_scanner_bar;
    [SerializeField]
    RectTransform error_text;
    [SerializeField]
    QRPopupManager qrPopupManager;

    bool stopScanning = false;
    bool pauseScanning = false;
    DatabaseReference database;

    public void SwitchScene(string sceneName) => SceneManager.LoadScene(sceneName);

    void Start()
    {
        // Initialise variables
        renderer = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture(1080, 1920);
        renderer.texture = webcamTexture;
        StartCoroutine(GetQRCode());

        // Move QR scanner bar
        qr_scanner_bar.DOAnchorPosY(-570, 2f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);

        // Initialise firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                database = FirebaseDatabase.GetInstance("https://heartland-hampers-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;

            }
            else
            {
                Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    IEnumerator GetQRCode()
    {
        stopScanning = false;
        pauseScanning = false;

        IBarcodeReader barCodeReader = new BarcodeReader();
        webcamTexture.Play();

        // Update size and rotation based on detected camera
        renderer.SetNativeSize();
        renderer.rectTransform.rotation = Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.back);

        // Expand camera to fill screen
        float updatedScale = 1080 / renderer.rectTransform.sizeDelta.x;
        float heightScale = 1920 / renderer.rectTransform.sizeDelta.y;
        if (heightScale > updatedScale) // Take whichever ratio is lowest: height or width
            updatedScale = heightScale;
        renderer.rectTransform.localScale = new Vector3(updatedScale, updatedScale, 1);

        // Check for QR
        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);
        while (!stopScanning)
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var Result = barCodeReader.Decode(snap.GetRawTextureData(), webcamTexture.width, webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (Result != null && !pauseScanning)
                {
                    QrCode = Result.Text;
                    if (!string.IsNullOrEmpty(QrCode))
                    {
                        // Check if valid QR code from us!
                        if (QrCode.StartsWith("heartland_") && int.TryParse(QrCode.Split("heartland_")[1], out int id))
                        {
                            // ID Stuff here!
                            database.Child("machine_id").SetValueAsync(id); // Update machine id
                            CloudScriptManager.Instance.ExecGetBoxID(id => {
                                database.Child("box_id").SetValueAsync(id);

                                // Show locker popup
                                qrPopupManager.ShowLockerPopup(id, () => SwitchScene("HomePage"));
                            }, error => Debug.LogError("Error getting box id!" + error.ToString())); // Update box ID

                            stopScanning = true;
                        }
                        else
                        {
                            // Invalid QR scanned
                            error_text.gameObject.SetActive(true);
                            DOTween.Sequence()
                                .Append(error_text.DOShakeAnchorPos(0.5f, new Vector2(20, 0)))
                                .AppendInterval(0.5f)
                                .AppendCallback(() => pauseScanning = false);

                            pauseScanning = true;
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.LogError("ERROR: " + ex.Message); }
            yield return null;
        }
        webcamTexture.Stop();
    }
}
