﻿using System;
using System.Collections.Generic;
using System.IO;

namespace XiaomiMediaDistributor
{
    public sealed class PhotoDistributor : Distributor
    {
        #region .ctor

        public PhotoDistributor(string sourceDirectory, string targetDirectory) : base(sourceDirectory, targetDirectory)
        {
        }

        #endregion

        #region Helpers

        protected override void DisplayFilesCount(int filesCount)
        {
            Console.WriteLine($"Обнаружено {filesCount} фотографий");
        }        

        protected override IEnumerable<string> GetFilesList(string sourceDirectory)
        {
            try
            {
                return Directory.EnumerateFiles(sourceDirectory, "*.jpg", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошла ошибка при поиске фотографий: " + e.Message);
            }

            return new List<string>();
        }

        protected override void DisplayStartMessage()
        {
            Console.WriteLine("Запуск распределения фотографий");
        }

        protected override void DisplayFinishMessage()
        {
            Console.WriteLine("Распределение фотографий завершено");
        }

        #endregion
    }
}
