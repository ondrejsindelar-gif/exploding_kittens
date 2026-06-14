using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace exploding_kocka
{
    internal class Program
    {
        static int m = 1;
        static void Main(string[] args)
        {
            List<Card> karty_vsechny = new List<Card>();
            string nazevSouboru = "karty.json";
            try
            {
                // Zkontrolujeme, zda soubor vůbec existuje
                if (File.Exists(nazevSouboru))
                {
                    // Přečteme celý JSON jako jeden dlouhý text
                    string jsonText = File.ReadAllText(nazevSouboru);

                    // Převod textu z JSONu zpět na tvůj List<Card>
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    karty_vsechny = JsonSerializer.Deserialize<List<Card>>(jsonText, options);

                    Console.WriteLine($"Karty úspěšně načteny z JSONu. Celkem karet: {karty_vsechny.Count}");
                }
                else
                {
                    Console.WriteLine($"CHYBA: Soubor '{nazevSouboru}' nebyl nalezen!");
                    Console.WriteLine("Ujisti se, že máš u souboru nastaveno 'Copy to Output Directory' na 'Copy if newer'.");
                    return; // Ukončí program, protože bez karet nelze hrát
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Došlo k chybě při čtení nebo zpracování JSON souboru: {ex.Message}");
                return;
            }//nacitani do jsonu
            List<PLAYER> hraci = new List<PLAYER>();
            List<Card> akt_k = karty_vsechny; // karty s kterymi se pracuje tu hru
            
            Random random = new Random();
            Console.WriteLine("===================================================================================");
            Console.WriteLine("Ahoj jdes hrat hru exploding kittens vyber kolik vas bude hrat(2-5) a pustime se do hry");
            int pocet = 0; // promena na vyber poctu hracu a secret odbocky na pridani karet
            int i = 0; // je to promena na funkci while cyklu
            while (i == 0)
            {
                while (!int.TryParse(Console.ReadLine(), out pocet))
                {
                    Console.WriteLine("neplatne");
                }
                Console.WriteLine($"vytvorili jste hru s {pocet} hraci  a ted pravidla");
                Console.WriteLine("===================================================================================");
                Console.WriteLine("=== PRAVIDLA HRY ===\r\n\r\n1. Každý hráč dostane:\r\n   - 1x Zneškodnit\r\n   - 6 náhodné karty\r\n\r\n2. Ve svém tahu můžeš zahrát libovolný počet karet.\r\n\r\n3. Na konci tahu si musíš dobrat kartu,\r\n   pokud jsi nepoužil kartu Přeskočit nebo Útok.\r\n\r\n4. Pokud vytáhneš Koťátko:\r\n   - se Zneškodnit přežiješ,\r\n   - bez Zneškodnit vypadáváš ze hry.\r\n\r\n5. Karty:\r\n   - Přeskočit = konec tahu bez dobrání.\r\n   - Útok = další hráč hraje 2 tahy.\r\n   - Pohled do budoucnosti = zobrazí 3 vrchní karty.\r\n   - Míchej = zamíchá balíček.\r\n   - Dej mi kartu = vezme náhodnou kartu soupeři.\r\n   - KočKarta = nemá žádný efekt.\r\n\r\n6. Vyhrává poslední hráč,\r\n   který zůstane ve hře.\r\n\r\n====================");
                if (pocet > 1 && pocet < 6) // kdyz zjisti ze to je platny pocet hracu spusti se vytvareni hracu
                {
                    i = 1;

                    for (int w = 0; w < pocet ; w++)
                    {
                        List<Card> promena = new List<Card>();
                        Card zneskodnit = akt_k.First(c => c.Type == "Zneskodnit");//najde prvni kartu zneskodnit
                        promena.Add(zneskodnit);
                        akt_k.Remove(zneskodnit);
                        for (int p = 0; p < 6; p++)
                        {
                            int o = random.Next(5, akt_k.Count);
                            promena.Add(akt_k[o]);                           
                            akt_k.RemoveAt(o);
                        }

                        Console.Write("Zadej jmeno: ");
                        string jmeno = Console.ReadLine(); // ulozi jmeno hrace
                        hraci.Add(new PLAYER(promena, jmeno)); // prez konstruktor vytvori hrace    
                    }//tvorba hracu do listu 

                    int hracNaTahu = 0;
                    while (hraci.Count > 1)
                    {
                        //Ochranná obrazovka pro výměnu hráčů
                        Console.Clear();
                        Console.WriteLine("===================================================================");
                        Console.WriteLine($"[ PŘEDÁVÁNÍ TAHU ]");
                        Console.WriteLine($"Na řadě je: {hraci[hracNaTahu].jmeno}");
                        Console.WriteLine("Ostatní ať se nedívají! Až budeš u klávesnice, stiskni ENTER.");
                        Console.WriteLine("===================================================================");
                        Console.ReadLine(); // Čeká na enter
                        Console.Clear();
                        Console.WriteLine($"--- Nyní hraješ ty, {hraci[hracNaTahu].jmeno}! ---");
                        int pocetHracuPredTahem = hraci.Count;
                        // Spuštění tahu pro aktuálního hráče
                        tah(hraci, akt_k, hracNaTahu);

                        //Zastavení na konci tahu
                        Console.WriteLine("\n[ Tah skončil. Stiskni ENTER pro skrytí obrazovky a předání tahu. ]");
                        Console.ReadLine(); // Čeká na enter před dalším smazáním

                        // 4. Posunutí indexu na dalšího hráče
                       
                        if (hraci.Count == pocetHracuPredTahem)
                        {
                            hracNaTahu++;
                        }

                        // Pokud jsme na konci seznamu hráčů, jedeme zase od prvního
                        if (hracNaTahu >= hraci.Count)
                        {
                            hracNaTahu = 0;
                        }
                    }

                   
                    Console.Clear();
                    Console.WriteLine("===================================================================");
                    Console.WriteLine($"KONEC HRY! Vítězem se stává: {hraci[0].jmeno}!");
                    Console.WriteLine("===================================================================");

                }
                else if (pocet == 22)
                {
                    Console.WriteLine("=====================");
                    Console.WriteLine("jsi v pridavani karty napis jaky bude Type a potom ktere to bude Id");
                    string type = Console.ReadLine();
                    pridejKartu(karty_vsechny,type);
                }//pridani karty
                else
                {
                    Console.WriteLine("problem");
                }//spustise kdyz to je neplatne
            }
        }
        static void pridejKartu(List<Card> seznamKaret, string typKarty, string nazevSouboru = "karty.json")
        {
            // 1. Najdeme nejvyšší stávající ID, abychom ho nepřepsali. Pokud je list prázdný, začneme od 1.
            int noveId = 1;
            if (seznamKaret.Count > 0)
            {
                noveId = seznamKaret.Max(c => c.Id) + 1;
            }

            // 2. Vytvoříme novou kartu a přidáme ji do našeho aktuálního listu v paměti
            Card novaKarta = new Card { Id = noveId, Type = typKarty };
            seznamKaret.Add(novaKarta);

            try
            {
                // 3. Převedeme celý aktualizovaný list zpět na pěkně naformátovaný JSON text
                var options = new JsonSerializerOptions { WriteIndented = true };
                string novyJsonText = JsonSerializer.Serialize(seznamKaret, options);

                // 4. Zapíšeme to do souboru (tím se přepíše starý soubor karty.json)
                File.WriteAllText(nazevSouboru, novyJsonText);

                Console.WriteLine($"Úspěch! Karta '{typKarty}' s ID {noveId} byla přidána a uložena do JSONu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání nové karty do JSONu: {ex.Message}");
            }
        }
        static List<Card> tah(List<PLAYER> hraci, List<Card> pakl, int ktery_hrac)//vezme hrace co hrajou a balicek karet ktery je ted pouzivany
        {
            
            int q = 0;
            Console.WriteLine("vyber co chces pouzit :");
            vypis(hraci[ktery_hrac].Hand);
            Console.WriteLine("nebo ukonci pomoci Konec a dober kartu");
            
            while (q != 1)
            {

                string karta_vyber = kontrola_karta(hraci[ktery_hrac].Hand);//hrac vybere kartu s kterou chce pracovat


                switch (karta_vyber)
                {
                    case "KocKarta":
                        kocKarta(hraci, ktery_hrac);
                        break;
                    case "DejMiKartu":
                        laskvst(hraci, ktery_hrac);
                        break;
                    case "Michej":
                        michej(pakl);
                        Card micheej = hraci[ktery_hrac].Hand.First(c => c.Type == "Michej");
                        hraci[ktery_hrac].Hand.Remove(micheej);
                        break;//done
                    case "PohledDoBudoucnosti":
                        budoucnost(pakl);
                        Card pohled = hraci[ktery_hrac].Hand.First(c => c.Type == "PohledDoBudoucnosti");
                        hraci[ktery_hrac].Hand.Remove(pohled);
                        break;
                    case "Utok":
                        Card utok = hraci[ktery_hrac].Hand.First(c => c.Type == "Utok");
                        hraci[ktery_hrac].Hand.Remove(utok);
                        q = 1;
                        m = 2;
                        break;
                    case "Preskocit":
                        q = 1;
                        Card preskoc = hraci[ktery_hrac].Hand.First(c => c.Type == "Preskocit");
                        hraci[ktery_hrac].Hand.Remove(preskoc);
                        break;
                    case "Konec":
                        q = 1;
                        konc_hraci(hraci, pakl, ktery_hrac, m);
                        konc_pakl(hraci, pakl, ktery_hrac, m);
                        m = 1;
                        break;
                }
                if (hraci[ktery_hrac].Hand.Any(c => c.Type == "Kotatko"))
                {
                    Card zneskodnit =
                        hraci[ktery_hrac].Hand.FirstOrDefault(c => c.Type == "Zneskodnit");

                    if (zneskodnit != null)
                    {
                        hraci[ktery_hrac].Hand.Remove(zneskodnit);
                        Console.WriteLine("Kotatko zneskodneno.");
                        Card kotatko = hraci[ktery_hrac].Hand.First(c => c.Type == "Kotatko");

                        pakl.Add(kotatko);
                        
                        hraci[ktery_hrac].Hand.Remove(kotatko);
                    }
                    else
                    {
                        Console.WriteLine($"{hraci[ktery_hrac].jmeno} vybuchl!");
                        hraci.RemoveAt(ktery_hrac);
                    }
                }

            }
            return pakl;
        }
        static List<Card> michej(List<Card> baliceek)//michani funguje
        {
            Random random = new Random();
            for (int i = 0; i < baliceek.Count; i++)
            {
                int r = random.Next(0, baliceek.Count);
                Card value = baliceek[r];
                baliceek[r] = baliceek[i];
                baliceek[i] = value;
            }
            
            return baliceek;
        }
        static void budoucnost(List<Card> baliceek)
        {
            Console.Clear();
            Console.WriteLine("tohle jsou nasledujici 3 kraty :");
            for (int i = 0; i < Math.Min(3, baliceek.Count); i++)
            {
                Console.WriteLine(baliceek[i].Type);
            }
            Console.ReadLine();
            Console.Clear();
        }//done
        static List<PLAYER> kocKarta(List<PLAYER> hraci, int kolikaty)
        {
            Random random = new Random();
            if (hraci[kolikaty].Hand.Count(c => c.Type == "KocKarta") >= 2)
            {
                Console.WriteLine("kockarty potrebujes 2 a pak nekomu muzes sebrat kartu\r\n\r\n Tady jsou vsichni hraci: ");
                foreach (var hrac in hraci)
                {
                    Console.WriteLine(hrac.jmeno);
                }// vypise hrace kterym muze brat
                string vybrany = kontrola_hrac(hraci);
                Console.WriteLine($"vybrali jste hrace {vybrany} stisknete enter a dostanete kartu");
                Console.ReadLine();
                Console.Clear();
                int index = hraci.FindIndex(h => h.jmeno == vybrany);//najdu index na kterem je vybrany hrac              
                int nahodnyIndex = random.Next(0, hraci[index].Hand.Count);
                Card karta2 = hraci[kolikaty].Hand.First(c => c.Type == "KocKarta");
                Card karta = hraci[index].Hand[nahodnyIndex];
                hraci[index].Hand.RemoveAt(nahodnyIndex);
                hraci[kolikaty].Hand.Add(karta);
                hraci[kolikaty].Hand.Remove(karta2);
                Card karta3 = hraci[kolikaty].Hand.First(c => c.Type == "KocKarta");
                hraci[kolikaty].Hand.Remove(karta3);
                vypis(hraci[kolikaty].Hand);

            }
            return hraci;
        }// jede toooooo
        static List<PLAYER> laskvst(List<PLAYER> hraci, int kolikaty)
        {
            Console.WriteLine("laskavost dela to ze vyberes nekoho a ten ti musi dat kartu podle jeho výběru\r\n\r\n Tady jsou vsichni hraci: ");
            foreach (var hrac in hraci)
            {
                Console.WriteLine(hrac.jmeno);
            }
            string vybrany = kontrola_hrac(hraci);
            Console.WriteLine($"vybrali jste hrace {vybrany} stisknete enter a on vybere kartu co vám dá");
            Console.ReadLine();
            Console.Clear();
            int index = hraci.FindIndex(h => h.jmeno == vybrany);

            foreach (var item in hraci[index].Hand)
            {
                Console.WriteLine(item.Type);
            }
            string vyber = kontrola_karta(hraci[index].Hand);
            Card karta = hraci[index].Hand.First(c => c.Type == vyber);
            Card karta2 = hraci[kolikaty].Hand.First(c => c.Type == "DejMiKartu");
            hraci[index].Hand.Remove(karta);
            hraci[kolikaty].Hand.Add(karta);
            hraci[kolikaty].Hand.Remove(karta2);
            return hraci;
        }//fungujuuuuuuueueeueueueueu
        static string kontrola_karta(List<Card> handda)
        {
            string vybrany = " ";
            while (vybrany != "Konec" && !handda.Any(hrc => hrc.Type == vybrany))// odchyt spravne promene 
            {
                Console.Write(" ");
                vybrany = Console.ReadLine();
            }
            return vybrany;
        }// odchyt at je type validni
        static string kontrola_hrac(List<PLAYER> hraci)
        {
            string vybrany = " ";
            while (!hraci.Any(hrc => hrc.jmeno == vybrany))// odchyt spravne promene 
            {
                Console.Write(" ");
                vybrany = Console.ReadLine();
            }
            return vybrany;
        }// odchyt jestli dany prvek obsahuje tohoto hrace
        static void vypis(List<Card> precti)// zobrazeni balicku pro moje debugovani
        {
            foreach (var card in precti)
            {
                Console.WriteLine($"{card.Type}");
            }
        }
        static List<PLAYER> konc_hraci(List<PLAYER> hraci, List<Card> baliceek,int kolikaty,int utok)
        {
            if (utok == 1)
            {
                hraci[kolikaty].Hand.Add(baliceek[0]);
            }
            else if(baliceek.Count != 1)
            {
                hraci[kolikaty].Hand.Add(baliceek[0]);
                hraci[kolikaty].Hand.Add(baliceek[1]);
            }

            return hraci;
        }
        static List<Card> konc_pakl(List<PLAYER> hraci, List<Card> baliceek, int kolikaty,int utok)
        {
            if (utok == 1)
            {
                baliceek.RemoveAt(0);
            }
            else if (baliceek.Count != 1)
            {
                baliceek.RemoveAt(0);
                baliceek.RemoveAt(0);
            }
            return baliceek;
        }
    }
}
