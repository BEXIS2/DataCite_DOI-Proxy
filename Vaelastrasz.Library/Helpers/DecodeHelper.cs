using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Vaelastrasz.Library.Helpers
{
    public static class DecodeHelper
    {
        public static string SafeDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Nur decodieren, wenn überhaupt Encoding enthalten ist
            if (!value.Contains('%'))
                return value;

            try
            {
                var decoded = WebUtility.UrlDecode(value);

                // Sicherheitscheck: falls schon decodiert → nicht nochmal verändern
                return decoded;
            }
            catch
            {
                // Fallback: Original zurückgeben (keine Exception nach außen)
                return value;
            }
        }
    }
}
