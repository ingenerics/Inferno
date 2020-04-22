using System;
using System.IO;
using System.Text.Json;

namespace WorldCup.Repo
{
    internal class RepoFactory
    {
        public static IRepo<TKey, TValue> Build<TValue, TKey>(string relativePath, Func<TValue, TKey> keySelector)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + relativePath;
            var jsonString = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<TValue[]>(jsonString, options);

            return new RepoBase<TKey, TValue>(items, keySelector);
        }
    }
}
