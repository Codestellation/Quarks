using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Codestellation.Quarks.Collections;

namespace Codestellation.Quarks.Tests.Collections;

public class CompilationTests
{
    public CompilationTests()
    {
        //This is not test by itself. But it checks ambiguous calls between methods
        Array.Empty<int>().ConvertToArray(x => x);
        Array.Empty<int>().ConvertToList(x => x);

        new List<int>().ConvertToArray(x => x);
        new List<int>().ConvertToList(x => x);

        new Dictionary<int, int>().ConvertToArray(x => x);
        new Dictionary<int, int>().ConvertToList(x => x);

        new ReadOnlyCollection<int>(Array.Empty<int>()).ConvertToArray(x => x);
        new ReadOnlyCollection<int>(Array.Empty<int>()).ConvertToList(x => x);
    }
}