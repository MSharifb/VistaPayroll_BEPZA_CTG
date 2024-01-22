using System;

namespace DAL.PGM.CustomEntities
{
    public  class BonusTypeCustomModel
    {
        public int Id { get; set; }
        
        public string BonusType { get; set; }
       
        public int? ReligionId { get; set; }

        public string Religion { get; set; }
      
        public Boolean IsTaxable { get; set; }
    }
}
