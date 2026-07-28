using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RobloxFlagInjector
{
    public enum FlagValueType
    {
        String,
        Integer,
        Boolean
    }

    public class FlagEntry
    {
        public string Name { get; set; } = "";
        public string RawValue { get; set; } = ""; // always stored as text in the grid
        public FlagValueType ValueType { get; set; } = FlagValueType.String;

        public object GetTypedValue()
        {
            switch (ValueType)
            {
                case FlagValueType.Boolean:
                    return bool.TryParse(RawValue, out var b) && b;
                case FlagValueType.Integer:
                    return long.TryParse(RawValue, out var i) ? i : 0L;
                default:
                    return RawValue;
            }
        }
    }

    /// <summary>
    /// Handles reading and writing ClientAppSettings.json, which is just a
    /// flat JSON object of "FFlagName": value pairs.
    /// </summary>
    public static class FlagFile
    {
        public static List<FlagEntry> Load(string path)
        {
            var entries = new List<FlagEntry>();

            if (!File.Exists(path))
                return entries;

            string text = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(text))
                return entries;

            using var doc = JsonDocument.Parse(text);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var entry = new FlagEntry { Name = prop.Name };

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        entry.ValueType = FlagValueType.Boolean;
                        entry.RawValue = prop.Value.GetBoolean().ToString();
                        break;
                    case JsonValueKind.Number:
                        entry.ValueType = FlagValueType.Integer;
                        entry.RawValue = prop.Value.GetRawText();
                        break;
                    default:
                        entry.ValueType = FlagValueType.String;
                        entry.RawValue = prop.Value.GetString() ?? "";
                        break;
                }

                entries.Add(entry);
            }

            return entries;
        }

        public static void Save(string path, List<FlagEntry> entries)
        {
            var dict = new Dictionary<string, object>();
            foreach (var e in entries)
            {
                if (string.IsNullOrWhiteSpace(e.Name))
                    continue;
                dict[e.Name] = e.GetTypedValue();
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(dict, options);
            File.WriteAllText(path, json);
        }
    }
}
