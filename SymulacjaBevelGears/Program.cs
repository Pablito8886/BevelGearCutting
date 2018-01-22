using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpenUI;


    class Program
    {

        private static UI theUI;
        public static Session theSession;
        public static UFSession theUFSession;



        public static void Main(string[] args)
        {
            //Starting UG session
            theSession = Session.GetSession();
            theUFSession = UFSession.GetUFSession();

            Part workPart = theSession.Parts.Work;

            //open the LOG listening in NX
            ListingWindow lw = theSession.ListingWindow;
            lw.Open();

            

            //Calling a ReadFile class
            ReadFile readFile = new ReadFile();
            readFile.ReadData();

            lw.WriteLine("Srednica glowicy frezowej: "+ readFile.d0);
            lw.WriteLine("Sredni kat zarysu nozy glowicy frezowej: "+readFile.alf2);
            lw.WriteLine("Szerokosc rozstawienia nozy glowicy frezowej: "+readFile.w2);
            lw.WriteLine("Promien zaaokraglenia wierzcholka noza: "+readFile.r02);
            lw.WriteLine("Kat zarysu noza na stronie wkleslej: "+readFile.alf2wk);
            lw.WriteLine("Kat zarysu noza na stronie wypuklej: "+readFile.alf2wp);
            lw.WriteLine("Ustawienie promieniowe glowicy: " + readFile.u);
            lw.WriteLine("ustawienie katowe glowicy: " + readFile.q);
            lw.WriteLine("liczba zebow: " + readFile.z);
            lw.WriteLine("kierunek pochylenia linii zeba: " + readFile.kierunekLinii);
            lw.WriteLine("dlugosc tworzacej stozka podzialowego: " + readFile.re);
            lw.WriteLine("kat stozka podzialowego: " + readFile.del);
            lw.WriteLine("kat stozka glow: " + readFile.delA);
            lw.WriteLine("kat stozka stop: " + readFile.delF);
            lw.WriteLine("szerokosc wienca zebatego: " + readFile.B);
            lw.WriteLine("odleglosc wierzcholka stozka podzialowego od wierzcholkow glow: " + readFile.lwa);
            lw.WriteLine("odleglosc wierzcholka stozka podzialowego od wierzcholkow stop: " + readFile.lwf);
            lw.WriteLine("przesuniecie otoczki do plaszczyzny kolyski po osi otoczki : " + readFile.xp);
            lw.WriteLine("przesuniecie otoczki do plaszczyzny kolyski w kierunku do niej normalnym : " + readFile.xb);
            lw.WriteLine("powiekszenie otoczki : " + readFile.mat2);
            lw.WriteLine("wysokosc glowy zeba : " + readFile.hae);
            lw.WriteLine("wysokosc stopy zeba : " + readFile.hfe);
            lw.WriteLine("jednostkowy kad symulacji oddtaczania : " + readFile.katIteracji);
            lw.WriteLine("liczba krokow symulacji : " + readFile.kroki);
            lw.WriteLine("przelozenie oddataczania : " + readFile.iOdt);
            lw.WriteLine("przesuniecie hipoidalne : " + readFile.A);
            lw.WriteLine("promien zaaokroglenia noza strony wkleslej : " + readFile.r02wk);
            lw.WriteLine("promien zaaokroglenia noza strony wypuklej : " + readFile.r02wp);
            lw.WriteLine("kat skrecenia wrzeciona przedmioto obrabianego : " + readFile.del2);
            lw.WriteLine("obrot glowicy o kat J wynikajacy z TILT-u : " + readFile.tiltJ);
            lw.WriteLine("obrot glowicy o kat I wynikajacy z TILT-u : " + readFile.tiltI);
            lw.WriteLine("strona obrabiana ([1]-wypukla , [0]-wklesla : " + readFile.wyp);
            lw.WriteLine("wspolczynnik modyfikacji oddtaczania : " + readFile.kM);
            lw.WriteLine("wysokosc ostrzy glowicy : " + readFile.wys);
        }

        public static int GetUnloadOption(string dummy) { return (int)Session.LibraryUnloadOption.Immediately; }
        
    }

