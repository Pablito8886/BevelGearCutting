using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;


    class ToolGeometry
    {

        public void ToolSketch(Part workPart,Session theSession)
        {
            string com;

            try
            {
                //CordSystemAndGeometry Class object
                CordSystemAndGeometry geomObj = new CordSystemAndGeometry();

                //ReadFile Class object
                ReadFile data = new ReadFile();
                data.ReadData();

                // Definition of cradle center point
                //Point ptCradleCenter = geomObj.Point3DCreation(workPart, 0, 0, 0);
                Point3d ptCradleCenter = new Point3d(0, 0, 0);

                //Definition of cradle Csys
               // Point ptX = geomObj.Point3DCreation(workPart, 10, 0, 0);
               // Point ptY = geomObj.Point3DCreation(workPart, 0, 10, 0);
                Point3d ptX = new Point3d(10, 0, 0);
                Point3d ptY = new Point3d(0, 10, 0);
                NXOpen.Features.DatumCsys csysCradle = geomObj.CsysCreation(workPart, ptCradleCenter, ptX, ptY);

                //Definition of Cradle Angle sign
                // Hand of spiral
                // -- LEFT: -Q
                // -- RIGHT: +Q
                if (data.kierunekLinii == 0) { data.q = -data.q; }
                else if (data.kierunekLinii != 1 && data.kierunekLinii != 0) { throw new Exception("Niepoprawne dane dotyczace Pochylenia Linii Zeba"); }
                if (data.q==0) { throw new Exception("Niepoprawne dane dotyczace Ustawienia Katowego Glowicy"); }
               
                // Taken Q to calculations:
                com="Q przyjete do obliczen: "+data.q.ToString();
                data.PrintComToLog(theSession, com);

                //Changing polar to cartesian cordinates
                double H = data.u * Math.Cos(data.q * (Math.PI / 180));
                double V = data.u * Math.Sin(data.q * (Math.PI / 180));

                //------ Cutter sketch creation ------

                //Total tooth depth
                double mat = data.hae + data.hfe;
                //Starting points and line
                Point3d pGlo1=new Point3d(ptCradleCenter.X - data.wys, ptCradleCenter.Y + H, ptCradleCenter.Z + V);
                Point3d pGlo2 = geomObj.Point3DPolar(workPart, pGlo1, -mat, 0);
                Line lGlo1 = geomObj.UnAssLines(workPart, pGlo1, pGlo2);

                //cutter outline
                Point3d pGlo8 = geomObj.Point3DPolar(workPart, pGlo2, data.d0 / 2, -90);
                Point3d pGlo10 = geomObj.Point3DPolar(workPart, pGlo8, data.wys+mat, 0);
                Point3d pGlo9 = geomObj.Point3DPolar(workPart, pGlo10, data.w2 / 2, -90);
                Point3d pGlo5 = pGlo9;
                Point3d pGlo4 = geomObj.Point3DPolar(workPart, pGlo5, data.wys / Math.Cos(data.alf2wk*Math.PI/180), 180+data.alf2wk);
                Point3d pGlo3 = geomObj.Point3DPolar(workPart, pGlo4, mat, 180);
                Point3d pGlo6 = geomObj.Point3DPolar(workPart, pGlo9, data.w2 / 2, 90);
                Point3d pGlo7 = geomObj.Point3DPolar(workPart, pGlo6, data.wys / Math.Cos(data.alf2wp * Math.PI / 180), 180-data.alf2wp);
                Line lGlo2 = geomObj.UnAssLines(workPart, pGlo2, pGlo3);
                Line lGlo3 = geomObj.UnAssLines(workPart, pGlo4, pGlo3);
                Line lGlo4 = geomObj.UnAssLines(workPart, pGlo5, pGlo4);
                Line lGlo5 = geomObj.UnAssLines(workPart, pGlo5, pGlo6);
                Line lGlo6 = geomObj.UnAssLines(workPart, pGlo6, pGlo7);
                Line lGlo7 = geomObj.UnAssLines(workPart, pGlo7, pGlo1);

                Point3d pointt = new Point3d(0, 0, 0);
                //Point pointInput = workPart.Points.CreatePoint(pointt);
                Point3d pointt2 = new Point3d(0, 20, 0);
                //Point pointInput2 = workPart.Points.CreatePoint(pointt2);

                Curve linia1 = workPart.Curves.CreateLine(pointt, pointt2);
                linia1.SetVisibility(SmartObject.VisibilityOption.Visible);

                //Sketch def
                //Axis normal do sketch plane
                Point3d helpPointN = new Point3d(0, 0, 1);
                DatumAxis axisNor = geomObj.DatumAxisCreation(workPart, ptCradleCenter, helpPointN);
                DatumPlane plane = geomObj.DatumPlaneCreation(workPart, pGlo2, axisNor);

                //Axis perpendicular to the sketch plane
                Point3d helpPointP = new Point3d(1, 0, 0);
                DatumAxis axisPer = geomObj.DatumAxisCreation(workPart, ptCradleCenter, helpPointP);
                Sketch sketchTool = geomObj.SketchCreation(workPart, theSession, pGlo2, axisPer, plane, csysCradle);

                //Attach geometry to the sketch
                sketchTool.AddGeometry(lGlo2);
                sketchTool.AddGeometry(lGlo3);
                sketchTool.AddGeometry(lGlo4);
                sketchTool.AddGeometry(lGlo5);
                sketchTool.AddGeometry(lGlo6);
                sketchTool.AddGeometry(lGlo7);
                sketchTool.Update();

                //Cutter radius creation
                        //Concave side
                        Point3d helpPoint1 = geomObj.Point3dOnCurve(workPart, "0.5", lGlo5);
                        Point3d helpPoint2 = geomObj.Point3dOnCurve(workPart, "0.5", lGlo4);
                        Arc r02wk = geomObj.FilletOnSketch(theSession, lGlo5, lGlo4, helpPoint1, helpPoint2, data.r02wk);
                        //Convex side
                        helpPoint1 = geomObj.Point3dOnCurve(workPart, "0.5", lGlo5);
                        helpPoint2 = geomObj.Point3dOnCurve(workPart, "0.5", lGlo6);
                        Arc r02wp = geomObj.FilletOnSketch(theSession, lGlo5, lGlo6, helpPoint1, helpPoint2, data.r02wp);
                 sketchTool.Update();

                //Creation a solid cutter
                        //Axis def
                        Vector3d vectorCutterAxis=new Vector3d(1,0,0);
                        Axis axisCutter = geomObj.AxisCreation(workPart, pGlo1, vectorCutterAxis);
                        Body cutter=geomObj.Revolve(workPart, axisCutter, sketchTool, lGlo6, pGlo1,0,360);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString());}
        }
    }

