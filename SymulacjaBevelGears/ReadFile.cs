using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;


    class ReadFile
    {
        //Properites
        public double d0 { get; set; }
        public double alf2 { get; set; }
        public double w2 { get; set; }
        public double r02 { get; set; }
        public double alf2wk { get; set; }
        public double alf2wp { get; set; }
        public double u { get; set; }
        public double q { get; set; }
        public double z { get; set; }
        public double kierunekLinii { get; set; }
        public double re { get; set; }
        public double del { get; set; }
        public double delA { get; set; }
        public double delF { get; set; }
        public double B { get; set; }
        public double lwa { get; set; }
        public double lwf { get; set; }
        public double xp { get; set; }
        public double xb { get; set; }
        public double mat2 { get; set; }
        public double hae { get; set; }
        public double hfe { get; set; }
        public double katIteracji { get; set; }
        public double kroki { get; set; }
        public double iOdt { get; set; }
        public double A { get; set; }
        public double r02wk { get; set; }
        public double r02wp { get; set; }
        public double del2 { get; set; }
        public double tiltJ { get; set; }
        public double tiltI { get; set; }
        public double wyp { get; set; }
        public double kM { get; set; }
        public double wys { get; set; }




        //---- DO EDYCJI ------//
        static string directory = @"C:\PAWEL\Project\Bevel_Pattern_Check\Symulacja_NX_nowe\katalog_vs_2010\dane.txt";
        //-------   -----------//


        public void ReadData()
        {
            // Creating FileStream Objects
            List<double> list = new List<double>();
            FileStream fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);

            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                string[] splitLines;
                double dupa;
                while ((line = streamReader.ReadLine()) != null)
                {
                    splitLines = line.Split('=');
                    MessageBox.Show(splitLines[1]);
                    dupa = Double.Parse(splitLines[1].Replace(".",","));
                    list.Add(dupa);
                }
            }

            //Attach vales to properties
            try
            {
                if (list.Count == 34)
                {
                    d0 = list[0];
                    alf2 = list[1];
                    w2 = list[2];
                    r02 = list[3];
                    alf2wk = list[4];
                    alf2wp = list[5];
                    u = list[6];
                    q = list[7];
                    z = list[8];
                    kierunekLinii = list[9];
                    re = list[10];
                    del = list[11];
                    delA = list[12];
                    delF = list[13];
                    B = list[14];
                    lwa = list[15];
                    lwf = list[16];
                    xp = list[17];
                    xb = list[18];
                    mat2 = list[19];
                    hae = list[20];
                    hfe = list[21];
                    katIteracji = list[22];
                    kroki = list[23];
                    iOdt = list[24];
                    A = list[25];
                    r02wk = list[26];
                    r02wp = list[27];
                    del2 = list[28];
                    tiltI = list[29];
                    tiltJ = list[30];
                    wyp = list[31];
                    kM = list[32];
                    wys = list[33];
                }
                else { throw new IndexOutOfRangeException("Niepoprawna liczba argumentów w pliku inputowym"); }
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }
    }

