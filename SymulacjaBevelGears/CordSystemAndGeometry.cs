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

        public Point3d Point3DPolar(Part workPart, Point3d ptOrigin3d,double radius, double angle)
        {
            Vector3d myVector = new Vector3d();
            myVector.X = radius*Math.Cos(angle*Math.PI/180);
            myVector.Y = radius * Math.Sin(angle * Math.PI / 180);
            myVector.Z = 0;

            Offset myOffset = workPart.Offsets.CreateOffset(ptOrigin3d, myVector, SmartObject.UpdateOption.WithinModeling);
            Point ptOrigin = workPart.Points.CreatePoint(ptOrigin3d);

            Point outPoint = workPart.Points.CreatePoint(myOffset, ptOrigin, SmartObject.UpdateOption.WithinModeling);
            outPoint.SetVisibility(SmartObject.VisibilityOption.Visible);

            Point3d outPoint3D = new Point3d(outPoint.Coordinates.X, outPoint.Coordinates.Y,outPoint.Coordinates.Z);

            return outPoint3D;
        }

        public Point3d Point3dOnCurve(Part workPart,string disPercents,Line line)
        {
            Unit nullUnit = null;
            Expression expression = workPart.Expressions.CreateSystemExpressionWithUnits(disPercents, nullUnit);
            
            Scalar scalar4 = workPart.Scalars.CreateScalarExpression(expression, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point point2 = workPart.Points.CreatePoint(line, scalar4, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point3d outPoint = new Point3d(point2.Coordinates.X, point2.Coordinates.Y, point2.Coordinates.Z);

            return outPoint;
        }

        public DatumAxis DatumAxisCreation(Part workPart, Point3d first3d, Point3d second3d)
        {
            //Tworzenie "buildera dla osi Y".
            NXOpen.Features.Feature nullFeatures_Feature = null;
            NXOpen.Features.DatumAxisBuilder datumAxisBuilder;
            datumAxisBuilder = workPart.Features.CreateDatumAxisBuilder(nullFeatures_Feature);

            datumAxisBuilder.IsAssociative = true;
            datumAxisBuilder.Type = NXOpen.Features.DatumAxisBuilder.Types.TwoPoints;
            datumAxisBuilder.ResizedEndDistance = 0.0;

            Point first = workPart.Points.CreatePoint(first3d);
            Point second = workPart.Points.CreatePoint(second3d);

            datumAxisBuilder.Point1 = first;
            datumAxisBuilder.Point2 = second;

            NXObject vector = datumAxisBuilder.Commit();

            string journalName = vector.JournalIdentifier.ToString();
            DatumAxis datumAxis = (DatumAxis)workPart.Datums.FindObject(journalName);

            datumAxisBuilder.Destroy();

            return datumAxis;
        }

        public DatumPlane DatumPlaneCreation (Part workPart,Point3d point3d,DatumAxis axis)
        {
            NXOpen.Features.Feature nullFeatures_Feature = null;

            NXOpen.Features.DatumPlaneBuilder datumPlaneBuilder1;
            datumPlaneBuilder1 = workPart.Features.CreateDatumPlaneBuilder(nullFeatures_Feature);

            Plane plane1;
            plane1 = datumPlaneBuilder1.GetPlane();

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

            Point point = workPart.Points.CreatePoint(point3d);

            NXObject[] geom1 = new NXObject[2];
            geom1[0] = point;
            geom1[1] = axis;
            plane1.SetGeometry(geom1);

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            bool flip1;
            flip1 = plane1.Flip;

            NXOpen.Features.Feature feature1;
            feature1 = datumPlaneBuilder1.CommitFeature();

            NXOpen.Features.DatumPlaneFeature datumPlaneFeature1 = (NXOpen.Features.DatumPlaneFeature)feature1;
            DatumPlane datumPlane1;
            datumPlane1 = datumPlaneFeature1.DatumPlane;

            datumPlane1.SetReverseSection(false);

            datumPlaneBuilder1.Destroy();

            return datumPlane1;
        }

        public Line UnAssLines(Part workPart, Point3d first, Point3d last)
        {
            Line line = workPart.Curves.CreateLine(first, last);
            line.SetVisibility(SmartObject.VisibilityOption.Visible);

            return line;
        }

        public Line AssLines(Part workPart, Point3d first3d, Point3d last3d)
        {
            Line nullLine = null;

            NXOpen.Features.AssociativeLineBuilder associativeLineBuilder1=workPart.BaseFeatures.CreateAssociativeLineBuilder(nullLine);
 
            associativeLineBuilder1.StartPointOptions = NXOpen.Features.AssociativeLineBuilder.StartOption.Point;

            Point first = workPart.Points.CreatePoint(first3d);

            associativeLineBuilder1.StartPoint.Value = first;
 
            associativeLineBuilder1.EndPointOptions = NXOpen.Features.AssociativeLineBuilder.EndOption.Point;

            Point last = workPart.Points.CreatePoint(last3d);
 
            associativeLineBuilder1.EndPoint.Value = last;

            NXOpen.Features.AssociativeLine myLine=(NXOpen.Features.AssociativeLine)associativeLineBuilder1.Commit();

            associativeLineBuilder1.Destroy();

            NXObject[] myEntities=myLine.GetEntities();

            Line lineOut=(Line)myEntities[0];
            return lineOut;

        }

        public Arc ArcCreation(Part workPart, Line line1, Line line2)
        {
            Tag[] tagg = new Tag[2];
            tagg[0] = line1.Tag;
            tagg[1] = line2.Tag;
            double[] center = new Double[3] { 0, 0, 0 };
            int[] trims = new int[3];
            trims[0] = 0;
            trims[1] = 0;
            trims[2] = 0;
            int[] alm = new int[2] { 0, 0 };
            Tag fillet;

            UFSession theUfSession = UFSession.GetUFSession();
            theUfSession.Curve.CreateFillet(0, tagg, center, 3, trims, alm, out fillet);

            NXOpen.TaggedObject taggedObject = NXOpen.Utilities.NXObjectManager.Get(fillet);
            Arc arc = (Arc)taggedObject;

            return arc;
        }

        public Sketch SketchCreation(Part workPart,Session theSession,Point3d point3d, DatumAxis axis, DatumPlane plane, NXOpen.Features.DatumCsys Csys)
        {
            Sketch nullSketch = null;
            SketchInPlaceBuilder sketchInPlaceBuilder1;
            sketchInPlaceBuilder1 = workPart.Sketches.CreateNewSketchInPlaceBuilder(nullSketch);

            SketchAlongPathBuilder sketchAlongPathBuilder1;
            sketchAlongPathBuilder1 = workPart.Sketches.CreateSketchAlongPathBuilder(nullSketch);

            Point point = workPart.Points.CreatePoint(point3d);

            sketchInPlaceBuilder1.SketchOrigin = point;

            sketchInPlaceBuilder1.PlaneOrFace.Value = plane;

            sketchInPlaceBuilder1.Axis.Value = axis;

            NXObject nXObject1;
            nXObject1 = sketchInPlaceBuilder1.Commit();

            Sketch sketch1 = (Sketch)nXObject1;
            NXOpen.Features.Feature feature1;
            feature1 = sketch1.Feature;

            sketch1.Activate(NXOpen.Sketch.ViewReorient.False);

            sketchInPlaceBuilder1.Destroy();

            sketchAlongPathBuilder1.Destroy();

            return sketch1;
        }

        public Arc FilletOnSketch(Session theSession,Line line1,Line line2,Point3d helpPoint1,Point3d helpPoint2, double radius)
        {
            Arc[] fillet;
            SketchConstraint[] constraints1;
            fillet = theSession.ActiveSketch.Fillet(line1, line2, helpPoint1, helpPoint2, radius, NXOpen.Sketch.TrimInputOption.True, NXOpen.Sketch.CreateDimensionOption.False, NXOpen.Sketch.AlternateSolutionOption.False, out constraints1);

            Arc arc = fillet[0];

            return arc;

        }


        #region Csys Creation
        public NXOpen.Features.DatumCsys CsysCreation(Part workPart,Point3d pointOrigin3d,Point3d pointAxis13d,Point3d pointAxis23d)
        {
            NXOpen.Features.Feature nullFeatures_Feature = null;
            NXOpen.Features.DatumCsysBuilder datumCsysBuilder;
            datumCsysBuilder = workPart.Features.CreateDatumCsysBuilder(nullFeatures_Feature);

            Point pointOrigin = workPart.Points.CreatePoint(pointOrigin3d);
            Point pointAxis2 = workPart.Points.CreatePoint(pointAxis13d);
            Point pointAxis1 = workPart.Points.CreatePoint(pointAxis23d);

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



