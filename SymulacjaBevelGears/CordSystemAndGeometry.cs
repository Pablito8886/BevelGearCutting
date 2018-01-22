using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;

    class CordSystemAndGeometry
    {

        public Point Point3DCreation(Part workPart,double x,double y,double z)
        {
            Point3d originPointTemp = new Point3d(x, y, z);

            Point originPoint = workPart.Points.CreatePoint(originPointTemp);
            originPoint.SetVisibility(SmartObject.VisibilityOption.Visible);

            return originPoint;
        }

        public DatumAxis DatumAxisCreation(Part workPart, Point first, Point second)
        {
            //Tworzenie "buildera dla osi Y".
            NXOpen.Features.Feature nullFeatures_Feature = null;
            NXOpen.Features.DatumAxisBuilder datumAxisBuilder;
            datumAxisBuilder = workPart.Features.CreateDatumAxisBuilder(nullFeatures_Feature);

            datumAxisBuilder.IsAssociative = true;
            datumAxisBuilder.Type = NXOpen.Features.DatumAxisBuilder.Types.TwoPoints;
            datumAxisBuilder.ResizedEndDistance = 0.0;

            datumAxisBuilder.Point1 = first;
            datumAxisBuilder.Point2 = second;

            NXObject vector = datumAxisBuilder.Commit();

            string journalName = vector.JournalIdentifier.ToString();
            DatumAxis datumAxis = (DatumAxis)workPart.Datums.FindObject(journalName);

            datumAxisBuilder.Destroy();

            return datumAxis;
        }


        #region
        public NXOpen.Features.DatumCsys CsysCreation(Part workPart,Point pointOrigin,Point pointAxis1,Point pointAxis2)
        {
            NXOpen.Features.Feature nullFeatures_Feature = null;
            NXOpen.Features.DatumCsysBuilder datumCsysBuilder;
            datumCsysBuilder = workPart.Features.CreateDatumCsysBuilder(nullFeatures_Feature);

            Xform xform1;
            xform1 = workPart.Xforms.CreateXform(pointOrigin, pointAxis1, pointAxis2, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

            CartesianCoordinateSystem cartesianCoordinateSystem;
            cartesianCoordinateSystem = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            datumCsysBuilder.Csys = cartesianCoordinateSystem;
            datumCsysBuilder.ComponentsCreation = true;
            datumCsysBuilder.FixedSizeDatum = true;
            datumCsysBuilder.DisplayScaleFactor = 1.25;

            NXObject cSys = datumCsysBuilder.Commit();

            string journalName = cSys.JournalIdentifier.ToString();
            NXOpen.Features.DatumCsys datumCsys = (NXOpen.Features.DatumCsys)workPart.Features.FindObject(journalName);

            datumCsysBuilder.Destroy();

            return datumCsys;
        }

        public NXOpen.Features.DatumCsys CsysCreation(Part workPart, Point pointOrigin, DatumAxis Axis1, DatumAxis Axis2)
        {
            NXOpen.Features.Feature nullFeatures_Feature = null;
            NXOpen.Features.DatumCsysBuilder datumCsysBuilder;
            datumCsysBuilder = workPart.Features.CreateDatumCsysBuilder(nullFeatures_Feature);
            
            Sense sense1=new Sense();
            Sense sense2=new Sense();

            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(Axis1,sense1,SmartObject.UpdateOption.WithinModeling);

            Direction direction2;
            direction2 = workPart.Directions.CreateDirection(Axis2, sense2, SmartObject.UpdateOption.WithinModeling);

            Xform xform1;
            xform1 = workPart.Xforms.CreateXform(pointOrigin, direction1, direction2, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

            CartesianCoordinateSystem cartesianCoordinateSystem;
            cartesianCoordinateSystem = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            datumCsysBuilder.Csys = cartesianCoordinateSystem;
            datumCsysBuilder.ComponentsCreation = true;
            datumCsysBuilder.FixedSizeDatum = true;
            datumCsysBuilder.DisplayScaleFactor = 1.25;

            NXObject cSys = datumCsysBuilder.Commit();

            string journalName = cSys.JournalIdentifier.ToString();
            NXOpen.Features.DatumCsys datumCsys = (NXOpen.Features.DatumCsys)workPart.Features.FindObject(journalName);

            datumCsysBuilder.Destroy();

            return datumCsys;
        }
        #endregion


    }



