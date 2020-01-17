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

public class Projet : MonoBehaviour
{

    public Vector3 seuilBas;
    public Vector3 seuilhaut;

    public Vector3 seuilBasBlue;
    public Vector3 seuilhautBlue;

    public Image Level;

    private VideoCapture fluxVideo;
    private Mat image;
    private Mat imageHSV = new Mat();
    private Hsv seuilbasHsv;
    private Hsv seuilhautHsv;
    private Texture texture;

    void Start()
    {
        image = new Mat();
        fluxVideo = new VideoCapture(0, VideoCapture.API.Any);
        fluxVideo.FlipHorizontal = true;
        fluxVideo.ImageGrabbed += ProcessFrame;
    }

    // Update is called once per frame
    void Update()
    {
        fluxVideo.Grab();

        //converti
        Image<Gray, byte> imageSeuilLimit = Convert(seuilBas, seuilhaut);
        Image<Gray, byte> imageSeuilBlue = Convert(seuilBasBlue, seuilhautBlue);

        //dilate pour affiner les trais
        var strutElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(2, 2));
        CvInvoke.Dilate(imageSeuilLimit, imageSeuilLimit, strutElement, new Point(2, 2), 1, BorderType.Default, new MCvScalar());

        //detection de contours
        DrawLimit(imageSeuilBlue, "blue");
        DrawLimit(imageSeuilLimit, "limit");

        CvInvoke.Imshow("Labirynthe", image);
        CvInvoke.WaitKey(24);

        //La texture
        if (Input.GetKeyDown(KeyCode.A))
        {
            //CvInvoke.CvtColor(image, imageSeuilLimit, ColorConversion.Gray2Bgr);
            Texture2D tex = new Texture2D(imageSeuilLimit.Width, imageSeuilLimit.Height, TextureFormat.BGRA32, false);
            Debug.Log(imageSeuilLimit.Width);
            Debug.Log(imageSeuilLimit.Height);
            Debug.Log(imageSeuilLimit.Bytes.Length);
            tex.LoadRawTextureData(imageSeuilLimit.Bytes);
            tex.Apply();
            tex.LoadImage(imageSeuilLimit.Bytes);
            Level.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);
        }
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

    //converti l'image en noir et blnc pour plus de precision
    Image<Gray, byte> Convert(Vector3 seuilb, Vector3 seuilh)
    {
        CvInvoke.CvtColor(image, imageHSV, ColorConversion.Bgr2Hsv);
        CvInvoke.Blur(imageHSV, imageHSV, new Size(4, 4), new Point(1, 1));
        seuilbasHsv = new Hsv(seuilb.x, seuilb.y, seuilb.z);
        seuilhautHsv = new Hsv(seuilh.x, seuilh.y, seuilh.z);
        Image<Hsv, byte> imgConverti = imageHSV.ToImage<Hsv, byte>();
        Image<Gray, byte> imgseuil = imgConverti.InRange(seuilbasHsv, seuilhautHsv);
        return imgseuil;
    }

    void DrawLimit(Image<Gray, byte> imageSeuil,String name)
    {
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        Mat m = new Mat();
        CvInvoke.FindContours(imageSeuil, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
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
            CvInvoke.PutText(image, name, new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
        }
    }
}
