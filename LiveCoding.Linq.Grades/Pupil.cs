using System;
using System.Collections.Generic;

namespace LiveCoding.Linq.Grades
{
    class Pupil
    {
        public IEnumerable<Tuple<int, int>> Grades { get; set; }
    }
}
