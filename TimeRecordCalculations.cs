//Project for a client: calculating time records, based upon registries of entries and exits
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp3
{


    class Program
    {
        static void Main(string[] args)
        {
            //parameters: SkippingNextEntry: skipping timpespan, shiftgap - parameter of the shift, inn - code of entry, outt - code of exit
            TimeSpan SkippingNextEntry = TimeSpan.Parse("0.13:00:00.00");
            TimeSpan ShiftGap = TimeSpan.Parse("0.06:00:00.00");
            string sFileContents = "";
            FileStream FS;
            string inn = "62";
            string outt = "63";
            string smokein = "12";
            string smokeout = "13";
            //finding a file name
            string path = @"C:\Users\Burzol\Desktop\praca Bapiego\CVisualSzarpie\";
            string filename = "PREvents2.csv";
            //getting data in
            using (StreamReader oStreamReader = new StreamReader(File.OpenRead(path + filename)))
            {
                sFileContents = oStreamReader.ReadToEnd();
            }
            List<string[]> oCsvList = new List<string[]>();
            string[] sFileLines = sFileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string sFileLine in sFileLines)
            {
                oCsvList.Add(sFileLine.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }
            List<DataIn> CSV = new List<DataIn>();


            int max = oCsvList.Count;
            for (int i = 0; i < max; i++)
            {
                CSV.Add(new DataIn() { VIN = Convert.ToString(i), Date = DateTime.Parse(oCsvList[i][1] + "T" + oCsvList[i][2]), Type = oCsvList[i][3], IdCard = oCsvList[i][0] }
               );
            }

            // getting data from working
            // working.csv file structure: Id; VIN; IdCard; DateIn; Type; Shift; Paired; IdPaired; DateOut; IdCard; Worked;
            filename = "working.csv";
            using (StreamReader oStreamReader = new StreamReader(File.OpenRead(path + filename)))
            {
                sFileContents = oStreamReader.ReadToEnd();
            }
            sFileLines = sFileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            oCsvList.Clear();
            foreach (string sFileLine in sFileLines)
            {
                oCsvList.Add(sFileLine.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }
            List<Working> wrk = new List<Working>();
            max = oCsvList.Count - 1;
            for (int i = 0; i < max; i++)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dat1 = DateTime.ParseExact(oCsvList[i][3], "yyyy-MM-dd HH:mm:ss", provider);
                DateTime dat2 = DateTime.ParseExact(oCsvList[i][8], "yyyy-MM-dd HH:mm:ss", provider);
                wrk.Add(new Working() { Id = Convert.ToString(oCsvList[i][0]), DateIn = dat1, Type = Convert.ToString(oCsvList[i][4]), Shift = Convert.ToString(oCsvList[i][5]), Paired = Boolean.Parse(oCsvList[i][6]), DateOut = dat2, IdCard = Convert.ToString(oCsvList[i][2]), VIN = Convert.ToString(oCsvList[i][1]), IdPaired = Convert.ToString(oCsvList[i][7]), Worked = Convert.ToDouble(oCsvList[i][9]) }
                    );

            }
            //end getting data from files

            //finding the last edited
            int j = 0;
            j = wrk.Count;
            //adding new lines from file to working from csv
            foreach (DataIn loo in CSV.Skip(j))
            {
                bool add = true;
                string idek3 = string.Concat(loo.Date.Year, loo.Date.Month, loo.Date.Day, "#", loo.Date.Hour, loo.Date.Minute, "#", loo.IdCard, "#", loo.Type);
                //sprawdzanie czy tego ju� nie ma
                var query = wrk.Where(p => p.Id == idek3);
                foreach (var carss in query)
                { add = false; }
                //if next entry is without exit and it is no more than 13 hours
                //if next entry is without exit and it is more than 13 hours then we skip first entry
                if (loo.Type == inn && add)
                {
                    var queryy = wrk.Where(p => p.DateIn >= loo.Date.Add(-SkippingNextEntry) && p.DateIn < loo.Date && p.Type == inn);
                    foreach (var car in queryy)
                    {
                        add = false;
                        var quer = wrk.Where(p => p.DateIn >= loo.Date.Add(-SkippingNextEntry) && p.DateIn < loo.Date && p.DateIn > car.DateIn && p.Type == "63");
                        foreach (var cars in quer)
                        { add = true; }
                    }
                }
                //if there is next exit without entry
                if (loo.Type == outt && add)
                {
                    query = wrk.Where(p => p.IdCard == loo.IdCard);
                    foreach (var car in query)
                    {
                        if (car.Type == outt)
                        { add = false; }
                        else
                        { add = true; }
                    }

                }

                if (add)
                {
                    // Console.WriteLine(idek3);
                    wrk.Add(new Working() { VIN = Convert.ToString(wrk.Count + 1), DateIn = loo.Date, Id = idek3, Type = loo.Type, IdCard = loo.IdCard, DateOut = DateTime.Parse("1999-12-19"), Shift = "NONE", Paired = false, Worked = 0, IdPaired = " " }
                        );
                }
            }

            //trying to pair 63 with 62 and calculate the work
            //wrk.Reverse();
            foreach (Working loo in wrk)
            {
                //going out
                if (loo.Type == outt && loo.Paired == false)
                {
                    foreach (Working upd in wrk)
                    {

                        //if its DateOut we should find first unpaired DateIn nad pair it with this DateIn
                        if (loo.IdCard == upd.IdCard && upd.Type == inn && upd.Paired == false && loo.DateIn >= upd.DateIn && loo.Paired == false)
                        {
                            loo.IdPaired = upd.Id;
                            upd.DateOut = loo.DateIn;
                            upd.Paired = true;
                            loo.Paired = true;
                            upd.IdPaired = loo.Id;
                            //since we found DateOut we can calculate working time
                            TimeSpan myTime = upd.DateOut - upd.DateIn;
                            double totalminutes = (Math.Round(myTime.TotalMinutes / 60, 2));
                            upd.Worked = totalminutes;
                            break;
                        }
                    }
                }
                //smoke break
                if (loo.Type == smokeout && loo.Paired == false)
                {
                    foreach (Working upd in wrk)
                    {
                    if(loo.IdCard == upd.IdCard && upd.Type == smokein && upd.Paired == false && loo.DateIn >= upd.DateIn && loo.Paired == false)
                    {
                        loo.IdPaired = upd.Id;
                        upd.DateOut = loo.DateIn;
                        upd.Paired = true;
                        loo.Paired = true;
                        upd.IdPaired = loo.Id;
                        //since we found DateOut we can calculate working time
                        TimeSpan myTime = upd.DateOut - upd.DateIn;
                        double totalminutes = (Math.Round(myTime.TotalMinutes / 60, 2));
                        upd.Worked = totalminutes;
                        break;
                    }
                    }
                }
            }

            //here we have to group calculations to one shift - asuming that one day is a one shift

            foreach (Working loo in wrk)
            {
                if (loo.Type == inn && loo.Paired == true && loo.Shift == "NONE")
                {
                    loo.Shift = loo.DateIn.Date.ToString();
                    TimeSpan day = TimeSpan.Parse("1.00:00:00.00");
                    var queryy = wrk.Where(p => p.DateIn.Date == loo.DateIn.Add(-day).Date && p.DateIn > loo.DateIn.Add(-ShiftGap) && p.Id != loo.Id && p.Shift != "NONE");
                    foreach (var car in queryy)
                    {
                        Console.WriteLine(loo.DateIn + "/" + car.DateIn);
                        loo.Shift = car.Shift;
                    }
                }
            }


            /*
             seeing the data
            foreach (Working loo in wrk)
            {
                if(loo.Paired)
                {
                Console.WriteLine("{0} Daty: {5}-{6} {1}:{2} {3}:{4} {7}", loo.IdCard, loo.DateIn.Hour,loo.DateIn.Minute,loo.DateOut.Hour,loo.DateOut.Minute,loo.DateIn.Day,loo.DateOut.Day,loo.Worked);
                }
            }
            */

            // saving to files
            filename = "Wynik " + DateTime.Today.Year + "-" + DateTime.Today.Month + ".csv";
            using (FileStream fs = File.Create(path + filename)) ;

            FS = new FileStream(path + filename, FileMode.Create, FileAccess.Write);
            StreamWriter FW = new StreamWriter(FS);

            foreach (Working loo in wrk)
            {
                if (loo.Type == inn & loo.Paired == true)
                {
                    FW.WriteLine("{0};{1};{2}", loo.IdCard, loo.Shift, loo.Worked);
                }
            }


            FW.Close();
            FS.Close();

            FS = new FileStream(path + "working.csv", FileMode.Create, FileAccess.Write);
            StreamWriter FW2 = new StreamWriter(FS);
            foreach (Working loo in wrk)
            {
                FW2.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", loo.Id, loo.VIN, loo.IdCard, loo.DateIn, loo.Type, loo.Shift, loo.Paired, loo.IdPaired, loo.DateOut, loo.Worked);
            }
            FW2.Close();
            FS.Close();

            filename = "Wynik przerwa papierosowa" + DateTime.Today.Year + "-" + DateTime.Today.Month + ".csv";

            FS = new FileStream(path + filename, FileMode.Create, FileAccess.Write);
            StreamWriter FW3 = new StreamWriter(FS);
            foreach (Working loo in wrk)
            {
                if(loo.Type == smokein && loo.Paired)
                {
                    FW3.WriteLine("{0};{1};{2};{3}", loo.IdCard, loo.DateIn, loo.DateOut, loo.Worked);
                }
            }

            FW3.Close();
            FS.Close();

             // Console.ReadLine();

        }
    }

    class DataIn
    {
        public string IdCard { get; set; }
        public DateTime Date { get; set; }
        public string VIN { get; set; }
        public string Type { get; set; }
    }

    class Working
    {
        public string VIN { get; set; }
        public string Id { get; set; }
        public double Worked { get; set; }
        public DateTime DateIn { get; set; }
        public string Type { get; set; }
        public string Shift { get; set; }
        public Boolean Paired { get; set; }
        public DateTime DateOut { get; set; }
        public string IdCard { get; set; }
        public string IdPaired { get; set; }
    }
}
