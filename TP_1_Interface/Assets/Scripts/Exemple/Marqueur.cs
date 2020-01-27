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

public class Marqueur : MonoBehaviour
{
    private VideoCapture fluxVideo;
    Mat image;
    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
    VectorOfPoint approx = new VectorOfPoint();
    VectorOfVectorOfPoint candidates = new VectorOfVectorOfPoint();
    Mat m = new Mat();
    
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
        Mat grey = new Mat();
        CvInvoke.CvtColor(image, grey, ColorConversion.Bgr2Gray);
        CvInvoke.AdaptiveThreshold(grey, grey, 255, AdaptiveThresholdType.GaussianC, ThresholdType.BinaryInv, 21, 11);
        CvInvoke.FindContours(grey, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            double perimeter = CvInvoke.ArcLength(contours[i], true);
            CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);
            if(approx.Size == 4)
            {
                if (CvInvoke.ContourArea(contours[i]) > 300)
                {
                    var rect = CvInvoke.BoundingRectangle(approx);
                    if (rect.Height > 0.95 * rect.Width || rect.Height < 0.95 * rect.Width)
                    {
                        candidates.Push(approx);
                        CvInvoke.DrawContours(image, contours, i, new MCvScalar(0, 255, 0), 4);
                        CvInvoke.Rectangle(image, rect, new MCvScalar(255, 0, 0));
                    }
                }
            }
        }
        for (int i =0; i < candidates.Size; i++)
        {
            System.Drawing.PointF[] pts = new System.Drawing.PointF[4];
            pts[0] = new System.Drawing.PointF(0, 0);
            pts[1] = new System.Drawing.PointF(64-1, 0);
            pts[2] = new System.Drawing.PointF(64-1, 64-1);
            pts[3] = new System.Drawing.PointF(0, 64-1);
            VectorOfPointF perfect = new VectorOfPointF(pts);

            System.Drawing.PointF[] sample_pts = new System.Drawing.PointF[4];
            for (int ii = 0; ii < 4; ii++)
                sample_pts[ii] = new System.Drawing.PointF(candidates[i][ii].X, candidates[i][ii].Y);
            VectorOfPointF sample = new VectorOfPointF(sample_pts);
            var tf = CvInvoke.GetPerspectiveTransform(sample, perfect);

            Mat warped = new Mat();
            CvInvoke.WarpPerspective(image, warped, tf, new System.Drawing.Size(64, 64));
            CvInvoke.Imshow("yo", warped);
        }
        
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
}
