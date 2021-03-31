using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCViz
{
    class ViewModelBase: ObservableObject
    {
        public string Title { get; private set; }
    }
}
