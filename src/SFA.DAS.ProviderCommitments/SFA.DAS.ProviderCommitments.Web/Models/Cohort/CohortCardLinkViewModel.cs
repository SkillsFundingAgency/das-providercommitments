﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CohortCardLinkViewModel
    {
        public int Count { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string LinkId { get; set; }

        public CohortCardLinkViewModel(int count, string description, string url, string linkId)
        {
            if (count > 0)
            {
                Url = url;
            }
            Count = count;
            Description = description;
            LinkId = linkId;
        }
    }
}
