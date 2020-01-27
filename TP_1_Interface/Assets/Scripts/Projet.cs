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
    public Image UiCamera;
    public Vector3 seuilBas;
    public Vector3 seuilhaut;

    public Vector3 seuilBasBlue;
    public Vector3 seuilhautBlue;
    public bool isDrawing = false;
   

    VectorOfVectorOfPoint contours;
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
        /*fluxVideo.FlipVertical = true;
        fluxVideo.FlipHorizontal = true;*/
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


        
        if (Input.GetKeyDown(KeyCode.S))
        {
            //extrude le background pour avoir que les platformes
            ExtrudeBackGround(image, imageSeuilBlue);

            //creation du sprite
            gameObject.GetComponent<SpriteRenderer>().sprite = PNG2Sprite.LoadNewSprite("./Assets/test.png", 100.0f);
            gameObject.AddComponent<PolygonCollider2D>();

            isDrawing = true;
        }
            //La texture
        if (Input.GetKeyDown(KeyCode.A))
        {
            Texture2D tex = new Texture2D(fluxVideo.Width, fluxVideo.Height, TextureFormat.BGRA32, false);
            Mat hh = new Mat();
            CvInvoke.CvtColor(imageSeuilBlue, hh, ColorConversion.Gray2Bgra);
            //tex.LoadImage(hh.ToImage<Bgra, byte>().Bytes);
            tex.LoadRawTextureData(hh.ToImage<Bgra, byte>().Bytes);
            tex.Apply();
            //Level.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);
        }
        //webcam non traité
        
        Texture2D tex2 = new Texture2D(fluxVideo.Width, fluxVideo.Height, TextureFormat.BGRA32, false);
        tex2.LoadRawTextureData(image.ToImage<Bgra, byte>().Bytes);
        tex2.Apply();
        UiCamera.sprite = Sprite.Create(tex2, new Rect(0.0f, 0.0f, tex2.width, tex2.height), new Vector2(0.5f, 0.5f), 1.0f);
        //CvInvoke.Imshow("image de base", image);
        CvInvoke.WaitKey(24);
    }

    //Detruis la fenetre de la camera lorqu'on sort du debug
    /*private void OnDestroy()
    {
        fluxVideo.Dispose();
        CvInvoke.DestroyAllWindows();
    }*/

    //plus de FPS
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

    //fonction pour dessiner les limits des obj et creer leur centroide
    void DrawLimit(Image<Gray, byte> imageSeuil,String name)
    {
        contours = new VectorOfVectorOfPoint();
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

    void ExtrudeBackGround(Mat imgToExtrude, Image<Gray, byte> maskImage)
    {
        Image<Gray, byte> imgInverser = maskImage.InRange(new Gray(0), new Gray(100));
        VectorOfByte buf = new VectorOfByte();
        var mask = imgInverser;
        Mat imageBGRA = new Mat();
        CvInvoke.CvtColor(imgToExtrude, imageBGRA, ColorConversion.Bgr2Bgra);
        imageBGRA.SetTo(new MCvScalar(255, 255, 255, 0), mask);
        CvInvoke.Imencode(".png", imageBGRA, buf);
        byte[] arr = buf.ToArray();
        System.IO.File.WriteAllBytes("./Assets/test.png", arr); // creation du PNG
    }
}
