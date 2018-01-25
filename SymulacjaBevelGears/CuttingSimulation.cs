using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;


    class CuttingSimulation
    {
        public void CuttingSGM(Part workPart, Session theSession)
        {
            ToolGeometry glowica = new ToolGeometry();
            Body cutter=glowica.ToolSketch(workPart, theSession);

            BlankGeometry otoczka = new BlankGeometry();
            Body bodyBlank=otoczka.BlankGeometryCreation(workPart, theSession);
        }
    }

