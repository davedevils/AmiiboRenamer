using AmiiboJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Amiibo_Renamer
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please Drag and Drop file to rename.");
                Console.WriteLine("Press Enter for close the program");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Loading ...");
            DllAmiiboDB();

            string contents = File.ReadAllText("amiibo.json");
            var AmiiboJson = AmiiboHelp.FromJson(contents);
            
            long NumberOfFile = args.LongCount();

            for (int Number = 0; Number < NumberOfFile; Number++)
            {
                if (File.Exists(args[Number]))
                {
                    /*
                     *  AmiiBot ID
                     * Game & Character ID      - 2 byte    - 00 00
                     * Character variant        - 1 byte    - 00
                     * Amiibo Figure Type       - 1 byte    - 00
                     * Amiibo Model Number      - 2 byte    - 00 00
                     * Amiibo Series            - 1 byte    - 00
                     * 0x02                     - 1 byte    - 00
                     */
                    byte[] fileBytes = File.ReadAllBytes(args[Number]);
                    byte[] IdBytes = new byte[8];
                    Buffer.BlockCopy(fileBytes, 0x54, IdBytes, 0, 8);
                    string AmiiboId = ByteArrayToString(IdBytes);

                    string CharacterId = AmiiboId.Substring(0, 4);

                    string TypeId = AmiiboId.Substring(6, 2);

                    string GameSeriesId = AmiiboId.Substring(8, 3);

                    string AmiiboSeriesId = AmiiboId.Substring(12, 2);

                    string Character, GameSeries, AmiiboSeries, Type = "UNKNOW";

                    Amiibo AmiiboFounded;
                    if (AmiiboJson.Amiibos.TryGetValue("0x" + AmiiboId, out AmiiboFounded))
                    {
                        Console.WriteLine("------------------------------------------------------------");
                        Console.WriteLine("ID Found : [" + AmiiboId + "]");
                        Console.WriteLine("Name: " + AmiiboFounded.Name);

                        AmiiboJson.AmiiboSeries.TryGetValue("0x" + AmiiboSeriesId, out AmiiboSeries);
                        AmiiboJson.GameSeries.TryGetValue("0x" + GameSeriesId, out GameSeries);
                        AmiiboJson.Characters.TryGetValue("0x" + CharacterId, out Character);
                        AmiiboJson.Types.TryGetValue("0x" + TypeId, out Type);

                        Console.WriteLine("AmiiboSerie: " + AmiiboSeries);
                        Console.WriteLine("Serie: " + GameSeries);
                        Console.WriteLine("Character: " + Character);
                        Console.WriteLine("Type: " + Type);
                        Console.WriteLine(" -------- ");

                        if (GameSeries == "" || GameSeries == null)
                            GameSeries = AmiiboSeries;

                        string NewName = AmiiboFounded.Name.Replace("/", "-") + " (" + GameSeries + " - " + Character.Replace("/", "-") + ") [" + AmiiboId.ToUpper() + "].bin";

                        FileInfo file = new FileInfo(args[Number]);

                        if (NewName.Trim() != file.Name)
                        {
                            try
                            {
                                if (File.Exists(file.Directory.FullName + "\\" + NewName))
                                {
                                    System.IO.File.Delete(file.Directory.FullName + "\\" + NewName);
                                }
                                System.IO.File.Move(args[Number], file.Directory.FullName + "\\" + NewName);
                                Console.WriteLine("Renamed to : " + NewName);
                            }
                            catch (System.IO.IOException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }

                        Console.WriteLine("------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(AmiiboId + " is not existing id ... Invalid amiibo ?");
                    }
                }
                else
                {
                    Console.WriteLine(args[Number] + " not exist");
                }


            }

            try
            {
                System.IO.File.Delete("amiibo.json");
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Finish !\n");
            Console.WriteLine("Press Enter for close the program");
            Console.ReadLine();
        }


        public static void DllAmiiboDB()
        {
            string name = "amiibo.json";

            if (File.Exists(name))
                File.Delete(name);

            using (var client = new WebClient())
            {
                string tmpurl = "https://raw.githubusercontent.com/davedevils/AmiiboAPI/master/database/amiibo.json";
                client.DownloadFile(tmpurl, name);
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
