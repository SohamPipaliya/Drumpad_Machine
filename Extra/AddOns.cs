using Drumpad_Machine.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Json = Newtonsoft.Json.JsonConvert;

namespace Drumpad_Machine.Extra
{
    public static class AddOns
    {
        public static string Path(string directory = "Data", string filename = "Data.json") =>
            System.IO.Path.Combine(Directory.GetCurrentDirectory(), directory, filename);

        public async static Task WriteToJson(this IEnumerable<IFormFile> value) => await Task.Run(async () =>
        {
            var list = await ReadFromJson();
            foreach (var item in value)
            {
                list.Add(new() { ID = Guid.NewGuid(), Name = item.FileName, Size = item.Length / 1024 });
            }
            await File.WriteAllTextAsync(Path(), Json.SerializeObject(list));
        });

        public async static Task WriteToJson(this IList<Audio> value) => await Task.Run(async () =>
        {
            await File.WriteAllTextAsync(Path(), Json.SerializeObject(value));
        });

        public async static Task<List<Audio>> ReadFromJson() => await Task.Run(async () =>
        {
            try
            {
                if (File.Exists(Path()))
                    return Json.DeserializeObject<List<Audio>>(File.ReadAllText(Path())) ?? new();
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
            }
            return new();
        });

        public async static Task LogAsync(this Exception value) => await Task.Run(async () =>
        {
            string paths = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Logs", "DrumpadException.log");
            using (StreamWriter writer = new(paths, true) { AutoFlush = true })
            {
                await writer.WriteLineAsync($"{DateTime.Now}");
                await writer.WriteLineAsync(value.Message);
                await writer.WriteLineAsync(value.StackTrace);
                await writer.WriteLineAsync();
                await writer.FlushAsync();
                writer.Close();
            }
        });

        public static async Task<byte[]> GetJsonFileBytes()
        {
            StringBuilder sb = new();
            try
            {
                sb.Append('[');
                sb.Append('\n');
                var list = await ReadFromJson();
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append("  {");
                    sb.Append('\n');
                    sb.Append($"    \"Name\": \"api/GetData/{list[i].Name}\"");
                    sb.Append('\n');
                    sb.Append("  }");
                    if (i != list.Count - 1)
                        sb.Append(',');
                    sb.Append('\n');
                }
                sb.Append(']');
                return Encoding.UTF8.GetBytes(sb.ToString());
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return new byte[0];
            }
            finally
            {
                sb = null;
            }
        }
    }
}