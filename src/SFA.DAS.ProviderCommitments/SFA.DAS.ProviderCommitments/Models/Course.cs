using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ProviderCommitments.Models
{
    public class Course
    {
        public Course(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
