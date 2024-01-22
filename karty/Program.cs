namespace karty {
    internal class Program {
        static void Main(string[] args) {

            Random random = new Random();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //0 - červené ♥
            //1 - listy
            //2 - žaludy
            //3 - kule

            /*
            ┌───────────┐┐┐┐
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            └───────────┘┘┘┘
            */



            string[] znaky = { "❤️", "🍀", "🌰", "🎱" };
            int sirka = 10;   //10,8,4
            int vyska = 7;   //7 ,5,3
            int rychlosthrani = 500; //ms


            int[] balicek_karet = new int[(4 * 8) * 2];

            for (int n = 0; n < balicek_karet.Length / 2; n++) {
                balicek_karet[n * 2] = (n / 8);
                balicek_karet[n * 2 + 1] = (n % 8) + 7;
            }
            int[] lizaci_balicek = Enumerable.Repeat(1, 32).ToArray();
            int[] odhazovaci_balicek = Enumerable.Repeat(0, 32).ToArray();
            int[] hrac1_ruka = Enumerable.Repeat(0, 32).ToArray();
            int[] hrac2_ruka = Enumerable.Repeat(0, 32).ToArray();
            int kartanastole;
            int pozicekurzoru = 0;
            int nahodnakarta;
            nahodnakarta = random.Next(0, lizaci_balicek.Length);
            while (lizaci_balicek[nahodnakarta] == 0) { nahodnakarta = random.Next(0, lizaci_balicek.Length); }
            lizaci_balicek[nahodnakarta] = 0;
            kartanastole = nahodnakarta;
            odhazovaci_balicek[nahodnakarta] = 1;

            LizniKartu(lizaci_balicek, hrac1_ruka, 4);
            LizniKartu(lizaci_balicek, hrac2_ruka, 4);


            int[] kartakurzor = new int[2];
            int predtimkaret;
            bool muzehrathrac1 = true;
            bool muzehrathrac2 = true;


            while (PocetKaretVRuce(hrac1_ruka) != 0 && PocetKaretVRuce(hrac2_ruka) != 0) {
                if (PocetKaretVRuce(hrac1_ruka) > 10 || PocetKaretVRuce(hrac2_ruka) > 10) { sirka = 4; vyska = 3; } else if (PocetKaretVRuce(hrac1_ruka) > 5 || PocetKaretVRuce(hrac2_ruka) > 5) { sirka = 8; vyska = 5; } else { sirka = 10; vyska = 7; }



                predtimkaret = PocetKaretVRuce(hrac2_ruka);

                Console.Clear();
                VypisStolu(kartanastole, balicek_karet, hrac1_ruka, hrac2_ruka, lizaci_balicek, vyska, sirka, znaky, pozicekurzoru, odhazovaci_balicek);
                while (PocetKaretVRuce(hrac2_ruka) == predtimkaret && muzehrathrac1) {
                    PokudJePotrebaMichej(lizaci_balicek, odhazovaci_balicek, kartanastole);
                    predtimkaret = PocetKaretVRuce(hrac2_ruka);
                    kartakurzor = NactiSipku(pozicekurzoru, PocetKaretVRuce(hrac2_ruka), kartanastole, balicek_karet, hrac2_ruka, lizaci_balicek, odhazovaci_balicek);
                    pozicekurzoru = kartakurzor[0];
                    kartanastole = kartakurzor[1];
                    Console.Clear();
                    VypisStolu(kartanastole, balicek_karet, hrac1_ruka, hrac2_ruka, lizaci_balicek, vyska, sirka, znaky, pozicekurzoru, odhazovaci_balicek);

                }
                PokudJePotrebaMichej(lizaci_balicek, odhazovaci_balicek, kartanastole);
                if (PocetKaretVRuce(hrac2_ruka) < predtimkaret) {
                    if (kartanastole % 8 == 0) { LizniKartu(lizaci_balicek, hrac1_ruka, 2); muzehrathrac2 = false; } else if (kartanastole % 8 == 7) { muzehrathrac2 = false; } else if (kartanastole % 8 == 5) { kartanastole = (VyberKartyProHrace(znaky)); }
                }
                Console.Clear();
                VypisStolu(kartanastole, balicek_karet, hrac1_ruka, hrac2_ruka, lizaci_balicek, vyska, sirka, znaky, pozicekurzoru, odhazovaci_balicek);
                muzehrathrac1 = true;



                if (PocetKaretVRuce(hrac2_ruka) != 0 && muzehrathrac2) {
                    Thread.Sleep(rychlosthrani);

                    predtimkaret = PocetKaretVRuce(hrac1_ruka);
                    for (int i = 0; i < PocetKaretVRuce(hrac1_ruka) * 2; i++) {
                        nahodnakarta = random.Next(0, PocetKaretVRuce(hrac1_ruka));
                        if (MohuZahratKartu(kartanastole, JakaKartaJeOznacena(hrac1_ruka, nahodnakarta))) {
                            PokudJePotrebaMichej(lizaci_balicek, odhazovaci_balicek, kartanastole);
                            kartanastole = JakaKartaJeOznacena(hrac1_ruka, nahodnakarta);
                            odhazovaci_balicek[JakaKartaJeOznacena(hrac1_ruka, nahodnakarta)] = 1;
                            hrac1_ruka[JakaKartaJeOznacena(hrac1_ruka, nahodnakarta)] = 0;
                            if (kartanastole % 8 == 0) { LizniKartu(lizaci_balicek, hrac2_ruka, 2); muzehrathrac1 = false; } else if (kartanastole % 8 == 7) { muzehrathrac1 = false; } else if (kartanastole % 8 == 5) { kartanastole = VyberKartyProPc(hrac1_ruka); }
                            break;
                        }
                    }
                    if (predtimkaret == PocetKaretVRuce(hrac1_ruka)) {
                        PokudJePotrebaMichej(lizaci_balicek, odhazovaci_balicek, kartanastole);
                        LizniKartu(lizaci_balicek, hrac1_ruka, 1);
                    }
                    Console.Clear();
                    VypisStolu(kartanastole, balicek_karet, hrac1_ruka, hrac2_ruka, lizaci_balicek, vyska, sirka, znaky, pozicekurzoru, odhazovaci_balicek);
                }
                muzehrathrac2 = true;
            }

            EndingScreen(hrac2_ruka);
        }

        static void EndingScreen(int[] hrac2_ruka, string? message = null) {
            Console.WriteLine();
            if (message == null) {
                if (hrac2_ruka.Sum() == 0) {
                    Console.WriteLine("You win!");
                } else {
                    Console.WriteLine("You lose!");
                }
            } else {
                Console.WriteLine(message);
            }
            Console.WriteLine("\nPress SPACE to exit");
            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) { }
            Environment.Exit(0);
        }

        static string StvorKarty(int znak, int cislo, string[] znaky, int sirka, int vyska, bool vibrana_karta) {

            string symbol;
            if (cislo == 10) { symbol = "X"; } else if (cislo == 11) { symbol = "J"; } else if (cislo == 12) { symbol = "Q"; } else if (cislo == 13) { symbol = "K"; } else if (cislo == 14) { symbol = "A"; } else { symbol = "" + cislo; }


            string karta = "";
            if (vibrana_karta) {
                karta += ("▄");
                for (int x = 0; x < sirka; x++) { karta += ("▄"); }
                karta += ("▄" + "\n");


                karta += ("█ " + symbol);
                for (int x = 0; x < sirka - 2; x++) {
                    karta += (" ");
                }
                karta += ("█" + "\n");


                for (int y = 0; y < vyska - 2; y++) {
                    karta += ("█");
                    for (int x = 0; x < sirka; x++) {
                        if ((vyska - 2) / 2 == y && (sirka - 2) / 2 == x) { karta += znaky[znak]; x += 2; }
                        karta += (" ");
                    }
                    karta += ("█" + "\n");
                }
                karta += ("█");



                for (int x = 0; x < sirka - 2; x++) {
                    karta += (" ");
                }
                karta += (symbol + " █" + "\n");



                karta += ("▀");
                for (int x = 0; x < sirka; x++) { karta += ("▀"); }
                karta += ("▀");
            } else {
                karta += ("╔");
                for (int x = 0; x < sirka; x++) { karta += ("═"); }
                karta += ("╗" + "\n");


                karta += ("║ " + symbol);
                for (int x = 0; x < sirka - 2; x++) {
                    karta += (" ");
                }
                karta += ("║" + "\n");


                for (int y = 0; y < vyska - 2; y++) {
                    karta += ("║");
                    for (int x = 0; x < sirka; x++) {
                        if ((vyska - 2) / 2 == y && (sirka - 2) / 2 == x) { karta += znaky[znak]; x += 2; }
                        karta += (" ");
                    }
                    karta += ("║" + "\n");
                }
                karta += ("║");



                for (int x = 0; x < sirka - 2; x++) {
                    karta += (" ");
                }
                karta += (symbol + " ║" + "\n");



                karta += ("╚");
                for (int x = 0; x < sirka; x++) { karta += ("═"); }
                karta += ("╝");
            }

            return (karta);

        }
        static int PocetKaretVRuce(int[] ruka) {
            return ruka.Count(i => i == 1);
        }

        static string SpojDvaStringy(string prvni, string druhy) {

            string[] castiA = prvni.Split('\n');
            string[] castiB = druhy.Split('\n');
            string spojeny = "";
            for (int n = 0; n < castiA.Length; n++) {
                spojeny += castiA[n] + castiB[n] + "\n";
            }

            return spojeny.Substring(0, spojeny.Length - 1);
        }

        static void LizniKartu(int[] balicek_karet, int[] kartyhrace, int kolik) {
            Random random = new Random();
            int nahodnakarta = random.Next(0, balicek_karet.Length);
            for (int n = 0; n < kolik; n++) {
                while (balicek_karet[nahodnakarta] == 0) { nahodnakarta = random.Next(0, balicek_karet.Length); }
                balicek_karet[nahodnakarta] = 0;
                kartyhrace[nahodnakarta] = 1;
            }
        }

        static void PokudJePotrebaMichej(int[] lizaci_balicek, int[] odhazovaci_balicek, int kartanastole) {
            if (lizaci_balicek.Sum() == 0) {
                Array.Copy(odhazovaci_balicek, lizaci_balicek, odhazovaci_balicek.Length);
                Array.Fill(odhazovaci_balicek, 0);
                odhazovaci_balicek[kartanastole] = 1;
                lizaci_balicek[kartanastole] = 0;
            }
        }

        static void VypisStolu(int kartanastole, int[] balicek_karet, int[] kartyhrace1, int[] kartyhrace2, int[] kartyvbalicku, int vyska, int sirka, string[] znaky, int pozicekurzoru, int[] odhazovaci_balicek) {



            string jednakarta;
            string ruka = "";
            for (int n = 0; n <= vyska; n++) {
                ruka += "\n";
            }
            for (int n = 0; n < kartyhrace1.Length; n++) {
                if (kartyhrace1[n] == 1) {
                    //jednakarta = StvorKarty(balicek_karet[n * 2], balicek_karet[n * 2 + 1], znaky, sirka, vyska, false);
                    jednakarta = StvorObracenouKartu(vyska, sirka);
                    ruka = SpojDvaStringy(ruka, jednakarta);
                }
            }
            Console.WriteLine(ruka);



            int pocetkaret = 0;
            for (int n = 0; n < kartyvbalicku.Length; n++) {
                if (kartyvbalicku[n] == 1) { pocetkaret++; }
            }
            int pocetkaretnastolku = 0;
            for (int n = 0; n < odhazovaci_balicek.Length; n++) {
                if (odhazovaci_balicek[n] == 1) { pocetkaretnastolku++; }
            }


            string balicekstrana = "┐\n│\n│\n│\n│\n│\n│\n│\n┘";
            string balicek = StvorObracenouKartu(7, 10);

            int pomerbalicku = 4;

            for (int n = 0; n < pocetkaret / pomerbalicku; n++) { balicek = SpojDvaStringy(balicek, balicekstrana); }
            balicek = SpojDvaStringy(balicek, "  \n  \n  \n  \n  \n  \n  \n  \n  ");
            string odhazovacibalicek = StvorKarty(balicek_karet[kartanastole * 2], balicek_karet[kartanastole * 2 + 1], znaky, 10, balicekstrana.Length / 2 - 1, false);
            for (int n = 0; n < pocetkaretnastolku / pomerbalicku - 1; n++) { odhazovacibalicek = SpojDvaStringy(odhazovacibalicek, balicekstrana); }
            Console.WriteLine(SpojDvaStringy(balicek, odhazovacibalicek));


            ruka = "";
            for (int n = 0; ruka.Length <= vyska; n++) {
                ruka += "\n";
            }
            int kolikatakarta = 0;
            for (int n = 0; n < kartyhrace2.Length; n++) {
                if (kartyhrace2[n] == 1) {
                    ruka = SpojDvaStringy(ruka, StvorKarty(balicek_karet[n * 2], balicek_karet[n * 2 + 1], znaky, sirka, vyska, pozicekurzoru == kolikatakarta));
                    kolikatakarta++;
                }
            }
            Console.WriteLine(ruka);

        }

        static int[] NactiSipku(int pozicekurzoru, int pocetKvruce, int kartanastole, int[] balicek_karet, int[] kartyhrac, int[] lizaci_balicek, int[] odhazovaci_balicek) {
            ConsoleKey sipka = Console.ReadKey().Key;
            while (!((sipka == ConsoleKey.RightArrow && pozicekurzoru != pocetKvruce - 1) || (sipka == ConsoleKey.LeftArrow && pozicekurzoru != 0) || sipka == ConsoleKey.Enter || sipka == ConsoleKey.DownArrow)) {
                Console.Write("\b \b");
                sipka = Console.ReadKey().Key;

            }
            if (sipka == ConsoleKey.LeftArrow) { return new int[] { pozicekurzoru - 1, kartanastole }; } else if (sipka == ConsoleKey.RightArrow) { return new int[] { pozicekurzoru + 1, kartanastole }; } else if (sipka == ConsoleKey.Enter) {
                if (MohuZahratKartu(kartanastole, JakaKartaJeOznacena(kartyhrac, pozicekurzoru))) {
                    kartanastole = JakaKartaJeOznacena(kartyhrac, pozicekurzoru);
                    odhazovaci_balicek[(JakaKartaJeOznacena(kartyhrac, pozicekurzoru))] = 1;
                    kartyhrac[JakaKartaJeOznacena(kartyhrac, pozicekurzoru)] = 0;
                    if (pozicekurzoru != 0) {
                        pozicekurzoru--;
                    }


                }
                return new int[] { pozicekurzoru, kartanastole };
            } else if (sipka == ConsoleKey.DownArrow) {
                PokudJePotrebaMichej(lizaci_balicek, odhazovaci_balicek, kartanastole);
                LizniKartu(lizaci_balicek, kartyhrac, 1);
                return new int[] { pozicekurzoru, kartanastole };
            } else { return new int[] { pozicekurzoru, kartanastole }; }

        }

        static bool MohuZahratKartu(int kartanastole, int hranakarta) {
            if (hranakarta % 8 == 5) { return true; }
            if (hranakarta % 8 == kartanastole % 8) { return true; }
            if (hranakarta / 8 == kartanastole / 8) { return true; } else { return false; }

        }

        static int JakaKartaJeOznacena(int[] kartyhrace, int pozicekurzoru) {
            int vysledek = 0;
            pozicekurzoru++;
            for (int n = 0; pozicekurzoru != 0; n++) {
                vysledek = n;
                if (kartyhrace[n] == 1) { pozicekurzoru--; }
            }
            return vysledek;
        }

        static int VyberKartyProHrace(string[] znaky) {
            int vyskamenice = 3;
            int sirkamenice = 4;
            int pozicekurzoru = 0;
            string vyberumenice = "";
            for (int n = 0; n < vyskamenice; n++) {
                Console.WriteLine("\n");
            }
            ConsoleKey sipka = ConsoleKey.E;
            while (sipka != ConsoleKey.Enter) {
                for (int n = 0; vyberumenice.Length <= vyskamenice; n++) {
                    vyberumenice += "\n";
                }
                for (int n = 0; n < znaky.Length; n++) {
                    vyberumenice = SpojDvaStringy(vyberumenice, StvorKarty(n, 5 + 7, znaky, sirkamenice, vyskamenice, n == pozicekurzoru));
                }
                Console.CursorTop -= sirkamenice + 1;
                Console.CursorLeft = 0;
                Console.WriteLine(vyberumenice);
                vyberumenice = "";
                sipka = Console.ReadKey().Key;
                while (!((sipka == ConsoleKey.RightArrow && pozicekurzoru != 4 - 1) || (sipka == ConsoleKey.LeftArrow && pozicekurzoru != 0) || sipka == ConsoleKey.Enter)) {
                    Console.Write("\b \b");
                    sipka = Console.ReadKey().Key;

                }
                if (sipka == ConsoleKey.LeftArrow) { pozicekurzoru--; } else if (sipka == ConsoleKey.RightArrow) { pozicekurzoru++; }
            }


            return (pozicekurzoru * 8 + 5);
        }


        static int VyberKartyProPc(int[] rukahracepole) {
            int nejveci = 0;
            int kolikmanejveci = 0;
            int kolikmaaktualni = 0;
            for (int znak = 0; znak < 4; znak++) {
                for (int cislo = 0; cislo < 8; cislo++) {
                    if (rukahracepole[znak * 8 + cislo] == 1 && cislo != 5) { kolikmaaktualni++; }
                }
                if (kolikmanejveci < kolikmaaktualni) {
                    kolikmanejveci = kolikmaaktualni;
                    nejveci = znak;
                }

            }
            return (nejveci * 8 + 5);
        }

        static string StvorObracenouKartu(int vyska, int sirka) {
            string karta = "";
            karta += "┌";
            for (int i = 0; i < sirka; i++) {
                karta += "─";
            }
            karta += "┐\n";

            for (int i = 0; i < vyska; i++) {
                karta += "│";
                for (int n = 0; n < sirka; n++) {
                    karta += "╳";
                }
                karta += "│\n";
            }
            karta += "└";
            for (int i = 0; i < sirka; i++) {
                karta += "─";
            }
            karta += "┘";

            return (karta);


        }

    }
}