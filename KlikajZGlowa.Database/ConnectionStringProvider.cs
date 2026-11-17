using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Database
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString(string databaseFileName, string projectFolder)
        {
            // Pobieramy katalog bin aplikacji
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            // Od razu przechodzimy do szukania w folderze projektu źródłowego (cztery poziomy w górę z bin/Debug/net10.0/)
            string projectPath = Path.GetFullPath(Path.Combine(baseFolder, "..", "..", "..", "..", projectFolder, databaseFileName));

            if (File.Exists(projectPath))
            {
                return BuildConnectionString(projectPath);
            }

            // Jeśli powyższe nie zadziałało, sprawdzamy alternatywną strukturę (np. o 3 poziomy w górę w zależności od wersji .NET)
            string alternativeProjectPath = Path.GetFullPath(Path.Combine(baseFolder, "..", "..", "..", projectFolder, databaseFileName));
            if (File.Exists(alternativeProjectPath))
            {
                return BuildConnectionString(alternativeProjectPath);
            }

            throw new FileNotFoundException($"Database file '{databaseFileName}' not found in '{projectPath}' or '{alternativeProjectPath}'.");
        }

        private static string BuildConnectionString(string filePath)
        {
            return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={filePath};Integrated Security=True";
        }
    }
}
