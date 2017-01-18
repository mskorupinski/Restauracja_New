using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahApps.Metro.Application1

{
    class dolisty
    {
        public string Nazwa { get; set; }

        public string Ilosc { get; set; }
        public string Cena { get; set; }

    }

    class pozycja
    {
        private int produkt_id = 0;
        private string nazwa = "";
        private double cena;
        private int ilosc = 0;
        public pozycja(int p, int i, string n, double c)
        {
            produkt_id = p;
            ilosc = i;
            Nazwa = n;
            Cena = c;
        }
        public int Produkt_id
        {
            get
            {
                return produkt_id;
            }
            set
            {
                produkt_id = value;
            }

        }
        public int Ilosc
        {
            get
            {
                return ilosc;
            }
            set
            {
                ilosc = value;
            }

        }
        public string Nazwa
        {
            get
            {
                return nazwa;
            }
            set
            {
                nazwa = value;
            }

        }

        public double Cena
        {
            get
            {
                return cena;
            }
            set
            {
                cena = value;
            }

        }


    }


    class zamowienie
    {
        private int numer_stolika = 0;
        private int id_klienta = 0;
        private int id_rezerwacji = 0;
        private int id_stolika = 0;

        public zamowienie(int n, int ik, int ir, int ids)
        {
            numer_stolika = n;
            id_klienta = ik;
            id_rezerwacji = ir;
            id_stolika = ids;
        }
        private List<pozycja> zamowione = new List<pozycja>();

        public int Numer_stolika
        {
            get
            {
                return numer_stolika;
            }
            set
            {
                numer_stolika = value;
            }

        }

        public int Id_klienta
        {
            get
            {
                return id_klienta;
            }
            set
            {
                id_klienta = value;
            }

        }

        public int Id_rezerwacji
        {
            get
            {
                return id_rezerwacji;
            }
            set
            {
                id_rezerwacji = value;
            }

        }
        public int Id_stolika
        {
            get
            {
                return id_stolika;
            }
            set
            {
                id_stolika = value;
            }

        }

        public void Dodaj(int p, int i, string m, double c)
        {
            zamowione.Add(new pozycja(p, i, m, c));
        }
        public void Wyswietl(System.Windows.Controls.ListView lista)
        {

            foreach (pozycja temp in zamowione)
            {

                lista.Items.Add(new dolisty { Nazwa = temp.Nazwa.ToString(), Ilosc = temp.Ilosc.ToString(), Cena = temp.Cena.ToString() });


            }


        }
        public void Usun(string p, int i, string c)
        {
            int z = 0;
            int licznik = 0;
            bool jest = false;
            foreach (pozycja temp in zamowione)
            {
                if (temp.Nazwa == p && temp.Ilosc == i && c == temp.Cena.ToString())
                {
                    jest = true;
                    licznik = z;
                }
                z++;
            }
            if (jest == true) { zamowione.RemoveAt(licznik); }
        }

    }

}
