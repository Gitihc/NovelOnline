using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    public  class Relevance : EntityBase
    {
        public Relevance()
        {
            this.Description = string.Empty;
            this.Key = string.Empty;
            this.Status = 0;
            this.OperateTime = DateTime.Now;
            this.OperatorId = string.Empty;
            this.FirstId = string.Empty;
            this.SecondId = string.Empty;
        }
        public string Description { get; set; }
        public string Key { get; set; }
        public int Status { get; set; }
        public DateTime OperateTime { get; set; }
        public string OperatorId { get; set; }
        public string FirstId { get; set; }
        public string SecondId { get; set; }
    }
}
