using AnmReader.src.CSV;
using BrawlhallaANMReader.utils;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

//TODO: implement a proper way to store CSV file headers
//  an idea would be to make a serperate CSV class, further sbstracting the deseralization and serialization by having a builtin stream etc
//TODO: add unit testing 
namespace BrawlhallaANMReader.CSV
{
    /// <summary>
    /// Serializes and Deserializes CSV files
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvSerializer<T> where T : class, new()
    {
        public bool IgnoreReferenceTypesExceptString { get; set; } = true;
        public bool HasHeader { get; set; } = false;
        public string Delimiter { get; set; } = ",";

        private readonly List<PropertyInfo> _properties;
        private string? _header;

        public CsvSerializer()
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                                BindingFlags.Instance |
                                                BindingFlags.GetProperty |
                                                BindingFlags.SetProperty);

            IQueryable<PropertyInfo> query = properties.AsQueryable();

            if (IgnoreReferenceTypesExceptString)
            {
                query = query.Where(prop => prop.PropertyType.IsValueType || prop.PropertyType.Name == "String");
            }

            _properties = (from prop in properties
                           where prop.GetCustomAttribute<CsvIgnoreAttribute>() == null
                           //orderby prop.Name
                           select prop).ToList();
        }

        /// <summary>
        /// Deserializes CSV file from stream
        /// </summary>
        /// <param name="stream">file stream of CSV file</param>
        /// <returns>list of class instances of T</returns>
        /// <exception cref="InvalidCsvFormatException">Csv file is improperly parsed or faulty</exception>
        public IList<T> Deserialize(Stream stream)
        {
            string[]? collumns;

            using TextFieldParser tfp = new(stream);
            tfp.TextFieldType = FieldType.Delimited;
            tfp.SetDelimiters(",");

            List<T> data = new();

            try
            {
                if (HasHeader)
                {
                    _header = tfp.ReadLine();
                }
                collumns = tfp.ReadFields();
                for(int i = 0; i<collumns.Length; i++)
                {
                    collumns[i] = collumns[i].Replace(".", "_");
                }
                collumns.ToList().ForEach(a => Logger.Debug(a));
                if (collumns is null)
                    throw new InvalidCsvFormatException(@"Failed to read collumns");
            }
            catch (Exception ex)
            {
                Logger.Error("The CSV File is Invalid. See Inner Exception for more inoformation.");
                throw new InvalidCsvFormatException("The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }

            int n = 1;
            while (!tfp.EndOfData)
            {
                string[]? cells = tfp.ReadFields() ?? throw new InvalidCsvFormatException(@"Reader finished all lines before end of file.");
                if (cells.Length == 1 && cells[0] != null && cells[0] == "EOF")
                    break;

                T datum = new();
                for (int i = 0; i < cells.Length; i++)
                {
                    string val = cells[i];
                    string col = collumns[i];

                    if (val.IndexOf("\"") > 0 && val.Length > 0)
                    {
                        val = $"\"{val.Replace("\"", "\"\"")}\"";
                    }

                    PropertyInfo? prop = _properties.FirstOrDefault(a => a.Name.Equals(col, StringComparison.InvariantCultureIgnoreCase));

                    if (prop is not null)
                    {
                        try
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                            object? convertedValue = converter.ConvertFrom(val);
                            prop.SetValue(datum, convertedValue, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Cannot Parse {col}:'{val}'");
                            throw new InvalidCsvFormatException($"Cannot Parse {col}:'{val}'", ex);
                        }
                    }
                }
                data.Add(datum);
            }
            return data;
        }

        public void Serialize(Stream stream, IList<T> data)
        {
            StringBuilder sb = new();
            List<string> values = new();

            if (HasHeader)
            {
                sb.AppendLine(_header);
            }

            sb.AppendLine(string.Join(Delimiter, _properties.Select(a => a.Name.Replace("_",".")).ToArray()));

            foreach (T item in data)
            {
                values.Clear(); //🍆
                foreach (PropertyInfo prop in _properties)
                {
                    String value = prop.GetValue(item)?.ToString() ?? "";

                    if (value.IndexOf(',') > 0)
                        value = $"\"{value}\"";

                    values.Add(value);
                }

                sb.AppendLine(string.Join(Delimiter.ToString(), values.ToArray()));
            }

            using StreamWriter sw = new(stream);
            sw.Write(sb.ToString().Trim());
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class CsvIgnoreAttribute : Attribute { }
}
