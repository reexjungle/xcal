using System;
using System.Text;
using System.Text.RegularExpressions;
using reexmonkey.infrastructure.operations.contracts;

namespace reexmonkey.infrastructure.operations.concretes
{
    public class GuidKeyGenerator: IGuidKeyGenerator
    {
        private string seed = null;
        private bool compact = false;

        public GuidKeyGenerator(bool compact = true)
        {
            this.compact = compact;
            this.seed = string.Empty;
        }

        public GuidKeyGenerator(string seed, bool compact = true)
        {
            var pattern = @"^(\(|\{)?(?<block1>0?[x]?[0-9a-f]{8})[\-]{1}?(?<block2>0?[x]?[0-9a-f]{4})[\-]{1}?(?<block3>0?[x]?[0-9a-f]{4})[\-]{1}?(?<block4>0?[x]?[0-9a-f]{4})[\-]{1}?(?<block5>0?[x]?[0-9a-f]{12})(\)|\})?$";
            if (Regex.IsMatch(seed, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)) this.seed = seed;
            else throw new FormatException("seed does not match GUID format");
            this.compact = compact;
        }

        public string GetNextKey()
        {
            var key = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(this.seed)) 
                    key = (Guid.NewGuid() == Guid.Empty) 
                        ? GetNextKey() 
                        : Guid.NewGuid().ToString();
                else key = new Guid(this.seed).ToString();
               
                return compact
                   ?key.Replace("-", string.Empty)
                   :key;
            }
            catch (ArgumentNullException) { throw; }
            catch (FormatException) { throw; }
            catch (OverflowException) { throw; }
            catch (Exception) { throw; }
        }
    }

    public class FPIKeyGenerator<T>: IFPIKeyGenerator
        where T: IEquatable<T>
    {
        private IKeyGenerator<T> discriminator;

        public string ISO { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string LanguageId { get; set; }
        public Authority Authority { get; set; }

        public FPIKeyGenerator(IKeyGenerator<T> discriminator = null)
        {
            if (discriminator != null) this.discriminator = discriminator;
        }

        public string GetNextKey()
        {
            var sb = new StringBuilder();
            if (Authority == Authority.ISO) sb.Append(this.ISO);
            else if (Authority == Authority.NonStandard) sb.Append("+");
            else if (Authority == Authority.None) sb.Append("-");
            if(!string.IsNullOrEmpty(this.Owner) && this.discriminator != null)sb.AppendFormat("//{0}-{1}", this.Owner, this.discriminator.GetNextKey());
            else if(!string.IsNullOrEmpty(this.Owner))sb.AppendFormat("//{0}", this.Owner);
            if(!string.IsNullOrEmpty(this.Description))sb.AppendFormat("//{0}", this.Description);            
            if(!string.IsNullOrEmpty(this.LanguageId))sb.AppendFormat("//{0}", this.LanguageId);      
            return sb.ToString();
        }
    }

    public class IntegralKeyGenerator: IIntegralKeyGenerator
    {
        private int counter = 0;
        public int GetNextKey()
        {
            return counter++;
        }

        public IntegralKeyGenerator()
        {
            this.counter = 0;
        }

        public IntegralKeyGenerator(int seed)
        {
            this.counter = seed;
        }
    }

    public class LongKeyGenerator: ILongKeyGenerator
    {
        private long counter = 0;

        public long GetNextKey()
        {
            return ++counter;
        }

        public LongKeyGenerator()
        {
            this.counter = 0;
        }

        public LongKeyGenerator(long seed)
        {
            this.counter = seed;
        }
    }

    public static class GeneratorExtensions
    {
        public static string ToUrn(this string fpi)
        {
            return string.Format("urn:{0}", fpi.Replace("//", ":"));
        }

        public static string ToFpi(this string urn)
        {
            return string.Format("urn:{0}", urn.Substring(4).Replace(":", "//"));
        }
    }
}
