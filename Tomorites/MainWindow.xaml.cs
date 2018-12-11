using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Tomorites
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> karakter_kombinaciok = new List<string>();

        bool e = true;
        void Kombo(char[] tomb, string prefix, int tomb_hossza, int hanyszor)
        {
            if (hanyszor == 0)
            {
                if (!e)
                    karakter_kombinaciok.Add(prefix);
                e = false;
                return;
            }

            for (int i = 0; i < tomb_hossza; ++i)
            {
                string uj_prefix = prefix + tomb[i];

                Kombo(tomb, uj_prefix, tomb_hossza, hanyszor - 1);
            }
        }

        string file_nev_kiszedes(string szoveg, char mi_szerint)
        {
            string[] s = szoveg.Split(mi_szerint);
            return s[s.Length - 1];
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void tomoritendoSzovegTallozasBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog szoveg_tallozasa_dlg = new OpenFileDialog();

            szoveg_tallozasa_dlg.Multiselect = true;
            szoveg_tallozasa_dlg.FileName = "";
            szoveg_tallozasa_dlg.Title = "Tömörítendő szöveg tallózása";
            szoveg_tallozasa_dlg.DefaultExt = ".html";
            szoveg_tallozasa_dlg.Filter = "html|*.html";

            string bekert_szoveg_nev = "";

            if (szoveg_tallozasa_dlg.ShowDialog() == true)
            {
                bekert_szovegek_szama_lbl.Content = string.Format("Bekért szövegek száma: {0}", szoveg_tallozasa_dlg.FileNames.Length);

                foreach (string eleresi_ut in szoveg_tallozasa_dlg.FileNames)
                {
                    bekert_szoveg_nev = file_nev_kiszedes(eleresi_ut, '\\').Replace('.', '_');

                    List<string> bekert_szoveg = new List<string>();

                    StreamReader szoveg_sr = new StreamReader(eleresi_ut);
                    while (!szoveg_sr.EndOfStream)
                    {
                        string[] szavak = szoveg_sr.ReadLine().Split(' ');
                        foreach (var item in szavak)
                        {
                            if (item != "")
                                bekert_szoveg.Add(item);
                        }
                    }
                    szoveg_sr.Close();

                    //melyik szóból mennyi
                    Dictionary<string, int> szavak_db = new Dictionary<string, int>();

                    for (int i = 0; i < bekert_szoveg.Count; i++)
                    {
                        if (szavak_db.ContainsKey(bekert_szoveg[i]))
                            szavak_db[bekert_szoveg[i]] += 1;
                        else
                            szavak_db.Add(bekert_szoveg[i], 1);

                    }

                    var rendezett_szavak = szavak_db.ToList();
                    rendezett_szavak.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

                    //ami hoszabb mint 1 karakter
                    int szavak_szama = 0;
                    for (int i = 0; i < rendezett_szavak.Count; i++)
                    {
                        if (rendezett_szavak[i].Key.ToString().Length > 1 && rendezett_szavak[i].Value > 1)
                        {
                            szavak_szama++;
                        }
                    }

                    string spec = "!#%&'()*+,-.:;<=>?@[]^_`{|}~";
                    char[] spec_karakterek = new char[spec.Length];
                    spec_karakterek = spec.ToCharArray();

                    int szukseges_kombok_szama = 0;
                    while (szavak_szama - Math.Pow(spec_karakterek.Length, szukseges_kombok_szama) > 0)
                    {
                        szukseges_kombok_szama++;
                    }

                    //szukseges_kombok_szama - 1 a jó de ez most nem érdekel senkit
                    for (int i = 0; i <= szukseges_kombok_szama; i++)
                    {
                        Kombo(spec_karakterek, "", spec_karakterek.Length, i);
                    }

                    List<string> megmaradt_szavak = new List<string>();
                    //kiszűröm azokat a szavakat amiknek adok egy kombót
                    for (int i = 0; i < rendezett_szavak.Count; i++)
                    {
                        if (rendezett_szavak[i].Key.ToString().Length > 2 && rendezett_szavak[i].Value > 1)
                        {
                            megmaradt_szavak.Add(rendezett_szavak[i].Key.TrimStart());
                        }
                    }

                    Dictionary<string, string> kulcs = new Dictionary<string, string>();
                    for (int i = 0; i < megmaradt_szavak.Count; i++)
                    {
                        kulcs.Add(karakter_kombinaciok[i], megmaradt_szavak[i]);
                    }

                    /*mentés
                    SaveFileDialog kulcs_mentese_savedlg = new SaveFileDialog();
                    kulcs_mentese_savedlg.FileName = bekert_szoveg_nev + "_kulcs";
                    kulcs_mentese_savedlg.Title = "Kulcs mentése";
                    kulcs_mentese_savedlg.DefaultExt = ".txt";*/

                    //addig megy amíg el nem menti a felhasználó
                   /* do
                    {
                        MessageBox.Show("Kulcs mentése", "Tömörített szöveg kulcsának mentése", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    }
                    while (kulcs_mentese_savedlg.ShowDialog() == false);*/

                    StreamWriter kulcs_mentese_sw = new StreamWriter(Path.GetDirectoryName(eleresi_ut) + "/" + bekert_szoveg_nev + "_kulcs.txt");
                    foreach (var item in kulcs)
                    {
                        kulcs_mentese_sw.WriteLine("{0}\t{1}", item.Key, item.Value);
                    }
                    kulcs_mentese_sw.Close();

                    string kesz_szoveg = "";
                    bool mehet_e = false;
                    for (int i = 0; i < bekert_szoveg.Count; i++)
                    {
                        mehet_e = false;
                        foreach (var item in kulcs)
                        {
                            if (bekert_szoveg[i] == item.Value)
                            {
                                kesz_szoveg += item.Key + " ";
                                mehet_e = true;
                            }
                        }
                        if (!mehet_e)
                        {
                            kesz_szoveg += bekert_szoveg[i] + " ";
                        }
                    }

                   /* SaveFileDialog szoveg_mentese_savedlg = new SaveFileDialog();

                    szoveg_mentese_savedlg.FileName = "tömörített_" + bekert_szoveg_nev;
                    szoveg_mentese_savedlg.Title = "Tömörített szöveg mentése";
                    szoveg_mentese_savedlg.DefaultExt = ".txt";

                    do
                    {
                        MessageBox.Show("Tömörített szöveg mentése", "Tömörített szöveg mentése", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    }
                    while (szoveg_mentese_savedlg.ShowDialog() == false);
                    */
                    StreamWriter tomoritett_szoveg_sw = new StreamWriter(Path.GetDirectoryName(eleresi_ut) + "/" + bekert_szoveg_nev + "_tomoritett.txt");
                    tomoritett_szoveg_sw.Write(kesz_szoveg);
                    tomoritett_szoveg_sw.Close();
                }
            }
        }


        List<string> bekert_titkositott_szoveg = new List<string>();

        private void kicsomagolandoSzovegTallozasBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog szoveg_tallozasa_dlg = new OpenFileDialog();

            szoveg_tallozasa_dlg.FileName = "";
            szoveg_tallozasa_dlg.Title = "Kicsomagolandó szöveg tallózása";
            szoveg_tallozasa_dlg.DefaultExt = ".txt";
            szoveg_tallozasa_dlg.Filter = "text file|*.txt|javascript|*.js|html|*.html|css|*.css";

            Nullable<bool> result = szoveg_tallozasa_dlg.ShowDialog();
            if (result == true)
            {
                string bekert_szoveg_eleresi_ut = szoveg_tallozasa_dlg.FileName;

                StreamReader bekert_szoveg_sr = new StreamReader(bekert_szoveg_eleresi_ut);
                while (!bekert_szoveg_sr.EndOfStream)
                {
                    string[] szavak = bekert_szoveg_sr.ReadLine().Split(' ');
                    foreach (var item in szavak)
                    {
                        bekert_titkositott_szoveg.Add(item);
                    }
                }
                bekert_szoveg_sr.Close();
            }
        }



        private void kulcsTallozasBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog bekert_kulcs_dlg = new OpenFileDialog();

            bekert_kulcs_dlg.FileName = "";
            bekert_kulcs_dlg.Title = "Kicsomagolandó szöveg kulcsának tallózása";
            bekert_kulcs_dlg.DefaultExt = ".txt";
            bekert_kulcs_dlg.Filter = "text file|*.txt|javascript|*.js|html|*.html|css|*.css";

            Nullable<bool> result = bekert_kulcs_dlg.ShowDialog();
            if (result == true)
            {
                string bekert_kulcs_nev = bekert_kulcs_dlg.SafeFileName.Replace('.', '_');

                string bekert_kulcs_eleresi_ut = bekert_kulcs_dlg.FileName;

                Dictionary<string, string> bekert_titkositott_szoveg_kulcsa = new Dictionary<string, string>();

                StreamReader bekert_kulcs_sr = new StreamReader(bekert_kulcs_eleresi_ut);
                while (!bekert_kulcs_sr.EndOfStream)
                {
                    string[] szavak = bekert_kulcs_sr.ReadLine().Split('\t');
                    bekert_titkositott_szoveg_kulcsa.Add(szavak[0], szavak[1]);
                }
                bekert_kulcs_sr.Close();


                string kesz_szoveg = "";
                bool mehet_e = false;
                for (int i = 0; i < bekert_titkositott_szoveg.Count; i++)
                {
                    mehet_e = false;
                    foreach (var item in bekert_titkositott_szoveg_kulcsa)
                    {
                        if (bekert_titkositott_szoveg[i] == item.Key)
                        {
                            kesz_szoveg += item.Value;
                            mehet_e = true;
                        }
                    }
                    if (!mehet_e)
                    {
                        kesz_szoveg += bekert_titkositott_szoveg[i];
                    }
                    kesz_szoveg += " ";
                }


                SaveFileDialog kicsomagolt_szoveg_savedlg = new SaveFileDialog();

                kicsomagolt_szoveg_savedlg.FileName = "kicsomagolt_szöveg_" + bekert_kulcs_nev;
                kicsomagolt_szoveg_savedlg.Title = "Kicsomagolt szöveg mentése mentése";
                kicsomagolt_szoveg_savedlg.DefaultExt = ".html";

                do
                {
                    MessageBox.Show("Kicsomagolt szöveg mentése", "Kicsomagolt szöveg mentése", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                }
                while (kicsomagolt_szoveg_savedlg.ShowDialog() == false);

                StreamWriter kicsomagolt_szoveg_sw = new StreamWriter(kicsomagolt_szoveg_savedlg.FileName);
                kicsomagolt_szoveg_sw.Write(kesz_szoveg);
                kicsomagolt_szoveg_sw.Close();


                /*törlöm a listák és szótárak tartalmát
                hogy ha mégegyszer tömörít a felhasználó
                akkor csak jó adatok legyenek bennük*/
                karakter_kombinaciok.Clear();
                bekert_titkositott_szoveg.Clear();
                bekert_titkositott_szoveg_kulcsa.Clear();
            }
        }
    }
}
