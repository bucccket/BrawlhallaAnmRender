using BrawlhallaANMReader.Swz.Utils;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BrawlhallaANMReader.Swz.CSV;

/// <summary>
/// CsvSerializer serializes classes to .csv files and deserializes textstreams
/// </summary>
/// <typeparam name="T">Type of class to serialize</typeparam>
public class CsvSerializer<T> where T : class, new()
{
    /// <value> Indicates, whether the first line of the file contains the file name instead of the column names </value>
    public bool HasHeader { get; set; } = false;
    /// <value> Delimited for CSV file (e.g. \t for TSV files) </value>
    public string Delimiter { get; set; } = ",";

    /// <value> Type Reflection properties for base type <typeparamref name="T"/> </value>
    private readonly List<PropertyInfo> _properties;
    /// <value> <see cref="HasHeader">Header</see> line text </value>
    private string? _header;

    /// <summary> Fetches all properties  </summary>
    public CsvSerializer()
    {
        Type type = typeof(T);

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                            BindingFlags.Instance |
                                            BindingFlags.GetProperty |
                                            BindingFlags.SetProperty);

        _properties = properties.Where(prop => prop.GetCustomAttribute<CsvIgnoreAttribute>() == null).ToList();
    }

    /// <summary>
    /// Deserializes CSV file stream
    /// </summary>
    /// <param name="stream">File stream to CSV file</param>
    /// <returns>List of instances of generic type</returns>
    /// <exception cref="InvalidCsvFormatException">Malformed CSV input file</exception>
    /// <exception cref="NotImplementedException">Recursively nested subclasses are yet to be implemented</exception>
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
        catch
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
                            ?? throw new InvalidCsvFormatException($"Cannot find class instance of property {subclass[0]}");
                        PropertyInfo subinfo = prop.PropertyType.GetProperty(subclass[1])
                            ?? throw new InvalidCsvFormatException($"Cannot find subclass property {subclass[1]}");

                        TypeConverter converter = TypeDescriptor.GetConverter(subinfo.PropertyType);
                        object? convertedValue = converter.ConvertFrom(val);
                        subinfo.SetValue(subinst, convertedValue);
                    }
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);

                        object? convertedValue = converter.ConvertFrom(val);
                        prop.SetValue(datum, convertedValue);
                    }
                }
            }
            data.Add(datum);
        }
        return data;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="data"></param>
    /// <exception cref="NotImplementedException">Serialization is not done!!</exception>
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
            values.Clear();
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
