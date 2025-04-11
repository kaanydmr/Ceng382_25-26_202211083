using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Reflection;



/*

 Promt:
 Utility Class for JSON Export
• Create a new C# class file named Utils.cs.
• Inside it, implement a generic method that can export any class to JSON.
• The method should work with any model class.
• This class must be implemented as a singleton, so it can be accessed from anywhere in the
project.



*/

namespace Week5.Utilities
{
    public class Utils
    {
        private static Utils _instance;
        private static readonly object _lock = new object();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Utils();
                        }
                    }
                }
                return _instance;
            }
        }

        public string ExportToJson<T>(IEnumerable<T> data, List<string> selectedColumns = null)
        {
            if (data == null)
                return "[]";

            if (selectedColumns == null || !selectedColumns.Any())
            {
                // Export all properties if no columns selected
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                // Export only selected properties
                var result = data.Select(item => {
                    var type = typeof(T);
                    var properties = type.GetProperties();
                    var selectedProperties = new Dictionary<string, object>();
                    
                    foreach (var prop in properties)
                    {
                        if (selectedColumns.Contains(prop.Name))
                        {
                            selectedProperties[prop.Name] = prop.GetValue(item);
                        }
                    }
                    
                    return selectedProperties;
                });
                
                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}