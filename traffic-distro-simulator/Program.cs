using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace traffic_distro_simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var trafficVolumesPerHour = new List<(int hour, int reqs, Dictionary<(char app, int vm), int> stats)>()
            {
                (08, 230, new Dictionary<(char app, int vm), int>()),  //Hour, 1000's of Requests
                (09, 250, new Dictionary<(char app, int vm), int>()),
                (10, 260, new Dictionary<(char app, int vm), int>()),
                (11, 270, new Dictionary<(char app, int vm), int>()),
                (12, 280, new Dictionary<(char app, int vm), int>()),
                (13, 300, new Dictionary<(char app, int vm), int>()),
                (14, 310, new Dictionary<(char app, int vm), int>()),
                (15, 320, new Dictionary<(char app, int vm), int>()),
                (16, 320, new Dictionary<(char app, int vm), int>()),
                (17, 270, new Dictionary<(char app, int vm), int>()),
                (18, 260, new Dictionary<(char app, int vm), int>()),
                (19, 240, new Dictionary<(char app, int vm), int>()),
                (20, 210, new Dictionary<(char app, int vm), int>()),
                (21, 180, new Dictionary<(char app, int vm), int>()),
                (22, 150, new Dictionary<(char app, int vm), int>())
            };

            var numVMs = 5;
            var appTrafficRatios = new List<(char name, double min, double max)>()
            {
                //Ensure boundaries are contiguous if you add more apps
                ('C', 0, 0.01),     //App Service A - Recieves approx 1% of traffic by volume
                ('F', 0.01, 0.3),   //App Service B - Recieves approx 24% of traffic by volume
                ('T', 0.3, 1)      //App Service B - Recieves approx 75% of traffic by volume
            };

            var random = new Random();

            foreach(var setting in trafficVolumesPerHour)
            {
                for(int i = 0; i < setting.reqs * 1000; i++)
                {
                    var whichApp = random.NextDouble();

                    var app = appTrafficRatios.FirstOrDefault(c => c.min <= whichApp && whichApp < c.max).name;

                    var whichVM = random.Next(1, 100000);

                    var vm = whichVM % numVMs;

                    var key = (app, vm);

                    if (setting.stats.ContainsKey(key)) setting.stats[key]++;
                    else setting.stats.Add(key, 1);
                }
            }

            WriteToCsv(trafficVolumesPerHour);
        }

        private static void WriteToCsv(List<(int hour, int reqs, Dictionary<(char app, int vm), int> stats)> trafficVolumesPerHour)
        {
            using (var sw = new StreamWriter("output.csv"))
            {
                sw.WriteLine("\"DateTime\",\"Application\",\"Virtual Machine\",\"# of Requests\"");

                foreach(var setting in trafficVolumesPerHour)
                {
                    foreach(var key in setting.stats.Keys)
                    {
                        sw.WriteLine("\"2020-07-23 {0:00}:00:00.000\",\"APP-{1}\",\"VM-{2}\",\"{3}\"", setting.hour, key.app, key.vm + 1, setting.stats[key]);
                    }
                }
            }
        }
    }
}
