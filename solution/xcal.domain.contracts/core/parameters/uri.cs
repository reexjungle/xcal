using System;

namespace xcal.domain.contracts.core.parameters
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
