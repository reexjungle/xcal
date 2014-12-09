using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace reexjungle.infrastructure.operations.concretes
{
    public static class EncodingExtensions
    {

        public static byte[] ToUtf8Bytes(this string unicode)
        {
            byte[] bytes = null;
            try
            {
                var utf8 = new UTF8Encoding();
                bytes = utf8.GetBytes(unicode);
            }
            catch (ArgumentNullException ) { throw; }
            catch (EncoderFallbackException ) { throw; }
            catch (Exception) { throw; }
            return bytes;
        }

        public static string ToUtf8String(this string unicode)
        {
            string encoded = null;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(unicode);
                encoded = Encoding.UTF8.GetString(bytes);
            }
            catch (ArgumentNullException ) { throw; }
            catch (EncoderFallbackException ) { throw; }
            return encoded;
        }

        /// <summary>
        /// Converts plain text to its equivalent encoded ASCII raw binary.
        /// </summary>
        /// <param name="unicode">The unicode to be byte-encoded.</param>
        /// <returns>The Base64-based binary value encoded from the plain text</returns>
        /// <exception cref="ArgumentNullException">Throw when the plain text argument is null</exception>
        /// <exception cref="EncoderFallbackException">Throw when encoding the plain text to Base64 fails</exception>
        public static byte[] ToAsciiBytes(this string unicode)
        {
            if (unicode == null) throw new ArgumentNullException();
            try
            {
                return Encoding.ASCII.GetBytes(unicode);        
            }
            catch (ArgumentNullException) { throw; }
            catch (EncoderFallbackException) { throw; }
        }

        public static string ToAsciiString(this string unicode)
        {
            string encoded = null;
            try
            {
                var bytes = Encoding.Default.GetBytes(unicode);
                encoded = Encoding.ASCII.GetString(bytes);
            }
            catch (ArgumentNullException) { throw; }
            catch (EncoderFallbackException) { throw; }
            return encoded;
        }

        /// <summary>
        /// Converts plain text to its equivalent encoded Base64 string
        /// </summary>
        /// <param name="unicode">The plain text (unencoded) that is to be encoded</param>
        /// <returns>The Base64 string encoded from the plain text</returns>
        /// <exception cref="ArgumentNullException">Thrown when the plain text argument is null</exception>
        /// <exception cref="EncoderFallbackException">Throw when encoding the plain text to Base64 fails</exception>
        public static string ToBase64String(this string unicode)
        {
            string base64 = string.Empty;
            try
            {
                var bytes = Encoding.Unicode.GetBytes(unicode);
                base64 = Convert.ToBase64String(bytes);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.ToString(), ex);
            }
            catch (EncoderFallbackException ex)
            {
                throw new EncoderFallbackException(ex.ToString(), ex);
            }
            return base64;
        }

        /// <summary>
        /// Converts a Base64 string to its equivalent encoded plain text
        /// </summary>
        /// <param name="base64">The base64 text (encoded) that is to be decoded</param>
        /// <returns>Plain text decoded from the Base64 text</returns>
        /// <exception cref="ArgumentNullException">Thrown when the plain text argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when conversion from Base64 to raw binary data fails</exception>
        /// <exception cref="FormatException">Thrown when conversion from Base64 to raw binary data fails</exception>
        /// <exception cref="DecoderFallbackException">Thrown when decoding from raw binary data to plain text fails</exception>
        public static string ToUnicodeString(this string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                var unicode = Encoding.Unicode.GetString(bytes);
                return unicode;
            }
            catch (ArgumentNullException) { throw; }
            catch (EncoderFallbackException) { throw; }
            catch (DecoderFallbackException) { throw; }


        }

        /// <summary>
        /// Converts a Base64 string to its equivalent raw 8-bit unsigned integer array.
        /// </summary>
        /// <param name="base64">The base64 text (encoded) that is to be decoded</param>
        /// <returns>Raw binary data decoded from the Base64 text</returns>
        /// <exception cref="ArgumentNullException">Thrown when the plain text argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when conversion from Base64 to raw binary data fails</exception>
        /// <exception cref="FormatException">Thrown when conversion from Base64 to raw binary data fails</exception>
        public static IEnumerable<byte> ToBytes(this string base64)
        {
            try
            {
                return Convert.FromBase64String(base64);
            }
            catch (ArgumentNullException) { throw; }
            catch (FormatException) { throw; }
            

        }

    }
}
