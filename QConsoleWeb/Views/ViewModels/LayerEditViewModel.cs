using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class LayerEditViewModel
    {
        public Layer CurrentLayer { get; set; }
        public IEnumerable<LayerGrants> LayerGrantsList { get; set; }
        public bool IsAddDocFiles { get; set; }
    }
}
