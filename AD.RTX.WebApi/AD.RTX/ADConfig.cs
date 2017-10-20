using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AD.RTX.WebApi
{
    public class ADConfig
    {
        public string DoMainPath { get; set; }
        public string DoMainOUPath { get; set; }
        public string AdminUser { get; set; }
        public string Password { get; set; }
        public string DoMainPath1 { get; set; }
        public string DoMainPath2 { get; set; }
        public string DoMainPath3 { get; set; }
        public void SetDoMainPath()
        {
            DoMainPath = DoMainPath1 + DoMainPath3;
        }
        public void SetDoMainOUPath()
        {
            DoMainPath = DoMainPath1 + DoMainPath2 + DoMainPath3;
        }
    }
}