﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLCore.Language.AST
{
    public class GraphQLLocation
    {
        public int End { get; set; }
        public int Start { get; set; }
    }
}