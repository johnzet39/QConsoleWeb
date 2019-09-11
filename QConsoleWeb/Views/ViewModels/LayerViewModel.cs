﻿using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class LayerViewModel
    {
        public IEnumerable<Layer> Layers { get; set; }
        public IEnumerable<Layer> Dictionaries { get; set; }
    }
}
