using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XiaomiMediaDistributor
{
    public abstract class Distributor
    {
        #region Fields

        private readonly string _sourceDirectory;
        private readonly string _targetDirectory;

        private readonly Dictionary<int, int> _yearDirectories;
        private readonly Dictionary<string, int> _monthDirectories;

        #endregion

        #region .ctor

        public Distributor(string sourceDirectory, string targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;

            _yearDirectories = new Dictionary<int, int>();
            _monthDirectories = new Dictionary<string, int>();
        }

        #endregion

        #region Methods

        public void Distribute()
        {           

            if (!IsDirectoryExists(_sourceDirectory))
            {
                Console.WriteLine($"Папка '{_sourceDirectory}' не существует!");
                return;
            }
            if (!IsDirectoryExists(_targetDirectory))
            {
                Console.WriteLine($"Папка '{_targetDirectory}' не существует!");
                return;
            }

            var files = GetFilesList(_sourceDirectory).ToList();

            DisplayFilesCount(files.Count);

            if (files.Count > 0)
            {
                DisplayStartMessage();
                StartDistribution(files);
                DisplayFinishMessage();
            }
        }

        #endregion

        #region Helpers

        private bool IsDirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        protected abstract IEnumerable<string> GetFilesList(string sourceDirectory);

        protected abstract void DisplayFilesCount(int filesCount);

        private void StartDistribution(List<string> files)
        {
            var totalCount = files.Count;

            for (int i = 0; i < totalCount; i++)
            {
                var fileInfo = GetFileInfo(files[i]);

                if (fileInfo == null) continue;

                var fileDate = GetFileDate(fileInfo);

                if (!CheckYearDirectory(fileDate)) continue;

                if (!CheckMonthDirectory(fileDate)) continue;

                CopyFile(fileInfo, fileDate);
                
                DisplayProcessPercent(totalCount, i + 1);
            }
        }

        protected FileInfo GetFileInfo(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    return new FileInfo(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Произошла ошибка при чтении файла '{filePath}': {e.Message}");
                }
            }

            return null;
        }

        private DateTime GetFileDate(FileInfo fileInfo)
        {
            //пример имени: IMG_20180408_105125(1)

            var year = int.Parse(fileInfo.Name.Substring(4, 4));
            var month = int.Parse(fileInfo.Name.Substring(8, 2));
            var day = int.Parse(fileInfo.Name.Substring(10, 2));

            return new DateTime(year, month, day);
        }

        private bool CheckYearDirectory(DateTime fileDate)
        {
            if (_yearDirectories.ContainsKey(fileDate.Year)) return true;

            return CreateYearDirectory(fileDate.Year);
        }

        private bool CreateYearDirectory(int year)
        {
            var directoryPath = Path.Combine(_targetDirectory, year.ToString());

            try
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось создать папку '{directoryPath}': {e.Message}");
            }

            return false;
        }

        private string GetMonthString(DateTime fileDate)
        {
            return $"{fileDate.Year}{fileDate.Month:00}";
        }

        private bool CheckMonthDirectory(DateTime fileDate)
        {
            if (_monthDirectories.ContainsKey(GetMonthString(fileDate))) return true;

            return CreateMonthDirectory(fileDate);
        }

        private bool CreateMonthDirectory(DateTime fileDate)
        {
            var directoryPath = Path.Combine(_targetDirectory, fileDate.Year.ToString(), $"{fileDate.Month:00}");

            try
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось создать папку '{directoryPath}': {e.Message}");
            }

            return false;
        }

        private void CopyFile(FileInfo fileInfo, DateTime fileDate)
        {
            try
            {
                var bytes = File.ReadAllBytes(fileInfo.FullName);
                var newFilePath = Path.Combine(_targetDirectory, fileDate.Year.ToString(), $"{fileDate.Month:00}", fileInfo.Name);
                File.WriteAllBytes(newFilePath, bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"При копировании файла '{fileInfo.FullName}' возникла ошибка: " + e.Message);
            }
        }

        private void DisplayProcessPercent(int totalCount, int finishedCount)
        {
            var percent = finishedCount * 100 / (double)totalCount;

            var needToShow = percent % 10 == 0;

            if (needToShow)
                Console.WriteLine($"Выполнено {percent}%");
        }

        protected abstract void DisplayStartMessage();

        protected abstract void DisplayFinishMessage();

        #endregion
    }
}
