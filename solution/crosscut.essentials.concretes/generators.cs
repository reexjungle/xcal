using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reexmonkey.crosscut.essentials.contracts;

namespace reexmonkey.crosscut.essentials.concretes
{
    public class GuidKeyGenerator: IKeyGenerator<string>
    {
        public string GetNextKey()
        {
            return (Guid.NewGuid() == Guid.Empty)? GetNextKey(): Guid.NewGuid().ToString();
        }

    }

    public class IntegralKeyGenerator: IKeyGenerator<int>
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

    public class LongKeyGenerator: IKeyGenerator<long>
    {
        private long counter = 0;

        public long GetNextKey()
        {
            return counter++;
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

}
