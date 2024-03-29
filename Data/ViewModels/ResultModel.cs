﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModels
{
    public class ResultModel
    {
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
        public bool Succeed { get; set; }
        public object ResponseFailed { get; set; }

    }

    public class ResultMessage 
    {
        public bool IsSuccessStatus { get; set; }
        public object Response{ get; set; }
      

    }
}
