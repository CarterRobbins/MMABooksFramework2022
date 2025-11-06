using System;
using System.Collections.Generic;
using System.Text;

namespace MMABooksTools
{
    /// <summary>
    /// IReadDB is part of the "data tier" of the framework.
    /// It supports reading from the data source.
    /// </summary>
    public interface IReadDB
    {
        IBaseProps Retrieve(Object key);
        object RetrieveAll();
    }
}
