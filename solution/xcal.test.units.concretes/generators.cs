using FizzWare.NBuilder;
using reexjungle.xmisc.foundation.concretes;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.concretes
{
    public class ContentGenerator<TContent> : xmisc.infrastructure.contracts.IGenerator<TContent>
    {
        private readonly IList<TContent> candidates;

        public ContentGenerator(IList<TContent> candidates)
        {
            if (candidates.NullOrEmpty()) throw new ArgumentException("candidates");
            this.candidates = candidates;
        }

        public TContent GetNext()
        {
            return Pick<TContent>.RandomItemFrom(candidates);
        }

        public void Reset()
        {
            candidates.Clear();
        }
    }
}