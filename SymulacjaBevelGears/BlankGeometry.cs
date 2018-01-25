using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;

    class BlankGeometry
    {
        Body bodyBlank;

        public Body BlankGeometryCreation(Part workPart, Session theSession)
         {

             try
             {
                 //CordSystemAndGeometry Class object
                 CordSystemAndGeometry geomObj = new CordSystemAndGeometry();

                 //ReadFile Class object
                 ReadFile data = new ReadFile();
                 data.ReadData();

                 //Creating a pitch cone
                 Point3d pOto1 = new Point3d(0, 0, data.A);
                 Point3d pOto2 = geomObj.Point3DPolar(workPart, pOto1, data.re, 90);
                 Line lOto1 = geomObj.UnAssLines(workPart, pOto1, pOto2);

                 //Creating a symmetry line
                 Point3d pOto90 = geomObj.Point3DPolar(workPart, pOto1, data.re, (90 - data.del));
                 Line lOto90 = geomObj.UnAssLines(workPart, pOto1, pOto90);

                 //Toe line
                 Point3d pOto91 = geomObj.Point3DPolar(workPart, pOto1, (data.re - data.B), 90);
                 Point3d pOto92 = geomObj.Point3DPolar(workPart, pOto91, (data.hae + data.hfe), 0);
                 Point3d pOto93 = geomObj.Point3DPolar(workPart, pOto91, (data.hae + data.hfe), 180);
                 Line lOto94 = geomObj.UnAssLines(workPart, pOto92, pOto93);

                 //Heel line
                 Point3d pOto95 = geomObj.Point3DPolar(workPart, pOto2, (data.hae + data.hfe), 0);
                 Point3d pOto96 = geomObj.Point3DPolar(workPart, pOto2, (data.hae + data.hfe), 180);
                 Line lOto95 = geomObj.UnAssLines(workPart, pOto95, pOto96);

                 //Lwa point
                 Point3d pOto100 = geomObj.Point3DPolar(workPart, pOto1, -data.lwa, (90 - data.del));

                 //Lwf point
                 Point3d pOto101 = geomObj.Point3DPolar(workPart, pOto1, -data.lwf, (90 - data.del));

                 //Addendum line
                 Point3d pOto102 = geomObj.Point3DPolar(workPart, pOto100, data.re + data.B, 90 - data.del + data.delA);
                 Line lOto102 = geomObj.UnAssLines(workPart, pOto100, pOto102);

                 //Dedendum line
                 Point3d pOto103 = geomObj.Point3DPolar(workPart, pOto101, data.re + data.B, 90 - data.del + data.delF);
                 Line lOto103 = geomObj.UnAssLines(workPart, pOto101, pOto103);

                 //---Sketch----
                 //Toe
                 Point3d pOto104 = geomObj.Point3dByIntersection(workPart, lOto94, lOto102);
                 Point3d pOto105 = geomObj.Point3dByIntersection(workPart, lOto94, lOto103);
                 Line lOto104 = geomObj.UnAssLines(workPart, pOto104, pOto105);

                 //Heel
                 Point3d pOto106 = geomObj.Point3dByIntersection(workPart, lOto95, lOto102);
                 Point3d pOto107 = geomObj.Point3dByIntersection(workPart, lOto95, lOto103);
                 Line lOto106 = geomObj.UnAssLines(workPart, pOto106, pOto107);

                 //Addendum line
                 Line lOto120 = geomObj.UnAssLines(workPart, pOto104, pOto106);

                 //Closing line Toe side
                 Point3d pOto108 = geomObj.Point3DPolar(workPart, pOto105, data.re, -data.del);
                 Line lOto108 = geomObj.UnAssLines(workPart, pOto105, pOto108);
                 Point3d pOto110 = geomObj.Point3dByIntersection(workPart, lOto90, lOto108);
                 Line lOto112 = geomObj.UnAssLines(workPart, pOto110, pOto105);

                 //Closing line Heel side
                 Point3d pOto109 = geomObj.Point3DPolar(workPart, pOto107, data.re, -data.del);
                 Line lOto109 = geomObj.UnAssLines(workPart, pOto107, pOto109);
                 Point3d pOto111 = geomObj.Point3dByIntersection(workPart, lOto90, lOto109);
                 Line lOto113 = geomObj.UnAssLines(workPart, pOto107, pOto111);

                 //Sketch
                 Point3d ptX = new Point3d(10, 0, 0);
                 Point3d ptY = new Point3d(0, 10, 0);
                 Point3d ptZ = new Point3d(0, 0, 10);
                 Point3d ptCradleCenter = new Point3d(0, 0, 0);
                 NXOpen.Features.DatumCsys csysCradle = geomObj.CsysCreation(workPart, ptCradleCenter, ptX, ptY);
                 DatumAxis datumAxisNor = geomObj.DatumAxisCreation(workPart, ptCradleCenter, ptZ);
                 DatumAxis datumAxisHoriz = geomObj.DatumAxisCreation(workPart, pOto1, pOto90);
                 DatumPlane datumPlane = geomObj.DatumPlaneCreation(workPart, pOto1, datumAxisNor);
                 Sketch sketchBlank = geomObj.SketchCreation(workPart, theSession, pOto1, datumAxisHoriz, datumPlane, csysCradle);
                 sketchBlank.AddGeometry(lOto104);
                 sketchBlank.AddGeometry(lOto106);
                 sketchBlank.AddGeometry(lOto120);
                 sketchBlank.AddGeometry(lOto112);
                 sketchBlank.AddGeometry(lOto113);

                 //Axis for body revolution
                 Axis axisBodyRev = geomObj.AxisCreation(workPart, pOto1, lOto90);

                 //Blank creation
                 Body blank = geomObj.Revolve(workPart, axisBodyRev, sketchBlank, lOto104, pOto104, 0, 360);

                 //Rotation of Blank to have dedendum linie parallel to the cutter plane
                 //Axis of rotation - Cradle axis Z
                 Line lineZ = geomObj.UnAssLines(workPart, ptCradleCenter, ptZ);
                 Axis axisZ = geomObj.AxisCreation(workPart, ptCradleCenter, lineZ);
                 Body bodyRot = geomObj.RotationBodyAboutAxis(workPart, blank, axisZ, data.del - data.del2);


                 // Moving the Blank by XP
                 Line dedendumLine = geomObj.UnAssLines(workPart, pOto105, pOto107);
                 Direction directionXB = workPart.Directions.CreateDirection(dedendumLine, Sense.Forward, SmartObject.UpdateOption.WithinModeling);
                 Body bodyXP = geomObj.MoveBodyAlongVector(workPart, bodyRot, directionXB, data.xp);

                 // Moving the Blank by XB
                 Line xLine = geomObj.UnAssLines(workPart, ptCradleCenter, ptX);
                 Direction directionX = workPart.Directions.CreateDirection(xLine, Sense.Forward, SmartObject.UpdateOption.WithinModeling);
                 Body bodyXB = geomObj.MoveBodyAlongVector(workPart, bodyXP, directionX, data.xb);

                 bodyBlank = bodyXB;
             }
            catch(Exception ex)
             {
                MessageBox.Show(ex.ToString());
             }
             return bodyBlank;
        }
    }

