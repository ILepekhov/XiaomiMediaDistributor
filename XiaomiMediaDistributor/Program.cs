using System;

namespace XiaomiMediaDistributor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к папке с фото и видео: ");
            var sourceDirectory = Console.ReadLine();

            Console.WriteLine("Введите путь к папке, в которую будут помещены сортированные файлы: ");
            var targetDirectory = Console.ReadLine();

            new PhotoDistributor(sourceDirectory, targetDirectory).Distribute();
            new VideoDistributor(sourceDirectory, targetDirectory).Distribute();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadLine();
        }
    }
}
