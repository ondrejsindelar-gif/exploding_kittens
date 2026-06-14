using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exploding_kocka
{
    internal class PLAYER
    {
       public string jmeno { get; set; }
        public List<Card> Hand { get; set; }
       

        public PLAYER(List<Card> ruka, string name)//,Card prvnikart)
        {
            jmeno = name;
            // prvni = prvnikart;
            List<Card> nazev = new List<Card>();
            // nazev.Add(prvnikart);
            nazev.AddRange(ruka);
            Hand = nazev;
        }
        public void say()
        {
            Console.WriteLine($"{this.jmeno} ma tyto karty : ");
            for (int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine(this.Hand[i].Type + this.Hand[i].Id);
            }
        }
    }
}
