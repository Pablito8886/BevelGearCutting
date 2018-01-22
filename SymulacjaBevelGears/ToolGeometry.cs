using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;


    class ToolGeometry
    {

        public void ToolSketch(Part workPart)
        {
            CordSystemAndGeometry geomObj = new CordSystemAndGeometry();

            Point punkt0=geomObj.Point3DCreation(workPart, 0, 0, 0);
            Point punkt1=geomObj.Point3DCreation(workPart, 10, 0, 0);
            Point punkt2=geomObj.Point3DCreation(workPart, 0, 10, 0);
            Point punkt3=geomObj.Point3DCreation(workPart, 0, 0, 10);
            Point punkt4 = geomObj.Point3DCreation(workPart, 0, 10, 10);


            DatumAxis axisX = geomObj.DatumAxisCreation(workPart, punkt0, punkt1);
            DatumAxis axisY = geomObj.DatumAxisCreation(workPart, punkt0, punkt4);

            NXOpen.Features.DatumCsys csys = geomObj.CsysCreation(workPart, punkt0, punkt1, punkt2);
            NXOpen.Features.DatumCsys csys1 = geomObj.CsysCreation(workPart,punkt0,axisX,axisY);
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

