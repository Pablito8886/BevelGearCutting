using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
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

        public Point3d Point3dByIntersection(Part workPart, Line line1, Line line2)
        {
            Unit nullUnit = null;
            Expression expression = workPart.Expressions.CreateSystemExpressionWithUnits("50", nullUnit);
            Scalar scalar4 = workPart.Scalars.CreateScalarExpression(expression, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Point point1 = workPart.Points.CreatePoint(line1, scalar4, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Point point2 = workPart.Points.CreatePoint(line2, scalar4, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point pointInter = workPart.Points.CreatePoint(line1, line2, point1, point2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point3d outPointInter = new Point3d(pointInter.Coordinates.X, pointInter.Coordinates.Y, pointInter.Coordinates.Z);
            
            return outPointInter;
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

        public Axis AxisCreation(Part workPart,Point3d point3d,Vector3d vector3d)
        {
                        Direction direction1 = workPart.Directions.CreateDirection(point3d, vector3d, NXOpen.SmartObject.UpdateOption.WithinModeling);
                        Point nullPoint = null;
                        Axis axis = workPart.Axes.CreateAxis(nullPoint, direction1, SmartObject.UpdateOption.WithinModeling);
            return axis;
        }

        public Axis AxisCreation(Part workPart,Point3d point3d,Line line)
        {
                        Sense sense=new Sense();
                        Direction direction1 = workPart.Directions.CreateDirection(line,sense, NXOpen.SmartObject.UpdateOption.WithinModeling);
                        Point nullPoint = null;
                        Axis axis = workPart.Axes.CreateAxis(nullPoint, direction1, SmartObject.UpdateOption.WithinModeling);
            return axis;
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

        public Body RotationBodyAboutAxis(Part workPart,Body body,Axis axisOfRotation,double angleOfRotation)
        {
            System.Globalization.CultureInfo USculture = new System.Globalization.CultureInfo("en-US");

            NXOpen.Features.MoveObject nullFeatures_MoveObject = null;
            NXOpen.Features.MoveObjectBuilder moveObjectBuilder11;
            moveObjectBuilder11 = workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeatures_MoveObject);
            NXOpen.GeometricUtilities.OrientXpressBuilder orientXpressBuilder2;
            orientXpressBuilder2 = moveObjectBuilder11.TransformMotion.OrientXpress;
            orientXpressBuilder2.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;
            orientXpressBuilder2.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;
            Point3d manipulatororigin1;
            manipulatororigin1 = moveObjectBuilder11.TransformMotion.ManipulatorOrigin;
            Matrix3x3 manipulatormatrix1;
            manipulatormatrix1 = moveObjectBuilder11.TransformMotion.ManipulatorMatrix;
            moveObjectBuilder11.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Angle;
            bool added1 = moveObjectBuilder11.ObjectToMoveObject.Add(body);

            moveObjectBuilder11.TransformMotion.AngularAxis = axisOfRotation;
            moveObjectBuilder11.TransformMotion.DistanceValue.RightHandSide = "10";
            moveObjectBuilder11.TransformMotion.Angle.RightHandSide = angleOfRotation.ToString(USculture);

            NXObject movedpinion1;
            movedpinion1 = moveObjectBuilder11.Commit();
            NXObject[] outBodies = moveObjectBuilder11.GetCommittedObjects();
            moveObjectBuilder11.Destroy();

            Body bodyOut = (Body)outBodies[0];

            return bodyOut;
        }

        public Body MoveBodyAlongVector(Part workPart, Body body, Direction direction, double distance)
        {
            System.Globalization.CultureInfo USculture = new System.Globalization.CultureInfo("en-US");
            
            NXOpen.Features.MoveObject nullFeatures_MoveObject = null;
            NXOpen.Features.MoveObjectBuilder moveObjectBuilder1;
            moveObjectBuilder1 = workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeatures_MoveObject);

            moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            moveObjectBuilder1.TransformMotion.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            moveObjectBuilder1.TransformMotion.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            Point3d manipulatororigin1;
            manipulatororigin1 = moveObjectBuilder1.TransformMotion.ManipulatorOrigin;

            Matrix3x3 manipulatormatrix1;
            manipulatormatrix1 = moveObjectBuilder1.TransformMotion.ManipulatorMatrix;

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Distance;

            moveObjectBuilder1.TransformMotion.DistanceVector = direction;

            bool added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body);

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = distance.ToString(USculture);

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            Body bodyOut = (Body)objects1[0];

            return bodyOut;
        }

        public Body Revolve(Part workPart,Axis axisRotation,Sketch sketch, Line lineSketch,Point3d point3dSketch,double degStart,double degEnd)
        {
                        //RevolveBulidier
                        NXOpen.Features.Feature nullFeature1 = null;
                        NXOpen.Features.RevolveBuilder revolveBuilder1 = workPart.Features.CreateRevolveBuilder(nullFeature1);

                        //Section
                        Section section1 = workPart.Sections.CreateSection(0.02, 0.02, 0.02);

                        revolveBuilder1.Section = section1;
                        revolveBuilder1.Axis = axisRotation;
                        revolveBuilder1.Tolerance = 0.02;
                        section1.DistanceTolerance = 0.02;
                        section1.ChainingTolerance = 0.02;

                        //Line assignment
                         NXOpen.Features.Feature[] features1 = new NXOpen.Features.Feature[1];
                         NXOpen.Features.SketchFeature sketchFeature1 = (NXOpen.Features.SketchFeature)sketch.Feature;
                         features1[0] = sketchFeature1;

                         CurveFeatureRule curveFeatureRule1;
                         curveFeatureRule1 = workPart.ScRuleFactory.CreateRuleCurveFeature(features1);

                         section1.AllowSelfIntersection(true);
                         
                         SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                         rules1[0] = curveFeatureRule1;
                         NXObject nullNXObject = null;
                         
                         //Selection
                         section1.AddToSection(rules1, lineSketch, nullNXObject, nullNXObject, point3dSketch, NXOpen.Section.Mode.Create, false);
                         
                         revolveBuilder1.Axis = axisRotation;
                         
                         //Sheet Body selection
                         revolveBuilder1.FeatureOptions.BodyType = NXOpen.GeometricUtilities.FeatureOptions.BodyStyle.Sheet;
                         revolveBuilder1.Section = section1;
                         
                         //Ustawienia
                         revolveBuilder1.Limits.StartExtend.Value.RightHandSide = degStart.ToString();
                         revolveBuilder1.Limits.EndExtend.Value.RightHandSide = degEnd.ToString();
                         revolveBuilder1.ParentFeatureInternal = false;

                         NXObject feature_revolve1 = revolveBuilder1.CommitFeature();

                         NXOpen.Features.BodyFeature bodyFeature1 = (NXOpen.Features.BodyFeature)feature_revolve1;
                         Body[] body = bodyFeature1.GetBodies();
                         return body[0];
        }

    }



