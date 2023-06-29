using BrawlhallaANMReader.utils;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BrawlhallaANMReader.CSV
{
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

            _properties = (from prop in properties
                           where prop.GetCustomAttribute<CsvIgnoreAttribute>() == null
                           //orderby prop.Name
                           select prop).ToList();
        }

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
                if (collumns is null)
                    throw new InvalidCsvFormatException(@"Failed to read collumns");
            }
            catch (Exception ex)
            {
                Logger.Error("The CSV File is Invalid. See Inner Exception for more inoformation.");
                throw;
            }

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

                    string[] subclass = col.Split('.');
                    PropertyInfo? prop = _properties.FirstOrDefault(a => a.Name.Equals(subclass[0], StringComparison.InvariantCultureIgnoreCase));
                    if (prop is not null)
                    {
                        if (subclass.Length > 1)
                        {
                            if (subclass.Length > 2)
                                throw new NotImplementedException("cannot take higher object depth than 1");
                            object subinst = prop.GetValue(datum) 
                                ?? throw new InvalidCsvFormatException($"Cannot find object instance of property {subclass[0]}");
                            PropertyInfo subinfo = prop.PropertyType.GetProperty(subclass[1]) 
                                ?? throw new InvalidCsvFormatException($"Cannot find subproperty {subclass[1]}");

                            TypeConverter converter = TypeDescriptor.GetConverter(subinfo.PropertyType);
                            object? convertedValue = converter.ConvertFrom(val);
                            subinfo.SetValue(subinst, convertedValue, null);
                        }
                        else
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                            try
                            {
                                object? convertedValue = converter.ConvertFrom(val);
                                prop.SetValue(datum, convertedValue);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                                throw;
                            }
                        }
                    }
                }
                data.Add(datum);
            }
            return data;
        }

        public void Serialize(Stream stream, IList<T> data)
        {
            throw new NotImplementedException();

            StringBuilder sb = new();
            List<string> values = new();

            if (HasHeader)
            {
                sb.AppendLine(_header);
            }

            sb.AppendLine(string.Join(Delimiter, _properties.Select(a => a.Name.Replace("_", ".")).ToArray()));

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
