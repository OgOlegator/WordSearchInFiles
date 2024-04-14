using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using WordSearchInFiles.Models;

namespace WordSearchInFiles.Services
{
    /// <summary>
    /// Сервис поиска слова в файлах
    /// </summary>
    public class SearchIWordService
    {
        private const string SolutionAndApiSubPath = "WordSearchInFiles\\WordSearchInFiles";

        public async Task<List<string>> ExecuteAsync(string word)
        {
            var listTasks = new List<Task<SearchResultDto>>();
 
            foreach (var file in GetFilesPath()) 
            {
                listTasks.Add(SearchWordInFileAsync(file, word));
            }

            await Task.WhenAll(listTasks);

            return listTasks
                .Where(task => task.Result.Result)
                .Select(task => task.Result.FileName)
                .ToList();
        }

        /// <summary>
        /// Получить пути к файлам, в которых осуществляется поиск
        /// </summary>
        /// <returns>Пути к файлам</returns>
        /// <exception cref="Exception"></exception>
        private List<string> GetFilesPath()
        {
            try
            {
                var projectLocation = Assembly.GetExecutingAssembly().Location;

                var apiFileLocation = projectLocation.Split(SolutionAndApiSubPath).First() + SolutionAndApiSubPath;

                var path = Path.Combine(apiFileLocation, "files");

                var files = Directory.GetFiles(path);

                return files.ToList();
            }
            catch (DirectoryNotFoundException)
            {
                throw new Exception("Папка содержащая файлы с тестами не найдена");
            }
            catch (PathTooLongException)
            {
                throw new Exception("Путь к папке с текстами слишком длинный");
            }
            catch 
            {
                throw new Exception("Не удалось получить файлы с текстами");
            }
        }

        /// <summary>
        /// Поиск слова в файле
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="word">Искомое слово</param>
        /// <returns>Результат поиска</returns>
        /// <exception cref="Exception">Ошибка поиска</exception>
        private async Task<SearchResultDto> SearchWordInFileAsync(string filePath, string word)
        {
            try
            {
                var textFile = string.Empty;

                using (var file = new StreamReader(filePath))
                    textFile = await file.ReadToEndAsync();

                var result = new SearchResultDto
                {
                    FileName = Path.GetFileName(filePath),
                    Result = Regex.IsMatch(textFile.ToLower(), $"\\b{word.ToLower()}\\b")   
                };

                return result;
            }
            catch 
            {
                throw new Exception($"Ошибка при поиске слова в файле {Path.GetFileName(filePath)}");
            }
        }
    }
}
