using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class ST_COUNTRY
    {
        [Column("CTY_INTCOD")]
        public string CtyIntcod { get; set; }

        [Column("CTY_SHONAM_ENG")]
        public string CtyShonamEng { get; set; }

        [Column("CTY_SHONAM_GER")]
        public string CtyShonamGer { get; set; }

        [Column("CTY_SHONAM_FRA")]
        public string CtyShonamFra { get; set; }

        [Column("CTY_SHONAM_ITA")]
        public string CtyShonamIta { get; set; }

        [Column("CTY_NAME_ENG")]
        public string CtyNameEng { get; set; }

        [Column("CTY_NAME_GER")]
        public string CtyNameGer { get; set; }

        [Column("CTY_NAME_FRA")]
        public string CtyNameFra { get; set; }

        [Column("CTY_NAME_ITA")]
        public string CtyNameIta { get; set; }

        [Column("ORD")]
        public int? Ord { get; set; }
    }
}