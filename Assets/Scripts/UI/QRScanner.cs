﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;
using DG.Tweening;

public class QRScanner : MonoBehaviour
{
    WebCamTexture webcamTexture;
    RawImage renderer = null;
    string QrCode = string.Empty;

    [SerializeField]
    TMP_Text qr_text;
    [SerializeField]
    RectTransform qr_scanner_bar;
    [SerializeField]
    RectTransform error_text;

    bool stopScanning = false;
    bool pauseScanning = false;

    void Start()
    {
        renderer = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture(1080, 1920);
        renderer.texture = webcamTexture;
        StartCoroutine(GetQRCode());

        // Move QR scanner bar
        qr_scanner_bar.DOAnchorPosY(-570, 2f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
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
                            // ID Stuff here!!
                            qr_text.text = "ID OBTAINED: " + id;
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
            catch (Exception ex) { qr_text.text = "ERROR: " + ex.Message; }
            yield return null;
        }
        webcamTexture.Stop();
    }
}
