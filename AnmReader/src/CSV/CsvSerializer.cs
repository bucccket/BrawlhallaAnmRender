using BrawlhallaANMReader.utils;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;


//TODO: add safeguarding to deserialization
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
        public int SkippedLines { get; set; } = 0;

        private readonly List<PropertyInfo> _properties;

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
                           orderby prop.Name
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

            try
            {
                if (SkippedLines > 0)
                {
                    for (int i = 0; i < SkippedLines; i++)
                    {
                        tfp.ReadLine();
                    }
                }
                collumns = tfp.ReadFields();
                if (collumns is null)
                    throw new InvalidCsvFormatException(@"Failed to read collumns");
            }
            catch (Exception ex)
            {
                Logger.Error("The CSV File is Invalid. See Inner Exception for more inoformation.");
                throw new InvalidCsvFormatException("The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }

            List<T> data = new();

            while (!tfp.EndOfData)
            {
                string[]? cells = tfp.ReadFields() ?? throw new InvalidCsvFormatException(@"Reader finished all lines before end of file.");
                if (cells == null)
                    throw new InvalidCsvFormatException(@"Reader finished all lines before end of file.");

                if (cells.Length == 1 && cells[0] != null && cells[0] == "EOF")
                    break;

                T datum = new();
                for (int i = 0; i < cells.Length; i++)
                {
                    string val = cells[i];
                    string col = collumns[i];

                    PropertyInfo? prop = _properties.FirstOrDefault(a => a.Name.Equals(col, StringComparison.InvariantCultureIgnoreCase));

                    if (prop is null)
                        continue;

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
                data.Add(datum);
            }
            return data;
        }

        public void Serialize(Stream stream, IList<T> data)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class CsvIgnoreAttribute : Attribute { }
}
