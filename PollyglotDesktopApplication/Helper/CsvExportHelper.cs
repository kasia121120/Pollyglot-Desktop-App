using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PollyglotDesktopApp.Helper
{
    public static class CsvExportHelper
    {
        public static bool ExportToCsv(string tytulRaportu, string domyslnaNazwaPliku, IEnumerable<string> linie)
        {
            var dialog = new SaveFileDialog
            {
                Title = $"Zapisz {tytulRaportu}",
                Filter = "Pliki CSV (*.csv)|*.csv",
                FileName = domyslnaNazwaPliku
            };

            if (dialog.ShowDialog() != true)
                return false;

            var tekst = string.Join(Environment.NewLine, linie ?? Array.Empty<string>());
            File.WriteAllText(dialog.FileName, tekst, new UTF8Encoding(true));
            return true;
        }
    }
}
