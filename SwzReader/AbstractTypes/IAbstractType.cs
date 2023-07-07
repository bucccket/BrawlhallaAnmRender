using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaANMReader.Swz.AbstractTypes;
public interface IAbstractType
{
    public static abstract void Parse(Stream stream);
}
