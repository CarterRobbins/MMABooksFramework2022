using System;
using System.Collections.Generic;
using System.Text;

namespace MMABooksTools
{
    /// <summary>
    /// IWriteDB is part of the "data tier" of the framework.
    /// It supports writing to the data source.
    /// </summary>
    public interface IWriteDB
    {
        IBaseProps Create(IBaseProps props);
        bool Update(IBaseProps props);
        bool Delete(IBaseProps props);

    }
}
