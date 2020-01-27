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
public class Croissanceregion : MonoBehaviour
{
    // Start is called before the first frame update
    private int frameWidth;
    private int frameHeight;
    private Texture2D tex;
    private Mat imgHSV;

    Mat imgInput;
    Vector3 mousePosition;
    Image<Rgb, byte> imgConverti;

    void Start()
    {
        imgInput = CvInvoke.Imread("D:/Gamagora/Interface/TP_1_Interface/Assets/rondvide2.png");
        Debug.Log(imgInput.Width);
        Debug.Log(imgInput.Height);
        imgConverti = imgInput.ToImage<Rgb, byte>();
    }

    // Update is called once per frame
    void Update()
    {
        CvInvoke.Imshow("hello", imgInput);

        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
            CvInvoke.Circle(imgInput, new Point((int)mousePosition.x, (int)Math.Abs(mousePosition.y - imgInput.Height)), 7, new MCvScalar(0, 0, 0), -1);
            Debug.Log(mousePosition);
        }

        
    }

    private void OnDestroy()
    {
        CvInvoke.DestroyAllWindows();
    }

    void Croissance()
    {

    }
}
