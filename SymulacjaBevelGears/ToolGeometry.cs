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
                Point ptCradleCenter = geomObj.Point3DCreation(workPart, 0, 0, 0);

                //Definition of cradle Csys
                Point ptX = geomObj.Point3DCreation(workPart, 10, 0, 0);
                Point ptY = geomObj.Point3DCreation(workPart, 0, 10, 0);
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
                Point pGlo1=geomObj.Point3DCreation(workPart, ptCradleCenter.Coordinates.X - data.wys, ptCradleCenter.Coordinates.Y + H, ptCradleCenter.Coordinates.Z + V);
                Point pGlo2 = geomObj.Point3DPolar(workPart, pGlo1, -mat, 0);
                Line lGlo1 = geomObj.UnAssLines(workPart, pGlo1, pGlo2);
                //cutter outline
                Point pGlo8 = geomObj.Point3DPolar(workPart, pGlo2, data.d0 / 2, -90);
                Point pGlo10 = geomObj.Point3DPolar(workPart, pGlo8, data.wys+mat, 0);
                Point pGlo9 = geomObj.Point3DPolar(workPart, pGlo10, data.w2 / 2, -90);
                Point pGlo5 = pGlo9;
                Point pGlo4 = geomObj.Point3DPolar(workPart, pGlo5, data.wys / Math.Cos(data.alf2wk*Math.PI/180), 180+data.alf2wk);
                Point pGlo3 = geomObj.Point3DPolar(workPart, pGlo4, mat, 180);
                Point pGlo6 = geomObj.Point3DPolar(workPart, pGlo9, data.w2 / 2, 90);
                Point pGlo7 = geomObj.Point3DPolar(workPart, pGlo6, data.wys / Math.Cos(data.alf2wp * Math.PI / 180), 180-data.alf2wp);
                Line lGlo2 = geomObj.UnAssLines(workPart, pGlo2, pGlo3);
                Line lGlo3 = geomObj.UnAssLines(workPart, pGlo4, pGlo3);
                Line lGlo4 = geomObj.UnAssLines(workPart, pGlo5, pGlo4);
                Line lGlo5 = geomObj.UnAssLines(workPart, pGlo5, pGlo6);
                Line lGlo6 = geomObj.UnAssLines(workPart, pGlo6, pGlo7);
                Line lGlo7 = geomObj.UnAssLines(workPart, pGlo7, pGlo1);

                Tag[] tagg=new Tag[2];
                tagg[0]=lGlo7.Tag;
                tagg[1]=lGlo6.Tag;
                double[] center=new Double[3] {0,0,0};
                int[] trims = new int[3];
                trims[0] = 1;
                trims[1] = 1;
                trims[2] = 1;
                int[] dupa = new int[2] { 0, 0 }; 
                Tag fillet;

                UFSession theUfSession = UFSession.GetUFSession();
                theUfSession.Curve.CreateFillet(0, tagg, center, 3, trims,dupa,out fillet);

                NXOpen.TaggedObject taggedObject = NXOpen.Utilities.NXObjectManager.Get(fillet);
                Arc arc = (Arc)taggedObject;
                arc.Highlight();

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString());}

        }
        
        
        
        
        // NXOpen.Features.DatumCsysBuilder datumCsysBuilder1;


/*





            Point3d origin111 = new Point3d(0.0, 0.0, 0.0);
            Matrix3x3 matrix11;
            matrix11.Xx = 1.0;
            matrix11.Xy = 0.0;
            matrix11.Xz = 0.0;
            matrix11.Yx = 0.0;
            matrix11.Yy = 1.0;
            matrix11.Yz = 0.0;
            matrix11.Zx = 0.0;
            matrix11.Zy = 0.0;
            matrix11.Zz = 1.0;
            workPart.WCS.SetOriginAndMatrix(origin111, matrix11);


        */
    }

