using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCaseAndWhitespace(this string source, string value)
        {
            return source.Replace(" ", "").IndexOf(value.Replace(" ", ""), StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
