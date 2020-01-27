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

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    private VideoCapture fluxVideo;
    private int frameWidth;
    private int frameHeight;
    private Texture2D tex;
    private Mat imgHSV;

    public Vector3 seuilBas;
    public Vector3 seuilhaut;

    public Hsv seuilbasHsv;
    public Hsv seuilhautHsv;

    void Start()
    {
        fluxVideo = new VideoCapture(0, VideoCapture.API.Any);
        fluxVideo.FlipHorizontal = true;
    }

    // Update is called once per frame
    void Update()
    {
        Mat image;
        image = fluxVideo.QueryFrame();
        seuilbasHsv = new Hsv(seuilBas.x, seuilBas.y, seuilBas.z);
        seuilhautHsv = new Hsv(seuilhaut.x, seuilhaut.y, seuilhaut.z);

        Mat imgHSV = image.Clone();
        //CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);
        CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);
        CvInvoke.Blur(imgHSV, imgHSV, new Size(4,4),new Point(1,1));
        Image<Hsv, byte> imgConverti = imgHSV.ToImage<Hsv, byte>();
        Image<Gray, byte> imgseuil = imgConverti.InRange(seuilbasHsv, seuilhautHsv);

        //webcam non traité
        //Texture2D tex = new Texture2D(fluxVideo.Width, fluxVideo.Height, TextureFormat.BGRA32, false);
        //tex.LoadRawTextureData(imgseuil.ToImage<Bgra, byte>().Bytes);
        //tex.Apply();
        //tex.LoadImage(img.ToImage<Color,DepthType.Cv8S>(false));
        //this.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);

        var strutElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(2, 2));
        CvInvoke.Erode(imgseuil, imgseuil, strutElement, new Point(2, 2), 5, BorderType.Default, new MCvScalar());
        CvInvoke.Dilate(imgseuil, imgseuil, strutElement, new Point(2, 2), 8, BorderType.Default, new MCvScalar());

        //detection de contours
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        Mat m = new Mat();
        CvInvoke.FindContours(imgseuil, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

        for (int i = 0; i < contours.Size; i++)
        {
            double perimeter = CvInvoke.ArcLength(contours[i], true);
            VectorOfPoint approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);
            CvInvoke.DrawContours(image, contours, i, new MCvScalar(0, 0, 255));

            //centroide
            var moments = CvInvoke.Moments(contours[i]);
            int x = (int)(moments.M10 / moments.M00);
            int y = (int)(moments.M01 / moments.M00);
            CvInvoke.Circle(image, new Point(x, y), 7, new MCvScalar(0, 0, 0), -1);
            CvInvoke.PutText(image, "Orange", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
        }
        CvInvoke.Imshow("yo", image);
        CvInvoke.WaitKey(24);
    }

    private void OnDestroy()
    {
        fluxVideo.Dispose();
        CvInvoke.DestroyAllWindows();
    }
}
