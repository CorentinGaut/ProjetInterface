using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;//Point
using Emgu.CV;
using Emgu.CV.Util;//Vectors
using Emgu.CV.CvEnum;//Utility for constants
using Emgu.CV.Structure;
using System;
using System.IO; //memory stream

public class Reconnaissance : MonoBehaviour
{
    private VideoCapture fluxVideo;
    private CascadeClassifier _frontFacesCascadeClassifier;
    private CascadeClassifier _eyesCascadeClassifier;
    private string _absolutePathToFrontFacesCascadeClassifier = "haarcascade_frontalcatface.xml";
    private string _absolutePathToEyesCascadeClassifier = "haarcascade_eye_tree_eyeglasses.xml";
    private Rectangle[] frontfaces;
    private Rectangle[] eyes;
    private int MIN_FACE_SIZE = 50;
    private int MAX_FACE_SIZE = 200;
    Mat image;
    // Start is called before the first frame update
    void Start()
    {
        image = new Mat();
        fluxVideo = new VideoCapture(0, VideoCapture.API.Any);
        fluxVideo.FlipHorizontal = true;
        fluxVideo.ImageGrabbed += ProcessFrame;
        _frontFacesCascadeClassifier = new CascadeClassifier(fileName: "D:/Gamagora/Interface/TP_1_Interface/Assets/" + _absolutePathToFrontFacesCascadeClassifier);
        _eyesCascadeClassifier = new CascadeClassifier(fileName: "D:/Gamagora/Interface/TP_1_Interface/Assets/" + _absolutePathToEyesCascadeClassifier);
    }

    void Update()
    {
        fluxVideo.Grab();
        detectAndDisplay();
        //webcam non traité
        /*Texture2D tex = new Texture2D(fluxVideo.Width, fluxVideo.Height, TextureFormat.BGRA32, false);
        tex.LoadRawTextureData(image.ToImage<Bgra, byte>().Bytes);
        tex.Apply();
        tex.LoadImage(image.ToImage<Color,DepthType.Cv8S>(false));
        this.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);*/
        CvInvoke.Imshow("yo", image);
        CvInvoke.WaitKey(24);
    }

    private void OnDestroy()
    {
        fluxVideo.Dispose();
        CvInvoke.DestroyAllWindows();
    }

    private void ProcessFrame(object sender, EventArgs e)
    {
        try
        {
            fluxVideo.Retrieve(image, 0);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    void detectAndDisplay()
    {
        frontfaces = _frontFacesCascadeClassifier.DetectMultiScale(image,1.1,5,new Size(MIN_FACE_SIZE, MIN_FACE_SIZE),new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));
        eyes = _eyesCascadeClassifier.DetectMultiScale(image, 1.1, 2, new Size(MIN_FACE_SIZE, MIN_FACE_SIZE), new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));
        for (int i = 0; i < frontfaces.Length; i++)
        {
            CvInvoke.Rectangle(image, frontfaces[i], new MCvScalar(255, 0, 0));
        }

        for (int i = 0; i < eyes.Length; i++)
        {
            Point center = new Point(eyes[i].X + (eyes[i].Width) / 2, eyes[i].Y + (eyes[i].Height) / 2);
            CvInvoke.Circle(image,center, 10, new MCvScalar(0, 255, 0));
        }
    }
}

