﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get;set; }
    }
}
