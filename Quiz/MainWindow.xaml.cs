using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Quiz
{
    public partial class MainWindow : Window
    {
        // Lista a betöltött szavak tárolására
        private List<Word> wordsList = new List<Word>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Indítás gomb kattintásának eseménye
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // A ComboBox-ból kiválasztott nehézségi szint értékének lekérése
            string selectedDifficulty = ((ComboBoxItem)DifficultyComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(selectedDifficulty))
            {
                MessageBox.Show("Kérlek válassz egy nehézségi szintet!", "Hiba");
                return;
            }

            // Fájl kiválasztás
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV fájlok (*.csv)|*.csv|JSON fájlok (*.json)|*.json|XML fájlok (*.xml)|*.xml";
            openFileDialog.Title = "Fájl kiválasztása";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                try
                {
                    // Fájl formátumának megfelelő betöltés
                    if (fileExtension == ".csv")
                    {
                        LoadCsv(filePath);
                    }
                    else if (fileExtension == ".json")
                    {
                        LoadJson(filePath);
                    }
                    else if (fileExtension == ".xml")
                    {
                        LoadXml(filePath);
                    }
                    else
                    {
                        MessageBox.Show("Nem támogatott fájlformátum!", "Hiba");
                        return;
                    }

                    // Indíthatjuk el a kvízt a betöltött adatok alapján
                    StartQuiz();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a fájl betöltésekor: {ex.Message}", "Hiba");
                }
            }
        }

        // Kvíz indítása
        private void StartQuiz()
        {
            // Például a választott nehézségi szint alapján szűrhetünk a szavakon
            MessageBox.Show($"A kvíz indítása a {wordsList.Count} szóval!", "Kvíz indítása");

            // Itt kezdheted el a kvíz logikát (pl. kérdések generálása)
        }

        // CSV fájl betöltése (az első sor kihagyásával)
        private void LoadCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); // Első sor kihagyása
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length == 2)
                {
                    wordsList.Add(new Word { Szo = values[0], Translation = values[1] });
                }
            }
        }

        // JSON fájl betöltése (az első sort itt nem kell kihagyni, mert JSON-ban nincsen fejléc)
        private void LoadJson(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            var words = JsonConvert.DeserializeObject<List<Word>>(jsonData);
            wordsList.AddRange(words);
        }

        // XML fájl betöltése (XML fejléc automatikusan kihagyva)
        private void LoadXml(string filePath)
        {
            var doc = XDocument.Load(filePath);
            foreach (var element in doc.Descendants("Word"))
            {
                var word = element.Element("Word")?.Value;
                var translation = element.Element("Translation")?.Value;
                if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(translation))
                {
                    wordsList.Add(new Word { Szo = word, Translation = translation });
                }
            }
        }
    }

    // JSON fájl struktúra
    public class Word
    {
        public string Szo { get; set; }
        public string Translation { get; set; }
    }
}
