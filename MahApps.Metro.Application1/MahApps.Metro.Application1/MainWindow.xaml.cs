using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;


namespace MahApps.Metro.Application1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        dane logowanie;
        List<zamowienie> Zamowienia;
        public MainWindow()
        {
            InitializeComponent();
            wypelnijStanowiska();
            wyswietlStolik();
            wyswietlPracownicy();
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            string cmd = "Select * from Wyswietl_Stolik";
            sqlCmd.CommandText = cmd;
            SqlDataReader DR = sqlCmd.ExecuteReader();
            Zamowienia = new List<zamowienie>();
            while (DR.Read())
            {
                Zamowienia.Add(new zamowienie(Convert.ToInt32(DR[0]), 0, 0, Convert.ToInt32(DR[2])));

            }


            sqlConn.Close();

        }
        private String serwer = "DESKTOP-BU2VMS6\\SQLEXPRESS";
        private void Zaloguj_Sie_Click(object sender, RoutedEventArgs e)
        {
            logowanie = new dane();
            SqlConnection sqlConn = new SqlConnection("Data Source=" + serwer + ";Network Library=dbmssocn;Initial Catalog = RESTAURACJA; User ID = " + Text_Logowanie.Text + ";Password = " + Text_Haslo.Password + ";");
            try
            {

                // otwórz połączenie:
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;

                logowanie.Ustaw_haslo(Text_Haslo.ToString());
               

                cmd.CommandText = "Select Stanowisko from Restauracja.dbo.Pracownik where Login_pracownik= '" + Text_Logowanie.Text.ToString() + "' and Status_zatrudnienia='zatrudniony'";

                string login = cmd.ExecuteScalar().ToString();

                logowanie.Ustaw_login(Text_Logowanie.Text);
                if (login != "Menager")
                {
                    Karta_menu.Visibility = Visibility.Hidden;
                    Karta_pracownicy.Visibility = Visibility.Hidden;
                    Karta_stolik.Visibility = Visibility.Hidden;
                }
                else
                {
                    Karta_menu.Visibility = Visibility.Visible;
                    Karta_pracownicy.Visibility = Visibility.Visible;
                    Karta_stolik.Visibility = Visibility.Visible;
                }
                Text_Logowanie.Text = "";
                Text_Haslo.Clear();
                sqlConn.Close();
                //asgjhasfgjas
                Logowanie.Visibility = Visibility.Hidden;
                Panel.Visibility = Visibility.Visible;
                Wyloguj.Visibility = Visibility.Visible;
                textBox_dodaj_menu.Visibility = Visibility.Hidden;
                label_DodajMenu.Visibility = Visibility.Hidden;
                label_UsunMenu.Visibility = Visibility.Hidden;
                comboBox_Copy.Visibility = Visibility.Hidden;
               

            }
            catch (System.Data.SqlClient.SqlException se)
            {
                MessageBox.Show(se.Message, se.Source);

            }


        }

        class dane
        {
            string login = "";
            string hasło = "";

            public string Login
            {
                get
                {
                    return login;
                }
            }
            public string Hasło
            {
                get
                {
                    return hasło;
                }
            }
            public void Ustaw_login(string l) { login = l; }
            public void Ustaw_haslo(string h) { hasło = h; }
        }



        private void Tekst_Menu_Nr_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void Tekst_Menu_Cena_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
            if (e.Text[e.Text.Length - 1] == ',')
            {
                e.Handled = false;

            }
        }

        private void Przycisk_Menu_Dodaj_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;



            if (comboBox.Text == "")
            {
                MessageBox.Show("Wybierz menu", "Błąd");
            }
            else
            {
                if (Tekst_Menu_Cena.Text != "")
                {
                    sqlCmd.CommandText = "Select Id_menu from Menu where Opis = '" + comboBox.Text + "'and aktualnosc='aktualne'";
                    int id_menu = (int)sqlCmd.ExecuteScalar();
                    string result = Tekst_Menu_Cena.Text.Replace(",", ".");

                    string insert = "Select count(*) from Produkt where Opis_produktu = '" + Tekst_Menu_Opis.Text + "' and Cena_produktu='" + result + "' and Id_menu= "+ id_menu;
                    sqlCmd.CommandText = insert.Replace(",", ".");
                    int liczba_wierszu = (int)sqlCmd.ExecuteScalar();
                    if (liczba_wierszu == 0)
                    {

                        string temp = "insert into Produkt (Opis_produktu, Cena_produktu, Id_menu,Aktualnosc, Kategoria) Values('" + Tekst_Menu_Opis.Text + "','" + result + "'," + id_menu + ", 'aktualny','" + comboBox4.Text + "');";

                        sqlCmd.CommandText = temp;
                        sqlCmd.ExecuteReader();
                    }
                    if (liczba_wierszu == 1)
                    {

                        string temp = "update Produkt set Aktualnosc='aktualny' where Opis_produktu='" + Tekst_Menu_Opis.Text + "' and Cena_produktu= " + result + " and Id_menu=" + id_menu;
                        sqlCmd.CommandText = temp;
                        sqlCmd.ExecuteReader();

                    }
                    sqlConn.Close();

                    wyswietlMenu();

                }
            }


        }

        private void Przycisk_Menu_Usun_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            DataRowView row = (DataRowView)Dane_Menu.SelectedItem;

            if (row != null)
            {


                string alter = "Select Count(*) from Pozycja_Produkt where Opis ='" + row[0].ToString() + "' and Cena =" + row[1].ToString().Replace(",", ".");
                sqlCmd.CommandText = alter;
                int liczba_wierszu = (int)sqlCmd.ExecuteScalar();
                if (liczba_wierszu == 0)
                {
                    sqlCmd.CommandText = "Delete From Produkt Where Opis_produktu = '" + row[0].ToString() + "' and Cena_produktu = " + row[1].ToString().Replace(",", ".");
                    sqlCmd.ExecuteReader();

                }
                if (liczba_wierszu >= 1)
                {
                    string temp = "update Produkt set Aktualnosc='nieaktualny' where Opis_produktu='" + row[0].ToString() + "' and Cena_produktu= " + row[1].ToString().Replace(",", ".");
                    sqlCmd.CommandText = temp;
                    sqlCmd.ExecuteReader();

                }
                sqlConn.Close();

                wyswietlMenu();


            }
        }




        private void comboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (comboBox.Items.Count > 0) comboBox.Items.Clear();

            string Sql = "select Opis from Menu where aktualnosc='aktualne'";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();

            while (DR.Read())
            {
                comboBox.Items.Add(DR[0]);

            }

            conn.Close();
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            wyswietlMenu();
        }

        public void wyswietlMenu()
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            SqlDataReader dataReader = null;

            Tekst_Menu_Cena.Text = "";
            Tekst_Menu_Opis.Text = "";
            sqlCmd.CommandText = "select Opis, Cena, Kategoria from Wyswietl_Menu where Rodzaj='" + comboBox.Text + "'";

            dataReader = sqlCmd.ExecuteReader();

            if (dataReader != null)
            {
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                Dane_Menu.ItemsSource = dt.DefaultView;

                dataReader.Close();
            }

            sqlConn.Close();
        }

        public void wyswietlStolik()
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            SqlDataReader dataReader = null;

            Tekst_Stoliki_Numer.Text = "";
            Tekst_Stoliki_Miejsca.Text = "";
            sqlCmd.CommandText = "select Numer, Miejsca from Wyswietl_Stolik where Numer>0 ";

            dataReader = sqlCmd.ExecuteReader();

            if (dataReader != null)
            {
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                Dane_Stoliki.ItemsSource = dt.DefaultView;

                dataReader.Close();
            }

            sqlConn.Close();
        }




        private void Tekst_Menu_Cena_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Tekst_Stoliki_Numer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;

        }

        private void Tekst_Stoliki_Miejsca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void Przycisk_Stoliki_Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (Tekst_Stoliki_Numer.Text == "" || Convert.ToInt32(Tekst_Stoliki_Numer.Text) == 0)
            {
                MessageBox.Show("Nie podano numeru stolika", "Błąd");
            }
            else if (Tekst_Stoliki_Miejsca.Text == "" || Convert.ToInt32(Tekst_Stoliki_Miejsca.Text) == 0)
            {
                MessageBox.Show("Nie podano ilości miejsc", "Błąd");
            }
            else
            {
                SqlConnection sqlConn = new SqlConnection("Server= " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;




                string alter = "Select Count(*) from Stolik where Numer_stolika =" + Tekst_Stoliki_Numer.Text.ToString();
                sqlCmd.CommandText = alter;
                int liczba_wierszu = (int)sqlCmd.ExecuteScalar();
                if (liczba_wierszu == 0)
                {
                    sqlCmd.CommandText = "insert into Stolik (Ilosc_miejsc, Numer_stolika) Values(" + Tekst_Stoliki_Miejsca.Text.ToString() + "," + Tekst_Stoliki_Numer.Text.ToString() + ")";
                    sqlCmd.ExecuteNonQuery();
                    alter = "Select Id_stolika from Stolik where Numer_stolika =" + Tekst_Stoliki_Numer.Text.ToString();
                    sqlCmd.CommandText = alter;
                    int id = (int)sqlCmd.ExecuteScalar();
                    Zamowienia.Add(new zamowienie(Convert.ToInt32(Tekst_Stoliki_Numer.Text), 0, 0, id));

                }
                if (liczba_wierszu >= 1)
                {
                    MessageBox.Show("Stolik o podanym numerze już istnieje", "Błąd");

                }

                wyswietlStolik();
                sqlConn.Close();

            }

        }

        private void Przycisk_Stoliki_Usun_Click(object sender, RoutedEventArgs e) // trzeba sprawdzić na danych z rezerwacja bez rezerwacji 
        {

            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            DataRowView row = (DataRowView)Dane_Stoliki.SelectedItem;

            if (row != null)
            {


                DateTime pora = DateTime.Now;
                string alter = "Select Count(*) from Stolik_Rezerwacja where (Numer =" + row[0].ToString() + " and Data>'" + pora.Year + "-" + pora.Month + "-" + pora.Day + "' and Czas>='" + pora.Hour + ":" + pora.Minute + ":" + pora.Second + "') or  (Numer = " + row[0].ToString() + " and Data= '" + pora.Year + "-" + pora.Month + "-" + pora.Day + "' and Czas>= '" + pora.Hour + ":" + pora.Minute + ":" + pora.Second + "')";
                sqlCmd.CommandText = alter;
                int liczba_wierszy = (int)sqlCmd.ExecuteScalar();
                //jeżeli istnieje rezerwacja na kolejne dni nie mozna usunąć 
                if (liczba_wierszy == 0)
                {
                    //zlicza czy kiedykolwiek byla rezerwacja na ten stolik
                    string alter3 = "Select Count(*) from Stolik_Rezerwacja where (Numer =" + row[0].ToString() + " and Data<'" + pora.Year + "-" + pora.Month + "-" + pora.Day + "') or ( Czas<='" + pora.Hour + ":" + pora.Minute + ":" + pora.Second + "'" + " and  Data= '" + pora.Year + "-" + pora.Month + "-" + pora.Day + "' and Numer =" + row[0].ToString() + ")";
                    sqlCmd.CommandText = alter3;
                    int liczba_wierszy3 = (int)sqlCmd.ExecuteScalar();
                    alter = "Select Count(*) from Stolik_Zamowienie where Numer =" + row[0].ToString();
                    sqlCmd.CommandText = alter;
                    int liczba_wierszy2 = (int)sqlCmd.ExecuteScalar();
                    liczba_wierszy2 += liczba_wierszy3;
                    //jezeli nie bylo rezerwacji dla tego stolika ani nie przyszedl klient bez rezerwacji  mozna usunac 
                    if (liczba_wierszy2 == 0)
                    {
                        sqlCmd.CommandText = "delete Stolik where Numer_stolika = " + row[0].ToString();
                        sqlCmd.ExecuteReader();
                    }
                    // jezeli byla kiedy rezerwacja albo przyszedl klient bez stolika to nie tracimy informacji o liczbie miejsc
                    else if (liczba_wierszy2 >= 1)
                    {

                        string temp = "update Stolik set Numer_stolika = 0 where Numer_stolika= " + row[0].ToString();
                        sqlCmd.CommandText = temp;
                        sqlCmd.ExecuteReader();
                    }
                    Usunstolik(Convert.ToInt32(row[0].ToString()));
                }
                else
                {

                    MessageBox.Show("Stolik zarezerwowany-nie można go usunąć", "Błąd");

                }


            }
            else
            {

                MessageBox.Show("Wybierz stolik", "Błąd");

            }
            wyswietlStolik();
            sqlConn.Close();






        }
        public void Usunstolik(int numer_stolika)
        {
            int z = 0;
            int licznik = 0;
            bool jest = false;
            foreach (zamowienie temp in Zamowienia)
            {
                if (temp.Numer_stolika == numer_stolika)
                {
                    jest = true;
                    licznik = z;
                }
                z++;
            }
            if (jest == true) { Zamowienia.RemoveAt(licznik); }
        }


        private void Przycisk_Stoliki_Edytuj_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            DataRowView row = (DataRowView)Dane_Stoliki.SelectedItem;

            if (row != null)
            {

                if (Tekst_Stoliki_Numer.Text == "" || Tekst_Stoliki_Numer.Text == "0") { MessageBox.Show("Nie podano numeru", "Błąd"); }



                else
                {

                    DateTime pora = DateTime.Now;
                    string alter = "Select Count(*) from Wyswietl_Stolik where Numer =" + Tekst_Stoliki_Numer.Text;
                    sqlCmd.CommandText = alter;
                    int liczba_wierszy = (int)sqlCmd.ExecuteScalar();
                    if (liczba_wierszy == 0)
                    {

                        string temp = "update Stolik set Numer_stolika = " + Tekst_Stoliki_Numer.Text + " where Numer_stolika= " + row[0].ToString();
                        sqlCmd.CommandText = temp;
                        sqlCmd.ExecuteReader();

                        Zmien_numer_stolik(Convert.ToInt32(Tekst_Stoliki_Numer.Text), Convert.ToInt32(row[0].ToString()));
                    }
                    else
                    {

                        MessageBox.Show("Istenie już stolik o podanym numerze", "Błąd");

                    }


                }

            }
            else
            {

                MessageBox.Show("Wybierz stolik", "Błąd");

            }
            wyswietlStolik();
            sqlConn.Close();



        }
        public void Zmien_numer_stolik(int numer_stolika_nowy, int numer_stolika_stary)
        {

            foreach (zamowienie temp in Zamowienia)
            {
                if (temp.Numer_stolika == numer_stolika_stary)
                {
                    temp.Numer_stolika = numer_stolika_nowy;
                }

            }

        }


        public void wypelnijStanowiska()
        {
            comboBox_Pracownicy.Items.Add("Kelner");
            comboBox_Pracownicy.Items.Add("Kelnerka");
            comboBox_Pracownicy.Items.Add("Menager");
            comboBox_Pracownicy.Items.Add("Pomoc kuchenna");
            comboBox_Pracownicy.Items.Add("Kucharz");
            comboBox_Pracownicy.Items.Add("Barman");
        }

        public void wyswietlPracownicy()
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            SqlDataReader dataReader = null;

            //Tekst_Pracownicy_Imie.Text = "";
            //Tekst_Pracownicy_Nazwisko.Text = "";
            sqlCmd.CommandText = "select Imie, Nazwisko, Stanowisko from Wyswietl_Pracownik";

            dataReader = sqlCmd.ExecuteReader();

            if (dataReader != null)
            {
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                Dane_Pracownicy.ItemsSource = dt.DefaultView;

                dataReader.Close();
            }

            sqlConn.Close();
        }

        private void Przycisk_Pracownicy_Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (Tekst_Pracownicy_Imie.Text == "")
            {
                MessageBox.Show("Nie podano imienia.", "Błąd");
            }
            else if (Tekst_Pracownicy_Nazwisko.Text == "")
            {
                MessageBox.Show("Nie podano nazwiska", "Błąd");
            }
            else if (Tekst_Pracownicy_Haslo.Text == "")
            {
                MessageBox.Show("Nie podano hasla", "Błąd");
            }
            else if (comboBox_Pracownicy.Text == "")
            {
                MessageBox.Show("Nie wybrano stanowiska", "Błąd");
            }
            else if (Tekst_Pracownicy_Login.Text == "")
            {
                MessageBox.Show("Nie podano loginu", "Błąd");
            }
            else
            {
                SqlConnection sqlConn = new SqlConnection("Server= " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;


                string alter = " select count(*) from master.dbo.syslogins where name = '" + Tekst_Pracownicy_Login.Text + "'";
                sqlCmd.CommandText = alter;
                int liczba_wierszu = (int)sqlCmd.ExecuteScalar();
                if (liczba_wierszu == 0)
                {
                    sqlCmd.CommandText = "insert into Pracownik (Imie, Nazwisko, Stanowisko,Login_pracownik) Values(" + "'" + Tekst_Pracownicy_Imie.Text + "'" + "," + "'" + Tekst_Pracownicy_Nazwisko.Text + "'" + "," + "'" + comboBox_Pracownicy.Text.ToString() + "'" + ",'" + Tekst_Pracownicy_Login.Text + "'" + ");" +
                           "CREATE LOGIN " + Tekst_Pracownicy_Login.Text + " WITH PASSWORD ='" + Tekst_Pracownicy_Haslo.Text + "'; " + "CREATE USER " + Tekst_Pracownicy_Login.Text + "User" + " FOR LOGIN " + Tekst_Pracownicy_Login.Text + ";" + "use Restauracja " +
                           " EXEC sp_addrolemember 'db_datareader','" + Tekst_Pracownicy_Login.Text + "User'" +
                           " use Restauracja " +
                           " EXEC sp_addrolemember 'db_datawriter','" + Tekst_Pracownicy_Login.Text + "User'";
                    sqlCmd.ExecuteReader();
                    Tekst_Pracownicy_Imie.Text = "";
                    Tekst_Pracownicy_Nazwisko.Text = "";
                    Tekst_Pracownicy_Login.Text = "";
                    Tekst_Pracownicy_Haslo.Text = "";

                }
                if (liczba_wierszu >= 1)
                {
                    MessageBox.Show("Pracownik o takim loginie juz występuje.", "Błąd");

                }
                wyswietlPracownicy();
                sqlConn.Close();

            }
        }

        private void Przycisk_Pracownicy_Usun_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            DataRowView row = (DataRowView)Dane_Pracownicy.SelectedItem;

            if (row != null)
            {


                if (Tekst_Pracownicy_Imie.Text == "")
                {
                    MessageBox.Show("Nie podano imienia.", "Błąd");
                }
                else if (Tekst_Pracownicy_Nazwisko.Text == "")
                {
                    MessageBox.Show("Nie podano nazwiska", "Błąd");
                }

                else if (comboBox_Pracownicy.Text == "")
                {
                    MessageBox.Show("Nie wybrano stanowiska", "Błąd");
                }
                else
                {
                    string wybierz = "Select Login from Wyswietl_pracownik where Imie= '" + Tekst_Pracownicy_Imie.Text + "'and  Nazwisko= '" + Tekst_Pracownicy_Nazwisko.Text + "'and  Stanowisko = '" + comboBox_Pracownicy.Text + "'";
                    // string temp = "update Pracownik set Imie = '" + Tekst_Pracownicy_Imie.Text + "', Nazwisko = '" + Tekst_Pracownicy_Nazwisko.Text + "', Stanowisko = '" + comboBox_Pracownicy.Text + "'" +
                    //" where Imie = '" + row[0].ToString() + "' and Nazwisko= '" + row[1].ToString() + "' and Stanowisko = '" + row[2].ToString() + "'";
                    sqlCmd.CommandText = wybierz;
                    string login = sqlCmd.ExecuteScalar().ToString();
                    string usun = "Drop User " + login + "User; " + "Drop Login " + login + "; update Pracownik set Status_zatrudnienia = 'zwolniony'  where Imie = '" + row[0].ToString() + "' and Nazwisko= '" + row[1].ToString() + "' and Stanowisko = '" + row[2].ToString() + "';";
                    sqlCmd.CommandText = usun;
                    sqlCmd.ExecuteReader();
                    Tekst_Pracownicy_Imie.Text = "";
                    Tekst_Pracownicy_Nazwisko.Text = "";
                    comboBox_Pracownicy.Text = "";

                }

            }
            else
            {

                MessageBox.Show("Wybierz pracownika", "Błąd");

            }

            wyswietlPracownicy();
            sqlConn.Close();
        }

        private void Przycisk_Pracownicy_Edytuj_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            DataRowView row = (DataRowView)Dane_Pracownicy.SelectedItem;

            if (row != null)
            {


                if (Tekst_Pracownicy_Imie.Text == "")
                {
                    MessageBox.Show("Nie podano imienia.", "Błąd");
                }
                else if (Tekst_Pracownicy_Nazwisko.Text == "")
                {
                    MessageBox.Show("Nie podano nazwiska", "Błąd");
                }

                else if (comboBox_Pracownicy.Text == "")
                {
                    MessageBox.Show("Nie wybrano stanowiska", "Błąd");
                }
                else
                {

                    string temp = "update Pracownik set Imie = '" + Tekst_Pracownicy_Imie.Text + "', Nazwisko = '" + Tekst_Pracownicy_Nazwisko.Text + "', Stanowisko = '" + comboBox_Pracownicy.Text + "'" +
                    " where Imie = '" + row[0].ToString() + "' and Nazwisko= '" + row[1].ToString() + "' and Stanowisko = '" + row[2].ToString() + "'";
                    sqlCmd.CommandText = temp;
                    sqlCmd.ExecuteReader();
                    Tekst_Pracownicy_Imie.Text = "";
                    Tekst_Pracownicy_Nazwisko.Text = "";
                    comboBox_Pracownicy.Text = "";

                }

            }
            else
            {

                MessageBox.Show("Wybierz pracownika", "Błąd");

            }

            wyswietlPracownicy();
            sqlConn.Close();


        }



        private void Dane_Pracownicy_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            DataRowView row = (DataRowView)Dane_Pracownicy.SelectedItem;
            if (row != null)
            {
                string temp1 = row[0].ToString();
                string temp2 = row[1].ToString();
                string temp3 = row[2].ToString();

                Tekst_Pracownicy_Imie.Text = temp1;
                Tekst_Pracownicy_Nazwisko.Text = temp2;
                comboBox_Pracownicy.Text = temp3;
            }
        }

        private void comboBox2_Initialized(object sender, EventArgs e)
        {
            comboBox2.Items.Add("hh");
            comboBox2.SelectedItem = "hh";
            comboBox2.Items.Add("10");
            comboBox2.Items.Add("11");
            comboBox2.Items.Add("12");
            comboBox2.Items.Add("13");
            comboBox2.Items.Add("14");
            comboBox2.Items.Add("15");
            comboBox2.Items.Add("16");
            comboBox2.Items.Add("17");
            comboBox2.Items.Add("18");
            comboBox2.Items.Add("19");
            comboBox2.Items.Add("20");
            comboBox2.Items.Add("21");
            comboBox2.Items.Add("22");



        }

        private void comboBox3_Initialized(object sender, EventArgs e)
        {
            comboBox3.Items.Add("mm");
            comboBox3.SelectedItem = "mm";
            comboBox3.Items.Add("00");
            comboBox3.Items.Add("15");
            comboBox3.Items.Add("30");
            comboBox3.Items.Add("45");


        }

        private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WyswietlRezerwacje(int a)
        {

            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            SqlDataReader dataReader = null;


            if (a == 0)
            {
                DateTime thisDay = DateTime.Today;

                sqlCmd.CommandText = "select Imie, Nazwisko, Data, Godzina, Stolik from Wyswietl_Rezerwacja where Data='" + thisDay.Year + "/" + thisDay.Month + "/" + thisDay.Day + "'";

            }
            if (a == 1)
            {
                sqlCmd.CommandText = "select Imie, Nazwisko, Data, Godzina, Stolik from Wyswietl_Rezerwacja where Data='" + dzien.SelectedDate.Value.Year + "/" + dzien.SelectedDate.Value.Month + "/" + dzien.SelectedDate.Value.Day + "'";


            }
            dataReader = sqlCmd.ExecuteReader();

            if (dataReader != null)
            {

                DataTable dt = new DataTable();
                dt.Load(dataReader);
                Dane_Rezerwacja.ItemsSource = dt.DefaultView;

                dataReader.Close();
            }

            sqlConn.Close();


        }

        private void Dane_Rezerwacja_Initialized(object sender, EventArgs e)
        {
            WyswietlRezerwacje(0);
        }

        private void comboBox1_DropDownOpened(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0) comboBox1.Items.Clear();

            string Sql = "select Numer, Miejsca from Wyswietl_Stolik where not Numer=0";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();

            while (DR.Read())
            {
                comboBox1.Items.Add(DR[0].ToString());

            }

            conn.Close();
        }

        private void Przycisk_Rezerwacja_Dodaj_Click(object sender, RoutedEventArgs e)
        {


            if (dzien.SelectedDate.ToString() == "")
            {
                MessageBox.Show("Nie podano daty", "Błąd");
            }
            else if (comboBox2.SelectedItem.ToString() == "hh" || comboBox3.SelectedItem.ToString() == "mm")
            {
                MessageBox.Show("Nie podano godziny", "Błąd");
            }
            else if (comboBox1.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Nie wybrano stolika", "Błąd");
            }
            else if (Tekst_Rezerwacja_Imie.Text == "")
            {
                MessageBox.Show("Nie podano imienia", "Błąd");
            }
            else if (Tekst_Rezerwacja_Nazwisko.Text == "")
            {
                MessageBox.Show("Nie podano nazwiska", "Błąd");
            }
            else if (Tekst_Rezerwacja_Numer.Text == "")
            {
                MessageBox.Show("Nie podano numeru telefonu", "Błąd");
            }
            else
            {
                SqlConnection sqlConn = new SqlConnection("Server= " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                string zapytanie = "select Id_stolika from Stolik where Numer_stolika=" + comboBox1.SelectedItem.ToString();
                sqlCmd.CommandText = zapytanie;
                int numer = Convert.ToInt32(sqlCmd.ExecuteScalar().ToString());

                string alter = "select count(*) from Wyswietl_Rezerwacja where Data='" + dzien.SelectedDate.Value.Year + " / " + dzien.SelectedDate.Value.Month + " / " + dzien.SelectedDate.Value.Day + "' and Godzina='" + comboBox2.SelectedItem.ToString() + ":" + comboBox3.SelectedItem.ToString() + ":00' and Stolik ='" + comboBox1.SelectedItem.ToString() + "'";
                sqlCmd.CommandText = alter;
                int czy_zarezerwowane = Convert.ToInt32(sqlCmd.ExecuteScalar().ToString());
                
                if (czy_zarezerwowane == 0)
                {
                    alter = "select count(*) from Klient where Imie = '" + Tekst_Rezerwacja_Imie.Text + "'and Nazwisko='" + Tekst_Rezerwacja_Nazwisko.Text + "'and Telefon='" + Tekst_Rezerwacja_Numer.Text + "';";
                    sqlCmd.CommandText = alter;
                    int liczba = Convert.ToInt32(sqlCmd.ExecuteScalar().ToString());
                    if (liczba == 0)
                    {
                        sqlCmd.CommandText = "insert into Klient (Imie,Nazwisko,Telefon) values ('" + Tekst_Rezerwacja_Imie.Text + "','" + Tekst_Rezerwacja_Nazwisko.Text + "','" + Tekst_Rezerwacja_Numer.Text + "');" + "select Id_klienta from Klient ORDER BY Id_klienta DESC;";
                        int id_dodanego_klienta = (int)sqlCmd.ExecuteScalar();

                        sqlCmd.CommandText = "insert into Rezerwacja (Id_klienta, Data_rezerwacji, Czas_rezerwacji, Numer_stolika) values (" + id_dodanego_klienta + ",'" + dzien.SelectedDate.Value.Year + "/" + dzien.SelectedDate.Value.Month + "/" + dzien.SelectedDate.Value.Day + "','" + comboBox2.SelectedItem.ToString() + ":" + comboBox3.SelectedItem.ToString() + ":00'," + numer + ");";
                        sqlCmd.ExecuteNonQuery();
                        Tekst_Rezerwacja_Imie.Text = "";
                        Tekst_Rezerwacja_Nazwisko.Text = "";
                        Tekst_Rezerwacja_Numer.Text = "";
                        comboBox2.SelectedItem = "hh";
                        comboBox3.SelectedItem = "mm";
                        comboBox1.Text = "";
                    }
                    if (liczba >= 1)
                    {
                        zapytanie = "select Id_klienta from Klient where Imie = '" + Tekst_Rezerwacja_Imie.Text + "'and Nazwisko='" + Tekst_Rezerwacja_Nazwisko.Text + "'and Telefon='" + Tekst_Rezerwacja_Numer.Text + "';";
                        sqlCmd.CommandText = zapytanie;
                        int id_klienta = (int)sqlCmd.ExecuteScalar();
                        zapytanie = "select Id_stolika from Stolik where Numer_stolika=" + comboBox1.SelectedItem.ToString();
                        sqlCmd.CommandText = zapytanie;
                        numer = (int)sqlCmd.ExecuteScalar();

                        sqlCmd.CommandText = "insert into Rezerwacja (Id_klienta, Data_rezerwacji, Czas_rezerwacji, Numer_stolika) values (" + id_klienta + ",'" + dzien.SelectedDate.Value.Year + "/" + dzien.SelectedDate.Value.Month + "/" + dzien.SelectedDate.Value.Day + "','" + comboBox2.SelectedItem.ToString() + ":" + comboBox3.SelectedItem.ToString() + ":00'," + numer + ");";
                        sqlCmd.ExecuteNonQuery();

                    }
                    WyswietlRezerwacje(1);
                    sqlConn.Close();
                }

                else
                {
                    MessageBox.Show("Nie można dokonać rezerwacji", "Błąd");
                    sqlConn.Close();

                }
                  
            }
        }

        private void Przycisk_Rezerwacja_Usun_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            DataRowView row = (DataRowView)Dane_Rezerwacja.SelectedItem;
            DateTime date = Convert.ToDateTime(row[2].ToString());
            sqlCmd.CommandText = "select Id from Wyswietl_Rezerwacja where Stolik = " + row[4].ToString() + ";";
            int numer = (int)sqlCmd.ExecuteScalar();
            sqlCmd.CommandText = "delete from Rezerwacja where  Data_rezerwacji='" + date.Year + "-" + date.Month + "-" + date.Day + "'and Czas_rezerwacji='" + row[3].ToString() + "'and Numer_stolika=" + numer + ";";
            sqlCmd.ExecuteNonQuery();
            WyswietlRezerwacje(1);

            sqlConn.Close();
        }

        private void dzien_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            WyswietlRezerwacje(1);

        }

        private void Dane_Rezerwacja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox4_Initialized(object sender, EventArgs e)
        {
            comboBox4.Items.Add("Napój");
            comboBox4.Items.Add("Danie główne");
            comboBox4.Items.Add("Deser");
            comboBox4.Items.Add("Przystawka");
            comboBox4.Items.Add("Zupa");
        }

        private void Zamowienie_Combo_Menu_DropDownOpened(object sender, EventArgs e)
        {

            if (Zamowienie_Combo_Menu.Items.Count > 0) Zamowienie_Combo_Menu.Items.Clear();

            string Sql = "select Opis from Menu where aktualnosc='aktualne'";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();

            while (DR.Read())
            {
                Zamowienie_Combo_Menu.Items.Add(DR[0]);

            }
            /*
           
            */
            conn.Close();
        }

        private void Zamowienie_Combo_Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Zamowienie_Combo_Menu.SelectedItem != null)
            {

                if (Zamowienie_Combo_Danie.Items.Count > 0) Zamowienie_Combo_Danie.Items.Clear();
                if (Zamowienie_Combo_Zupa.Items.Count > 0) Zamowienie_Combo_Zupa.Items.Clear();
                if (Zamowienie_Combo_Przystawka.Items.Count > 0) Zamowienie_Combo_Przystawka.Items.Clear();
                if (Zamowienie_Combo_Deser.Items.Count > 0) Zamowienie_Combo_Deser.Items.Clear();
                if (Zamowienie_Combo_Napoj.Items.Count > 0) Zamowienie_Combo_Napoj.Items.Clear();

                SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
                conn.Open();
                string Sql1 = "select Opis from Wyswietl_Menu where Rodzaj= '" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "' and Kategoria='Danie główne' ";
                SqlCommand cmd = new SqlCommand(Sql1, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    Zamowienie_Combo_Danie.Items.Add(DR[0]);

                }
                DR.Close();
                string Sql2 = "select Opis from Wyswietl_Menu where Rodzaj= '" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "' and Kategoria='Zupa'";
                cmd = new SqlCommand(Sql2, conn);
                DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    Zamowienie_Combo_Zupa.Items.Add(DR[0]);

                }
                DR.Close();
                string Sql3 = "select Opis from Wyswietl_Menu where Rodzaj= '" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "' and Kategoria='Przystawka'";
                cmd = new SqlCommand(Sql3, conn);
                DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    Zamowienie_Combo_Przystawka.Items.Add(DR[0]);

                }
                DR.Close();
                string Sql4 = "select Opis from Wyswietl_Menu where Rodzaj= '" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "' and Kategoria='Deser'";
                cmd = new SqlCommand(Sql4, conn);
                DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    Zamowienie_Combo_Deser.Items.Add(DR[0]);

                }
                DR.Close();
                string Sql5 = "select Opis from Wyswietl_Menu where Rodzaj= '" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "' and Kategoria='Napój'";
                cmd = new SqlCommand(Sql5, conn);
                DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    Zamowienie_Combo_Napoj.Items.Add(DR[0]);

                }
                conn.Close();
            }
            else
            {
                if (Zamowienie_Combo_Danie.Items.Count > 0) Zamowienie_Combo_Danie.Items.Clear();
                if (Zamowienie_Combo_Zupa.Items.Count > 0) Zamowienie_Combo_Zupa.Items.Clear();
                if (Zamowienie_Combo_Przystawka.Items.Count > 0) Zamowienie_Combo_Przystawka.Items.Clear();
                if (Zamowienie_Combo_Deser.Items.Count > 0) Zamowienie_Combo_Deser.Items.Clear();
                if (Zamowienie_Combo_Napoj.Items.Count > 0) Zamowienie_Combo_Napoj.Items.Clear();
            }
        }

        private void Strzalka_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)Dane_Rezerwacja.SelectedItem;
            if (row != null)
            {

                SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
                conn.Open();
                DateTime date = Convert.ToDateTime(row[2].ToString());
                string Sql = "select Idk from Wyswietl_Rezerwacja where Imie='" + row[0].ToString() + "' and Nazwisko= '" + row[1].ToString() + "' and Stolik = " + Convert.ToInt32(row[4]) + " and Data= '" + date.Year + "-" + date.Month + "-" + date.Day + "' and Godzina= '" + row[3].ToString() + "';";
                SqlCommand cmd = new SqlCommand(Sql, conn);
                int idk = (int)cmd.ExecuteScalar();


                Sql = "select Id_rezerwacji from Rezerwacja where Id_klienta= " + idk + " and Data_rezerwacji= '" + date.Year + "-" + date.Month + "-" + date.Day + "' and Czas_rezerwacji= '" + row[3].ToString() + "';";
                cmd = new SqlCommand(Sql, conn);
                int id_rezerwacji = (int)cmd.ExecuteScalar();

                foreach (zamowienie temp in Zamowienia)
                {
                    if (Convert.ToInt32(row[4]) == temp.Numer_stolika)
                    {
                        temp.Id_klienta = idk;
                        temp.Id_rezerwacji = id_rezerwacji;

                    }
                }
                Combo_Zamowienie_Stolik.Text = row[4].ToString();
                tabControl.SelectedIndex = (tabControl.SelectedIndex - 1);
            }
            else
                MessageBox.Show("Nie wybrano rezerwacji", "Błąd");
        }

        private void Combo_Zamowienie_Stolik_DropDownOpened(object sender, EventArgs e)
        {

            if (Combo_Zamowienie_Stolik.Items.Count > 0) Combo_Zamowienie_Stolik.Items.Clear();

            string Sql = "select Numer, Miejsca from Wyswietl_Stolik where not Numer=0";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();
            if (DR != null)
            {
                while (DR.Read())
                {
                    Combo_Zamowienie_Stolik.Items.Add(DR[0].ToString());

                }
            }
            conn.Close();


        }

        private void Combo_Zamowienie_Stolik_Initialized(object sender, EventArgs e)
        {
            if (Combo_Zamowienie_Stolik.Items.Count > 0) Combo_Zamowienie_Stolik.Items.Clear();

            string Sql = "select Numer, Miejsca from Wyswietl_Stolik where not Numer=0";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();

            while (DR.Read())
            {
                Combo_Zamowienie_Stolik.Items.Add(DR[0].ToString());

            }

            conn.Close();
        }

        private void Przeniesienie(ComboBox combo)
        {
            combo.SelectedItem.ToString();
        }

        private void WypelnijLiczbe(ComboBox combo)
        {
            for (int i = 1; i <= 10; i++)
            {
                combo.Items.Add(i.ToString());
            }
            combo.SelectedIndex = 0;
        }

        private void c1_Initialized(object sender, EventArgs e)
        {
            WypelnijLiczbe(c1);

        }

        private void c2_Initialized(object sender, EventArgs e)
        {
            WypelnijLiczbe(c2);
        }

        private void c3_Initialized(object sender, EventArgs e)
        {
            WypelnijLiczbe(c3);
        }

        private void c4_Initialized(object sender, EventArgs e)
        {
            WypelnijLiczbe(c4);
        }

        private void c5_Initialized(object sender, EventArgs e)
        {
            WypelnijLiczbe(c5);
        }


        private void obsluga(ComboBox combo_Danie, ComboBox c)
        {
            if (combo_Danie.SelectedItem != null)
            {
                foreach (zamowienie temp in Zamowienia)
                {
                    if (Convert.ToInt32(Combo_Zamowienie_Stolik.Text) == temp.Numer_stolika)
                    {
                        if (combo_Danie.SelectedItem != null && Zamowienie_Combo_Menu.SelectedItem != null)
                        {
                            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
                            conn.Open();
                            string Sql = "select * from Produkt_Nazwa_Menu where Nazwa='" + combo_Danie.SelectedItem.ToString() + "' and Menu='" + Zamowienie_Combo_Menu.SelectedItem.ToString() + "'";
                            SqlCommand cmd = new SqlCommand(Sql, conn);
                            SqlDataReader dataReader = cmd.ExecuteReader();

                            if (dataReader != null)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(dataReader);
                                temp.Dodaj(Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()), Convert.ToInt32(c.SelectedItem.ToString()), dt.Rows[0].ItemArray[1].ToString(), Convert.ToDouble(dt.Rows[0].ItemArray[2].ToString()));
                                listView.Items.Clear();
                                temp.Wyswietl(listView);
                                dataReader.Close();
                                
                            }
                            conn.Close();
                            decimal kwota = 0;
                            foreach (dolisty temp1 in listView.Items)
                            {
                                kwota += (Convert.ToDecimal(temp1.Cena) * Convert.ToInt32(temp1.Ilosc));

                            }
                            Suma.Content = "SUMA: " + kwota.ToString();

                        }

                    }
                }

            }

        }

        private void listView_Initialized(object sender, EventArgs e)
        {

        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            obsluga(Zamowienie_Combo_Danie, c1);
        }

        private void b2_Click(object sender, RoutedEventArgs e)
        {
            obsluga(Zamowienie_Combo_Zupa, c2);

        }

        private void b3_Click(object sender, RoutedEventArgs e)
        {
            obsluga(Zamowienie_Combo_Przystawka, c3);

        }

        private void b4_Click(object sender, RoutedEventArgs e)
        {
            obsluga(Zamowienie_Combo_Deser, c4);

        }

        private void b5_Click(object sender, RoutedEventArgs e)
        {
            obsluga(Zamowienie_Combo_Napoj, c5);

        }

        private void krzyżyk_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null && Combo_Zamowienie_Stolik.SelectedItem != null)
            {

                dolisty temp2 = (dolisty)listView.SelectedItem;
                zamowienie temp3= null;
                foreach (zamowienie temp in Zamowienia)
                {
                    if (Convert.ToInt32(Combo_Zamowienie_Stolik.Text) == temp.Numer_stolika)
                    {

                        temp.Usun(temp2.Nazwa, Convert.ToInt32(temp2.Ilosc), temp2.Cena);
                        temp3 = temp;
                    }


                    
                }
                listView.Items.Clear();
                decimal kwota = 0;
                foreach (dolisty temp1 in listView.Items)
                {
                    kwota += (Convert.ToDecimal(temp1.Cena) * Convert.ToInt32(temp1.Ilosc));

                }
                Suma.Content = "SUMA: " + kwota.ToString();
                temp3.Wyswietl(listView);
            }


        }

        private void Combo_Zamowienie_Stolik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Combo_Zamowienie_Stolik.SelectedItem != null)
            {
                foreach (zamowienie temp in Zamowienia)
                {
                    if (Convert.ToInt32(Combo_Zamowienie_Stolik.SelectedItem.ToString()) == temp.Numer_stolika)
                    {
                        listView.Items.Clear();
                        temp.Wyswietl(listView);
                    }
                }
            }
        }

        private void Combo_Zamowienie_Stolik_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (Combo_Zamowienie_Stolik.SelectedItem != null)
            {
                foreach (zamowienie temp in Zamowienia)
                {
                    if (Convert.ToInt32(Combo_Zamowienie_Stolik.SelectedItem.ToString()) == temp.Numer_stolika)
                    {
                        listView.Items.Clear();
                        temp.Wyswietl(listView);
                    }
                }
                decimal kwota = 0;
                foreach (dolisty temp1 in listView.Items)
                {
                    kwota += (Convert.ToDecimal(temp1.Cena) * Convert.ToInt32(temp1.Ilosc));

                }
                Suma.Content = "SUMA: " + kwota.ToString();
            }
        }

        private void rachunek_Click(object sender, RoutedEventArgs e)
        {
            if (listView != null)
            {
                SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                decimal kwota = 0;
                string cmd;

                foreach (dolisty temp in listView.Items)
                {
                    kwota += (Convert.ToDecimal(temp.Cena) * Convert.ToInt32(temp.Ilosc));
                }


                cmd = "Select Id_pracownika from Pracownik where Login_pracownik = '" + logowanie.Login + "'";
                sqlCmd.CommandText = cmd;
                int id_pracownik = (int)sqlCmd.ExecuteScalar();
                int numer_stolika = 0 ;
                int id_klienta = 0;
                int id_rezerwacja =0;
                foreach (zamowienie temp in Zamowienia)
                {
                    if (temp.Numer_stolika == Convert.ToInt32(Combo_Zamowienie_Stolik.SelectedItem.ToString()))
                    {
                        id_klienta = temp.Id_klienta;
                        numer_stolika = temp.Id_stolika;
                    }

                }
                foreach (zamowienie temp in Zamowienia)
                {
                    if (temp.Numer_stolika == Convert.ToInt32(Combo_Zamowienie_Stolik.SelectedItem.ToString()))
                    {
                       id_rezerwacja = temp.Id_rezerwacji;
                    }

                }
                
                DateTime czas = DateTime.Now;
                
                cmd = "Insert Into Zamowienie (Data, Numer_Stolika, Id_rezerwacji, Id_pracownika, Kwota) " +
                 "Values ('" + czas.Year + "-" + czas.Month + "-" + czas.Day + "', " + numer_stolika + ", "  + id_rezerwacja + "," + id_pracownik + "," + kwota + ")";
                sqlCmd.CommandText = cmd;
                sqlCmd.ExecuteNonQuery();
                listView.Items.Clear();
                foreach (zamowienie temp in Zamowienia)
                {
                    if(temp.Numer_stolika == Convert.ToInt32(Combo_Zamowienie_Stolik.SelectedItem.ToString()))
                    {
                        temp.Usun_All();
                    }
                    
                }
                Suma.Content = "Suma: ";
                sqlConn.Close();
            }
        }

        private void Wyloguj_Click(object sender, RoutedEventArgs e)
        {
            Logowanie.Visibility = Visibility.Visible;
            logowanie.Ustaw_haslo("");
            logowanie.Ustaw_login("");
            Panel.Visibility = Visibility.Hidden;
            Wyloguj.Visibility = Visibility.Hidden;
    
        }

        private void Dodaj_Menu_Click(object sender, RoutedEventArgs e)
        {
            if (label_DodajMenu.Visibility == Visibility.Visible)
            {
                label_DodajMenu.Visibility = Visibility.Hidden;
                textBox_dodaj_menu.Visibility = Visibility.Hidden;
                if (textBox_dodaj_menu.Text != "")
                {

                    SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConn;
                    string cmd = "Select count(*) from Menu where Opis='" + textBox_dodaj_menu.Text + "' and aktualnosc='aktualne'";
                    sqlCmd.CommandText = cmd;
                    int liczba = (int)sqlCmd.ExecuteScalar();
                    if (liczba == 0)
                    {
                        cmd = "Insert into Menu (Opis) Values ('" + textBox_dodaj_menu.Text + "')";
                        sqlCmd.CommandText = cmd;
                        sqlCmd.ExecuteNonQuery();
                        textBox_dodaj_menu.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Nie mozna dodać już istnieje takie menu.", "Bład");
                    }
                    sqlConn.Close();
                }
            }
            else
            {
                label_DodajMenu.Visibility = Visibility.Visible;
                textBox_dodaj_menu.Visibility = Visibility.Visible;
            }

        }

     
        private void comboBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if (comboBox_Copy.Items.Count > 0) comboBox_Copy.Items.Clear();

            string Sql = "select Opis from Menu where aktualnosc='aktualne'";
            SqlConnection conn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            SqlDataReader DR = cmd.ExecuteReader();

            while (DR.Read())
            {
                comboBox_Copy.Items.Add(DR[0]);

            }

            conn.Close();
        }

        private void Usun_Menu_Click(object sender, RoutedEventArgs e)
        {
            if (label_UsunMenu.Visibility == Visibility.Visible)
            {
                label_UsunMenu.Visibility = Visibility.Hidden;
                comboBox_Copy.Visibility = Visibility.Hidden;
                if (comboBox_Copy.SelectedItem != null)
                {
                    SqlConnection sqlConn = new SqlConnection("Server = " + serwer + ";Integrated Security = SSPI; Database = 'Restauracja'");

                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "Select Id_menu from Menu where Opis = '" + comboBox.Text + "'and aktualnosc='aktualne'";
                    int id_menu = (int)sqlCmd.ExecuteScalar();
                    sqlCmd.CommandText = "update Menu set aktualnosc='nieaktualne' where Opis='" + comboBox_Copy.SelectedItem.ToString() + "'";

                    sqlCmd.ExecuteNonQuery();

                    sqlCmd.CommandText = "update Produkt set Aktualnosc= 'nieaktualny' where Id_menu=" + id_menu;
                    sqlCmd.ExecuteNonQuery();
                   
                    sqlConn.Close();

                    wyswietlMenu();

                }
            }
            else
            {
                label_UsunMenu.Visibility = Visibility.Visible;
                comboBox_Copy.Visibility = Visibility.Visible;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
