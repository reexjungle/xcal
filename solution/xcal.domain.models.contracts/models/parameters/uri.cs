using System;

namespace reexjungle.xcal.core.domain.contracts.models.parameters
{
    public interface IURI
    {
        Uri Uri { get; set; }
    }

    public interface IDIR : IURI
    {

    }

    public interface IURL : IURI
    {

    }

    public interface IALTREP : IURI
    {

    }
}
